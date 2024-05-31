using Dapper;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using FTC_MES_MVC.Models.Dals.UserManage;
using FTC_MES_MVC.Models.UserManage;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace FTC_MES_MVC.Controllers
{
    public class ReportApiController : BaseApiController
    {
        dalReport oDal = new dalReport();
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }
        [HttpPost]
        public List<ReportForExcel> getReportForExcel(ReportQuery p_ReportQuery)
        {
            List<ReportForExcel> oReportForExcelList = new List<ReportForExcel>();
            try
            {
                oReportForExcelList = oDal.getReportForExcel(p_ReportQuery);
            }
            catch (Exception ex)
            {
                WriteLog_Error("getReportForExcel Error=" + ex.ToString());
            }
            return oReportForExcelList;
        }
        [HttpPost]
        public List<ReportForExcelTemplate> getReportForExcelTemplate()
        {
            List<ReportForExcelTemplate> oReportForExcelList = new List<ReportForExcelTemplate>();
            try
            {
                oReportForExcelList = oDal.getReportForExcelTemplate();
            }
            catch (Exception ex)
            {
                WriteLog_Error("getReportForExcelTemplate Error=" + ex.ToString());
            }
            return oReportForExcelList;
        }
        [HttpPost]
        public ApiReturnMessage UpdateSpreadValue(UpdateXlsData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdateSpreadValue(p_oUpdate, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateSpreadValue Error=" + ex.ToString());
            }
            return oApiReturnMessage;
        }
        [HttpPost]
        public HttpResponseMessage getExcelTemplateForRead(ReportQuery p_ReportQuery)
        {
            try
            {
                ReportForExcel oReportForExcel = new ReportForExcel();
                oReportForExcel=oDal.getSpreadValue(p_ReportQuery.DataID);

                List<SheetData> oSheetList = new List<SheetData>();
                SheetData oSheetData = new SheetData();
                oSheetData.SheetIndex = 0;
                RawData oRawData = new RawData();
                if (oReportForExcel!=null)
                {
                    foreach (var oItem in oReportForExcel.DATALIST)
                    {
                        oRawData = new RawData();
                        oRawData.RowIndex = oItem.ROWINDEX;
                        oRawData.CellIndex = oItem.CELLINDEX;
                        oRawData.CellData = oItem.CELLDATA;
                        oSheetData.DataList.Add(oRawData);
                    }
                    oSheetList.Add(oSheetData);
                }
                string sDestinationXlsFileName = $"{p_ReportQuery.ReportXlsFile.Split('.')[0]}_{User.Identity.Name}_ReadOnly_{DateTime.Now.ToString("yyyyMMddHHmmss")}.{p_ReportQuery.ReportXlsFile.Split('.')[1]}";
                sDestinationXlsFileName = SaveAsNewReport(p_ReportQuery.ReportXlsFile, sDestinationXlsFileName, oSheetList);
                if (File.Exists(sDestinationXlsFileName))
                {
                    byte[] fileBytes = File.ReadAllBytes(sDestinationXlsFileName);
                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // 指定MIME?型application/vnd.ms-excel
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = p_ReportQuery.ReportXlsFile
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("getExcelTemplateForRead Error=" + ex.ToString());
            }
            return null;
        }
        [HttpPost]
        public HttpResponseMessage getExcelTemplate(ReportQuery p_ReportQuery)
        {
            try
            {
                //------------建立測試資料---------------------
                List<SheetData> oSheetList = new List<SheetData>();
                SheetData oSheetData = new SheetData();
                oSheetData.SheetIndex = 0;
                RawData oRawData = new RawData();

                oRawData=ConvertCellNameToIndexes("A2");
                oRawData = ConvertCellNameToIndexes("B2");
                oRawData = ConvertCellNameToIndexes("C20");
                oRawData = ConvertCellNameToIndexes("AA20");

                oRawData.RowIndex = 2;
                oRawData.CellIndex = 2;
                oRawData.CellData = "bochi Test";
                oSheetData.DataList.Add(oRawData);
                oSheetList.Add(oSheetData);
                string sDestinationXlsFileName = $"{p_ReportQuery.ReportXlsFile.Split('.')[0]}_{User.Identity.Name}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.{p_ReportQuery.ReportXlsFile.Split('.')[1]}";
                sDestinationXlsFileName = SaveAsNewReport(p_ReportQuery.ReportXlsFile, sDestinationXlsFileName, oSheetList);
                if (File.Exists(sDestinationXlsFileName))
                {
                    byte[] fileBytes = File.ReadAllBytes(sDestinationXlsFileName);
                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // 指定MIME?型application/vnd.ms-excel
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = p_ReportQuery.ReportXlsFile
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
               WriteLog_Error("getExcelTemplate Error=" +  ex.ToString());
            }
            return null;
        }
        public string SaveAsNewReport(string sSourFileName, string sDestinationXlsFileName, List<SheetData> SheetList)
        {
            string path = HttpContext.Current.Server.MapPath("~/App_Data");
            string AreasPath = HttpContext.Current.Server.MapPath("~/Areas");
            string sSourFilePath = $@"{path}\Report\Template\{sSourFileName}";
            string sDestinationXlsFilePath = $@"{path}\Report\Template\History\{sSourFileName}".Replace(sSourFileName, sDestinationXlsFileName);
            string sDirectoryPath = $@"{path}\Report\Template\History";
            try
            {
                XSSFWorkbook excel;
                string sValue = "";
                //-------刪除超過7天的檔案----------------
                if (Directory.Exists(sDirectoryPath))
                {
                    string[] fileEntries = Directory.GetFiles(sDirectoryPath);
                    foreach (string FullfileName in fileEntries)
                    {
                        FileInfo fi = new FileInfo(FullfileName);
                        if (fi.LastWriteTime<DateTime.Now.AddDays(-1))
                        {
                            File.Delete(FullfileName);
                        }
                    }
                }
                if (File.Exists(sSourFilePath))
                {
                    using (FileStream Xlsfiles = new FileStream(sSourFilePath, FileMode.Open, FileAccess.Read))
                    {
                        excel = new XSSFWorkbook(Xlsfiles); // 將剛剛的Excel 讀取進入到工作簿中
                        Xlsfiles.Close();
                        IRow Row;
                        ICell cell;
                        ISheet sheet;
                        foreach (var qSheetData in SheetList)
                        {
                           if (qSheetData.SheetIndex< excel.NumberOfSheets)
                            {
                                sheet = excel.GetSheetAt(qSheetData.SheetIndex);
                                foreach (var qDataList in qSheetData.DataList)
                                {
                                    if (qDataList.RowIndex < sheet.LastRowNum )
                                    {
                                        Row = sheet.GetRow(qDataList.RowIndex);
                                        if (qDataList.CellIndex < Row.LastCellNum)
                                        {
                                            cell = Row.GetCell(qDataList.CellIndex);
                                            cell.SetCellValue(qDataList.CellData);
                                        }
                                    }
                                }
                            }
                        }
                        FileStream file = new FileStream(sDestinationXlsFilePath, FileMode.Create);//產生檔案
                        excel.Write(file);
                        file.Close();
                        cell = null;
                        Row = null;
                        sheet = null;
                        excel = null;
                      
                        return sDestinationXlsFilePath;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("SaveAsNewReport Error=" +  ex.ToString());
            }
            return sSourFilePath;
        }
        public RawData ConvertCellNameToIndexes(string cellName)
        {
            RawData oRawData = new RawData();
            int cellIndex = 0;
            int rowIndex = 0;
            try
            {
                if (!string.IsNullOrEmpty(cellName))
                {
                    string columnName = string.Empty;
                    int i = 0;
                    while (i < cellName.Length && char.IsLetter(cellName[i]))
                    {
                        columnName += cellName[i];
                        i++;
                    }
                    columnName = columnName.ToUpper();
                    cellIndex = 0;
                    for (int j = 0; j < columnName.Length; j++)
                    {
                        cellIndex = cellIndex * 26 + (columnName[j] - 'A' + 1);
                    }
                    rowIndex = int.Parse(cellName.Substring(i)) - 1;
                    oRawData.CellIndex = cellIndex-1  ;
                    oRawData.RowIndex = rowIndex;
                }
            }
            catch(Exception ex)
            {
                WriteLog_Error("ConvertCellNameToIndexes Error=" +  ex.ToString());
            }
            return oRawData;
        }
    }
}