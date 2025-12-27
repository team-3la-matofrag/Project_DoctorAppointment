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

        public IActionResult Book()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }
        public IActionResult MyAppointments()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");


            int userId = GetCurrentUserId();
            return View();

        }
      
    }
}
