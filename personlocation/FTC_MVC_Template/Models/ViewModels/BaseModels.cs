using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.ViewModels
{

    /// <summary>
    /// MES Tool Input參數
    /// </summary>
    public class QueryTool
    {
        /// <summary>
        /// 公司編號 ex:2 南亞
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 工廠編號 ex:8900 銅箔二廠
        /// </summary>
        public string FactoryID { get; set; }
        /// <summary>
        /// table名稱
        /// </summary>
        public string TABLE_NAME { get; set; }
        public List<TableSchemaDescription> Tables;

    }

    public class BaseConfig
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string MODIFYUSER { get; set; }
        public string MODIFYTIME { get; set; }
    }

    public class BaseConfig2 : BaseConfig
    {
        public string QSTIME { get; set; }
        public string QETIME { get; set; } 
        public string QSTATUS { get; set; }
        public string SYSID { get; set; }
        public string GUID { get; set; } 
        public string spreadsheet { get; set; }
    }
    enum ReturnCode
    {
        [Description("Api 執行成功")]
        Success = 0,
        [Description("Api 檢查傳入是否為null")]
        InputParameterIsNull = 1,
        [Description("Api 檢查傳入是否為空值")]
        InputParameterIsEmpty = 2,
        [Description("Api 檢查傳入參數驗證錯誤")]
        InputParameterCheckFail = 3,
        [Description("Api 檢查傳入參數已經存在")]
        InputParameterAlreadyExists = 4,
        [Description("機台狀態不正確")]
        LotDispatchByEqpCheckStatusIsWrong = 5,
        [Description("機台容納數量不足")]
        LotDispatchByEqpCheckCapacityIsNotenough = 6,
        [Description("Lot狀態不正確")]
        LotDispatchByLootCheckStatusIsWrong = 7,
        [Description("Api 發生無預期錯誤")]
        Other = 99
    }
    enum LotStatus
    {

    }
    /// <summary>
    /// Table Schema 說明結構
    /// </summary>
    public class TableSchemaDescription
    {
        /// <summary>
        /// 表格名稱
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 資料型別
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 最大長度
        /// </summary>
        public string MaxLength { get; set; }
        /// <summary>
        /// 預設值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 允許空值
        /// </summary>
        public string isNullAble { get; set; }
        /// <summary>
        /// 欄位備註
        /// </summary>
        public string Description { get; set; }
    }
    public class ApiReturnMessage
    {
        public int ReturnResult { get; set; }
        public int ReturnCode { get; set; } = 0;
        public string ReturnMessage { get; set; } = "Success";
    }
    public class LogMessage
    {
        public string RecTime { get; set; }
        public string ProjectID = "";
        public string ProjectName ="";
        public string FuctionName { get; set; }
        public string Status { get; set; }
        public string Info { get; set; }
        public string Error { get; set; }
    }
    public class LanguageDefine
    {
        public string LanguageName { get; set; }
        public string LanguageValue { get; set; }
        public Boolean Defalut { get; set; }
    }
    public class MesApHostDb
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string ApHostIP { get; set; }
        public string ApDbPort { get; set; }
        public string ApDbName { get; set; }
        public string ApDbUser { get; set; }
        public string ApDbPswd { get; set; }
        public string AFServer { get; set; }
        public string AFDB { get; set; }
        public string PIServer { get; set; }
        public string AFUser { get; set; }
        public string AFPswd { get; set; }
        public string AFAlarmElementName { get; set; }
        public string QMSFactoryID { get; set; }
        public string CssUrl { get; set; }
    }
    public class OueryData
    {
        public string iestimate { get; set; }
        public string iassured { get; set; }
        public string iapplicant { get; set; }
        public string itag { get; set; }
        public string itel { get; set; }
        public string mobile { get; set; }
    }


    public class UpdateValue
    {
        /// <summary>
        /// 範本編號
        /// </summary>
        public string ValueList { get; set; }

    }
    public class KeyValue
    {
        public string Type { get; set; }
        public string SYSID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class spreadsheet
    {
        public string change { get; set; }
        public string activeSheet { get; set; }
        public Sheet[] sheets { get; set; }
        public object[] names { get; set; }
        public float columnWidth { get; set; }
        public float rowHeight { get; set; }
        public string Error { get; set; } = "";
        public List<KeyValue> QRCodeInfo = new List<KeyValue>();
        public string images { get; set; }
    }

    public class Drawing
    {
        public string topLeftCell { get; set; }
        public float offsetX { get; set; }
        public float offsetY { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public string image { get; set; }
    }
    public class Sheet
    {
        public string name { get; set; }
        public Row[] rows { get; set; }
        public Column[] columns { get; set; }
        public string selection { get; set; }
        public string activeCell { get; set; }
        public float frozenRows { get; set; }
        public float frozenColumns { get; set; }
        public bool showGridLines { get; set; }
        public object gridLinesColor { get; set; }
        public object[] mergedCells { get; set; }
        public object[] hyperlinks { get; set; }
        public Defaultcellstyle defaultCellStyle { get; set; }
        public List<Drawing> drawings { get; set; } = new List<Drawing>();
    }

    public class Defaultcellstyle
    {
        public string fontFamily { get; set; }
        public string fontSize { get; set; }
    }

    public class Row
    {
        public float index { get; set; }
        public float height { get; set; } = 20;
        public Cell[] cells { get; set; }
    }

    public class Cell
    {
        public string value { get; set; }
        public string fontFamily { get; set; }
        public string formula { get; set; }
        public string format { get; set; }
        public float fontSize { get; set; }
        public bool bold { get; set; }
        public string textAlign { get; set; } = "center";
        public string verticalAlign { get; set; } = "center";
        public Borderbottom borderBottom { get; set; }
        public int index { get; set; }
        public string background { get; set; }
        public Borderright borderRight { get; set; }
        public Bordertop borderTop { get; set; }
        public Borderleft borderLeft { get; set; }
        public validation validation { get; set; }
        public bool wrap { get; set; }
        public string color { get; set; }
    }

    public class Borderleft
    {
        public float size { get; set; }
        public string color { get; set; }
    }

    public class Borderright
    {
        public float size { get; set; }
        public string color { get; set; }
    }

    public class Bordertop
    {
        public float size { get; set; }
        public string color { get; set; }
    }

    public class Borderbottom
    {
        public float size { get; set; }
        public string color { get; set; }
    }

    public class Column
    {
        public float index { get; set; }
        public float width { get; set; }
    }
    public class validation
    {
        public string from { get; set; }
        public string to { get; set; }
        public bool allowNulls { get; set; }
        public string dataType { get; set; }
        public string messageTemplate { get; set; }

    }
    public class QuerySendMessage
    {
        public string COMPANYID { get; set; }
        public string FACTORYID { get; set; }
        public string CHANNETYPE { get; set; }
        public string SENDRESULT { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

    }


    public class SaveFile
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string contentType { get; set; }
        public string base64 { get; set; }
        public string fileName { get; set; }
    }
    public class SaveFileHtml
    {
        //[System.Web.Mvc.AllowHtml]
        public string Html { get; set; }
    }
    public class Company
    {

        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
    public class Factory
    {

        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string FactoryName { get; set; }
    }


    public class ItemList
    {
        public string SysID { get; set; }
        public string ListName { get; set; } = "";
        public string ItemIndex { get; set; } = "0";
        public string ItemName { get; set; } = "";
        public string ItemValue { get; set; } = "";
        public string ItemMemo { get; set; } = "";
        public string TargetType { get; set; } = "";
        public string Target { get; set; } = "";
        public DateTime ModifyDate { get; set; }
        public string ModifyUser { get; set; } = "";
    }

    public class ItemListQuery
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string ListName { get; set; }
        public string Type { get; set; }
        public string ItemMemo { get; set; }
        public string TargetType { get; set; }
        public string Target { get; set; }
        public string ItemName { get; set; }
        public string OrderBy { get; set; }
    }
    public class ItemListUpdate
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string ModifyUser { get; set; }
        public string TargetType { get; set; }
        public string Target { get; set; }
        public List<ItemList> ItemLists { get; set; }
    }
    /// <summary>
    /// Table:FTCMEQPVERSION
    /// </summary>
    public class EQPVERSION
    {
        public string EQPID { get; set; }
        public string EQPNAME { get; set; }
        public string EQPTYPE { get; set; }
    }
    public class QueryManageTool
    {
        /// <summary>
        /// 公司編號 ex:2 南亞
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 工廠編號 ex:8900 銅箔二廠
        /// </summary>
        public string FactoryID { get; set; }
        /// <summary>
        /// table名稱
        /// </summary>
        public string TABLE_NAME { get; set; }
        public string ModifyUser { get; set; }
        public int? id { get; set; }
        public string ObjectName { get; set; }

        public string ObjectValue { get; set; }
        public string ObjectDesc { get; set; }
        public List<PrpAttr> Attrs { get; set; }
    }
    public class PrpAttr
    {
        public string PARAMETERCODE { get; set; }
        public string PARAMETERVALUE { get; set; }
    }
    public partial class HighchartModel
    {

        public string title { get; set; }
        public string subtitle { get; set; }

        public string yAxis_title { get; set; }

        public xAxis xAxis { get; set; }

        public List<series> series
        {
            get;
            set;
        }
    }
    public partial class xAxis
    {
        public string[] categories { get; set; }

    }
    public partial class series
    {

        public string name { get; set; }
        public string type { get; set; }
        public int xAxis { get; set; }
        public int yAxis { get; set; }
        public List<dynamic> data { get; set; }

        public List<DataList> DataList
        {
            get;
            set;
        }
        public tooltip tooltip { get; set; }
        public int pointWidth { get; set; }

        public List<PieChartDataList> PieChartDataList
        {
            get;
            set;
        }
        public Boolean enableMouseTracking { get; set; }
        public string color { get; set; }
        public Double fillOpacity { get; set; }
    }

    public partial class DataList
    {
        public string name { get; set; }
        public DateTime RecTime { get; set; }
        public Double Value { get; set; }
        public Decimal MaxValue { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
    public partial class tooltip
    {
        public string valueSuffix { get; set; }
    }
    public partial class PieChartDataList
    {
        public string id { get; set; }
        public string name { get; set; }
        public Double y { get; set; }
    }
    public partial class SqlScriptList
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string DB { get; set; }
        public string ScriptName { get; set; }
    }
    public enum DalReturnCode
    {
        [Description("執行失敗")]
        Failed = -1,
        [Description("執行成功")]
        Success = 0,
        [Description("傳入參數驗證失敗")]
        InputParameterInvalid = 1,
        [Description("資料庫連線錯誤")]
        ConectionError = 2,
        [Description("異動資料不存在")]
        DataNotExisted = 3,
        [Description("新增資料已存在(重複)")]
        DataDuplicated = 4,
        [Description("邏輯錯誤")]
        LogicInvalid = 5,
        [Description("發生未預期錯誤")]
        Other = 99
    }
}