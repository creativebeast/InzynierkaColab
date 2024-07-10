using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Inzynierka.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inzynierka.Controllers
{
    public class WorkerController : BaseController
    {
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(ILogger<WorkerController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult WorkerData()
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

            Company? company = Company.getCompanyByID(_context, companyId.Value);
            if (company == null)
            {
                TempData["Error"] = "Coś poszło nie tak, nie znaleziono firmy...";
                return RedirectToAction("Index", "Home");
            }

            List<Worker>? companyWorkers = Worker.GetWorkersByCompanyID(_context, companyId.Value);
            List<WorkerUsername>? workersUsername = companyWorkers == null ? null : Worker.GetWorkersUsernames(_context, companyWorkers);

            ViewData["Company"] = company;
            ViewData["Workers"] = workersUsername;

            return View();
        }


        public IActionResult AddWorker(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Manager))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = int.Parse(collection["companyId"].ToString() ?? "-1");
            if (companyId == -1)
            {
                TempData["Error"] = "Nie można znaleźć firmy, do której chcesz dodać pracownika...";
                return RedirectToAction("Index", "Home");
            }

            string registrationToken = TokenHelper.CreateReferalToken();
            if (!AuthToken.AddReferalToken(_context, registrationToken, companyId))
            {
                TempData["Error"] = "Nie udało się utworzyć kodu referencyjnego dla Twojej firmy...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Kod referencyjny dla Twojej firmy został pomyślnie utworzony - {registrationToken}";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UpdateWorker(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Manager))
            {
                return RedirectToAction("Login", "Home");
            };

            int companyId = int.Parse(collection["companyId"].ToString() ?? "-1");
            int workerId = int.Parse(collection["workerId"].ToString() ?? "-1");

            if (companyId == -1 || workerId == -1)
            {
                TempData["Error"] = "Nie można znaleźć firmy, z której chcesz usunąć pracownika...";
                return RedirectToAction("Index", "Home");
            }

            int workerPrivilages = int.Parse(collection["privilages"].ToString() ?? "-1");
            string? workerIdentity = collection["workerIdentity"];

            if (workerPrivilages == -1 || string.IsNullOrWhiteSpace(workerIdentity))
            {
                TempData["Error"] = "Niektóre pola zostały puste...";
                return RedirectToAction("Index", "Home");
            }

            Worker updatedWorker = new Worker()
            {
                ID = workerId,
                Privilages = workerPrivilages,
                WorkerId = workerIdentity
            };

            bool madeChangesInWorker;
            bool updateSuccess = Worker.UpdateCompanyWorker(_context, updatedWorker, companyId, out madeChangesInWorker);

            if (updateSuccess == false && madeChangesInWorker == false)
            {
                TempData["Error"] = "Nie można znaleźć docelowego pracownika do aktualizacji...";
                return RedirectToAction("Index", "Home");
            }
            else if (updateSuccess == true && madeChangesInWorker == false)
            {
                TempData["Error"] = "Nie znaleziono zmian między starymi a nowymi danymi...";
                return RedirectToAction("Index", "Home");
            }
            else if (updateSuccess == false && madeChangesInWorker == true)
            {
                TempData["Error"] = "Nie można zaktualizować docelowego...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = "Dane pracownika zaktualizowane";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeleteWorker(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Manager))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = int.Parse(collection["companyId"].ToString() ?? "-1");
            int workerId = int.Parse(collection["workerId"].ToString() ?? "-1");

            if (companyId == -1 || workerId == -1)
            {
                TempData["Error"] = "Nie można znaleźć firmy, z której chcesz usunąć pracownika...";
                return RedirectToAction("Index", "Home");
            }

            if (!Worker.DeleteWorkerFromCompany(_context, companyId, workerId))
            {
                TempData["Error"] = "Nie można usunąć pracownika z firmy...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = "Pracownik usunięty z firmy";
            return RedirectToAction("Index", "Home");
        }
    }
}
