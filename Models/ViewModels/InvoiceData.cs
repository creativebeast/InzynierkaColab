using System.Text.RegularExpressions;
using Inzynierka.Helpers;

namespace Inzynierka.Models.ViewModels
{
    public class InvoiceData
    {
        public Invoice InvoiceInfo { get; set; }
        public ProductList ProductListInfo { get; set; }
        public List<Product> ProductsInfo { get; set; }
        public InvoiceData()
        {

        }
        public InvoiceData(Invoice invoiceInfo, ProductList productListInfo, List<Product> productsInfo)
        {
            InvoiceInfo = invoiceInfo;
            ProductListInfo = productListInfo;
            ProductsInfo = productsInfo;
        }
    }
}
