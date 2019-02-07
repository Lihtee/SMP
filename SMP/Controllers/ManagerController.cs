using System;
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
            //List<Project> projects = new List<Project>();

            //projects.AddRange(
            //    _DataManager.teamRepository.GetTeamsByPerson(((Person)Session["user"]).IdPerson)
            //    .Where(team => team.Project.parrentProject == null)
            //    .Select(team => team.Project)
            //    .OrderBy(p => p.endDateTime));

            //foreach (var t in _DataManager.teamRepository.GetTeamsByPerson(((Person)Session["user"]).IdPerson).Where(team => team.Project.parrentProject == null))
            //{
            //    if (t.Project.parrentProject == null)
            //        projects.Add(t.Project);
            //}

            //ViewData["projects"] = projects;
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
            GetPersons(idProject);
            GetWorks(idProject);
            GetTreeView(idProject);

            Project p = (Project) ViewData.Model;
            int left = (p.endDateTime - DateTime.Now).Days;
            if (left > p.reserve)
                left = p.reserve;
            ViewData["leftReserve"] = left.ToString();

            ViewData["Length"] = (p.endDateTime - p.startDateTime).Days / 2;

            if (((IEnumerable<Team>) ViewData["works"]).Count() != 0)
                ViewData["endOfLastWork"] =
                    (p.endDateTime - ((IEnumerable<Team>) ViewData["works"]).Last().Project.endDateTime).Days;
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
                GetPersons(id);
                GetWorks(id);
                GetTreeView(id);
                return RedirectToAction("Project", new {idProject = id});
            }

            GetProject(id);
            GetPersons(id);
            GetWorks(id);
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
            //Достаем работу и емейл персоны.
            var work = _DataManager.projectRepository.GetProjectById(workId);
            var team = _DataManager.teamRepository.GetTeamByWork(workId);
            string email = team.Person.email;

            //Отправить уведомление об отмене работы.
            MailSender sender = new MailSender();
            //sender.SendCanceledWork(email, work);
            sender.Send(new WordCanceledMail(email, work));

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
            GetPersons(projectId);

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

        private void GetPersons(int idProject)
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

        private void GetWorks(int idProject)
        {
            ViewData["works"] = _DataManager.teamRepository.GetTeamsByParrentProject(idProject)
                .OrderBy(w => w.Project.endDateTime);
        }

        private void GetTreeView(int idProject)
        {
            var works = _DataManager.projectRepository.GetProjectsByParrentId(idProject).ToList();
            ViewData["treeView"] = TreeViewModel<Project>.BuildTree(
                works,
                x => _DataManager.projectRepository.GetProjectsByParrentId(x.IdProject).ToList());
        }

        private void GetPath(int idProject)
        {
            List<Project> path = new List<Project>();
            Project project = _DataManager.projectRepository.GetProjectById(idProject);
            do
            {
                path.Add(project);
                project = project.parrentProject;
            } while (project != null);
            path.Reverse();
            ViewData["path"] = path;
        }

        private void GetTeam(int idProject)
        {
            SortedList<int, string> sl = new SortedList<int, string>();
            foreach (Team t in _DataManager.teamRepository.GetTeamsByProject(_DataManager.projectRepository.GetProjectById(idProject).parrentProject.IdProject))
            {
                if (t.Person.Position == Position.Исполнитель)
                    sl.Add(t.Person.IdPerson, t.Person.firstName + ' ' + t.Person.surName);
            }
            ViewData["Team"] = new SelectList(sl, "Key", "Value");
        }

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

            GetWorks(projectId);
            GetProject(projectId);
            GetPath(projectId);
            GetTeam(projectId);
            var vm = new WorkViewModel(projectId);

            return View(vm);
        }
        //team - это не айди команды, а айди персоны.
        [HttpPost]
        public ActionResult Work(string projectId, string projectName, 
            string projectStart, string projectEnd, string projectDescription, 
            Object selectedInlineWorks, int team, string submit)
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
                List<Project> lastWorks = (from a
                                           in _DataManager.addictionRepository.GetAddictionsByNextProjectID(id)
                                           select a.lastProject).ToList();
                List<int> selectedWorks = (from w
                                           in (IEnumerable<string>)selectedInlineWorks
                                           select Convert.ToInt32(w)).ToList();
                //Запоминаем время последней предшествующей работы
                DateTime lastDate = new DateTime(2000, 1, 1);
                //Удаляем лишние предшествующие работы
                foreach(Project lw in lastWorks)
                {
                    if (selectedWorks.Contains(lw.IdProject))
                    {
                        selectedWorks.Remove(lw.IdProject);
                        if (lw.endDateTime > lastDate) lastDate = lw.endDateTime;
                    }
                    else
                        _DataManager.addictionRepository.DeleteAddiction(_DataManager.addictionRepository.GetAddictionByProjectsId(lw.IdProject, id).Id);
                }
                //Создаём последовательности
                foreach (int sw in selectedWorks)
                {
                    Project project = _DataManager.projectRepository.GetProjectById(sw);
                    if (project.endDateTime > lastDate) lastDate = project.endDateTime;
                    _DataManager.addictionRepository.AddAddiction(sw, id);
                }
                //Сдвигаем даты
                if (lastDate != new DateTime(2000,1,1) && lastDate > start)
                {
                    TimeSpan d = lastDate - start;
                    start += d;
                    end += d;
                }

                Project forPeriodAmending = _DataManager.projectRepository.GetProjectById(id);
                _DataManager.addictionRepository.AmendPeriod(forPeriodAmending);
                Project p = _DataManager.projectRepository.EditProject(id, projectName, projectDescription, start, end, 0, 0);

                //Отправить уведомление об изменинии в работе
                MailSender sender = new MailSender();
                string email = _DataManager.personRepository.GetPersonById(team).email;
                var work = _DataManager.projectRepository.GetProjectById(id);
                //sender.SendChangeWork(email, work);
                sender.Send(new WorkChangedMail(email, work));

                if (p.parrentProject == null)
                    return RedirectToAction("Project", new { idProject = id });
                else
                    return RedirectToAction("Work", new { projectId = id });
            }


            GetWorks(id);
            GetProject(id);
            GetPath(id);
            GetTeam(id);
            var vm = new WorkViewModel(Convert.ToInt32(projectId));

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddWork(int projectId)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            GetPath(projectId);
            GetProject(projectId);
            GetMainTeams(projectId);
            var vm = new WorkViewModel(Convert.ToInt32(projectId));

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddWork(string projectId, string projectName, 
            string projectStart, string projectEnd, string projectDescription,
            Object selectedInlineWorks, int personId, string submit)
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
            Project project = _DataManager.projectRepository.GetProjectById(id);
            //if (start < project.startDateTime || start > project.endDateTime ||
            //    end < project.startDateTime || end > project.endDateTime)
            //{
            //    ModelState.AddModelError("ProjectLength", "Начало или конец работы выходят за пределы проекта");
            //}

            if (CheckPersonTime(start, end, Convert.ToInt32(personId), id)) ModelState.AddModelError("ProjectLength", "Исполнитель на это время уже занят в другой работе");

            if (CheckPersonTime(start, end, Convert.ToInt32(personId), id)) ModelState.AddModelError("ProjectLength", "Исполнитель на это время уже занят в другой работе");

            if (ModelState.IsValid)
            {
                var p = _DataManager.projectRepository.AddProject(projectName, projectDescription, start, end, 0, 0, id);
                _DataManager.teamRepository.AddTeam(Convert.ToInt32(personId),p.IdProject);
                if (p.parrentProject.parrentProject != null) _DataManager.teamRepository.DeleteTeam(_DataManager.teamRepository.GetTeamByWork(p.parrentProject.IdProject).IdTeam);
               
                //Отправить уведомление на почту исполнителя
                var mailSender = new MailSender();
                string personMail = _DataManager.personRepository.GetPersonById(personId).email;
                mailSender.Send(new NewWorkTemplate(personMail, p));

                if (p.parrentProject == null)
                    return RedirectToAction("Project", new { idProject = p.IdProject });
                else
                    return RedirectToAction("Work", new { projectId = p.IdProject });
            }
            
            GetPath(id);
            GetProject(id);
            GetMainTeams(id);
            var vm = new WorkViewModel(Convert.ToInt32(projectId));

            return View(vm);
        }
        #endregion
    }
}
