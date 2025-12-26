using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.Helpers;


namespace Project.MVC.Controllers
{
    public abstract class BaseController : Controller
    {
        protected UserSession? CurrentUser
            => HttpContext.Session.GetObject<UserSession>("CurrentUser");

        protected bool IsAuthenticated()
        {
            return CurrentUser != null;
        }

        protected int GetCurrentUserId()
        {
            if (!IsAuthenticated())
                throw new UnauthorizedAccessException("User not logged in");

            return CurrentUser!.UserId;
        }

        protected string GetCurrentUserRole()
        {
            if (!IsAuthenticated())
                throw new UnauthorizedAccessException("User not logged in");

            return CurrentUser!.Role;
        }
    }
}
