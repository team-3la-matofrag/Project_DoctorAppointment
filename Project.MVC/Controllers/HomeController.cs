using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using System.Diagnostics;

namespace Project.MVC.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult Services()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult Doctors()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }
        public IActionResult Departments()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }


        public IActionResult About() => View();

        public IActionResult Contact() => View();
    }
}
