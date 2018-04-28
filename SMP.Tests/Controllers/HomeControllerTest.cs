using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMP;
using SMP.Controllers;
using SMP.Models;
using SMP.Models.Repositoryes;

namespace SMP.Tests.Controllers
{

    [TestClass]
    public class HomeControllerTest
    {
        DataManager _DataManager = new DataManager();

        [TestMethod]
        public void HK_Index()
        {
            // Arrange
            HomeController controller = new HomeController(_DataManager);
            //controller.Login("mrak0", "-41389077");

            // Act-
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HK_Abuot()
        {
            // Arrange
            HomeController controller = new HomeController(_DataManager);

            // Act-
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HK_Contact()
        {
            // Arrange
            HomeController controller = new HomeController(_DataManager);

            // Act-
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HK_LoginPage()
        {
            // Arrange

            HomeController controller = new HomeController(_DataManager);

            // Act-
            ViewResult asset = controller.Login() as ViewResult;

            // Assert
            Assert.IsNotNull(asset);
        }

        [TestMethod]
        public void HK_Login()
        {
            //_DataManager = new DataManager();
            //_DataManager.personRepository.AddPerson("Алексей", "Кожин", "Дмитриевич", "mrak0", "-41389077", 0);

            // Arrange
            HomeController controller = new HomeController(_DataManager);

            // Act
            ViewResult asset = controller.Login() as ViewResult;
            ViewResult result = controller.Login("mrak0", "-41389077") as ViewResult;

            // Assert
            Assert.AreNotEqual(asset, result);
        }

        [TestMethod]
        public void HK_Exit()
        {
            // Arrange
            HomeController controller = new HomeController(_DataManager);

            // Act
            ViewResult asset = controller.Login() as ViewResult;
            ViewResult result = controller.Exit() as ViewResult;

            // Assert
            Assert.AreEqual(asset, result);
        }
    }
}
