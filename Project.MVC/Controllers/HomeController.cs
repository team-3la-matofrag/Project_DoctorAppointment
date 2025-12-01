using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using System.Diagnostics;

namespace Project.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Services() => View();

        public IActionResult Departments() => View();

        public IActionResult Doctors() => View();

        public IActionResult Contact() => View();
    }
}
