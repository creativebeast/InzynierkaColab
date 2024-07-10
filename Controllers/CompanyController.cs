﻿using Inzynierka.DAL;
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
        public IActionResult CompanyData()
        {
            string companyIdString = HttpContext.Session.GetString("selectedCompany") ?? string.Empty;
            int? companyId = null;

            if (int.TryParse(companyIdString, out int parsedCompanyId))
            {
                companyId = parsedCompanyId;
            }

            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            if (!companyId.HasValue)
            {
                TempData["Error"] = "Coś poszło nie tak, nie znaleziono firmy...";
                return RedirectToAction("ChangeCompany", "Home");
            }

            Company? ownerCompany = Company.getCompanyByOwnerID(_context, GetSessionUserID(), companyId.Value);
            if (ownerCompany == null)
            {
                TempData["Error"] = "Coś poszło nie tak, nie znaleziono firmy...";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Company"] = ownerCompany;

            return View();
        }

        public IActionResult CreateCompany()
        {
            if (!CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }
        public IActionResult CreateNewCompany(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Login", "Home");
            }

            foreach (var item in collection)
            {
                if (item.Key == "companyProvince" || item.Key == "companyId")
                    continue;

                if (String.IsNullOrEmpty(item.Value))
                {
                    TempData["Error"] = $"Pole {item.Key} pozostało puste...";
                    return RedirectToAction("Index", "Home");
                }
            }

            if (!Company.CreateNewCompany(_context, collection, GetSessionUserID(), out string companyName))
            {
                TempData["Error"] = $"Nie udało się utworzyć nowej firmy...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Utworzono nową firmę - {companyName}";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UpdateCompany(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
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
                TempData["Error"] = $"Nie udało się znaleźć docelowej firmy...";
                return RedirectToAction("Index", "Home");
            }

            int companyId = int.Parse(collection["companyId"].ToString());

            if (!Company.UpdateCompanyData(_context, changedFields, GetSessionUserID(), companyId))
            {
                TempData["Error"] = $"Nie udało się wprowadzić zmian w istniejącej firmie...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Zaktualizowano dane firmy!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteCompany(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Owner))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = collection["companyId"].ToString() != null ? int.Parse(collection["companyId"].ToString()) : -1;
            if (companyId == -1)
            {
                TempData["Error"] = $"Nie udało się znaleźć docelowej firmy...";
                return RedirectToAction("Index", "Home");
            }

            if (!Company.DeleteCompanyByID(_context, GetSessionUserID(), companyId))
            {
                TempData["Error"] = $"Nie udało się zakończyć procesu usuwania...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Usunięto firmę";
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
