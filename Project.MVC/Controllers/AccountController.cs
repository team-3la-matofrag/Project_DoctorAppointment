using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.Services;


namespace Project.MVC.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }


        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // LOGIN 

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var response = await _api.PostAsync<LoginViewModel, UserSession>(
                    "/api/auth/login", model);

                _api.SetCurrentUser(response);

                TempData["Success"] = $"Welcome {response.FullName}";
                _api.SetCurrentUser(response);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["Error"] = "Invalid email or password";
                return RedirectToAction("Login");
            }
        }


        // REGISTER 

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _api.PostAsync<RegisterViewModel, UserSession>(
                    "/api/auth/register", model);

                _api.SetCurrentUser(user);

                TempData["Success"] = "Account created successfully";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Register");
            }
        }

        
        //  LOGOUT 

        public IActionResult Logout()
        {
            _api.ClearSession();
            TempData["Success"] = "Logged out successfully";
            return RedirectToAction("Login");
        }
    }
}
