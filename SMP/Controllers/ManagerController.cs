﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SMP.Models;
using SMP.Models.MailSender;
using SMP.ViewModels;

namespace SMP.Controllers
{
    public class ManagerController : Controller
    {
        private DataManager _DataManager;

        public ManagerController(DataManager _DM)
        {
            _DataManager = _DM;
        }

        public bool AccessControll()
        {
            if (((Person) Session["user"]) == null)
            {
                return false;
            }

            switch (((Person) Session["user"]).Position)
            {
                case Position.Исполнитель:

                    return false;
                    break;
            }

            return true;
        }

        public ActionResult AccesError()
        {
            return View();
        }

        #region Projects view

        public ActionResult Projects()
        {
            if (!AccessControll()) return RedirectToAction("AccesError");
            ViewData["projects"] =
                _DataManager.teamRepository.GetTeamsByPerson(((Person) Session["user"]).IdPerson)
                    .Where(team => team.Project.parrentProject == null)
                    .Select(team => team.Project)
                    .OrderBy(p => p.endDateTime);

            return View();
        }

        public ActionResult DeleteProject(int idProject)
        {
            _DataManager.projectRepository.DeleteProject(idProject, idProject);

            return RedirectToAction("Projects");
        }

        #endregion

        #region Project view

        [HttpGet]
        public ActionResult Project(int idProject)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            GetProject(idProject);
            GetExecutors(idProject);
            GetTeams(idProject);
            GetTreeView(idProject);

            Project p = (Project) ViewData.Model;
            int left = (p.endDateTime - DateTime.Now).Days;
            if (left > p.reserve)
                left = p.reserve;
            ViewData["leftReserve"] = left.ToString();

            ViewData["Length"] = (p.endDateTime - p.startDateTime).Days / 2;

            if (((IEnumerable<Team>) ViewData["teams"]).Count() != 0)
                ViewData["endOfLastWork"] =
                    (p.endDateTime - ((IEnumerable<Team>) ViewData["teams"]).Last().Project.endDateTime).Days;
            else
                ViewData["endOfLastWork"] = (p.endDateTime - p.startDateTime).Days;

            return View();
        }

        [HttpPost]
        public ActionResult Project(string projectId, string projectName, string projectStart, string projectEnd,
            string projectDescription, int length)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                ModelState.AddModelError("ProjectName", "Навание проекта не может быть пустым");

            if (string.IsNullOrWhiteSpace(projectStart))
                ModelState.AddModelError("ProjectStart", "Дата начала не может быть пустой");

            DateTime start = new DateTime();
            try
            {
                start = Convert.ToDateTime(projectStart);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectStart", "Дата начала не является датой");
            }

            if (string.IsNullOrWhiteSpace(projectEnd))
                ModelState.AddModelError("ProjectEnd", "Дата окончания не может быть пустой");

            DateTime end = new DateTime();
            try
            {
                end = Convert.ToDateTime(projectEnd);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectEnd", "Дата окончания не является датой");
            }

            if (start > end)
            {
                ModelState.AddModelError("ProjectLength", "Дата начала должна быть до даты окончания");
            }

            int id = Convert.ToInt32(projectId);

            if (ModelState.IsValid)
            {
                _DataManager.projectRepository.EditProject(id, projectName, projectDescription, start, end, 0, length);
                GetProject(id);
                GetExecutors(id);
                GetTeams(id);
                GetTreeView(id);
                return RedirectToAction("Project", new {idProject = id});
            }

            GetProject(id);
            GetExecutors(id);
            GetTeams(id);
            GetTreeView(id);

