using Inzynierka.DAL;
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

        public IActionResult InvoiceData()
        {
            return View();
        }
    }
}
