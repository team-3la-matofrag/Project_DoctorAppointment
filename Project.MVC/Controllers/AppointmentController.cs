using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Book() => View();
    }
}