            return View();
        }

        public ActionResult DeleteTeam(int teamId, int projectId)
        {
            _DataManager.teamRepository.DeleteTeam(teamId);

            return RedirectToAction("Project", new {idProject = projectId});
        }

        public ActionResult AddTeam(int personId, int projectId)
        {
            _DataManager.teamRepository.AddTeam(personId, projectId);

            return RedirectToAction("Project", new {idProject = projectId});
        }

        public ActionResult DeleteWork(int workId, int projectId)
        {
            _DataManager.projectRepository.DeleteProject(workId, workId);
            return RedirectToAction("Project", new {idProject = projectId});
        }

        public ActionResult DelWork(int workId)
        {
            var project = _DataManager.projectRepository.GetInnerProject(workId);
            return RedirectToAction("DeleteWork", new { workId, projectId = project.IdProject });
        }


        #endregion

        #region Add Project
        [HttpGet]
        public ActionResult AddProjectFirstStep(int? idProject)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");
            if (idProject.HasValue)
            {
                Project p = _DataManager.projectRepository.GetProjectById(idProject.Value);
                ViewData.Model = p;
                ViewData["Length"] = ((p.endDateTime - p.startDateTime).Days / 2).ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddProjectFirstStep(int? idProject, string projectName, string projectStart, string projectEnd, string projectDescription, int length, string submit)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                ModelState.AddModelError("ProjectName", "Навание проекта не может быть пустым");

            if (string.IsNullOrWhiteSpace(projectStart))
                ModelState.AddModelError("ProjectLength", "Дата начала не может быть пустой");

            DateTime start = new DateTime();
            try
            {
                start = Convert.ToDateTime(projectStart);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectLength", "Дата начала не является датой");
            }

            if (string.IsNullOrWhiteSpace(projectEnd))
                ModelState.AddModelError("ProjectLength", "Дата окончания не может быть пустой");

            DateTime end = new DateTime();
            try
            {
                end = Convert.ToDateTime(projectEnd);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectLength", "Дата окончания не является датой");
            }

            if (start > end)
            {
                ModelState.AddModelError("ProjectLength", "Дата начала должна быть до даты окончания");
            }

            if (((end - start).Days / 2) < length)
            {
                ModelState.AddModelError("ProjectLength", "Резерв должен составлять не более\n половины срока выполнения проекта");
            }

            if (ModelState.IsValid)
            {
                Project p = new Project();
                if (!idProject.HasValue)
                {
                    p = _DataManager.projectRepository.AddProject(projectName, projectDescription, start, end, 0, 1);
                    _DataManager.teamRepository.AddTeam(((Person)Session["user"]).IdPerson, p.IdProject);
                }
                else
                {
                    p = _DataManager.projectRepository.EditProject(idProject.Value, projectName, projectDescription, start, end, 0, length);
                }

                switch (submit)
                {
                    case "Сохранить изменения":
                        return RedirectToAction("Projects");
                        break;
                    case "К шагу 2":
                        return RedirectToAction("AddProjectSecondStep",routeValues: new { projectId = p.IdProject });
                        break;
                    case "Готово":
                        return RedirectToAction("AddProjectFirstStep", routeValues: new { idProject = p.IdProject });
                        break;
                }
            }

            return View();
        }
        
        public ActionResult AddProjectSecondStep(int projectId)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            GetProject(projectId);
            GetExecutors(projectId);

            return View();
        }

        public ActionResult DeleteTeamSecondStep(int teamId, int projectId)
        {
            _DataManager.teamRepository.DeleteTeam(teamId);

            return RedirectToAction("AddProjectSecondStep", new {projectId });
        }

        public ActionResult AddTeamSecondStep(int personId, int projectId)
        {
            _DataManager.teamRepository.AddTeam(personId, projectId);

            return RedirectToAction("AddProjectSecondStep", new {projectId });
        }
        #endregion

        #region suported functions
        private void GetProject(int idProject)
        {
            ViewData.Model = _DataManager.projectRepository.GetProjectById(idProject);
        }

        private void GetExecutors(int idProject)
        {
            List<Team> teams = new List<Team>();
            foreach (var t in _DataManager.teamRepository.GetTeamsByProject(idProject))
            {
                if (t.Person.Position == Position.Исполнитель &&
                    t.Person.IdPerson != ((Person)Session["user"]).IdPerson)
                    teams.Add(t);
            }
            ViewData["teams"] = teams;

            List<Person> persons = new List<Person>();
            var usedPersons = teams.Select(t => t.Person);
            foreach (var p in _DataManager.personRepository.GetPersons())
            {
                if (!usedPersons.Contains(p) &&
                    p.IdPerson != ((Person)Session["user"]).IdPerson)
                    persons.Add(p);
            }
            ViewData["persons"] = persons;
        }

        private void GetTeams(int idProject)
        {
            ViewData["teams"] = _DataManager.teamRepository.GetTeamsByProject(idProject)
                .OrderBy(w => w.Project.endDateTime);
        }

        private void GetTreeView(int idProject)
        {
            var works = _DataManager.projectRepository.GetProjectsByParrentId(idProject).ToList();
            ViewData["treeView"] = TreeViewModel<Project>.BuildTree(
                works,
                x => _DataManager.projectRepository.GetProjectsByParrentId(x.IdProject).ToList());
        }

        /// <summary>
        /// Записывает во ViewData список каких-то команд. Предположительно - список команд главного проекта. 
        /// </summary>
        /// <param name="idProject"></param>
        private void GetTeam(int idProject)
        {
            SortedList<int, string> sl = new SortedList<int, string>();
            var innerProject = _DataManager.projectRepository.GetInnerProject(idProject);
            foreach (Team t in _DataManager.teamRepository.GetTeamsByProject(innerProject.IdProject))
            {
                if (t.Person.Position == Position.Исполнитель)
                    sl.Add(t.Person.IdPerson, t.Person.firstName + ' ' + t.Person.surName);
            }
            ViewData["Team"] = new SelectList(sl, "Key", "Value");
        }

        /// <summary>
        ///  Записывает во ViewData список команд главного проекта.
        /// </summary>
        /// <param name="idProject"></param>
        private void GetMainTeams(int idProject)
        {
            SortedList<int, string> sl = new SortedList<int, string>();
            foreach (Team t in _DataManager.teamRepository.GetTeamsOfMainProject(idProject))
            {
                if (t.Person.Position == Position.Исполнитель)
                    sl.Add(t.Person.IdPerson, t.Person.firstName + ' ' + t.Person.surName);
            }
            ViewData["Team"] = new SelectList(sl, "Key", "Value");
        }

        /// <summary>
        ///  Проверяет, занята ли персона в указанный период времени.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="personId"></param>
        /// <param name="parrentID"></param>
        /// <returns></returns>
        private bool CheckPersonTime(DateTime start, DateTime end, int personId, int parrentID)
        {
            List<Project> works = (from p in _DataManager.projectRepository.GetProjectsByPersonId(personId)
                                   where p.parrentProject != null && p.IdProject != parrentID
                                   select p).ToList();

            foreach (Project work in works)
            {
                if ((start >= work.startDateTime && start <= work.endDateTime) || (end >= work.startDateTime && end <= work.endDateTime)) return true;
            }

            return false;
        }
        #endregion

        #region Works
        [HttpGet]
        public ActionResult Work(int projectId)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            var vm = new WorkWievModel(projectId, -1);

            return View(vm);
        }

        //team - это не айди команды, а айди персоны.
        [HttpPost]
        public ActionResult Work(string projectId, string projectName, string projectStart, string projectEnd, string projectDescription, int? team, string submit)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                ModelState.AddModelError("ProjectName", "Навание работы не может быть пустым");

            if (string.IsNullOrWhiteSpace(projectStart))
                ModelState.AddModelError("ProjectStart", "Дата начала не может быть пустой");

            DateTime start = new DateTime();
            try
            {
                start = Convert.ToDateTime(projectStart);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectStart", "Дата начала не является датой");
            }

            if (string.IsNullOrWhiteSpace(projectEnd))
                ModelState.AddModelError("ProjectEnd", "Дата окончания не может быть пустой");

            DateTime end = new DateTime();
            try
            {
                end = Convert.ToDateTime(projectEnd);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectEnd", "Дата окончания не является датой");
            }

            if (start > end)
            {
                ModelState.AddModelError("ProjectLength", "Дата начала должна быть до даты окончания");
            }

            int id = Convert.ToInt32(projectId);
            Project parrentProject = _DataManager.projectRepository.GetProjectById(id).parrentProject;
            if (start < parrentProject.startDateTime || start > parrentProject.endDateTime ||
                end < parrentProject.startDateTime || end > parrentProject.endDateTime)
            {
                ModelState.AddModelError("ProjectLength", "Начало или конец работы выходят за пределы проекта");
            }

            if (ModelState.IsValid)
            {
                Project p = _DataManager.projectRepository.EditProject(id, projectName, projectDescription, start, end, 0, 0);

                //Отправить уведомление об изменинии в работе
                if (team != null)
                {
                    _DataManager.teamRepository.DeleteTeam(_DataManager.teamRepository.GetTeamByWork(id).IdTeam);
                    _DataManager.teamRepository.AddTeam(team.Value, id);
                    MailSender sender = new MailSender();
                    string email = _DataManager.personRepository.GetPersonById(team.Value).email;
                    var work = _DataManager.projectRepository.GetProjectById(id);
                    sender.Send(new WorkChangedMail(email, work)); 
                }

                if (p.parrentProject?.parrentProject == null)
                    return RedirectToAction("Project", new { idProject = p.parrentProject.IdProject });
                else
                    return RedirectToAction("Work", new { projectId = p.parrentProject.IdProject });
            }

            // Во view отправляется неизмененная работа, как раньше. 
            // Todo Надо переделать так, чтобы во view отправлялась работа с теми же значениями полей.
            // Для этого можно улучшить структуру view model:
            // создать группу "валидных" значений, с которыми можно делать логику,
            // и группу значений "для показа", в которую смогут входить как валидные, так и не валидные, с этой группой не будет никакой логики.
            // Для этого нужно будет убрать валидацию модели из контроллеров
            var vm = new WorkWievModel(_DataManager.projectRepository.GetProjectById(id));
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddWork(int projectId, string backUrl)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            var vm = new WorkWievModel(-1, projectId, backUrl);

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddWork(string projectId, string projectName, string projectStart, string projectEnd, string projectDescription, int? personId, string submit, string backUrl)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                ModelState.AddModelError("ProjectName", "Навание работы не может быть пустым");

            if (string.IsNullOrWhiteSpace(projectStart))
                ModelState.AddModelError("ProjectStart", "Дата начала не может быть пустой");

            DateTime start = new DateTime();
            try
            {
                start = Convert.ToDateTime(projectStart);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectStart", "Дата начала не является датой");
            }

            if (string.IsNullOrWhiteSpace(projectEnd))
                ModelState.AddModelError("ProjectEnd", "Дата окончания не может быть пустой");

            DateTime end = new DateTime();
            try
            {
                end = Convert.ToDateTime(projectEnd);
            }
            catch (FormatException e)
            {
                ModelState.AddModelError("ProjectEnd", "Дата окончания не является датой");
            }

            if (start > end)
            {
                ModelState.AddModelError("ProjectLength", "Дата начала должна быть до даты окончания");
            }

            int id = Convert.ToInt32(projectId);
            Project project = _DataManager.projectRepository.GetProjectById(id);
            if (start < project.startDateTime || start > project.endDateTime ||
                end < project.startDateTime || end > project.endDateTime)
            {
                ModelState.AddModelError("ProjectLength", "Начало или конец работы выходят за пределы проекта");
            }

            if (CheckPersonTime(start, end, Convert.ToInt32(personId), id)) ModelState.AddModelError("ProjectLength", "Исполнитель на это время уже занят в другой работе");

            if (!personId.HasValue) ModelState.AddModelError("PersonId", "Исполнитель не выбран");

            if (ModelState.IsValid)
            {
                var p = _DataManager.projectRepository.AddProject(projectName, projectDescription, start, end, 0, 0, id);
                _DataManager.teamRepository.AddTeam(Convert.ToInt32(personId.Value),p.IdProject);
                if (p.parrentProject.parrentProject != null)
                    _DataManager.teamRepository.DeleteTeam(_DataManager.teamRepository.GetTeamByWork(p.parrentProject.IdProject)?.IdTeam);
               
                //Отправить уведомление на почту исполнителя
                var mailSender = new MailSender();
                string personMail = _DataManager.personRepository.GetPersonById(personId.Value).email;
                mailSender.Send(new NewWorkTemplate(personMail, p));

                if (p.parrentProject == null)
                    return RedirectToAction("Project", new { idProject = p.IdProject });
                else
                    return RedirectToAction("Work", new { projectId = p.IdProject });
            }

            var vm = new WorkWievModel(-1, project.IdProject, backUrl);
            return View(vm);
        }
        #endregion
    }
}
