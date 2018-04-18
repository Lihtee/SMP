using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMP.Models;

namespace SMP.Controllers
{
    public class HomeController : Controller
    {
        private DataManager _DataManager;

        public HomeController(DataManager _DM)
        {
            _DataManager = _DM;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Login window
        /// <summary>
        /// Открывает окно входа
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Проверяет коректность введённых данных и если они верны, выполняет вход, иначе выполняет ошибку
        /// </summary>
        /// <param name="login">Строка с введённым логином</param>
        /// <param name="password">Строка с введённым паролем</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                ModelState.AddModelError("Login", "Введите логин");

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("Password", "Введите пароль");

            var persons = (from g in _DataManager.personRepository.GetPersons()
                             where (g.login == login)
                             select g);

            Person person = new Person();

            if (persons.Count() == 0)
                ModelState.AddModelError("Login", "Пользователя с таким логином нету");
            else
            {
                person = persons.ElementAt(0);

                if (person.password != password.GetHashCode().ToString())
                    ModelState.AddModelError("Password", "Введён не верный пароль");
            }

            if (ModelState.IsValid)
            {
                Session["user"] = person;
                return RedirectToAction("Login");
            }

            return View();
        }

        /// <summary>
        /// Выполняет выход из системы
        /// </summary>
        /// <returns></returns>
        public ActionResult Exit()
        {
            Session.Remove("user");
            return RedirectToAction("Login");
        }
        #endregion
    }
}