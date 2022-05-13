using Aspose.Cells;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.BC.Utility
{

    public class BudgetAllocationExcelGeneratore<T> : ExportBase, IDisposable
    {
        public List<T> data;

        private Workbook _workbook;
        public string _headerText { get; set; }
        public IRepository _repository;
        public UserContext _userContext;
        public ILogger _logger;
        public BudgetAllocationExcelGeneratore() { }
        public void Save(Stream stream)
        {
            FillData();
            _workbook.Save(stream, SaveFormat.Xlsx);
        }
        public byte[] getByte()
        {
            MemoryStream ms = new MemoryStream();
            Save(ms);
            byte[] m_Bytes = ReadToEnd(ms);
            ms.Close();
            return m_Bytes;
        }
        public void FillData()
        {
            List<ExcelExportAttribute> attributes = new List<ExcelExportAttribute>();
            List<ExcelExportAttribute> attributes2 = new List<ExcelExportAttribute>();
            Type typeOfT = typeof(T);
            PropertyInfo[] propInfo = typeOfT.GetProperties();
            var LangMasterBC = new LangMasterBC(_logger, _repository);
            var Headertrans = LangMasterBC.GetLangBasedDataForCodeMaster(_userContext.Language);
            var codedetailscode = Headertrans.Where(a => a.Code == "EXPORTHEADER").FirstOrDefault();
            foreach (PropertyInfo property in propInfo)
            {
                var field = (ExcelExportAttribute)Attribute.GetCustomAttribute(property, typeof(ExcelExportAttribute), false);
                if (field != null)
                {
                    var headerText = codedetailscode.CodesDetail.Where(a => a.Code == field.headerText).FirstOrDefault();
                    var SubheaderText = codedetailscode.CodesDetail.Where(a => a.Code == field.subHeaderText).FirstOrDefault();
                    if (headerText != null)
                    {
                        field.headerText = headerText.Description;
                        field.orginalFeild = property.Name;
                        field.Type = property.PropertyType;
                        attributes.Add(field);
                    }
                    if (SubheaderText != null)
                    {
                        field.subHeaderText = SubheaderText.Description;
                        field.orginalFeild = property.Name;
                        field.Type = property.PropertyType;
                        attributes2.Add(field);
                    }
                }
            }

            attributes = attributes.OrderBy(st => st.order).ToList();

            //Create the workbook and the worksheet
            _workbook = new Workbook();
            Worksheet sheet = _workbook.Worksheets[0];
            sheet.Name = _headerText;

            Cells cells = sheet.Cells; // Cells
                                       // Create Styles

            sheet.Cells.Merge(0, 0, 1, attributes.Count);
            sheet.Cells[0, 0].PutValue(_headerText);
            Cell headerCell = cells.GetCell(0, 0);
            Style headerStyle = headerCell.GetStyle();
            headerStyle.Font.Color = Color.Brown;
            headerStyle.Font.IsBold = true;
            headerStyle.Font.Size = 12;
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.ForegroundColor = Color.White;
            Aspose.Cells.Font headerFont = headerStyle.Font;
            headerCell.SetStyle(headerStyle);






            int i = 0;
            int k = 0;
            foreach (var gdRow in data)
            {
                k = 0;
                foreach (var attr in attributes)
                {
                    var value = GetPropValue(gdRow, attr.orginalFeild.ToString());

                    var propType = attr.Type;
                    var converter = TypeDescriptor.GetConverter(propType);
                    dynamic convertedObject = converter.ConvertFromString(value.ToString());
                    if (!string.IsNullOrEmpty(attr.dateFormat))
                    {
                        value = convertedObject.ToString(attr.dateFormat);

                    }
                    if (!string.IsNullOrEmpty(attr.currencyFormat))
                    {
                        value = convertedObject.ToString(attr.currencyFormat);
                    }
                    var row = 1 + i;

                    cells[row, k].PutValue(attr.headerText + " : " + value); //Add data
                    Cell cell = cells.GetCell(row, k);
                    if (_headerText == "Budget Allocation" || _headerText == "Purchase Order" || _headerText == "Goods Receipt Notes" ||
                   _headerText == "Inventory Issue" || _headerText == "Purchase Requests" || _headerText == "Quotation Request" || _headerText == "Vendor Quotation")
                    {
                        Style style = cell.GetStyle();
                        var Status = "Status";
                        var values = GetPropValue(gdRow, Status);
                        if ((values.ToString() == "SUBMITTED") || (values.ToString() == "PURTRNSTSSUBMITTED"))
                        {
                            style.ForegroundColor = Color.Wheat;
                            style.Font.Color = Color.Black;
                        }
                        if ((values.ToString() == "APPROVED") || (values.ToString() == "PURTRNSTSAPPROVED"))
                        {
                            style.ForegroundColor = Color.LightGreen;
                            style.Font.Color = Color.Black;
                        }
                        if ((values.ToString() == "RETURNED") || (values.ToString() == "PURTRNSTSREJECTED"))
                        {
                            style.ForegroundColor = Color.DarkRed;
                            style.Font.Color = Color.White;
                        }

                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;


                        Aspose.Cells.Font font = style.Font;
                        cell.SetStyle(style);
                    }
                    k++;

                }
                if (_headerText == "Inventory Issue")
                {
                    var value2 = new object();
                    ICollection<ProdInvIssueDet> data1 = (ICollection<ProdInvIssueDet>)gdRow.GetType().GetProperty("ProdInvIssueDet").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Quotation Request")
                {
                    var value2 = new object();
                    ICollection<QuotationReqDet> data1 = (ICollection<QuotationReqDet>)gdRow.GetType().GetProperty("QuotationReqDet").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Purchase Requests")
                {
                    var value2 = new object();
                    ICollection<PurchaseRequestDet> data1 = (ICollection<PurchaseRequestDet>)gdRow.GetType().GetProperty("PurchaseRequestDetList").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Vendor Quotation")
                {
                    var value2 = new object();
                    ICollection<VendorQuotationDet> data1 = (ICollection<VendorQuotationDet>)gdRow.GetType().GetProperty("vendorquotationDets").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Goods Receipt Notes")
                {
                    var value2 = new object();
                    ICollection<GoodsRecNoteDet> data1 = (ICollection<GoodsRecNoteDet>)gdRow.GetType().GetProperty("GoodsReceiptNoteDet").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Purchase Order")
                {
                    var value2 = new object();
                    ICollection<PurchaseOrderDet> data1 = (ICollection<PurchaseOrderDet>)gdRow.GetType().GetProperty("PurchaseOrderDet").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }
                if (_headerText == "Service Requests")
                {
                    var value2 = new object();
                    ICollection<ServiceReqApproval> data1 = (ICollection<ServiceReqApproval>)gdRow.GetType().GetProperty("ServReqApproval").GetValue(gdRow, null);
                    k = 0;
                    i++;

                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data1)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;

                            value2 = GetPropValue(item, attr.orginalFeild.ToString());


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value2.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value2 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value2 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value2);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }

                }

                if (_headerText == "Budget Allocation")
                {
                    var value1 = new object();
                    ICollection<BudgAllocDet> data = (ICollection<BudgAllocDet>)gdRow.GetType().GetProperty("BudgAllocDet").GetValue(gdRow, null);
                    k = 0;
                    i++;
                    foreach (var item in attributes2)
                    {

                        var row = 1 + i;

                        cells[row, k].PutValue(item.subHeaderText); // Add table headers                       
                        Cell cell = cells.GetCell(row, k);
                        Style style = cell.GetStyle();

                        style.Font.Color = Color.White;
                        style.Font.IsBold = true;
                        style.Font.Size = 10;
                        style.Pattern = BackgroundType.Solid;
                        style.ForegroundColor = Color.LightBlue;
                        cell.SetStyle(style);
                        k++;
                    }
                    k = 0;
                    i++;
                    foreach (var item in data)
                    {
                        foreach (var attr in attributes2)
                        {
                            var row = 1 + i;
                            if (attr.orginalFeild.ToString() == "BudgetDetailAmount")
                            {
                                var BA = "BudgetAmount";
                                value1 = GetPropValue(item, BA);
                            }
                            else
                            {
                                value1 = GetPropValue(item, attr.orginalFeild.ToString());
                            }


                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value1.ToString());


                            //Add data

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value1 = convertedObject.ToString(attr.dateFormat);

                            }
                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value1 = convertedObject.ToString(attr.currencyFormat);
                            }

                            cells[row, k].PutValue(value1);
                            k++;
                        }
                        k = 0;
                        i = 1 + i;
                    }
                }


                i++;
            }

            //Set the columns to fit the size of their content
            sheet.AutoFitColumns();

        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _workbook.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
