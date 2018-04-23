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

        public ActionResult Projects()
        {
            SortedList<int, Project> projects = new SortedList<int, Project>();

            foreach (var t in _DataManager.teamRepository.GetTeams())
            {
                if (t.Person.IdPerson == ((Person)Session["user"]).IdPerson 
                    && t.Project.parrentProject == null)
                    projects.Add(t.Project.IdProject, t.Project);
            }

            ViewData["projects"] = projects;

            return View();
        }

        // GET: Manager/Create
        [HttpGet]
        public ActionResult Project(int idProject)
        {
            ViewData.Model = _DataManager.projectRepository.GetProjectById(idProject);
            return View();
        }

        // POST: Manager/Create
        [HttpPost]
        public ActionResult Project(FormCollection collection)
        {
            
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        // GET: Manager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manager/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Manager/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Manager/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
