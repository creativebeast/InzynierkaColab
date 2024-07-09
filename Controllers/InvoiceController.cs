using Inzynierka.DAL;
using Inzynierka.Helpers;
using Inzynierka.Models;
using Inzynierka.Models.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using QuestPDF.ExampleInvoice;
using QuestPDF.Fluent;
using System.Diagnostics;

namespace Inzynierka.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult InvoiceData()
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
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("ChangeCompany", "Home");
            }

            Company? company = Company.getCompanyByID(_context, companyId.Value);
            ViewData["Company"] = company;

            return View();
        }

        public IActionResult AddInvoice()
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
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("ChangeCompany", "Home");
            }

            int queryCompanyId = companyId.Value;
            Company? company = Company.getCompanyByID(_context, queryCompanyId);
            ViewData["Company"] = company;

            if (company == null)
            {
                TempData["Error"] = "Couldn't find your company...";
                return RedirectToAction("Index", "Home");
            }

            List<Client>? clients = Client.GetClientsRelatedToCompany(_context, queryCompanyId);
            ViewData["Clients"] = clients;

            return View(new List<ProductTemplate>());
        }

        public IActionResult CreateInvoice(IFormCollection collection, List<ProductTemplate> products)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = !String.IsNullOrEmpty(collection["companyId"]) ? int.Parse(collection["companyId"]) : -1;
            Invoice? newInvoice;
            //Path for existing Client
            if (collection["ClientData"].ToString() != "-1")
            {
                List<Client>? clients = Client.GetClientsRelatedToCompany(_context, companyID);
                if(clients == null || clients.Count == 0)
                {
                    TempData["Error"] = "Couldn't find client data...";
                    return RedirectToAction("Index", "Home");
                }

                int clientID = int.Parse(collection["clientId"]);
                Client? targetClient = Client.GetClientByID(_context, clientID);
                Company? seller = Company.getCompanyByID(_context, companyID);
                if(targetClient == null || seller == null)
                {
                    TempData["Error"] = "Couldn't get companies detail info...";
                    return RedirectToAction("Index", "Home");
                }
                string paymentMethod = collection["PaymentMethod"];
                var paymentDate = collection["PaymentDueDate"].ToString();
                bool includesDelivery = collection["IncludesDelivery"].ToString() == "1" ? true : false;

                if(!Invoice.CreateNewInvoice(_context, seller, targetClient, collection, paymentMethod, paymentDate, includesDelivery, products))
                {
                    TempData["Error"] = "Couldn't create new Invoice...";
                    return RedirectToAction("Index", "Home");
                }
            } 
            else
            {
                TempData["Error"] = "Something went wrong...";
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ExportInvoice()
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
                TempData["Error"] = "Something went wrong, no company found...";
                return RedirectToAction("ChangeCompany", "Home");
            }

            int queryCompanyId = companyId.Value;

            // Pobierz wszystkie faktury związane z firmą
            List<InvoiceData>? invoices = Invoice.GetRelatedCompanyInvoices(_context, queryCompanyId);

            if (invoices.Any())
            {
                return View(invoices);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ExportInvoiceAsPDF(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int invoiceID = !String.IsNullOrEmpty(collection["InvoiceID"]) ? int.Parse(collection["InvoiceID"]) : -1;
            if(invoiceID != -1)
            {
                InvoiceData? invoiceToExport = Invoice.GetInvoiceByID(_context, invoiceID);
                if(invoiceToExport != null)
                {
                    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                    InvoiceDocument doc = new InvoiceDocument(invoiceToExport);

                    DateTime creationDate = DateTime.Now;
                    string filename = $"{invoiceToExport.InvoiceInfo.Name}.pdf";

                    return File(doc.GeneratePdf(), "application/pdf", filename);
                }
            }
            TempData["Error"] = "Something went wrong...";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ExportInvoiceAsXPS(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int invoiceID = !String.IsNullOrEmpty(collection["InvoiceID"]) ? int.Parse(collection["InvoiceID"]) : -1;
            if (invoiceID != -1)
            {
                InvoiceData? invoiceToExport = Invoice.GetInvoiceByID(_context, invoiceID);
                if (invoiceToExport != null)
                {
                    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                    InvoiceDocument doc = new InvoiceDocument(invoiceToExport);

                    DateTime creationDate = DateTime.Now;
                    string filename = $"{invoiceToExport.InvoiceInfo.Name}.pdf";

                    return File(doc.GenerateXps(), "application/pdf", filename);
                }
            }
            TempData["Error"] = "Something went wrong...";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ExportInvoiceAsEXCEL(IFormCollection collection)
        {
            if (!CheckPrivilages(Privilages.Worker))
            {
                return RedirectToAction("Login", "Home");
            }

            int invoiceID = !String.IsNullOrEmpty(collection["InvoiceID"]) ? int.Parse(collection["InvoiceID"]) : -1;
            if (invoiceID != -1)
            {
                InvoiceData? invoiceToExport = Invoice.GetInvoiceByID(_context, invoiceID);
                if (invoiceToExport != null)
                {
                    string filename = invoiceToExport.InvoiceInfo.Name + ".xlsx";
                    byte[] exportedExcel = ExcelExporter.ExportInvoiceAsEXCEL(invoiceToExport);
                    return File(exportedExcel, "application/vnd.ms-excel", filename);
                }
            }
            TempData["Error"] = "Something went wrong...";
            return RedirectToAction("Index", "Home");
        }
    }
}
