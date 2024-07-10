using Inzynierka.DAL;
using Inzynierka.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Collections;

namespace Inzynierka.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ProjectContext _context;
        protected SqlCommandsManager _sqlCommandsManager;

        protected BaseController(ProjectContext context)
        {
            this._context = context;
            this._sqlCommandsManager = new SqlCommandsManager(_context);
        }

        public void SetSessionPrivilages(string username, string privilages, string userId)
        {
            HttpContext.Session.SetString("Privilages", privilages);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("UserID", userId);
        }

        public int GetSessionPrivilages()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Privilages")))
            {
                return int.Parse(HttpContext.Session.GetString("Privilages"));
            }
            else
            {
                return -1;
            }
        }

        public int GetSessionUserID()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserID")))
            {
                return int.Parse(HttpContext.Session.GetString("UserID"));
            }
            else
            {
                return -1;
            }
        }

        public string GetSessionUsername()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Username")))
            {
                return HttpContext.Session.GetString("Username");
            }
            else
            {
                return "";
            }
        }

        public void UnsetSession()
        {
            HttpContext.Session.Clear();
        }

        public void CreateErrorMessage(string message, bool unsetSession)
        {
            if (unsetSession)
                UnsetSession();

            TempData["Error"] = message;
        }

        public bool CheckPrivilages(Privilages neededPrivilages)
        {
            int sessionPrivilages = GetSessionPrivilages();
            if(sessionPrivilages >= (int)neededPrivilages)
            {
                return true;
            } else
            {
                TempData["error"] = "Niewystarczające uprawnienia...";
                return false;
            }
        }
        
        public enum Privilages
        {
            Guest = -1,
            Worker = 0,
            Manager = 1,
            Owner = 2,
            Admin = 3

        }
    }
}
