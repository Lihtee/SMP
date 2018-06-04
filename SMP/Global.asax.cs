using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SMP.Controllers;
using SMP.Models;
using SMP.Models.Repositoryes;

namespace SMP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FirstStart();
        }

        /// <summary>
        /// Проверяет наличие пользователей в системе
        /// </summary>
        private void FirstStart()
        {
            ModelContainer cont = new ModelContainer();

            PersonRepository personRepository = new PersonRepository(cont);
            
            if (cont.Person == null || cont.Person.Count() == 0)
            {
                personRepository.AddPerson("Менеджер", "Менеджер", "Менеджер",
                "Manager", "Manager", (int)Position.Менеджер);

                personRepository.AddPerson("Исполнитель", "Исполнитель", "Исполнитель",
                    "User", "User", (int)Position.Исполнитель);
            }
        }
    }
}
