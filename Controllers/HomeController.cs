using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inzynierka.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ProjectContext context) : base (context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            //User testUser = new User();
            //testUser.Phone = "123 123 123";
            //testUser.Username = "test user";
            //testUser.Email = "testEmail@email.test";
            //string password = "password123";
            //string refCode = "12345";
            //_sqlCommandsManager.CreateAccount(testUser, password, refCode);

            //User user = _sqlCommandsManager.CheckForUserLogin("test123", "123123");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}