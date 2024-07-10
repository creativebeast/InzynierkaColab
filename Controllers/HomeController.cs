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

        public HomeController(ILogger<HomeController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            User currentUser = Inzynierka.Models.User.GetUserById(_context, GetSessionUserID());
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Home");
            }

            List<Company> companies = new List<Company>();

            int privilages = GetSessionPrivilages();
            if (privilages == 0)
                companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());
            else if (privilages == 2 || privilages == 3)
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

        public IActionResult ChangeCompany()
        {
            User currentUser = Inzynierka.Models.User.GetUserById(_context, GetSessionUserID());
            if (currentUser == null)
            {
                TempData["error"] = "Niewystarczające uprawnienia...";
                return RedirectToAction("Login", "Home");
            }
            ViewData["User"] = currentUser;

            List<Company> companies = new List<Company>();

            int privilages = GetSessionPrivilages();
            if (privilages == 0)
                companies = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID());
            else if (privilages == 2 || privilages == 3)
                companies = Company.getCompaniesRelatedToOwner(_context, GetSessionUserID());

            ViewData["Companies"] = companies;
            return View();
        }

        public IActionResult SaveCompanyChange(IFormCollection? collection)
        {
            foreach (var item in collection)
            {
                if (String.IsNullOrEmpty(item.Value.ToString()))
                {
                    CreateErrorMessage("Proszę wybrać firmę", false);
                    return RedirectToAction("ChangeCompany", "Home");
                }

            }

            string company = collection["Companies"];
            if (company != null)
            {
                HttpContext.Session.SetString("selectedCompany", company);

                TempData["Success"] = "Wybrana firma została zapisana pomyślnie!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = "Nie udało się zmienić wybranej firmy";
                return RedirectToAction("ChangeCompany", "Home");
            }
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
            XElement specialStyling = XMLManager.CreateSingleStyling(new string[] { "test" }, new string[] { "testValue" }, "special-styling");
            XElement completeStyling = XMLManager.JoinMulitpleStyles(new XElement[] { textStyling, tableStyling, specialStyling });

            string creatorName = GetSessionUsername();
            int userId = GetSessionUserID();
            if (creatorName == null)
            {
                TempData["error"] = "Błąd: Nie udało się znaleźć danych sesji";
                return RedirectToAction("Index");
            }

            string referenceToken = DateTime.Now.ToShortDateString().ToString() + DateTime.Now.TimeOfDay.ToString();
            referenceToken += TokenHelper.CreateNumericToken(30);
            var charsToRemove = new string[] { "@", ",", ".", ";", "'", ":" };
            foreach (var c in charsToRemove)
            {
                referenceToken = referenceToken.Replace(c, string.Empty);
            }

            if (_sqlCommandsManager.CreateStyling(textStyling, tableStyling, specialStyling, creatorName, userId, stylings.stylingName, referenceToken) == 0)
            {
                TempData["error"] = "Nie udało się dodać stylizacji do bazy danych";
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }

        public IActionResult ChangeLayoutColor(IFormCollection collection)
        {
            string footersColor, panelsColor, textsColor, backgroundsColor;
            footersColor = panelsColor = textsColor = backgroundsColor = string.Empty;
            foreach (var item in collection)
            {
                switch (item.Key)
                {
                    case "footersColor": { footersColor = item.Value; break; }
                    case "panelsColor": { panelsColor = item.Value; break; }
                    case "textsColor": { textsColor = item.Value; break; }
                    case "backgroundsColor": { backgroundsColor = item.Value; break; }
                }
            }
            HttpContext.Session.SetString("footersColor", footersColor);
            HttpContext.Session.SetString("panelsColor", panelsColor);
            HttpContext.Session.SetString("textsColor", textsColor);
            HttpContext.Session.SetString("backgroundsColor", backgroundsColor);

            TempData["Success"] = "Stylizacja układu została zapisana pomyślnie!";
            return RedirectToAction("Index", "Home");
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
