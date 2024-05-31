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
using FTC_MES_MVC.Models.OrderDetail.ViewModels;
using FTC_MES_MVC.Models.DAL.OrderDetail;

namespace FTC_MES_MVC.Controllers
{   //建立類別apiOrderDetailController
    //繼承BaseApiController(共用的方法和屬性)
    public class apiOrderDetailController : BaseApiController
    {
        //建立dalOrderDetail的物件(oDal)來引用Model內dalOrderDetail物件的屬性及方法
        dalOrderDetail oDal = new dalOrderDetail();
        
        //釋放資源
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }
        public class DateRange
        {
            public string sStartDate { get; set; }
            public string sEndDate { get; set; }
        }
        [HttpPost]
        //建立清單 清單存取 回傳清單

        public List<dynamic> GetOrderDetail(DateRange dateRange)
        {
            //建立一個串列:mPersonData
            //串列內儲存的元素類型為PersonData
            List<dynamic> listReturn = new List<dynamic>();
            try
            {
                listReturn.Add (oDal.GetOrderPrice(dateRange.sStartDate, dateRange.sEndDate));
                listReturn.Add (oDal.GetOrderSummary(dateRange.sStartDate, dateRange.sEndDate));
            }
            catch (Exception ex)
            {
                WriteLog_Error("GetPersonData Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }





    }
}