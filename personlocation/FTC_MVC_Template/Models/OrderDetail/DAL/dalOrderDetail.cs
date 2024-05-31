using Dapper;
using FTC_MES_MVC.Models.PerosonLocation.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.ViewModels;
using System.Text;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using FTC_MES_MVC.Models.OrderDetail.ViewModels;

namespace FTC_MES_MVC.Models.DAL.OrderDetail
{
    public class dalOrderDetail : dalBase
    {
        protected string sSql { get; set; }

        /// <summary>
        /// 釋放所有API的資源
        /// </summary>
        protected void AllDispose()
        {
            Dispose();
        }
    

        public List<Order_Detail> GetOrderPrice(string p_sStartDate, string p_sEndDate)
        {
            //建立物件
            List<Order_Detail> mOrderDetailList = new List<Order_Detail>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                if(string.IsNullOrEmpty(p_sStartDate) || string.IsNullOrEmpty(p_sEndDate))
                {
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                    oApiReturnMessage.ReturnMessage = "查詢日期不可為空值!!";
                    return null;
                }

                //資料庫查詢語法            
                sbSql.Append(@"
                WITH DateRange AS (
                    SELECT CAST(@StartDate AS DATE) AS OrderDate
                    UNION ALL
                    SELECT DATEADD(DAY, 1, OrderDate)
                    FROM DateRange
                    WHERE OrderDate < @EndDate
                )
                SELECT 
                    CONVERT(NVARCHAR, ISNULL(a.OrderDate, dr.OrderDate), 23) AS OrderDate, 
                    ISNULL(SUM(ISNULL(b.Quantity, 0) * ISNULL(b.UnitPrice, 0)), 0) AS Total_price
                FROM DateRange dr
                LEFT JOIN [master].[dbo].[Orders] a ON a.OrderDate = dr.OrderDate
                LEFT JOIN [master].[dbo].[Order Details] b ON a.OrderID = b.OrderID
                GROUP BY ISNULL(a.OrderDate, dr.OrderDate)
                ORDER BY ISNULL(a.OrderDate, dr.OrderDate);"
                );

                 
                p.Add("@StartDate", p_sStartDate);
                p.Add("@EndDate", p_sEndDate);
                

                //執行一個SQL查詢(查詢語法字串,參數)
                //將查詢結果傳輸到 PersonData 物件的列表中
                mOrderDetailList = cn.Query<Order_Detail>(sbSql.ToString(), p).ToList();
                return mOrderDetailList;
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            //釋放資源
            finally
            {
                AllDispose();
            }
            return null;
        }

        public List<Order_Detail> GetOrderSummary(string p_sStartDate, string p_sEndDate)
        {
            //建立物件
            List<Order_Detail> mOrderDetailList = new List<Order_Detail>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                if (string.IsNullOrEmpty(p_sStartDate) || string.IsNullOrEmpty(p_sEndDate))
                {
                    oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                    oApiReturnMessage.ReturnMessage = "查詢日期不可為空值!!";
                    return null;
                }

                //資料庫查詢語法            
                sbSql.Append(@"
                SELECT * FROM	[master].[dbo].[v_OrderSummary]
                WHERE OrderDate >= @StartDate AND  OrderDate<= @EndDate;"
                );


                p.Add("@StartDate", p_sStartDate);
                p.Add("@EndDate", p_sEndDate);


                //執行一個SQL查詢(查詢語法字串,參數)
                //將查詢結果傳輸到 PersonData 物件的列表中
                mOrderDetailList = cn.Query<Order_Detail>(sbSql.ToString(), p).ToList();
                return mOrderDetailList;
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            //釋放資源
            finally
            {
                AllDispose();
            }
            return null;
        }


    }
}