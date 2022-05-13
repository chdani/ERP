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
    public class ExcelGenerator<T> : ExportBase, IDisposable
    {
        public List<T> data;

        private Workbook _workbook;
        public string _headerText { get; set; }
        public IRepository _repository;
        public UserContext _userContext;
        public ILogger _logger;
        public ExcelGenerator() { }
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
            var LangMasterBC = new LangMasterBC(_logger, _repository);
            var Headertrans = LangMasterBC.GetLangBasedDataForCodeMaster(_userContext.Language);
            var codedetailscode = Headertrans.Where(a => a.Code == "EXPORTHEADER").FirstOrDefault();

            Type typeOfT = typeof(T);
            PropertyInfo[] propInfo = typeOfT.GetProperties();
            foreach (PropertyInfo property in propInfo)
            {
                var field = (ExcelExportAttribute)Attribute.GetCustomAttribute(property, typeof(ExcelExportAttribute), false);

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
            headerStyle.Font.Color = Color.White;
            headerStyle.Font.IsBold = true;
            headerStyle.Font.Size = 12;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.ForegroundColor = Color.Brown;
            Aspose.Cells.Font headerFont = headerStyle.Font;
            headerCell.SetStyle(headerStyle);


            int i = 0;
            foreach (var item in attributes)
            {
                cells[1, i].PutValue(item.headerText); // Add table headers
                Cell cell = cells.GetCell(1, i);
                cell.PutValue(item.headerText); // Add table headers
                Style style = cell.GetStyle();
                style.Font.Color = Color.White;
                style.Font.IsBold = true;
                style.Font.Size = 10;
                style.Pattern = BackgroundType.Solid;
                style.ForegroundColor = Color.Brown;
                Aspose.Cells.Font font = style.Font;
                cell.SetStyle(style);
                i++;
            }



            i = 1;
            int k = 0;
            foreach (var gdRow in data)
            {
                k = 0;
                foreach (var attr in attributes)
                {
                    var value = GetPropValue(gdRow, attr.orginalFeild.ToString());
                    if (attr != null && attr.Type != null && attr.Type.Name != "Nullable`1")
                    {
                        var propType = attr.Type;
                        var converter = TypeDescriptor.GetConverter(propType);
                        dynamic convertedObject = converter.ConvertFromString(value.ToString());
                        if (!string.IsNullOrEmpty(attr.dateFormat))
                        {
                            value = convertedObject.ToString(attr.dateFormat);

                        }
                        var row = 1 + i;

                        //Add data
                        cells[row, k].PutValue(value);
                        if (_headerText == "Direct Invoice - Prepayment" || _headerText == "Direct Invoice - Postpayment")
                        {
                            var Status = "Status";
                            var StatusColour = GetPropValue(gdRow, Status);
                            Cell cell = cells.GetCell(row, k);
                            Style style = cell.GetStyle();
                            if (StatusColour.ToString() == "SUBMITTED")
                            {
                                style.ForegroundColor = Color.Wheat;
                                style.Font.Color = Color.Black;
                            }
                            if (StatusColour.ToString() == "RETUREND")
                            {
                                style.ForegroundColor = Color.DarkRed;
                                style.Font.Color = Color.White;
                            }

                            if (StatusColour.ToString() == "APPROVED")
                            {
                                style.ForegroundColor = Color.LightGreen;
                                style.Font.Color = Color.Black;
                            }
                            style.Font.IsBold = true;
                            style.Font.Size = 10;
                            style.Pattern = BackgroundType.Solid;
                            cell.SetStyle(style);
                        }

                        if (_headerText == "Ledger Histroy")
                        {
                            var Credit = "Credit";
                            var Colour = GetPropValue(gdRow, Credit);
                            var Debit = "Debit";
                            var Colour1 = GetPropValue(gdRow, Debit);
                            Cell cell = cells.GetCell(row, k);
                            Style style = cell.GetStyle();
                            if ((decimal)Colour > 0)
                            {
                                style.ForegroundColor = Color.LightCyan;
                                style.Font.Color = Color.Black;
                            }
                            else if ((decimal)Colour1 > 0)
                                style.ForegroundColor = Color.Wheat;
                            style.Font.Color = Color.Black;

                            style.Font.IsBold = true;
                            style.Font.Size = 10;
                            style.Pattern = BackgroundType.Solid;
                            cell.SetStyle(style);
                        }
                        if (_headerText == "Stock transaction report")
                        {
                            var StockIn = "StockIn";
                            var Colour = GetPropValue(gdRow, StockIn);
                            var StockOut = "StockOut";
                            var Colour1 = GetPropValue(gdRow, StockOut);
                            Cell cell = cells.GetCell(row, k);
                            Style style = cell.GetStyle();
                            if ((decimal)Colour > 0)
                            {
                                style.ForegroundColor = Color.LightCyan;
                                style.Font.Color = Color.Black;
                            }
                            else if ((decimal)Colour1 > 0)
                                style.ForegroundColor = Color.Wheat;
                            style.Font.Color = Color.Black;

                            style.Font.IsBold = true;
                            style.Font.Size = 10;
                            style.Pattern = BackgroundType.Solid;
                            cell.SetStyle(style);
                        }
                    }



                    k++;
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
