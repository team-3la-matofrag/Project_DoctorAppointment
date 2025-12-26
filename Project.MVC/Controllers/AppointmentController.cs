using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class AppointmentController : BaseController
    {

        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }
        public IActionResult Book() => View();
        public IActionResult MyAppointments()
        {
            int userId = GetCurrentUserId();
          
            return View();
        }

    }
}
