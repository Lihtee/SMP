﻿using System;
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
            if (((Person)Session["user"]) != null)
            {
                if (((Person)Session["user"]).Position == Position.Менеджер)
                {
                    return RedirectToAction("Projects", "Manager");
                }
                if (((Person)Session["user"]).Position == Position.Исполнитель)
                {
                    return RedirectToAction("Index", "Executer");
                }
            }
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
                if (person.Position == Position.Менеджер)
                    return RedirectToAction(actionName: "Projects", controllerName: "Manager");
                if (person.Position == Position.Исполнитель)
                    return RedirectToAction(actionName: "Index", controllerName: "Executer");
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