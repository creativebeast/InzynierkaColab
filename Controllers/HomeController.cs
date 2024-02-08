using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;

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
            List<Company> companies = new List<Company>();

            int privilages = GetSessionPrivilages();
            if(privilages == 0)
                companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());
            else if(privilages == 2)
                companies = Company.getCompaniesRelatedToOwner(_context, GetSessionUserID());

            ViewData["Companies"] = companies;
            ViewData["Privilages"] = GetSessionPrivilages();
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

            return View("Login");
        }

        public IActionResult Login()
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

        public IActionResult UpdateStyling()
        {
            StylingManager stylingManager = new StylingManager();

            return View("StylingSettingsTest", stylingManager);
        }

        public IActionResult CreateUserStyling(StylingManager stylings)
        {
            //XElement stylingFile = XMLManager.CreateStyling(stylings);]
            XElement textStyling = XMLManager.CreateSingleStyling(stylings._textStylingKeys, stylings._textStylingValues, "text-styling");
            XElement tableStyling = XMLManager.CreateSingleStyling(stylings._tableStylingKeys, stylings._tableStylingValues, "table-styling");
            XElement specialStyling = XMLManager.CreateSingleStyling(new string[] {"test"}, new string[] {"testValue"}, "special-styling");
            XElement completeStyling = XMLManager.JoinMulitpleStyles(new XElement[] { textStyling, tableStyling, specialStyling });

            string creatorName = GetSessionUsername();
            int userId = GetSessionUserID();
            if(creatorName == null)
            {
                TempData["error"] = "Error: Couldn't find session data";
                return RedirectToAction("Index");
            }

            string referenceToken = DateTime.Now.ToShortDateString().ToString() + DateTime.Now.TimeOfDay.ToString();
            referenceToken += TokenHelper.CreateNumericToken(30);
            var charsToRemove = new string[] { "@", ",", ".", ";", "'", ":" };
            foreach (var c in charsToRemove)
            {
                referenceToken = referenceToken.Replace(c, string.Empty);
            }

            if(_sqlCommandsManager.CreateStyling(textStyling, tableStyling, specialStyling, creatorName, userId, stylings.stylingName, referenceToken) == 0)
            {
                TempData["error"] = "Couldn't add stylings to the db";
                return RedirectToAction("Index");
            } 
            else
                return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            UnsetSession();
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}