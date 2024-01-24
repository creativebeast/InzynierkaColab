using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

        public IActionResult CompanyData(IFormCollection collection)
        {
            if (CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Index", "Home");
            }

            int companyId = int.Parse(collection["CompanyId"]);

            Company? ownerCompany = Company.getCompanyByOwnerID(_context, GetSessionUserID(), companyId);

            if(ownerCompany == null)
            {
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Company"] = ownerCompany;

            return View();
        }

        public IActionResult CreateCompany()
        {
            if (CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        public IActionResult CreateNewCompany(IFormCollection collection)
        {
            if (CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var item in collection)
            {
                if (item.Key == "companyProvince" || item.Key == "companyId")
                    continue;

                if (String.IsNullOrEmpty(item.Value))
                {
                    TempData["Error"] = $"Field {item.Key} was left empty...";
                    return RedirectToAction("Index", "Home");
                }
            }

            if(!Company.CreateNewCompany(_context, collection, GetSessionUserID(), out string companyName))
            {
                TempData["Error"] = $"Couldn't create new company...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Created new company - {companyName}";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UpdateCompany(IFormCollection collection)
        {
            if (CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Index", "Home");
            }

            Dictionary<string, string> changedFields = new Dictionary<string, string>();
            foreach (var item in collection)
            {
                if (item.Key == "companyProvince" || item.Key == "companyId")
                    continue;

                if (!String.IsNullOrWhiteSpace(item.Value))
                    changedFields.Add(item.Key.Replace("company", String.Empty), item.Value);
            }

            if (String.IsNullOrEmpty(collection["companyId"]))
            {
                TempData["Error"] = $"Couldn't find target company...";
                return RedirectToAction("Index", "Home");
            }

            int companyId = int.Parse(collection["companyId"].ToString());

            if (!Company.UpdateCompanyData(_context, changedFields, GetSessionUserID(), companyId))
            {
                TempData["Error"] = $"Couldn't make changes to existing company...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Company Data Updated!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteCompany(IFormCollection collection)
        {
            if (CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Index", "Home");
            }

            int companyId = collection["companyId"].ToString() != null ? int.Parse(collection["companyId"].ToString()) : -1;
            if(companyId == -1)
            {
                TempData["Error"] = $"Couldn't find target company...";
                return RedirectToAction("Index", "Home");
            }

            if(!Company.DeleteCompanyByID(_context, GetSessionUserID(), companyId))
            {
                TempData["Error"] = $"Couldn't complete deletion process...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Company Deleted";
            return RedirectToAction("Index", "Home");
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
