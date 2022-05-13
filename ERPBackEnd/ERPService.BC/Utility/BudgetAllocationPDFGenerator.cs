using Aspose.Pdf;
using Aspose.Pdf.Text;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERPService.BC.Utility
{
    public class BudgetAllocationPDFGenerator<T> : ExportBase, IDisposable
    {
        private Color _textColor, _backColor;
        private readonly Font _timeNewRomanFont;
        private readonly Page _pdfPage;
        private readonly Document _pdfDocument;
        public List<T> gridData;
        public string _headerText;
        public IRepository _repository;
        public UserContext _userContext;
        public ILogger _logger;

        private string ForegroundColor
        {
            get { return _textColor.ToString(); }
            set { _textColor = Color.Parse(value); }
        }
        private string BackgroundColor
        {
            get { return _backColor.ToString(); }
            set { _backColor = Color.Parse(value); }
        }
        public BudgetAllocationPDFGenerator()
        {
            _pdfDocument = new Document();
            _pdfDocument.PageInfo.Margin.Left = 36;
            _pdfDocument.PageInfo.Margin.Right = 36;
            _pdfPage = _pdfDocument.Pages.Add();
            _textColor = Color.Black;
            _backColor = Color.Transparent;
            _timeNewRomanFont = FontRepository.FindFont("Times New Roman");
            ForegroundColor = "#964B00";
            BackgroundColor = "#FFFFFF";
        }

        public void Save(Stream stream)
        {
            HeaderSection();
            GridSection();
            _pdfDocument.Save(stream);
        }

        public byte[] getByte()
        {
            MemoryStream ms = new MemoryStream();
            Save(ms);
            byte[] m_Bytes = ReadToEnd(ms);
            ms.Close();
            return m_Bytes;
        }

        private void GridSection()
        {
            List<PDFExportAttribute> attributes = new List<PDFExportAttribute>();
            List<PDFExportAttribute> attributes2 = new List<PDFExportAttribute>();
            var LangMasterBC = new LangMasterBC(_logger, _repository);
            var Headertrans = LangMasterBC.GetLangBasedDataForCodeMaster(_userContext.Language);
            var codedetailscode = Headertrans.Where(a => a.Code == "EXPORTHEADER").FirstOrDefault();
            Type typeOfT = typeof(T);
            PropertyInfo[] propInfo = typeOfT.GetProperties();
            foreach (PropertyInfo property in propInfo)
            {
                var field = (PDFExportAttribute)Attribute.GetCustomAttribute(property, typeof(PDFExportAttribute), false);
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

            // Initializes a new instance of the Table
            foreach (var gdRow in gridData)
            {
                var table = new Table();
                table = new Table
                {
                    ColumnAdjustment = ColumnAdjustment.AutoFitToWindow,
                    Border = new BorderInfo(BorderSide.Box, 1f, _textColor),
                    DefaultCellBorder = new BorderInfo(BorderSide.Box, 0.5f, _textColor),
                    DefaultCellPadding = new MarginInfo(4.5, 4.5, 4.5, 4.5),
                    Margin = { Bottom = 10 },
                    DefaultCellTextState = { Font = _timeNewRomanFont }
                };
                var headerRow = table.Rows.Add();
                var cell = new Cell();
                foreach (var item in attributes)
                {
                    var value = GetPropValue(gdRow, item.orginalFeild.ToString());
                    var propType = item.Type;
                    var converter = TypeDescriptor.GetConverter(propType);
                    dynamic convertedObject = converter.ConvertFromString(value.ToString());
                    if (!string.IsNullOrEmpty(item.dateFormat))
                    {
                        value = convertedObject.ToString(item.dateFormat);
                    }
                    if (!string.IsNullOrEmpty(item.currencyFormat))
                    {
                        value = convertedObject.ToString(item.currencyFormat);
                    }
                    cell = headerRow.Cells.Add(item.headerText + " : " + value.ToString());
                    if (item.hdrAlign != 0)
                    {
                        cell.Alignment = (HorizontalAlignment)item.hdrAlign;
                    }

                }
                if (_headerText == "Budget Allocation" || _headerText == "Purchase Order" || _headerText == "Goods Receipt Notes" || _headerText == "Inventory transfer"
                   || _headerText == "Inventory Issue" || _headerText == "Purchase Requests" || _headerText == "Quotation Request" || _headerText == "Vendor Quotation")
                {
                    foreach (Cell headerRowCell in headerRow.Cells)
                    {
                        var Status = "Status";
                        var value = GetPropValue(gdRow, Status);
                        if ((value.ToString() == "SUBMITTED") || (value.ToString() == "PURTRNSTSSUBMITTED"))
                        {
                            headerRowCell.BackgroundColor = Color.Wheat;
                            headerRowCell.DefaultCellTextState.ForegroundColor = Color.Black;
                        }
                        if ((value.ToString() == "APPROVED") || (value.ToString() == "PURTRNSTSAPPROVED"))
                        {
                            headerRowCell.BackgroundColor = Color.LightGreen;
                            headerRowCell.DefaultCellTextState.ForegroundColor = Color.Black;
                        }
                        if ((value.ToString() == "RETURNED") || (value.ToString() == "PURTRNSTSREJECTED"))
                        {
                            headerRowCell.BackgroundColor = Color.DarkRed;
                            headerRowCell.DefaultCellTextState.ForegroundColor = Color.White;
                        }


                    }
                    var row = table.Rows.Add();

                }

                var headerRow1 = table.Rows.Add();
                var cell1 = new Cell();
                foreach (var item in attributes2)
                {

                    cell1 = headerRow1.Cells.Add(item.subHeaderText);
                    if (item.hdrAlign != 0)
                    {
                        cell1.Alignment = (HorizontalAlignment)item.hdrAlign;
                    }

                }
                var row2 = table.Rows.Add();
                foreach (Cell headerRowCell2 in headerRow1.Cells)
                {
                    headerRowCell2.BackgroundColor = Color.LightSkyBlue;
                    headerRowCell2.DefaultCellTextState.ForegroundColor = Color.Black;
                }
                if (_headerText == "Inventory transfer")
                {
                    var value3 = new object();
                    ICollection<InventoryTransferDet> data1 = (ICollection<InventoryTransferDet>)gdRow.GetType().GetProperty("inventoryTransferDets").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Quotation Request")
                {
                    var value3 = new object();
                    ICollection<QuotationReqDet> data1 = (ICollection<QuotationReqDet>)gdRow.GetType().GetProperty("QuotationReqDet").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Inventory Issue")
                {
                    var value3 = new object();
                    ICollection<ProdInvIssueDet> data1 = (ICollection<ProdInvIssueDet>)gdRow.GetType().GetProperty("ProdInvIssueDet").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Vendor Quotation")
                {
                    var value3 = new object();
                    ICollection<VendorQuotationDet> data1 = (ICollection<VendorQuotationDet>)gdRow.GetType().GetProperty("vendorquotationDets").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Goods Receipt Notes")
                {
                    var value3 = new object();
                    ICollection<GoodsRecNoteDet> data1 = (ICollection<GoodsRecNoteDet>)gdRow.GetType().GetProperty("GoodsReceiptNoteDet").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Purchase Order")
                {
                    var value3 = new object();
                    ICollection<PurchaseOrderDet> data1 = (ICollection<PurchaseOrderDet>)gdRow.GetType().GetProperty("PurchaseOrderDet").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }
                if (_headerText == "Service Requests")
                {
                    var value3 = new object();
                    ICollection<ServiceReqApproval> data1 = (ICollection<ServiceReqApproval>)gdRow.GetType().GetProperty("ServReqApproval").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }

                if (_headerText == "Purchase Requests")
                {
                    var value3 = new object();
                    ICollection<PurchaseRequestDet> data1 = (ICollection<PurchaseRequestDet>)gdRow.GetType().GetProperty("PurchaseRequestDetList").GetValue(gdRow, null);
                    foreach (var item in data1)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {

                            value3 = GetPropValue(item, attr.orginalFeild.ToString());
                            var propType = attr.Type;
                            var converter = TypeDescriptor.GetConverter(propType);
                            dynamic convertedObject = converter.ConvertFromString(value3.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value3 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value3 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value3.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }
                    }
                }

                if (_headerText == "Budget Allocation")
                {
                    var value1 = new object();
                    ICollection<BudgAllocDet> data = (ICollection<BudgAllocDet>)gdRow.GetType().GetProperty("BudgAllocDet").GetValue(gdRow, null);

                    foreach (var item in data)
                    {
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes2)
                        {
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

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value1 = convertedObject.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value1 = convertedObject.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value1.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }

                    }
                }



                var lines = new TextFragment[2];
                // Create text fragment
                lines[0] = new TextFragment(_headerText);
                lines[0].TextState.FontSize = 20;
                lines[0].TextState.ForegroundColor = Color.White;
                lines[0].HorizontalAlignment = HorizontalAlignment.Center;

                _pdfPage.Paragraphs.Add(lines[0]);
                _pdfPage.Paragraphs.Add(table);
            }

        }

        private void HeaderSection()
        {
            var lines = new TextFragment[2];
            // Create text fragment
            lines[0] = new TextFragment(_headerText);
            lines[0].TextState.FontSize = 20;
            lines[0].TextState.ForegroundColor = _textColor;
            lines[0].HorizontalAlignment = HorizontalAlignment.Center;

            _pdfPage.Paragraphs.Add(lines[0]);

            lines[1] = new TextFragment($"DATE: {DateTime.Now:MM/dd/yyyy HH:mm}");
            // Set text properties                
            lines[1].TextState.Font = _timeNewRomanFont;
            lines[1].TextState.FontSize = 12;
            lines[1].HorizontalAlignment = HorizontalAlignment.Right;
            _pdfPage.Paragraphs.Add(lines[1]);

        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pdfPage.Dispose();
                    _pdfDocument.Dispose();
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
