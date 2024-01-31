using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult ClientData(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = int.Parse(collection["CompanyId"]);
            Company? targetCompany = null;

            if (GetSessionPrivilages() == 0)
                targetCompany = Company.getCompaniesRelatedToWorker(_context, GetSessionUserID())?.FirstOrDefault(c => c.ID == companyId);
            else if(GetSessionPrivilages() == 2)
                targetCompany = Company.getCompaniesRelatedToOwner(_context, GetSessionUserID())?.FirstOrDefault(c => c.ID == companyId);
            
            if(targetCompany == null)
            {
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Company"] = targetCompany;

            List<Client>? avalibleClients = Client.GetClientsRelatedToCompany(_context, companyId);

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
                    TempData["Error"] = $"Field {item.Key} was left empty...";
                    return RedirectToAction("Index", "Home");
                }
            }

            int relatedCompanyId = int.Parse(collection["companyId"].ToString());

            if (!Client.CreateNewClient(_context, collection, relatedCompanyId, out string companyName))
            {
                TempData["Error"] = $"Couldn't create new company...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Added new Client - {companyName}";
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
                TempData["Error"] = $"Couldn't update client...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Client Updated";
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
            if(!Client.DeleteTargetCompany(_context, clientID, companyID, GetSessionUserID()))
            {
                TempData["Error"] = $"Couldn't delete client...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Client Deleted";
            return RedirectToAction("Index", "Home");
        }
    }
}
