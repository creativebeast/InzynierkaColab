using Inzynierka.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Inzynierka.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public CompanyController(ILogger<HomeController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CompanyData()
        {
            return View();
        }

        public IActionResult Client()
        {
            return View();
        }

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult Worker()
        {
            return View();
        }

        public IActionResult Logs()
        {
            return View();
        }
    }
}
