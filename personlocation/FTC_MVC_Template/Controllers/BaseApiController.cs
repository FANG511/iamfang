using FTC_MES_MVC.Filters;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json;
using FTC_MES_MVC.Models.ViewModels;
using System.Text;
using System.Configuration;
using System.Web.Http.Cors;
using System.Diagnostics;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.Dals;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace FTC_MES_MVC.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [ApiLog]
    public class BaseApiController : ApiController
    {
        protected Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected LogMessage oLogMessage = new LogMessage();
        protected System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        protected string MIS_YearType = ConfigurationManager.AppSettings["MIS_YearType"].ToString();
        protected ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
        protected string RecTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        protected string gsModifyUser { get; set; } = System.Web.HttpContext.Current.User.Identity.Name;
        
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
        /// 寫入log
        /// </summary>
        protected void WriteLog()
        {
            oLogMessage.RecTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            oLogMessage.Error = oLogMessage.Error == null ? "" : oLogMessage.Error;
            oLogMessage.Status = oLogMessage.Status == null ? "" : oLogMessage.Status;
            string actionName = this.ControllerContext.RouteData!=null? this.ControllerContext.RouteData.Values["action"].ToString() :"";
            string controllerName = this.ControllerContext.RouteData!= null ? this.ControllerContext.RouteData.Values["controller"].ToString():"";
            oLogMessage.FuctionName = controllerName + "." + actionName;
            if (oLogMessage.Error != "")
            {
                //logger.Error(JsonConvert.SerializeObject(oLogMessage));
                logger.Error(correctLogForging($"FuctionName={oLogMessage.FuctionName} | Error={oLogMessage.Error}"));
            }
            else
            {
                //logger.Info(JsonConvert.SerializeObject(oLogMessage));
                logger.Info(correctLogForging($"FuctionName={oLogMessage.FuctionName} | Message={oLogMessage.Info}"));
            }
            oLogMessage.Error = "";
            oLogMessage.Status = "";
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Info(string Info)
        {
            oLogMessage.Info = Info;
            oLogMessage.Status = "Run";
            WriteLog();
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Error(string Error)
        {
            oLogMessage.Error = Error;
            oLogMessage.Status = "Run";
            WriteLog();
        }
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
        
        
        protected double ToNumeric(object value)
        {
            double v = 0;
            try
            {
                Double.TryParse(value.ToString().Trim(), out v);
            }
            catch (Exception ex)
            {
            }
            return v;
        }
        protected Boolean IsNumeric(object value)
        {
            double v = 0;
            Boolean result = true;
            try
            {
                Double.TryParse(value.ToString().Trim(), out v);
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }

        protected DateTime CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }
        protected double ConvertInt(double val)
        {
            try
            {
                return Math.Round((val / 1000), 0, MidpointRounding.AwayFromZero);

            }
            catch
            {
                return val;
            }
        }

        /// <summary>
        /// GetRetMsg: 多語系錯誤訊息取得函數
        /// </summary>
        /// <param name="p_sMsgSeq">訊息序號(ReturnCode)</param>
        /// <param name="p_sZhTwRetMsg">預設訊息(繁體中文)</param>
        /// <param name="p_dictKeyValMap">繼承類別(Class)目前語系下的所有訊息字典(Key:ReturnCode,Value:ReturnMsg)</param>
        /// <returns>回傳指定訊息序號對應目前語系的訊息</returns>
        /// <remarks>
        /// 在ApiController中, 若有自行檢核的錯誤訊息要回傳(非自DAL接回的ReturnMsg),則需要呼叫此方法
        /// 使用步驟:
        /// 1.在ApiController的建構式中執行下列語法:
        /// 
        /// </remarks>
        protected string GetRetMsg(string p_sMsgSeq, string p_sZhTwRetMsg, string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
        {
            //ReturnMessageDal oRetMsgSvc = null;
            //try {
            //    var oMethodInfo = new StackTrace().GetFrame(1).GetMethod();

            //    string sClasName = oMethodInfo.ReflectedType.Name;
            //    string sMethodName = oMethodInfo.Name;
            //    string sMsgId = sClasName + "." + sMethodName + "." + p_sMsgSeq;

            //    oRetMsgSvc = new ReturnMessageDal(p_sUserId, p_sCompanyId, p_sFactoryId, p_sLng);
            //    var oRetMsgList = oRetMsgSvc.GetAllByClass(sClasName);
            //    var dictRetMsg = oRetMsgList.Where(c => c.LANGUAGE == p_sLng).ToDictionary(p => p.MESSAGEID);
            //    oRetMsgSvc.Dispose();

            //    if (!dictRetMsg.ContainsKey(sMsgId))
            //        return p_sZhTwRetMsg;
            //    else
            //        return dictRetMsg[sMsgId].RETURNMESSAGE;
            //}
            //catch (Exception ex) {
            //    return ex.Message;
            //}
            //finally
            //{
            //    if (oRetMsgSvc != null) { oRetMsgSvc.Dispose(); }
            //}
            return "";
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
        public static string AesEncrypt(string rawString)
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

        public static string AesDecrypt(string encString)
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
        //移除危險字元
        public static string replaceSQLChar(string sValue)
        {
            string sRetValue = String.Empty;
            if (sValue == String.Empty) return sRetValue;
            sRetValue = sValue.Normalize(NormalizationForm.FormKC);  //先正規化內容
            Dictionary<string, string> replaceList = new Dictionary<string, string>();
            replaceList.Add("'", "''"); replaceList.Add(";", "；");
            replaceList.Add(",", "，"); replaceList.Add("?", "？");
            replaceList.Add("<", "＜"); replaceList.Add(">", "＞");
            replaceList.Add("(", "（"); replaceList.Add(")", "）");
            replaceList.Add("@", "＠"); replaceList.Add("=", "＝");
            replaceList.Add("+", "＋"); replaceList.Add("*", "＊");
            replaceList.Add("&", "＆"); replaceList.Add("#", "＃");
            replaceList.Add("%", "％"); replaceList.Add("$", "￥");
            replaceList.Add("delete", "ｄｅｌｅｔｅ");
            replaceList.Add("Delete", "Ｄｅｌｅｔｅ");
            replaceList.Add("DELETE", "ＤＥＬＥＴＥ");
            replaceList.Add("update", "ｕｐｄａｔｅ");
            replaceList.Add("Update", "Ｕｐｄａｔｅ");
            replaceList.Add("UPDATE", "ＵＰＤＡＴＥ");
            replaceList.Add("insert", "ｉｎｓｅｒｔ");
            replaceList.Add("Insert", "Ｉｎｓｅｒｔ");
            replaceList.Add("INSERT", "ＩＮＳＥＲＴ");
            replaceList.Add("drop", "ｄｒｏｐ"); replaceList.Add("Drop", "Ｄｒｏｐ");
            replaceList.Add("DROP", "ＤＲＯＰ"); replaceList.Add("create", "ｃｒｅａｔｅ");
            replaceList.Add("Create", "Ｃｒｅａｔｅ");
            replaceList.Add("CREATE", "ＣＲＥＡＴＥ"); replaceList.Add("exec", "ｅｘｅｃ");
            replaceList.Add("Exec", "Ｅｘｅｃ"); replaceList.Add("EXEC", "ＥＸＥＣ");
            foreach (string curKey in replaceList.Keys)
                sRetValue = sRetValue.Replace(curKey, replaceList[curKey]);
            return sRetValue;
        }
    }
}
