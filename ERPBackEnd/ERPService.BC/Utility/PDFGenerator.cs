using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using ERPService.Common.Interfaces;
using ERPService.Common.Shared;
using ERPService.DataModel.CTO;
using ERPService.DataModel.DTO;
using Serilog;

namespace ERPService.BC.Utility
{
    public class PDFGenerator<T> : ExportBase, IDisposable
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
        public PDFGenerator()
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

        public MemoryStream GetMemoryStream()
        {
            MemoryStream ms = new MemoryStream();
            Save(ms);
            return ms;
        }


        private void GridSection()
        {

            List<PDFExportAttribute> attributes = new List<PDFExportAttribute>();
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
                    if (headerText != null)
                    {
                        field.headerText = headerText.Description;
                        field.orginalFeild = property.Name;
                        field.Type = property.PropertyType;
                        attributes.Add(field);
                    }
                }
            }

            attributes = attributes.OrderBy(st => st.order).ToList();

            // Initializes a new instance of the Table
            var table = new Table
            {
                //ColumnWidths = string.Join(" ", attributes.Select(e=>e.width)),  //"100 100 100 78 78 150",
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
                cell = headerRow.Cells.Add(item.headerText);
                if (item.hdrAlign != 0)
                {
                    cell.Alignment = (HorizontalAlignment)item.hdrAlign;
                }
            }

            foreach (Cell headerRowCell in headerRow.Cells)
            {
                headerRowCell.BackgroundColor = _textColor;
                headerRowCell.DefaultCellTextState.ForegroundColor = _backColor;
            }
            var value = new object();
            foreach (var gdRow in gridData)
            {
                var row = table.Rows.Add();

                foreach (var attr in attributes)
                {

                    value = GetPropValue(gdRow, attr.orginalFeild.ToString());

                    if (attr != null && attr.Type != null && attr.Type.Name != "Nullable`1")
                    {
                        var propType = attr.Type;
                        var converter = TypeDescriptor.GetConverter(propType);
                        dynamic convertedObject = converter.ConvertFromString(value.ToString());

                        if (!string.IsNullOrEmpty(attr.currencyFormat))
                        {
                            value = convertedObject.ToString(attr.currencyFormat);
                        }

                        if (!string.IsNullOrEmpty(attr.dateFormat))
                        {
                            value = convertedObject.ToString(attr.dateFormat);
                        }

                        cell = row.Cells.Add(value.ToString());
                        if (_headerText == "Direct Invoice - Prepayment" || _headerText == "Direct Invoice - Postpayment")
                        {
                            var Status = "Status";
                            var StatusColour = GetPropValue(gdRow, Status);
                            if (StatusColour.ToString() == "SUBMITTED")
                                cell.BackgroundColor = Color.Wheat;

                            if (StatusColour.ToString() == "RETUREND")
                                cell.BackgroundColor = Color.DarkRed;

                            if (StatusColour.ToString() == "APPROVED")
                                cell.BackgroundColor = Color.LightGreen;
                        }
                        if (_headerText == "Ledger Histroy")
                        {
                            var Credit = "Credit";
                            var Colour = GetPropValue(gdRow, Credit);
                            var Debit = "Debit";
                            var Colour1 = GetPropValue(gdRow, Debit);
                            if ((decimal)Colour > 0)
                                cell.BackgroundColor = Color.LightCyan;
                            else if ((decimal)Colour1 > 0)
                                cell.BackgroundColor = Color.Wheat;
                        }
                        if (_headerText == "Stock transaction report")
                        {
                            var StockIn = "StockIn";
                            var Colour = GetPropValue(gdRow, StockIn);
                            var StockOut = "StockOut";
                            var Colour1 = GetPropValue(gdRow, StockOut);
                            if ((decimal)Colour > 0)
                                cell.BackgroundColor = Color.LightCyan;
                            else if ((decimal)Colour1 > 0)
                                cell.BackgroundColor = Color.Wheat;
                        }

                        if (attr.cellAlign != 0)
                        {
                            cell.Alignment = (HorizontalAlignment)attr.cellAlign;
                        }

                    }
                    else
                    {
                        cell = row.Cells.Add("");

                        if (attr.cellAlign != 0)
                        {
                            cell.Alignment = (HorizontalAlignment)attr.cellAlign;
                        }
                    }
                }
            }

            _pdfPage.Paragraphs.Add(table);
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
