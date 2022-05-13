using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.DataModel.CTO
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PDFExportAttribute : Attribute
    {
        public string headerText { get; set; }
        public string subHeaderText { get; set; }

        public string secondSubHeaderText { get; set; }

        public int width { get; set; }

        public int order { get; set; }

        /// <summary>
        /// None = 0,Left = 1,Center = 2,Right = 3,Justify = 4,
        /// </summary>
        public int hdrAlign { get; set; }

        /// <summary>
        /// None = 0,Left = 1,Center = 2,Right = 3,Justify = 4,
        /// </summary>
        public int cellAlign { get; set; }
        public string dateFormat { get; set; }
        public string orginalFeild { get; set; }
        public Type Type { get; set; }
        public string currencyFormat { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExcelExportAttribute : Attribute
    {
        public string secondSubHeaderText { get; set; }

        public string headerText { get; set; }

        public string subHeaderText { get; set; }
        public int size { get; set; }

        public int order { get; set; }

        /// <summary>
        /// None = 0,Left = 1,Center = 2,Right = 3,Justify = 4,
        /// </summary>
        public int HorizontalAlignment { get; set; }

        /// <summary>
        /// None = 0,Left = 1,Center = 2,Right = 3,Justify = 4,
        /// </summary>
        public int cellAlign { get; set; }
        public string dateFormat { get; set; }
        public string orginalFeild { get; set; }
        public Type Type { get; set; }
        public string currencyFormat { get; set; }
    }
}
