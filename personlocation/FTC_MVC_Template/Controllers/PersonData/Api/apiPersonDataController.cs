using Dapper;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.PerosonLocation.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FTC_MES_MVC.Models.DAL.dalPersonData;
using FTC_MES_MVC.Models.PerosonLocation;
using FTC_MES_MVC.Models.ViewModels;

namespace FTC_MES_MVC.Controllers
{   //建立類別apiPersonDataController
    //繼承BaseApiController(共用的方法和屬性)
    public class apiPersonDataController : BaseApiController
    {
        //建立dalPersonData的物件(oDal)來引用Model內dalPersonData物件的屬性及方法
        dalPersonData oDal = new dalPersonData();
        //釋放資源
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }

        // 接下來進行各種api

        /// <summary>
        /// 取得人員資料
        /// </summary>
        [HttpPost]
        //建立方法GetPersonData
        //傳遞PersonData物件作為參數
        //建立清單
        //清單存取
        //回傳清單
        public List<PersonData> GetPersonData(PersonData p_oQuery)
        {
            //建立一個串列:mPersonData
            //串列內儲存的元素類型為PersonData
            List<PersonData> mPersonData=new List<PersonData>();
         
        
            try
            {
                mPersonData = oDal.GetPersonData(p_oQuery);
             
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetPersonData Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return mPersonData;
        }

        /// <summary>
        /// 新增人員資料
        /// </summary>
        [HttpPost]
        //建立AddPersonData方法
        //建立oApiReturnMessage物件(負責儲存資料)
        //呼叫Models的AddPersonData方法
        //(傳入PersonData類別的參數p_oUpdate)
        //try{oApiReturnMessage儲存Models回傳的資料}
        //catch{oApiReturnMessage儲存錯誤訊息}
        //回傳oApiReturnMessage

        public ApiReturnMessage AddPersonData(PersonData p_oUpdate)
        {

            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                
                oApiReturnMessage = oDal.AddPerson(p_oUpdate);
                
                
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddPersonData Error" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除人員資料
        /// </summary>
        [HttpPost]
        //建立DeletePersonData方法
        //建立oApiReturnMessage物件(負責儲存資料)
        //呼叫Models的DelPersonData方法
        //(傳入PersonData類別的參數p_oUpdate)
        //try{oApiReturnMessage儲存Models回傳的資料}
        //catch{oApiReturnMessage儲存錯誤訊息}
        //回傳oApiReturnMessage
        public ApiReturnMessage DeletePersonData(PersonData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.DelPerson(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeletePersonData Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 更新人員資料
        /// </summary>
        [HttpPost]
        //建立UpdatePersonData方法
        //建立oApiReturnMessage物件(負責儲存資料)
        //呼叫Models的UpdPersonData方法
        //(傳入PersonData類別的參數p_oUpdate)
        //try{oApiReturnMessage儲存Models回傳的資料}
        //catch{oApiReturnMessage儲存錯誤訊息}
        //回傳oApiReturnMessage
        public ApiReturnMessage UpdatePersonData(PersonData p_oUpdate)
        {
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            try
            {
                oApiReturnMessage = oDal.UpdPerson(p_oUpdate);
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdatePersonData Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

       


      
    }
}