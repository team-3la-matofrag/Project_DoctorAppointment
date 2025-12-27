using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using System.Diagnostics;

namespace Project.MVC.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Doctors()
        {
            return View();
        }

        public IActionResult Departments()
        {
            return View();
        }

        public IActionResult About() => View();

        public IActionResult Contact() => View();
    }
}