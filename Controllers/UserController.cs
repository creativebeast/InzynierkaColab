using Microsoft.AspNetCore.Mvc;
using Inzynierka.DAL;

namespace Inzynierka.Controllers
{
    public class UserController : BaseController
    {
        protected UserController(ProjectContext context) : base(context)
        {
        }

        public IActionResult Index()
        {

            return View();
        }
    }
}
