using Dapper;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Models;
using System.Text;
using System.Security.Cryptography;
using FTC_MES_MVC.Models.UserManage;

namespace FTC_MES_MVC.Models.Dals.UserManage
{
    /// <summary>
    /// Com DAL，放SQL CMD
    /// </summary>
    public class dalReport : dalBase
    {
        string sSql = "";
        DynamicParameters p = new DynamicParameters();

        /// <summary>
        /// 釋放全部API的資源
        /// </summary>
        protected void AllDispose()
        {
            Dispose();
        }
        public ApiReturnMessage UpdateSpreadValue(UpdateXlsData p_oUpdate, string sIdentity_Name)
        {
            try
            {
                DynamicParameters p = new DynamicParameters();
                ReportForExcel oReportForExcel = new ReportForExcel();
                oReportForExcel.RPTID = p_oUpdate.XlsFileName;
                oReportForExcel.RPTNAME = p_oUpdate.XlsFileName;
                oReportForExcel.TEMPLATENAME = p_oUpdate.XlsFileName;
                oReportForExcel.MODIFYUSER = sIdentity_Name;
                ReportForExcelDetailData oReportForExcelDetailData;
                List<ReportForExcelDetailData> oDataList = new List<ReportForExcelDetailData>();
                List<int> RowIndexList = new List<int>();
                List<int> cellIndexList = new List<int>();
                foreach (var oItem in p_oUpdate.CellItems)
                {
                    oReportForExcelDetailData=new ReportForExcelDetailData();
                    oReportForExcelDetailData.DATAID = string.IsNullOrEmpty(p_oUpdate.DATAID)? oReportForExcel.DATAID: p_oUpdate.DATAID;
                    oReportForExcelDetailData.SHEETINDEX = 0;
                    oReportForExcelDetailData.ROWINDEX = (int)ToNumeric(oItem.Key.Split('-')[0]);
                    RowIndexList.Add(oReportForExcelDetailData.ROWINDEX);
                    oReportForExcelDetailData.CELLINDEX = (int)ToNumeric(oItem.Key.Split('-')[1]);
                    cellIndexList.Add(oReportForExcelDetailData.CELLINDEX);
                    oReportForExcelDetailData.CELLDATA = oItem.Value;
                    oDataList.Add(oReportForExcelDetailData);
                }
                if (string.IsNullOrEmpty(p_oUpdate.DATAID))
                {
                    sSql = @"INSERT INTO ReportForExcel (RPTID,RPTNAME,TEMPLATENAME,MODIFYUSER,MODIFYTIME,DATAID)
                        VALUES (@RPTID,@RPTNAME,@TEMPLATENAME,@MODIFYUSER,@MODIFYTIME,@DATAID)";
                    cn.Execute(sSql, oReportForExcel);
                    sSql = @"INSERT INTO ReportForExcelDetailData (DATAID,SHEETINDEX,ROWINDEX,CELLINDEX,CELLDATA)
                        VALUES (@DATAID,@SHEETINDEX,@ROWINDEX,@CELLINDEX,@CELLDATA)";
                    cn.Execute(sSql, oDataList);
                }
                else
                {
                    sSql = @"update ReportForExcel set MODIFYUSER=@MODIFYUSER where DATAID=@DATAID";
                    cn.Execute(sSql, oReportForExcel);
                    p.Add("DATAID", p_oUpdate.DATAID);
                    p.Add("ROWINDEX", RowIndexList);
                    p.Add("CELLINDEX", cellIndexList);
                    sSql = @"delete ReportForExcelDetailData where DATAID=@DATAID and ROWINDEX in @ROWINDEX and CELLINDEX in @CELLINDEX";
                    cn.Execute(sSql, p);

                    sSql = @"INSERT INTO ReportForExcelDetailData (DATAID,SHEETINDEX,ROWINDEX,CELLINDEX,CELLDATA)
                        VALUES (@DATAID,@SHEETINDEX,@ROWINDEX,@CELLINDEX,@CELLDATA)";
                    cn.Execute(sSql, oDataList);
                }
             
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }
        public ReportForExcel getSpreadValue(string p_DataID)
        {
            ReportForExcel oReportForExcel = new ReportForExcel();
            try
            {
                if (string.IsNullOrEmpty(p_DataID))
                {
                    return oReportForExcel;
                }
                DynamicParameters p = new DynamicParameters();
                p.Add("DATAID", p_DataID);
                sSql = @"select * from ReportForExcel where DATAID=@DATAID";
                oReportForExcel=cn.Query<ReportForExcel>(sSql, p).FirstOrDefault();
                sSql = @"select * from ReportForExcelDetailData where DATAID=@DATAID";
                var qDataList = cn.Query<ReportForExcelDetailData>(sSql, p).ToList();
                oReportForExcel.DATALIST = qDataList;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return oReportForExcel;
        }
        public List<ReportForExcelTemplate> getReportForExcelTemplate()
        {
            List<ReportForExcelTemplate> oReportForExcelList = new List<ReportForExcelTemplate>();
            try
            {
                DynamicParameters p = new DynamicParameters();
                sSql = @"select * from ReportForExcelTemplate";
                oReportForExcelList = cn.Query<ReportForExcelTemplate>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return oReportForExcelList;
        }
        public List<ReportForExcel> getReportForExcel(ReportQuery p_ReportQuery)
        {
            List<ReportForExcel> oReportForExcelList = new List<ReportForExcel>();
            try
            {
                DynamicParameters p = new DynamicParameters();
                sSql = @"select * from ReportForExcel where 1=1";
                if (!string.IsNullOrEmpty(p_ReportQuery.ReportXlsFile))
                {
                    p.Add("@ReportXlsFile", p_ReportQuery.ReportXlsFile);
                    sSql = sSql + " and TEMPLATENAME=@ReportXlsFile";
                }
                if (!string.IsNullOrEmpty(p_ReportQuery.DataID))
                {
                    p.Add("@DATAID", p_ReportQuery.DataID);
                    sSql = sSql + " and DATAID=@DATAID";
                }
                if (!string.IsNullOrEmpty(p_ReportQuery.StartTime))
                {
                    p.Add("@StartTime", p_ReportQuery.StartTime);
                    sSql = sSql + " and MODIFYTIME>=@StartTime";
                }
                if (!string.IsNullOrEmpty(p_ReportQuery.EndTime))
                {
                    p.Add("@EndTime", ToDate(p_ReportQuery.EndTime).AddDays(1).ToString("yyyy/MM/dd HH:mm:ss"));
                    sSql = sSql + " and MODIFYTIME<@EndTime";
                }
                oReportForExcelList = cn.Query<ReportForExcel>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return oReportForExcelList;
        }
    }
}