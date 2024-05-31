
using FTC_MES_MVC.Models.ViewModels;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Routing;
namespace FTC_MES_MVC.Filters
{
    public class ApiLogAttribute : ActionFilterAttribute
    {
        protected Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected LogMessage oLogMessage = new LogMessage();
        protected System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
       
        /// <summary>
        /// Action 結束觸發事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            sw.Stop();
            string sExpendTime = sw.Elapsed.TotalSeconds.ToString();
            string controllerName = filterContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionContext.ActionDescriptor.ActionName;
            oLogMessage.FuctionName = controllerName + "." + actionName;
            oLogMessage.Status = "End";
            oLogMessage.Info = "ExpendTime=" + sExpendTime;

            WriteLog();

            try
            {
                var content = filterContext.Response.Content;
                var bytes = content?.ReadAsByteArrayAsync().Result;
                if (bytes != null && bytes.Length > 0)
                {
                    var acceptEncoding = filterContext.Request.Headers.AcceptEncoding.Where(x => x.Value == "gzip" || x.Value == "deflate").ToList();
                    byte[] zlibbedContent;
                    if (acceptEncoding.FirstOrDefault()?.Value == "deflate")
                    {
                        zlibbedContent = DeflateByte(bytes);
                        filterContext.Response.Content = new ByteArrayContent(zlibbedContent);
                        filterContext.Response.Content.Headers.Add("Content-Encoding", "deflate");
                    }
                    else
                    {
                        zlibbedContent = GZipByte(bytes);
                        filterContext.Response.Content = new ByteArrayContent(zlibbedContent);
                        filterContext.Response.Content.Headers.Add("Content-Encoding", "gzip");
                    }
                }
                filterContext.Response.Content.Headers.Add("Content-Type", "application/json");
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
            }


            base.OnActionExecuted(filterContext);
        }
        #region DotNetZip
        private static byte[] DeflateByte(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new DeflateStream(
                    output, CompressionMode.Compress))
                {
                    compressor.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        private byte[] GZipByte(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var compressor = new GZipStream(
                    output, CompressionMode.Compress))
                {
                    compressor.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }
        #endregion
        /// <summary>
        /// Action 開始觸發事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                string requestBody = "";
                using (var stream = new MemoryStream())
                {
                    var context = (HttpContextBase)filterContext.Request.Properties["MS_HttpContext"];
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    context.Request.InputStream.CopyTo(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                string strOrigin = filterContext.Request.Headers.GetValues("Host").FirstOrDefault();
                string controllerName = filterContext.ControllerContext.ControllerDescriptor.ControllerName;
                string actionName = filterContext.ActionDescriptor.ActionName;
                sw.Start();
                oLogMessage.FuctionName = controllerName + "." + actionName;
                oLogMessage.Status = "Start";
                oLogMessage.Info = string.IsNullOrEmpty(requestBody) ? "" : requestBody;
                oLogMessage.Error = "";
                WriteLog();
                bool blCheckDomain = AllowCrossSite(strOrigin);
                // 如果不存在允許的網域清單，就回傳自訂的錯誤訊息
                if (!blCheckDomain)
                {
                    UnauthorizedObject result = new UnauthorizedObject()
                    {
                        code = "401",
                        message = "domain is not allow"
                    };
                    filterContext.Response = filterContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized, result);
                }
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
            }
        }
        public class UnauthorizedObject
        {
            public string code { get; set; }
            public string message { get; set; }
        }
        static Boolean AllowCrossSite(string OriginUrl)
        {
            try
            {
                // 設定允許的網域清單
                List<string> strAllowDomain = new List<string>();
                string sAllowCrossSite = ConfigurationManager.AppSettings["AllowCrossSite"].ToString();
                if (!string.IsNullOrEmpty(sAllowCrossSite))
                {
                    if (sAllowCrossSite == "*")
                    {
                        return true;
                    }
                    else
                    {
                        strAllowDomain = sAllowCrossSite.Split(';').ToList();
                    }
                }
                else
                {
                    return true;
                }

                // 確認呼叫端的網域是否存在於允許的清單中
                bool blCheckDomain = strAllowDomain.Contains(OriginUrl);

                // 如果不存在允許的網域清單，就回傳自訂的錯誤訊息
                if (!blCheckDomain)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        /// <summary>
        /// 寫入log
        /// </summary>
        protected void WriteLog()
        {
            oLogMessage.RecTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            oLogMessage.Error = oLogMessage.Error == null ? "" : oLogMessage.Error;
            oLogMessage.Status = oLogMessage.Status == null ? "" : oLogMessage.Status;

            if (oLogMessage.Error != "")
            {
                logger.Error(correctLogForging($"FuctionName={oLogMessage.FuctionName} | Error={oLogMessage.Error}"));
            }
            else
            {
                logger.Info(correctLogForging($"FuctionName={oLogMessage.FuctionName} | Message={oLogMessage.Info}"));
            }

            oLogMessage.Error = "";
            oLogMessage.Status = "";
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

    }
}