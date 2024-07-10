using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Inzynierka.Controllers
{
    public class ClientController : BaseController
    {
        private readonly ILogger<ClientController> _logger;

        public ClientController(ILogger<ClientController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult ClientData()
        {
            string companyIdString = HttpContext.Session.GetString("selectedCompany") ?? string.Empty;
            int? companyId = null;

            if (int.TryParse(companyIdString, out int parsedCompanyId))
            {
                companyId = parsedCompanyId;
            }

            User currentUser = Inzynierka.Models.User.GetUserById(_context, GetSessionUserID());
            if (currentUser == null)
            {
                TempData["error"] = "Niewystarczające uprawnienia...";
                return RedirectToAction("Login", "Home");
            }

            if (!companyId.HasValue)
            {
                TempData["Error"] = "Coś poszło nie tak, nie znaleziono firmy...";
                return RedirectToAction("ChangeCompany", "Home");
            }

            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            Company? targetCompany = null;

            if (GetSessionPrivilages() == 0)
                targetCompany = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID())?.FirstOrDefault(c => c.ID == companyId);
            else if (GetSessionPrivilages() == 2 || GetSessionPrivilages() == 3)
                targetCompany = Company.getCompaniesRelatedToOwner(_context, GetSessionUserID())?.FirstOrDefault(c => c.ID == companyId);

            if (targetCompany == null)
            {
                TempData["Error"] = "Coś poszło nie tak, nie znaleziono firmy...";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Company"] = targetCompany;

            List<Client>? avalibleClients = Client.GetClientsRelatedToCompany(_context, companyId.Value);

            ViewData["Clients"] = avalibleClients;

            return View();
        }


        public IActionResult CreateClient(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            foreach (var item in collection)
            {
                if (item.Key == "clientProvince" || item.Key == "companyId")
                    continue;

                if (String.IsNullOrEmpty(item.Value))
                {
                    TempData["Error"] = $"Pole {item.Key} pozostało puste...";
                    return RedirectToAction("Index", "Home");
                }
            }

            int relatedCompanyId = int.Parse(collection["companyId"].ToString());

            if (!Client.CreateNewClient(_context, collection, relatedCompanyId, out string companyName))
            {
                TempData["Error"] = $"Nie udało się utworzyć nowego klienta...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Dodano nowego klienta - {companyName}";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UpdateClient(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            Dictionary<string, string> changedFields = new Dictionary<string, string>();
            foreach (var item in collection)
            {
                if (item.Key == "clientProvince" || item.Key == "companyID" || item.Key == "clientID")
                    continue;

                if (!String.IsNullOrWhiteSpace(item.Value))
                    changedFields.Add(item.Key.Replace("client", String.Empty), item.Value);
            }

            int clientID = int.Parse(collection["clientID"].ToString());
            int companyID = int.Parse(collection["companyID"].ToString());

            if (!Client.UpdateTargetCompany(_context, changedFields, clientID, companyID))
            {
                TempData["Error"] = $"Nie udało się zaktualizować klienta...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Zaktualizowano klienta";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteClient(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int clientID = int.Parse(collection["ClientID"].ToString());
            int companyID = int.Parse(collection["CompanyID"].ToString());
            if (!Client.DeleteTargetCompany(_context, clientID, companyID, GetSessionUserID()))
            {
                TempData["Error"] = $"Nie udało się usunąć klienta...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Usunięto klienta";
            return RedirectToAction("Index", "Home");
        }
    }
}
