﻿using Inzynierka.DAL;
using Inzynierka.Models;
using Inzynierka.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Inzynierka.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger, ProjectContext context) : base(context)
        {
            _logger = logger;
        }

        public IActionResult InvoiceData(IFormCollection collection)
        {
            int companyID = int.Parse(collection["companyId"].ToString());
            Company? company = Company.getCompanyByID(_context, companyID);
            ViewData["Company"] = company;

            return View();
        }

        public IActionResult AddInvoice(IFormCollection collection)
        {
            int companyID = int.Parse(collection["companyId"].ToString());
            Company? company = Company.getCompanyByID(_context, companyID);
            ViewData["Company"] = company;
            if (company == null)
            {
                TempData["Error"] = "Couldn't find your company...";
                return RedirectToAction("Index", "Home");
            }

            List<Client>? clients = Client.GetClientsRelatedToCompany(_context, companyID);
            ViewData["Clients"] = clients;

            return View(new List<ProductTemplate>());
        }

        public IActionResult CreateInvoice(IFormCollection collection, List<ProductTemplate> products)
        {
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

        public IActionResult ExportInvoice(IFormCollection collection)
        {
            int companyID = !String.IsNullOrEmpty(collection["companyId"]) ? int.Parse(collection["companyId"]) : -1;
            if(companyID != -1)
            {
                //Get all Invoices related to company
                List<InvoiceData>? invoices = Invoice.GetRelatedCompanyInvoices(_context, companyID);
                return View(invoices);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
