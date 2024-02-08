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

        public IActionResult WorkerData(IFormCollection collection)
        {
            int companyId = int.Parse(collection["CompanyId"]);

            Company? company = Company.getCompanyByID(_context, companyId);
            if(company == null)
            {
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("Index", "Home");
            }

            List<Worker>? companyWorkers = Worker.GetWorkersByCompanyID(_context, companyId);
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
            if(companyId == -1)
            {
                TempData["Error"] = "Couldn't find company to add worker to...";
                return RedirectToAction("Index", "Home");
            }

            string registrationToken = TokenHelper.CreateReferalToken();
            if(!AuthToken.AddReferalToken(_context, registrationToken, companyId))
            {
                TempData["Error"] = "Couldn't create referal token for your company...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = $"Referal code for your company successfully created - {registrationToken}";
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
                TempData["Error"] = "Couldn't find company to delete worker from...";
                return RedirectToAction("Index", "Home");
            }

            int workerPrivilages = int.Parse(collection["privilages"].ToString() ?? "-1");
            string? workerIdentity = collection["workerIdentity"];

            if(workerPrivilages == -1 || String.IsNullOrWhiteSpace(workerIdentity))
            {
                TempData["Error"] = "Some of the fields were left empty...";
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

            if(updateSuccess == false && madeChangesInWorker == false)
            {
                TempData["Error"] = "Couldn't find target worker to update...";
                return RedirectToAction("Index", "Home");
            } 
            else if (updateSuccess == true && madeChangesInWorker == false)
            {
                TempData["Error"] = "No changes found between old and new data...";
                return RedirectToAction("Index", "Home");
            } 
            else if (updateSuccess == false && madeChangesInWorker == true)
            {
                TempData["Error"] = "Couldn't update target...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = "Worker data updated";
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

            if(companyId == -1 || workerId == -1)
            {
                TempData["Error"] = "Couldn't find company to delete worker from...";
                return RedirectToAction("Index", "Home");
            }

            if (!Worker.DeleteWorkerFromCompany(_context, companyId, workerId))
            {
                TempData["Error"] = "Couldn't delete worker from company...";
                return RedirectToAction("Index", "Home");
            }

            TempData["Success"] = "Worker removed from company";
            return RedirectToAction("Index", "Home");
        }
    }
}
