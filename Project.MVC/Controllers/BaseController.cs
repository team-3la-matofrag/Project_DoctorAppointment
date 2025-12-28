using Microsoft.AspNetCore.Mvc;
using Project.MVC.Models;
using Project.MVC.Helpers;

namespace Project.MVC.Controllers
{
    public abstract class BaseController : Controller
    {
       
        protected UserSession? CurrentUser
        {
            get
            {
                return HttpContext.Session.GetObject<UserSession>("CurrentUser");
            }
        }

        
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

        
        protected bool IsAdmin()
        {
            return IsAuthenticated() && CurrentUser!.Role == "Admin";
        }

        
        protected bool IsDoctor()
        {
            return IsAuthenticated() && CurrentUser!.Role == "Doctor";
        }

        
        protected bool IsPatient()
        {
            return IsAuthenticated() && CurrentUser!.Role == "Patient";
        }
    }
}