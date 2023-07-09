using Inzynierka.DAL;
using Inzynierka.Helpers;
using Microsoft.AspNetCore.Mvc;

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

        public void SetSessionPrivilages(string username, string privilages)
        {
            HttpContext.Session.SetString("Privilages", privilages);
            HttpContext.Session.SetString("Username", username);
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

        public string GetSessionUsername()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Privilages")))
            {
                return HttpContext.Session.GetString("Username");
            }
            else
            {
                return "";
            }
        }

        public void UnsetSessionPrivilages()
        {
            HttpContext.Session.Clear();
        }

    }
}
