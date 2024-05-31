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
namespace FTC_MES_MVC.Models.DAL.dalPersonData
{
    //建立類別dalPersonData
    //繼承 dalBase(共用的方法和屬性)

    public class dalPersonData : dalBase
    {
        protected string sSql { get; set; }

        /// <summary>
        /// 釋放所有API的資源
        /// </summary>
        protected void AllDispose()
        {
            Dispose();
        }

        /// <summary>
        /// 取得人員基本資料
        /// </summary>
        //GetPersonData方法:向資料庫查詢人員資料並回傳
        
        public List<PersonData> GetPersonData(PersonData p_oGet)
        {
            //建立物件
            List<PersonData> mPersonDataList = new List<PersonData>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                //資料庫查詢語法
                sbSql.Append(@"SELECT * FROM PersonData WHERE 1 = 1");

                //傳入的參數p_oGet不是空值
                if (p_oGet != null)
                {
                    //PersonID不可為空字串
                    if (!string.IsNullOrEmpty(p_oGet.PersonID))
                    {
                        //新增查詢PersonID語法的字串
                        //sbSql.Append(@" AND PersonID = @PersonID");
                        sbSql.Append(@" AND PersonID LIKE @PersonID+ '%'");


                        //p中添加一個參數:
                        //"@PersonID"參數名稱
                        //p_oGet.PersonID參數值
                        p.Add("@PersonID", p_oGet.PersonID);
                    }
                    //Name不可為空字串
                    if (!string.IsNullOrEmpty(p_oGet.Name))
                    {
                        //新增查詢Name語法的字串
                        sbSql.Append(@" AND Name =  @Name");

                        //p中添加一個參數
                        //"@Name"參數名稱
                        //p_oGet.Name參數值
                        p.Add("@Name", p_oGet.Name);
                    }
                    //Gender不可為空字串
                    if (!string.IsNullOrEmpty(p_oGet.Gender))
                    {
                        //新增查詢Gender語法的字串
                        sbSql.Append(@" AND Gender = @Gender");

                        //p中添加一個參數
                        //"@Gender"參數名稱
                        //p_oGet.Gender參數值
                        p.Add("@Gender", p_oGet.Gender);
                    }
                    //Phone不可為空字串
                    if (!string.IsNullOrEmpty(p_oGet.Phone))
                    {
                        //新增查詢Phone語法的字串
                        sbSql.Append(@"AND Phone = @Phone");

                        //p中添加一個參數
                        //"@Phone"參數名稱
                        //p_oGet.Phone參數值
                        p.Add("@Phone", p_oGet.Phone);

                    }
                    //CompanyID不可為空字串
                    if (!string.IsNullOrEmpty(p_oGet.CompanyID))
                    {
                        //新增查詢CompanyID語法的字串
                        sbSql.Append(@" AND CompanyID = @CompanyID");

                        //p中添加一個參數
                        //"@CompanyID"參數名稱
                        //p_oGet.CompanyID參數值
                        p.Add("@CompanyID", p_oGet.CompanyID);
                    }
                }
                //執行一個SQL查詢(查詢語法字串,參數)
                //將查詢結果傳輸到 PersonData 物件的列表中
                mPersonDataList= cn.Query<PersonData>(sbSql.ToString(), p).ToList();
                return mPersonDataList;
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

        /// <summary>
        ///  新增人員資料
        /// </summary>
        //
        public ApiReturnMessage AddPerson(PersonData p_oAdd)
        {
   
            try
            {
                //INSERT INTO:向PersonData資料表中的(欄位)新增數值
                //VALUES(具體的數值)
                sSql = @"INSERT INTO PersonData(PersonID, Name, Gender, Phone, CompanyId) 
                        VALUES (@PersonID,@Name,@Gender,@Phone,@CompanyID)";
                //執行SQL
                 cn.Execute(sSql, p_oAdd);
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            //釋放資源
            finally
            {
                AllDispose();
            }
            //回傳資料給Controller
            return oApiReturnMessage;
        }

        /// <summary>
        ///  刪除人員資料
        /// </summary>
        //建立刪除人員資料的方法
       public ApiReturnMessage DelPerson(PersonData p_oDel)
        {
            try
            {
                //sql刪除資料語法
                sSql = @"DELETE PersonData WHERE PersonID = @PersonID";

                //執行sql(語法,參數)
                cn.ExecuteScalar(sSql, p_oDel);
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            //釋放資源
            finally
            {
                AllDispose();
            }
            //回傳資料給Controller
            return oApiReturnMessage;
        }

        /// <summary>
        ///  更新人員資料
        /// </summary>
        //建立更新人員資料的方法
        public ApiReturnMessage UpdPerson(PersonData p_oUpd)
        {
            try
            {
                //sql更新資料語法
                sSql = @"UPDATE PersonData SET PersonID = @PersonID, Name = @Name, Gender = @Gender, Phone = @Phone, CompanyId = @CompanyID WHERE PersonID = @PersonID";
                
                //執行sql(語法,參數)
                cn.ExecuteScalar(sSql, p_oUpd);
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            //釋放資源
            finally
            {
                AllDispose();
            }
            //回傳資料給Controller
            return oApiReturnMessage;
        }

    }
}