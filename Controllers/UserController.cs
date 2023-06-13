using Microsoft.AspNetCore.Mvc;
using Inzynierka.DAL;

namespace Inzynierka.Controllers
{
    public class UserController : Controller
    {
        private ProjectContext db = new ProjectContext();

        public IActionResult Index()
        {
            return View();
        }
    }
}
