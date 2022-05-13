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
    public class QuotationDetSendMail<T> : ExportBase, IDisposable
    {
        private Color _textColor, _backColor;
        private readonly Font _timeNewRomanFont;
        private readonly Page _pdfPage;
        private readonly Document _pdfDocument;
        public List<T> gridData;
        public string _headerText;
        public VendorMailSend vendorMailSend;
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
        public QuotationDetSendMail()
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
            var table = new Table
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


                    if (attr.cellAlign != 0)
                    {
                        cell.Alignment = (HorizontalAlignment)attr.cellAlign;
                    }

                }
            }

            _pdfPage.Paragraphs.Add(table);
        }

        private void HeaderSection()
        {
            if (_headerText == "Quotation Request")
            {
                var lines = new TextFragment[8];
                // Create text fragment
                lines[0] = new TextFragment(_headerText);
                lines[0].TextState.FontSize = 20;
                lines[0].TextState.ForegroundColor = _textColor;
                lines[0].HorizontalAlignment = HorizontalAlignment.Center;

                _pdfPage.Paragraphs.Add(lines[0]);

                lines[1] = new TextFragment();
                // Set text properties                

                lines[1].HorizontalAlignment = HorizontalAlignment.Right;
                _pdfPage.Paragraphs.Add(lines[1]);

                lines[2] = new TextFragment($"Trans No: {vendorMailSend.QuotionReqNo}");
                lines[2].TextState.FontSize = 12;
                lines[2].TextState.ForegroundColor = _textColor;
                lines[2].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[2]);

                lines[3] = new TextFragment();

                lines[3].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[3]);

                lines[4] = new TextFragment($"Trans Date : {vendorMailSend.QuotationReqDate.ToShortDateString()}");
                lines[4].TextState.FontSize = 12;
                lines[4].TextState.ForegroundColor = _textColor;
                lines[4].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[4]);
                lines[5] = new TextFragment();

                lines[5].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[5]);

                lines[6] = new TextFragment($"Remarks : {vendorMailSend.Remarks}");
                lines[6].TextState.FontSize = 12;
                lines[6].TextState.ForegroundColor = _textColor;
                lines[6].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[6]);
                lines[7] = new TextFragment();

                lines[7].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[7]);
            }
            if (_headerText == "Purchase Order" || _headerText == "Goods Receipt Notes")
            {
                var lines = new TextFragment[10];
                // Create text fragment
                lines[0] = new TextFragment(_headerText);
                lines[0].TextState.FontSize = 20;
                lines[0].TextState.ForegroundColor = _textColor;
                lines[0].HorizontalAlignment = HorizontalAlignment.Center;

                _pdfPage.Paragraphs.Add(lines[0]);

                lines[1] = new TextFragment();
                // Set text properties                

                lines[1].HorizontalAlignment = HorizontalAlignment.Right;
                _pdfPage.Paragraphs.Add(lines[1]);

                lines[2] = new TextFragment($"Trans No: {vendorMailSend.QuotionReqNo}");
                lines[2].TextState.FontSize = 12;
                lines[2].TextState.ForegroundColor = _textColor;
                lines[2].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[2]);

                lines[3] = new TextFragment();

                lines[3].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[3]);

                lines[4] = new TextFragment($"Trans Date : {vendorMailSend.QuotationReqDate.ToShortDateString()}"); 
                lines[4].TextState.FontSize = 12;
                lines[4].TextState.ForegroundColor = _textColor;
                lines[4].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[4]);

                lines[7] = new TextFragment();

                lines[7].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[7]);
                if(_headerText != "Goods Receipt Notes")
                {
                    lines[9] = new TextFragment($"Purchase Request No: {vendorMailSend.purchaseTransNo}");
                    lines[9].TextState.FontSize = 12;
                    lines[9].TextState.ForegroundColor = _textColor;
                    lines[9].HorizontalAlignment = HorizontalAlignment.Left;

                    _pdfPage.Paragraphs.Add(lines[9]);
                }
                if (_headerText == "Goods Receipt Notes")
                {
                    lines[9] = new TextFragment($"Purchase Order No: {vendorMailSend.purchaseTransNo}");
                    lines[9].TextState.FontSize = 12;
                    lines[9].TextState.ForegroundColor = _textColor;
                    lines[9].HorizontalAlignment = HorizontalAlignment.Left;

                    _pdfPage.Paragraphs.Add(lines[9]);
                }
                lines[7] = new TextFragment();

                lines[7].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[7]);

                lines[8] = new TextFragment($"Remarks : {vendorMailSend.Remarks}");
                lines[8].TextState.FontSize = 12;
                lines[8].TextState.ForegroundColor = _textColor;
                lines[8].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[8]);
                lines[9] = new TextFragment();

                lines[9].HorizontalAlignment = HorizontalAlignment.Left;

                _pdfPage.Paragraphs.Add(lines[9]);
            }


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
