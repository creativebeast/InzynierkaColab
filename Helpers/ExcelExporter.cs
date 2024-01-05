using System.Globalization;
using System.Linq;
using Inzynierka.Models;
using Inzynierka.Models.ViewModels;
using Inzynierka.Helpers;
using OfficeOpenXml;
using NuGet.ContentModel;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Inzynierka.Helpers
{
    public class ExcelExporter
    {
        public static byte[] ExportInvoiceAsEXCEL(InvoiceData data)
        {
            using(var package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add(data.InvoiceInfo.Name);

                /*|------------------------|
                 *  Products related Data
                 *|------------------------|
                 */
                List<ExcelRange> generalLabelCells = new List<ExcelRange>();
                ExcelRange productsLabelCell = sheet.Cells[1, 1];
                productsLabelCell.Value = "Products:";

                ExcelRange productNameLabel = sheet.Cells[2, 1];
                productNameLabel.Value = "Name:";

                ExcelRange productQuantityLabel = sheet.Cells[2, 2];
                productQuantityLabel.Value = "Quantity:";

                ExcelRange productBruttoValueLabel = sheet.Cells[2, 3];
                productBruttoValueLabel.Value = "Brutto Value Per prod. :";

                ExcelRange productDiscountLabel = sheet.Cells[2, 4];
                productDiscountLabel.Value = "Discount:";

                ExcelRange productTotalValueLabel = sheet.Cells[2, 5];
                productTotalValueLabel.Value = "Total Value:";

                generalLabelCells.Add(productNameLabel);
                generalLabelCells.Add(productQuantityLabel);
                generalLabelCells.Add(productBruttoValueLabel);
                generalLabelCells.Add(productDiscountLabel);
                generalLabelCells.Add(productTotalValueLabel);

                int dataPlacement = 3;
                foreach(var product in data.ProductsInfo)
                {
                    ExcelRange prodName = sheet.Cells[dataPlacement, 1];
                    prodName.Value = product.Name;

                    ExcelRange prodQuantity = sheet.Cells[dataPlacement, 2];
                    prodQuantity.Value = product.Quantity;

                    ExcelRange prodBruttoValue = sheet.Cells[dataPlacement, 3];
                    prodBruttoValue.Style.Numberformat.Format = "$#,##0.00";
                    prodBruttoValue.Value = product.BruttoValue + "pln";

                    ExcelRange prodDiscount = sheet.Cells[dataPlacement, 4];
                    prodDiscount.Value = product.Discount;

                    ExcelRange prodTotalValue = sheet.Cells[dataPlacement, 5];
                    prodTotalValue.Style.Numberformat.Format = "$#,##0.00";
                    prodTotalValue.Value = product.TotalValue + "pln";
                }
                ExcelRange totalValueLabel = sheet.Cells[dataPlacement, 1];
                totalValueLabel.Value = "Total Value:";

                ExcelRange totalValue = sheet.Cells[dataPlacement, 2];
                totalValue.Style.Numberformat.Format = "$#,##0.00";
                totalValue.Value = data.ProductListInfo.TotalPostDiscountValue;

                dataPlacement = 3;
                /*|------------------------|
                 *  Seller related Data
                 *|------------------------|
                 */
                ExcelRange clientDataLabel = sheet.Cells[1, 9];
                clientDataLabel.Value = "Seller Data";

                ExcelRange clientNameLabel = sheet.Cells[2, 9];
                clientNameLabel.Value = "Name:";

                ExcelRange clientEmailLabel = sheet.Cells[3, 9];
                clientEmailLabel.Value = "Contact Info:";

                ExcelRange clientAddressLabel = sheet.Cells[4, 9];
                clientAddressLabel.Value = "Address:";

                ExcelRange clientNIPLabel = sheet.Cells[5, 9];
                clientNIPLabel.Value = "NIP:";

                ExcelRange clientPaymentDueLabel = sheet.Cells[6, 9];
                clientPaymentDueLabel.Value = "Payment Due:";


                generalLabelCells.Add(clientDataLabel);
                generalLabelCells.Add(clientNameLabel);
                generalLabelCells.Add(clientEmailLabel);
                generalLabelCells.Add(clientAddressLabel);
                generalLabelCells.Add(clientNIPLabel);
                generalLabelCells.Add(clientPaymentDueLabel);

                //Get seller data from parameters
                ExcelRange clientName = sheet.Cells[1, 10];
                clientDataLabel.Value = data.InvoiceInfo.SellerName;

                ExcelRange clientEmail = sheet.Cells[3, 10];
                clientEmailLabel.Value = data.InvoiceInfo.SellerEmail + ", " + data.InvoiceInfo.SellerPhone;

                ExcelRange clientAddress = sheet.Cells[4, 10];
                clientAddressLabel.Value = data.InvoiceInfo.SellerAdress + ", " + data.InvoiceInfo.SellerPostalCode;

                ExcelRange clientNIP = sheet.Cells[5, 10];
                clientNIPLabel.Value = data.InvoiceInfo.SellerNIP;

                ExcelRange clientPaymentDue = sheet.Cells[6, 10];
                clientPaymentDueLabel.Value = data.InvoiceInfo.PaymentDate;


                /*|------------------------|
                 *  Buyer related Data
                 *|------------------------|
                 */
                ExcelRange buyerDataLabel = sheet.Cells[1, 9];
                clientDataLabel.Value = "Buyer Data";

                ExcelRange buyerNameLabel = sheet.Cells[2, 9];
                clientNameLabel.Value = "Name:";

                ExcelRange buyerEmailLabel = sheet.Cells[3, 9];
                clientEmailLabel.Value = "Contact Info:";

                ExcelRange buyerAddressLabel = sheet.Cells[4, 9];
                clientAddressLabel.Value = "Address:";

                ExcelRange buyerNIPLabel = sheet.Cells[5, 9];
                clientNIPLabel.Value = "NIP:";

                generalLabelCells.Add(buyerDataLabel);
                generalLabelCells.Add(buyerNameLabel);
                generalLabelCells.Add(buyerEmailLabel);
                generalLabelCells.Add(buyerAddressLabel);
                generalLabelCells.Add(buyerNIPLabel);

                //Get buyer data from parameters
                ExcelRange buyerData = sheet.Cells[1, 10];
                clientDataLabel.Value = data.InvoiceInfo.BuyerName;

                ExcelRange buyerEmail = sheet.Cells[3, 10];
                clientEmailLabel.Value = data.InvoiceInfo.BuyerEmail + ", " + data.InvoiceInfo.BuyerPhone;

                ExcelRange buyerAddress = sheet.Cells[4, 10];
                clientAddressLabel.Value = data.InvoiceInfo.BuyerAdress + ", " + data.InvoiceInfo.BuyerPostalCode;

                ExcelRange buyerNIP = sheet.Cells[5, 10];
                clientNIPLabel.Value = data.InvoiceInfo.BuyerNIP;

                // Numbers
                //var moneyCell = sheet.Cells["A3"];
                //moneyCell.Style.Numberformat.Format = "$#,##0.00";
                //moneyCell.Value = 15.25M;

                // Easily write any Enumerable to a sheet
                // In this case: All Excel functions implemented by EPPlus
                //var funcs = package.Workbook.FormulaParserManager.GetImplementedFunctions()
                //    .Select(x => new { FunctionName = x.Key, TypeName = x.Value.GetType().FullName });
                //sheet.Cells["A4"].LoadFromCollection(funcs, true);

                foreach(ExcelRange cell in generalLabelCells)
                {
                    cell.Style.Font.Bold = true;
                }
                // Styling cells
                //var someCells = sheet.Cells["A1,A4:B4"];
                //someCells.Style.Font.Bold = true;
                //someCells.Style.Font.Color.SetColor(Color.Ivory);
                //someCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //someCells.Style.Fill.BackgroundColor.SetColor(Color.Navy);

                sheet.Cells.AutoFitColumns();
                package.SaveAs(new FileInfo(@"basicUsage.xslx"));

                return null;
            }
        }
    }
}
