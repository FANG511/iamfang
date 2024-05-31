using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Dapper;
using FTC_MES_MVC.Models.ViewModels;
using FTC_MES_MVC.Filters;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using FTC_MES_MVC.Models;

namespace FTC_MES_MVC.Controllers
{
    [Authentication]
    public class BaseController : Controller
    {
        protected  Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected  LogMessage oLogMessage=new LogMessage();
        protected System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        protected ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
        protected string MIS_YearType = ConfigurationManager.AppSettings["MIS_YearType"].ToString();
        protected string gsModifyUser { get; set; } = System.Web.HttpContext.Current.User.Identity.Name;
        protected string correctLogForging(string sMsgToWrite)
        {
            string retStr = sMsgToWrite.Normalize(NormalizationForm.FormKC); //正規化input string
            try
            {

                List<string> excludeList = new List<string>();                                            //應處理的換行符號
                excludeList.Add("%0a"); // \n
                excludeList.Add("%0A"); // \n
                excludeList.Add("%0d"); // \r
                excludeList.Add("%0D"); // \r
                excludeList.Add("\r");
                excludeList.Add("\n");
                excludeList.Add(@"\r");
                excludeList.Add(@"\n");
                foreach (string curItem in excludeList)
                    retStr = retStr.Replace(curItem, "");  //取代換行符號
            }
            catch (Exception ex)
            {

            }
            return retStr;
        }
        /// <summary>
        /// Action 開始觸發事件
        /// </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            oLogMessage.Error = "";

            sw.Start();
            base.OnActionExecuting(filterContext);
            // 參數資訊
            string parametersInfo = JsonConvert.SerializeObject(filterContext.ActionParameters, new JsonSerializerSettings()
            {
                ContractResolver = new ReadablePropertiesOnlyResolver()
            });
            string IP = filterContext.HttpContext.Request.UserHostAddress;
            oLogMessage.Status = "Start(" + IP + ")";
            oLogMessage.Info = string.IsNullOrEmpty(parametersInfo) ? "" : parametersInfo;

            string actionName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["action"].ToString() : "";
            string controllerName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["controller"].ToString() : "";
            string FuctionName = controllerName + "." + actionName;


            logger.Info(correctLogForging($"FuctionName={FuctionName} | Start | IP={IP} | parametersInfo={parametersInfo}"));
        }
        /// <summary>
        /// Action 結束觸發事件
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            sw.Stop();
            string sExpendTime = sw.Elapsed.TotalSeconds.ToString();
            base.OnActionExecuted(filterContext);
            string IP = filterContext.HttpContext.Request.UserHostAddress;
            oLogMessage.Status = "End";
            oLogMessage.Info = "ExpendTime=" + sExpendTime;
            // 參數資訊

            string actionName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["action"].ToString() : "";
            string controllerName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["controller"].ToString() : "";
            string FuctionName = controllerName + "." + actionName;
            logger.Info(correctLogForging($"FuctionName={FuctionName} | End | IP={IP} | ExpendTime={sExpendTime}"));
        }
        /// <summary>
        /// Action 結束開閉資料庫連線
        /// </summary>
        /// <param name="disposing"></param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //    }
        //    base.Dispose(disposing);

        //}
        
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Info(string Info)
        {
            string actionName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["action"].ToString() : "";
            string controllerName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["controller"].ToString() : "";
            string FuctionName = controllerName + "." + actionName;
            logger.Info(correctLogForging($"FuctionName={FuctionName} | Message={Info}"));
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Error(string Info)
        {
            string actionName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["action"].ToString() : "";
            string controllerName = this.ControllerContext != null ? this.ControllerContext.RouteData.Values["controller"].ToString() : "";
            string FuctionName = controllerName + "." + actionName;
            logger.Error(correctLogForging($"FuctionName={FuctionName} | Error={Info}"));
        }

        static string Aeskey = "XD5300@bochi";
        private class AesKeyIV
        {
            public Byte[] Key = new Byte[16];
            public Byte[] IV = new Byte[16];
            public AesKeyIV(string strKey)
            {
                var sha = new Sha256Digest();
                var hash = new byte[sha.GetDigestSize()];
                var data = Encoding.ASCII.GetBytes(strKey);
                sha.BlockUpdate(data, 0, data.Length);
                sha.DoFinal(hash, 0);
                Array.Copy(hash, 0, Key, 0, 16);
                Array.Copy(hash, 16, IV, 0, 16);
            }
        }
        public string AesEncrypt(string rawString)
        {
            try
            {
                var keyIv = new AesKeyIV(Aeskey);
                // Default - AES/GCM/NoPadding、System.Security.AES - AES/CBC/PKCS7
                var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
                cipher.Init(true, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var rawData = Encoding.UTF8.GetBytes(rawString);
                return Convert.ToBase64String(cipher.DoFinal(rawData));
            }
            catch (Exception ex)
            {
                return rawString;
            }
        }

        public string AesDecrypt(string encString)
        {
            try
            {
                if (encString.ToLower().Contains("data source"))
                {
                    return encString;
                }
                var keyIv = new AesKeyIV(Aeskey);
                // Default - AES/GCM/NoPadding、System.Security.AES - AES/CBC/PKCS7
                var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7");
                cipher.Init(false, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var encData = Convert.FromBase64String(encString);
                return Encoding.UTF8.GetString(cipher.DoFinal(encData));
            }
            catch (Exception ex)
            {
                return encString;
            }
        }
    }
    /// <summary>
    /// JsonSerializer 讀取屬性的解析器設定
    /// </summary>
    class ReadablePropertiesOnlyResolver : DefaultContractResolver
    {
        /// <summary>
        /// 建立可呈現（解析）的屬性
        /// </summary>
        /// <returns>呈現的屬性</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (typeof(Stream).IsAssignableFrom(property.PropertyType))
            {
                property.Ignored = true;
            }
            return property;
        }
        
    }
    

}
