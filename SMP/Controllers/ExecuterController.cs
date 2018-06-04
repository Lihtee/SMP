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
        // GET: Executer
        public ActionResult Index()
        {
            var vm = new ExecutorIndexViewModel(((Person)Session["user"]).IdPerson);
            return View(vm);
        }
        
        public ActionResult Work(string idWork)
        {
            var vm = new ExecutorWorkViewModel(Convert.ToInt32(idWork), ((Person)Session["user"]).IdPerson);
            return View(vm);
        }
        [HttpPost]
        public ActionResult FinishWork (string idWork, string action)
        {
            switch (action)
            {
                case "finish":
                    {
                        var vm = new ProjectRepository(new Models.ModelContainer());
                        vm.DoneProject(Convert.ToInt32(idWork));
                    }
                    break;
                case "back": break;
            }
            
            return RedirectToAction("Index");
        }
    }
}