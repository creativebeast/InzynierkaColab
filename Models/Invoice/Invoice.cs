using Inzynierka.DAL;
using System.ComponentModel.DataAnnotations;
using Inzynierka.Models.ViewModels;
using Inzynierka.Migrations;

namespace Inzynierka.Models
{
    public class Invoice
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public int SellerID { get; set; }
        public string SellerName { get; set; }
        public string SellerAdress { get; set; }
        public string SellerPostalCode { get; set; }
        public string ?SellerPhone { get; set; }
        public string ?SellerEmail { get; set; }
        public string ?SellerNIP { get; set; }
        public string SellerBankName { get; set; }
        public string SellerBankNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentDate { get; set; }
        public bool IncludesDelivery { get; set; }
        public int ProductListId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAdress { get; set; }
        public string BuyerPostalCode { get; set; }
        public string ?BuyerPhone { get; set; }
        public string ?BuyerEmail { get; set; }
        public string ?BuyerNIP { get; set; }
        public string BuyerBankName { get; set; }
        public string BuyerBankNumber { get; set; }
        //public int ProductList { get; set; }


        public static bool CreateNewInvoice(ProjectContext context, Company company, Client client, 
            string paymentMethod, string paymentDueDate, bool includesDelivery, List<Product>? products = null)
        {
            Invoice newInvoice = new Invoice()
            {
                Name = "Testowa Faktura",
                Number = "100 100 100 100",
                SellerName = company.Name,
                SellerAdress = $"{company.City}, {company.Street} - {company.LocalNumber}",
                SellerPostalCode = company.PostalCode,
                SellerEmail = company.ContactMail,
                SellerPhone = company.ContactNumber,
                SellerBankName = company.BankName,
                SellerBankNumber = company.BankAccountNumber,
                PaymentMethod = paymentMethod,
                PaymentDate = paymentDueDate,
                IncludesDelivery = includesDelivery,
                BuyerName = client.Name,
                BuyerAdress = $"{client.City}, {client.Street} - {client.LocalNumber}",
                BuyerPostalCode = client.PostalCode,
                BuyerEmail = client.ContactMail,
                BuyerPhone = client.ContactNumber,
                BuyerBankName = client.BankName,
                BuyerBankNumber = client.BankAccountNumber

            };

            //ExpandItLater
            newInvoice.ProductListId = 0;

            context.Invoices.Add(newInvoice);
            if(context.SaveChanges() != 1)
                return false;

            return true;
        }

