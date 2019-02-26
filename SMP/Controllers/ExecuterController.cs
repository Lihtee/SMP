using SMP.Models;
using SMP.Models.Repositoryes;
using SMP.ViewModels;
using SMP.Views.VievModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SMP.Controllers
{
    public class ExecuterController : Controller
    {
        DataManager _DataManager;
        public ExecuterController (DataManager _DM)
        {
            _DataManager = _DM;
        }
        // GET: ExecutorName
        public ActionResult Index()
        {
            var vm = new ExecutorIndexViewModel(((Person)Session["user"]).IdPerson);
            return View(vm);
        }
        
        public ActionResult Work(string idWork)
        {
            int idPerson = ((Person)Session["user"]).IdPerson;
            int idwork = Convert.ToInt32(idWork);
            if (!WorkExists(idPerson, idwork))
            {
                //У исполнителя нет заявленной работы, возвращаем его на главную
                return RedirectToAction("Index");
            }
            var vm = new ExecutorWorkViewModel(idwork, idPerson);
            return View(vm);
        }
        [HttpPost]
        public ActionResult FinishWork (string idWork, string action)
        {
            switch (action)
            {
                case "finish":
                    {
                        int idPerson = ((Person)Session["user"]).IdPerson;
                        int idwork = Convert.ToInt32(idWork);
                        if (!WorkExists(idPerson, idwork))
                        {
                            //У исполнителя нет заявленной работы, возвращаем его на главную
                            break;
                        }
                        var vm = new ProjectRepository(new Models.ModelContainer());
                        vm.DoneProject(idwork);
                    }
                    break;
                case "back": break;//На главную.
            }
            return RedirectToAction("Index");
        }

        private bool WorkExists(int idPerson, int idWork)
        {
            return _DataManager.teamRepository.GetTeam(idPerson, idWork) != default(Team);
        }
    }
}