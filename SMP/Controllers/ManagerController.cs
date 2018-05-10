using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMP.Models;

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
            if (((Person)Session["user"]) == null)
            {
                return false;
            }

            switch (((Person)Session["user"]).Position)
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
            List<Project> projects = new List<Project>();

            foreach (var t in _DataManager.teamRepository.GetTeamsByPerson(((Person)Session["user"]).IdPerson))
            {
                if (t.Project.parrentProject == null)
                    projects.Add(t.Project);
            }

            ViewData["projects"] = projects;

            return View();
        }

        public ActionResult DeleteProject(int idProject)
        {
            _DataManager.projectRepository.DeleteProject(idProject);

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

            return View();
        }
        
        [HttpPost]
        public ActionResult Project(string projectId, string projectName, string projectStart, string projectEnd, string projectDescription)
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
                ModelState.AddModelError("ProjectStart", "Дата начала должна быть до даты окончания");
                ModelState.AddModelError("ProjectEnd", "Дата окончания должна быть после даты начала");
            }

            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(projectId);
                _DataManager.projectRepository.EditProject(id, projectName, projectDescription, start, end, 0);
                GetProject(id);
                GetPersons(id);
                GetWorks(id);
                return RedirectToAction("Project", new { idProject = id });
            }

            return View();
        }

        public ActionResult DeleteTeam(int teamId, int projectId)
        {
            _DataManager.teamRepository.DeleteTeam(teamId);

            return RedirectToAction("Project", new { idProject = projectId });
        }

        public ActionResult AddTeam(int personId, int projectId)
        {
            _DataManager.teamRepository.AddTeam(personId, projectId);

            return RedirectToAction("Project", new { idProject = projectId });
        }
        
        public ActionResult DeleteWork(int workId, int projectId)
        {
            _DataManager.projectRepository.DeleteProject(workId);

            return RedirectToAction("Project", new { idProject = projectId });
        }
        #endregion

        #region Add Project
        [HttpGet]
        public ActionResult AddProjectFirstStep()
        {
            if (!AccessControll()) return RedirectToAction("AccesError");
            return View();
        }

        [HttpPost]
        public ActionResult AddProjectFirstStep(string projectName, string projectStart, string projectEnd, string projectDescription, string submit)
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
                ModelState.AddModelError("ProjectStart", "Дата начала должна быть до даты окончания");
                ModelState.AddModelError("ProjectEnd", "Дата окончания должна быть после даты начала");
            }

            if (ModelState.IsValid)
            {
                Project p = _DataManager.projectRepository.AddProject(projectName, projectDescription, start, end, 0);
                _DataManager.teamRepository.AddTeam(((Person)Session["user"]).IdPerson, p.IdProject);
                switch (submit)
                {
                    case "Сохранить изменения":
                        return RedirectToAction("Projects");
                        break;
                    case "К шагу 2":
                        return RedirectToAction("AddProjectSecondStep",routeValues: new { projectId = p.IdProject });
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

            return RedirectToAction("AddProjectSecondStep", new { projectId = projectId });
        }

        public ActionResult AddTeamSecondStep(int personId, int projectId)
        {
            _DataManager.teamRepository.AddTeam(personId, projectId);

            return RedirectToAction("AddProjectSecondStep", new { projectId = projectId });
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
            List<Team> works = _DataManager.teamRepository.GetTeamsByParrentProject(idProject);
            ViewData["works"] = works;
        }
        #endregion

        #region Works
        [HttpGet]
        public ActionResult Work(int projectId)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            GetProject(projectId);

            SortedList<int, string> sl = new SortedList<int, string>();
            foreach (Team t in _DataManager.teamRepository.GetTeamsByProject(_DataManager.projectRepository.GetProjectById(projectId).parrentProject.IdProject))
            {
                if (t.Person.Position == Position.Исполнитель)
                    sl.Add(t.Person.IdPerson, t.Person.firstName + ' ' + t.Person.surName);
            }
            ViewData["Team"] = new SelectList(sl, "Key", "Value");

            return View();
        }

        [HttpPost]
        public ActionResult Work(string projectId, string projectName, string projectStart, string projectEnd, string projectDescription, int personId, string submit)
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
                ModelState.AddModelError("ProjectStart", "Дата начала должна быть до даты окончания");
                ModelState.AddModelError("ProjectEnd", "Дата окончания должна быть после даты начала");
            }

            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(projectId);
                _DataManager.projectRepository.EditProject(id, projectName, projectDescription, start, end, 0);
                //GetProject(id);
                //GetPersons(id);
                //GetWorks(id);
                return RedirectToAction("Project", new { idProject = id });
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddWork(int projectId)
        {
            if (!AccessControll()) return RedirectToAction("AccesError");

            GetProject(projectId);

            SortedList<int, string> sl = new SortedList<int, string>();
            foreach (Team t in _DataManager.teamRepository.GetTeamsByProject(projectId))
            {
                if (t.Person.Position == Position.Исполнитель)
                    sl.Add(t.Person.IdPerson, t.Person.firstName + ' ' + t.Person.surName);
            }
            ViewData["Team"] = new SelectList(sl, "Key", "Value");

            return View();
        }

        [HttpPost]
        public ActionResult AddWork(string projectId, string projectName, string projectStart, string projectEnd, string projectDescription, int personId, string submit)
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
                ModelState.AddModelError("ProjectStart", "Дата начала должна быть до даты окончания");
                ModelState.AddModelError("ProjectEnd", "Дата окончания должна быть после даты начала");
            }

            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(projectId);
                var p = _DataManager.projectRepository.AddProject(projectName, projectDescription, start, end, 0, id);
                _DataManager.teamRepository.AddTeam(Convert.ToInt32(personId),p.IdProject);

                return RedirectToAction("Project", new { idProject = id });
            }

            return View();
        }
        #endregion
    }
}
