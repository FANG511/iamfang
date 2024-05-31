using Dapper;
using FTC_MES_MVC.Models.ViewModels;
using NLog;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class dalBase : IDisposable
    {
        protected Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected LogMessage oLogMessage = new LogMessage();
        protected System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        protected SqlConnection cn ;
        protected ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
        protected string MIS_YearType = ConfigurationManager.AppSettings["MIS_YearType"].ToString();
        protected string connectionString = ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString();
        public dalBase()
        {
            try
            {
                if (cn == null)
                {
                    cn = new SqlConnection(AesDecrypt(connectionString));
                }
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("BaseDal", ex.ToString());
            }
        }
        public void Dispose()
        {
            try
            {
                if (cn != null)
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("Dispose",ex.ToString());
            }
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Info(string FuctionName, string Info)
        {
            logger.Info(correctLogForging($"FuctionName={FuctionName} | Message={Info}"));
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        protected void WriteLog_Error(string FuctionName, string Error)
        {
            logger.Error(correctLogForging($"FuctionName={FuctionName} | Error={Error}"));
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
        protected DateTime ToDate(String date)
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
    }
}