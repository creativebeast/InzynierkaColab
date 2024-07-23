using Inzynierka.DAL;
using System.ComponentModel.DataAnnotations;
using Inzynierka.Models.ViewModels;
using Inzynierka.Migrations;
using Inzynierka.Helpers;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

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
        public string? MadeBy { get; set; }
        public DateTime? CreatedAt { get; set; }
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
                BuyerBankNumber = client.BankAccountNumber,
                CreatedAt = DateTime.Now
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
            Random random = new Random();
            int number = random.Next(100000, 10000000);
            //1.Create Invoice
            //2.Create ProductList
            //3.Insert empty ProductList to db to give it ID
            //4.Insert empty Invoice to db to give it ID

            Invoice newInvoice = new Invoice();
            //-----Create Invoice-----
            //--Seller Data--
            newInvoice.Name = collection["invoiceName"].ToString() ?? "";
            newInvoice.Number = number.ToString();
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
            newInvoice.BuyerAdress = collection["clientCity"] + ", " + collection["clientAdress"] + " " + collection["clientLocalNumber"] + ", ";
            newInvoice.BuyerAdress += String.IsNullOrWhiteSpace(collection["clientProvince"]) ? "" : collection["clientProvince"] + ", ";
            newInvoice.BuyerAdress += collection["clientPostalCode"];
            if (int.Parse(collection["clientIsCompany"].ToString()) == 1)
            {
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
            newInvoice.PaymentMethod = "Test";

            context.Invoices.Add(newInvoice);

            context.SaveChanges();

            ProductList newProductList = new ProductList()
            {
                InvoiceID = newInvoice.ID,
                CreationDate = DateTime.Now
            };
            context.ProductsList.Add(newProductList);

            context.SaveChanges();
            newInvoice.ProductListId = newProductList.ID;
            
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
                productToAdd.TotalNettoValue = String.IsNullOrEmpty(product.TotalNettoValue) ? 0 : Decimal.Parse(product.TotalNettoValue.Replace(".", ","));
                productToAdd.TotalBruttoValue = String.IsNullOrEmpty(product.TotalBruttoValue) ? 0 : Decimal.Parse(product.TotalBruttoValue.Replace(".", ","));
                productToAdd.TotalValue = String.IsNullOrEmpty(product.TotalValue) ? 0 : Decimal.Parse(product.TotalValue.Replace(".", ","));

                TotalNettoValue += productToAdd.TotalNettoValue;
                TotalBruttoValue += productToAdd.TotalBruttoValue;
                TotalValue += productToAdd.TotalValue;

                productsToAdd.Add(productToAdd);
            }
            newProductList.TotalNettoValue = TotalNettoValue;
            newProductList.TotalBruttoValue = TotalBruttoValue;
            newProductList.TotalPostDiscountValue = TotalValue;
            context.Products.AddRange(productsToAdd);

            if(context.SaveChanges() < 0)
            {
                return false;
            }

            return true;
        }

        public static List<InvoiceData> GetRelatedCompanyInvoices(ProjectContext context, int companyId)
        {
            List<Invoice>? invoices = context.Invoices.Where(i => i.SellerID == companyId)?.ToList();
            List<InvoiceData> invoiceDataList = new List<InvoiceData>();
            if(invoices == null)
            {
                return invoiceDataList;
            }

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

        public static InvoiceData? GetInvoiceByID(ProjectContext context, int invoiceId)
        {
            Invoice? targetInvoice = context.Invoices.FirstOrDefault(i => i.ID == invoiceId);
            if(targetInvoice == null)
                return null;

            ProductList? productList = context.ProductsList.Where(p => p.InvoiceID == targetInvoice.ID).FirstOrDefault();
            if (productList == null)
                return null;

            List<Product> products = context.Products.Where(p => p.ProductListID == productList.ID).ToList();
            if (products == null)
                return null;

            InvoiceData invoiceData = new InvoiceData(targetInvoice, productList, products);
            return invoiceData;
        }
        public Address GetAdress(CompanyEnu comp)
        {
            if(CompanyEnu.Buyer == comp)
            {
                Address buyerAdress = new Address();

                buyerAdress.CompanyName = this.BuyerName;
                buyerAdress.Street = this.BuyerAdress;
                Match result = Regex.Match(this.SellerAdress, @"^.*?(?=-)");
                buyerAdress.City = result.Success ? result.Value : "";
                buyerAdress.Email = this.BuyerEmail ?? "";
                buyerAdress.Phone = this.BuyerPhone ?? "";

                return buyerAdress;
            } 
            else
            {
                Address sellerAdress = new Address();

                sellerAdress.CompanyName = this.SellerName;
                sellerAdress.Street = this.SellerAdress;
                Match result = Regex.Match(this.SellerAdress, @"^.*?(?=-)");
                sellerAdress.City = result.Success ? result.Value : "";
                sellerAdress.Email = this.SellerEmail ?? "";
                sellerAdress.Phone = this.SellerPhone ?? "";

                return sellerAdress;
            }
        }

        public enum CompanyEnu
        {
            Buyer,
            Seller
        }
    }
}
