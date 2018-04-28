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
        public ActionResult Index(string idPerson)
        {
            var vm = new ExecutorIndexViewModel(Convert.ToInt32(idPerson));
            return View(vm);
        }
        
        public ActionResult Work(string idWork, string idPerson)
        {
            var vm = new ExecutorWorkViewModel(Convert.ToInt32(idWork), Convert.ToInt32(idPerson));
            return View(vm);
        }
        [HttpPost]
        public ActionResult FinishWork (string idWork, string idPerson, string action)
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
            
            return RedirectToAction("Index", new { idPerson = idPerson });
        }
    }
}