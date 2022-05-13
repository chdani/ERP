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
    public class EmbPrePaymentPDFGenerator<T> : ExportBase, IDisposable
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
        public EmbPrePaymentPDFGenerator()
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
            List<PDFExportAttribute> attributes3 = new List<PDFExportAttribute>();
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
                    var secondSubHeaderText = codedetailscode.CodesDetail.Where(a => a.Code == field.secondSubHeaderText).FirstOrDefault();

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
                    if (secondSubHeaderText != null)
                    {
                        field.secondSubHeaderText = secondSubHeaderText.Description;
                        field.orginalFeild = property.Name;
                        field.Type = property.PropertyType;
                        attributes3.Add(field);
                    }
                }
            }

            attributes = attributes.OrderBy(st => st.order).ToList();
            attributes2 = attributes2.OrderBy(st => st.order).ToList();
            attributes3 = attributes3.OrderBy(st => st.order).ToList();

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
                foreach (Cell headerRowCell in headerRow.Cells)
                {
                    var Status = "Status";
                    var value = GetPropValue(gdRow, Status);
                    if (value.ToString() == "SUBMITTED")
                    {
                        headerRowCell.BackgroundColor = Color.Wheat;
                        headerRowCell.DefaultCellTextState.ForegroundColor = Color.Black;
                    }
                    if (value.ToString() == "APPROVED")
                    {
                        headerRowCell.BackgroundColor = Color.LightGreen;
                        headerRowCell.DefaultCellTextState.ForegroundColor = Color.Black;
                    }
                    if (value.ToString() == "RETURNED")
                    {
                        headerRowCell.BackgroundColor = Color.DarkRed;
                        headerRowCell.DefaultCellTextState.ForegroundColor = Color.White;
                    }


                }
                var headerRow3 = table.Rows.Add();
                var cell3 = new Cell();
               
                ICollection<EmbPrePaymentEmbDet> data = (ICollection<EmbPrePaymentEmbDet>)gdRow.GetType().GetProperty("EmbPrePaymentEmbDet").GetValue(gdRow, null);
                foreach (var det in data)
                {

                    foreach (var item2 in attributes2)
                    {
                        var value = new object();
                        if (item2.orginalFeild.ToString() == "DetRemarks")
                        {
                            var BA = "Remarks";
                            value = GetPropValue(det, BA);
                        }
                        else
                        {

                            value = GetPropValue(det, item2.orginalFeild.ToString());
                        }

                        var propType = item2.Type;
                        var converter = TypeDescriptor.GetConverter(propType);
                        dynamic convertedObject = converter.ConvertFromString(value.ToString());
                        if (!string.IsNullOrEmpty(item2.dateFormat))
                        {
                            value = convertedObject.ToString(item2.dateFormat);
                        }
                        if (!string.IsNullOrEmpty(item2.currencyFormat))
                        {
                            value = convertedObject.ToString(item2.currencyFormat);
                        }
                        cell3 = headerRow3.Cells.Add(item2.subHeaderText + " : " + value.ToString());
                        if (item2.hdrAlign != 0)
                        {
                            cell3.Alignment = (HorizontalAlignment)item2.hdrAlign;
                        }


                       

                    }
                    foreach (Cell headerRowCell in headerRow3.Cells)
                    {

                        headerRowCell.BackgroundColor = Color.LavenderBlush;
                        headerRowCell.DefaultCellTextState.ForegroundColor = Color.Black;



                    }                   
                    
                    var row2 = table.Rows.Add();
                    var headerRow1 = table.Rows.Add();
                    var cell1 = new Cell();
                    foreach (var item1 in attributes3)
                    {

                        cell1 = headerRow1.Cells.Add(item1.secondSubHeaderText);
                        if (item1.hdrAlign != 0)
                        {
                            cell1.Alignment = (HorizontalAlignment)item1.hdrAlign;
                        }

                    }
                   
                    foreach (Cell headerRowCell2 in headerRow1.Cells)
                    {
                        headerRowCell2.BackgroundColor = Color.LightSkyBlue;
                        headerRowCell2.DefaultCellTextState.ForegroundColor = Color.Black;
                    }
                    ICollection<EmbPrePaymentInvDet> data2 = (ICollection<EmbPrePaymentInvDet>)det.GetType().GetProperty("EmbPrePaymentInvDet").GetValue(det, null);

                    foreach (var item3 in data2)
                    {
                       
                        row2 = table.Rows.Add();
                        foreach (var attr in attributes3)
                        {
                            var value1 = new object();

                            if (attr.orginalFeild.ToString() == "InvAmount")
                            {
                                var BA = "Amount";
                                value1 = GetPropValue(det, BA);
                            }
                            else if(attr.orginalFeild.ToString() == "InvRemarks")
                            {
                                var BA = "Remarks";
                                value1 = GetPropValue(det, BA);
                            }
                            else
                            {

                                value1 = GetPropValue(item3, attr.orginalFeild.ToString());
                            }

                            

                            var propType2 = attr.Type;
                            var converter2 = TypeDescriptor.GetConverter(propType2);
                            dynamic convertedObject2 = converter2.ConvertFromString(value1.ToString());

                            if (!string.IsNullOrEmpty(attr.currencyFormat))
                            {
                                value1 = convertedObject2.ToString(attr.currencyFormat);
                            }

                            if (!string.IsNullOrEmpty(attr.dateFormat))
                            {
                                value1 = convertedObject2.ToString(attr.dateFormat);
                            }

                            cell1 = row2.Cells.Add(value1.ToString());

                            if (attr.cellAlign != 0)
                            {
                                cell1.Alignment = (HorizontalAlignment)attr.cellAlign;
                            }
                        }

                    }
                    headerRow3 = table.Rows.Add();
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