        public static bool CreateNewInvoice(ProjectContext context, Company company, Client client, IFormCollection collection, 
            string paymentMethod, string paymentDueDate, bool includesDelivery, List<ProductTemplate>? products)
        {
            //1.Create Invoice
            //4.Insert empty Invoice to db to give it ID
            Invoice newInvoice = new Invoice();
            context.Invoices.Add(newInvoice);
            //2.Create ProductList
            //3.Insert empty ProductList to db to give it ID
            ProductList newProductList = new ProductList()
            {
                InvoiceID = newInvoice.ID
            };

            context.ProductsList.Add(newProductList);

            Decimal TotalNettoValue = 0;
            Decimal TotalBruttoValue = 0;
            Decimal TotalValue = 0;
            List<Product> productsToAdd = new List<Product>();
            foreach(ProductTemplate product in products)
            {
                Product productToAdd = new Product();
                productToAdd.ProductListID = newProductList.ID;
                productToAdd.Name = product.Name;
                productToAdd.Quantity = product.Quantity;
                productToAdd.NettoValue = String.IsNullOrEmpty(product.NettoValue) ? 0 : Decimal.Parse(product.NettoValue.Replace(".", ","));
                productToAdd.BruttoValue = String.IsNullOrEmpty(product.BruttoValue) ? 0 : Decimal.Parse(product.BruttoValue.Replace(".", ","));
                productToAdd.PostDiscountNettoValue = String.IsNullOrEmpty(product.PostDiscountNettoValue) ? 0 : Decimal.Parse(product.PostDiscountNettoValue.Replace(".", ","));
                productToAdd.Discount = product.Discount == null || product.Discount <= 0 ? 0 : (Decimal)product.Discount;
                productToAdd.VAT = String.IsNullOrEmpty(product.VAT) ? 0 : Decimal.Parse(product.VAT.Replace(".", ","));
                TotalNettoValue = String.IsNullOrEmpty(product.TotalNettoValue) ? 0 : Decimal.Parse(product.TotalNettoValue.Replace(".", ","));
                TotalBruttoValue = String.IsNullOrEmpty(product.TotalBruttoValue) ? 0 : Decimal.Parse(product.TotalBruttoValue.Replace(".", ","));
                TotalValue = String.IsNullOrEmpty(product.TotalValue) ? 0 : Decimal.Parse(product.TotalValue.Replace(".", ","));

                TotalNettoValue += productToAdd.TotalNettoValue;
                TotalBruttoValue += productToAdd.TotalBruttoValue;
                TotalValue += productToAdd.TotalValue;

                productsToAdd.Add(productToAdd);
            }

            context.Products.AddRange(productsToAdd);

            //-----Create Invoice-----
            //--Seller Data--
            newInvoice.Name = collection["invoiceName"].ToString() ?? "";
            newInvoice.Number = "0000-Test-0000-Test-0000-HardTyped-0000";
            newInvoice.SellerID = int.Parse(collection["companyId"]);
            newInvoice.SellerName = collection["companyName"];
            newInvoice.SellerAdress = collection["companyCity"] + ", " + collection["companyStreet"] + " " + collection["companyLocalNumber"];
            newInvoice.SellerAdress += String.IsNullOrWhiteSpace(collection["companyProvince"]) ? "" : collection["companyProvince"] + " ";
            newInvoice.SellerAdress += collection["companyPostalCode"];
            newInvoice.SellerPostalCode = collection["companyPostalCode"];
            newInvoice.SellerNIP = collection["companyNIP"];
            newInvoice.SellerBankName = collection["companyBankName"];
            newInvoice.SellerBankNumber = collection["companyBankNumber"];
            newInvoice.SellerPhone = collection["companyContactNumber"];
            newInvoice.SellerEmail = collection["companyContactEmail"];
            //--Buyer Data--
            newInvoice.BuyerName = collection["clientName"];
            newInvoice.BuyerAdress = collection["clientCity"] + ", " + collection["clientAdress"] + " " + collection["clientLocalNumber"];
            newInvoice.BuyerAdress += String.IsNullOrWhiteSpace(collection["clientProvince"]) ? "" : collection["clientProvince"] + " ";
            newInvoice.BuyerAdress += collection["clientPostalCode"];
            if (int.Parse(collection["clientIsCompany"].ToString()) == 1){
                newInvoice.BuyerNIP = collection["clientNIP"];
            }
            newInvoice.BuyerPostalCode = collection["clientPostalCode"];
            newInvoice.BuyerBankName = collection["companyBankName"];
            newInvoice.BuyerBankNumber = collection["companyBankNumber"];
            newInvoice.BuyerPhone = collection["companyContactNumber"].ToString() ?? "";
            newInvoice.BuyerEmail = collection["companyContactEmail"].ToString() ?? "";
            //--Misc--
            newInvoice.IncludesDelivery = collection["IncludesDelivery"] == "0" ? false : true;
            newInvoice.PaymentDate = collection["PaymentDueDate"];
            newInvoice.ProductListId = newProductList.ID;
            newInvoice.PaymentMethod = "Test";
            

            newProductList.TotalNettoValue = TotalNettoValue;
            newProductList.TotalBruttoValue = TotalBruttoValue;
            newProductList.TotalPostDiscountValue = TotalValue;
            newProductList.CreationDate = DateTime.Now;
            
            if(context.SaveChanges() < 0)
            {
                return false;
            }

            return true;
        }

        public static List<InvoiceData> GetRelatedCompanyInvoices(ProjectContext context, int companyId)
        {
            List<Invoice> invoices = context.Invoices.Where(i => i.SellerID == companyId).ToList();
            List<InvoiceData> invoiceDataList = new List<InvoiceData>();

            foreach(Invoice invoice in invoices)
            {
                ProductList? productList = context.ProductsList.Where(p => p.InvoiceID == invoice.ID).FirstOrDefault();
                if(productList != null)
                {
                    List<Product> products = context.Products.Where(p => p.ProductListID == productList.ID).ToList();

                    invoiceDataList.Add(new InvoiceData()
                    {
                        InvoiceInfo = invoice,
                        ProductListInfo = productList,
                        ProductsInfo = products
                    });
                }
                
            }

            return invoiceDataList;
        }
    }
}
