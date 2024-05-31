using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.ViewModels
{
    public class ReportQuery
    {
        public string ReportXlsFile { get; set; }
        public string DataID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
   
    public class WorkBookData
    {
        public List<SheetData> SheetList { get; set; }
    }
    public class SheetData
    {
        public int SheetIndex { get; set; } = 0;
        public List<RawData> DataList { get; set; } = new List<RawData>();
    }
    public class RawData
    {
        public int RowIndex { get; set; } = 0;
        public int CellIndex { get; set; } = 0;
        public string CellData { get; set; } = "";
    }
    public class ReportForExcel
    {
        public string RPTID { get; set; }
        public string RPTNAME { get; set; }
        public string TEMPLATENAME { get; set; }
        public string MODIFYUSER { get; set; }
        public DateTime MODIFYTIME { get; set; } = DateTime.Now;
        public string DATAID { get; set; } = Guid.NewGuid().ToString();
        public List<ReportForExcelDetailData> DATALIST = new List<ReportForExcelDetailData>();
    }
    public class ReportForExcelDetailData
    {
        public string DATAID { get; set; }
        public int SHEETINDEX { get; set; }
        public int ROWINDEX { get; set; }
        public int CELLINDEX { get; set; }
        public string CELLDATA { get; set; }
    }
    public class ReportForExcelTemplate
    {
        public string RPTID { get; set; }
        public string RPTNAME { get; set; }
        public string TEMPLATENAME { get; set; }
    }
    public class UpdateXlsData
    {
        public string XlsFileName { get; set; }
        public List<KeyValue> CellItems { get; set; }
        public string DATAID { get; set; }
    }
}