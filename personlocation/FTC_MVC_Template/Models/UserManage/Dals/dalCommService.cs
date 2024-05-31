using Dapper;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Models.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.Services
{
    public class dalCommService : dalBase
    {
        protected string sSql { get; set; }
        DynamicParameters p = new DynamicParameters();
        /// <summary>
        /// 取得MesApHostDb設定的公司及工廠基礎設定資料
        /// </summary>
        /// <returns></returns>
        public List<MesApHostDb> getMesApHostDb()
        {
            try
            {
                sSql = @"SELECT * FROM MesApHostDb";
                return cn.Query<MesApHostDb>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public List<LanguageDefine> getLanguageDefine()
        {
            
            List<LanguageDefine> LanguageDefines = new List<LanguageDefine>();
            LanguageDefine oLanguageDefine = new LanguageDefine();
            oLanguageDefine.LanguageName = "繁體";
            oLanguageDefine.LanguageValue = "zh-TW";
            oLanguageDefine.Defalut =true;
            LanguageDefines.Add(oLanguageDefine);
            try
            {
                sSql = @"SELECT * FROM LanguageDefine";
                LanguageDefines= cn.Query<LanguageDefine>(sSql, p).ToList();
                 
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return LanguageDefines;
        }
        public List<COMM_FUNCTIONLIST> getFunctionList()
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_FUNCTIONLIST WHERE PARENTID IS NOT NULL";
                return cn.Query<COMM_FUNCTIONLIST>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public int chkOldPassword(string sUserId, string sHashPwd)
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_USER WHERE USERID = @USERID AND PWD = @PWD";
                p.Add("@USERID", sUserId);
                p.Add("@PWD", sHashPwd);
                return cn.Query<COMM_USER>(sSql, p).Any() ? 1 : 0;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public List<string> getAuthorizedNode(string[] sarrRole)
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_ROLENODE WHERE ROLEID IN @ROLEID";
                p.Add("@ROLEID", sarrRole);
                return cn.Query<COMM_ROLENODE>(sSql, p).ToList().Select(o => o.NODEID).ToList();
            }
            catch(Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public string getNodeIdByUrl(string sUrl)
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_FUNCTIONLIST WHERE NODEURL LIKE @NODEURL";
                p.Add("@NODEURL", sUrl + "%");
                var qData = cn.Query<COMM_FUNCTIONLIST>(sSql, p).ToList();
                return qData.Count == 0 ? "" : qData.FirstOrDefault().NODEID;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public List<string> getRoleByUserId(string sUserId)
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_USER_HAS_ROLES WHERE USERID = @USERID";
                p.Add("@USERID", sUserId);
                return cn.Query<COMM_USER_HAS_ROLES>(sSql, p).ToList().Select(o => o.ROLEID).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public COMM_USER chkUserPwd(string sUserId, string sPwd)
        {
            
            try
            {
                sSql = @"SELECT * FROM COMM_USER WHERE USERID = @USERID AND PWD = @PWD";
                p.Add("@USERID", sUserId);
                p.Add("@PWD", sPwd);
                return cn.Query<COMM_USER>(sSql, p).FirstOrDefault();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public COMM_USER getUser(string sUserId)
        {
            
            try
            {
                sSql = "SELECT * FROM COMM_USER WHERE USERID = @USERID";
                p.Add("@USERID", sUserId);
                return cn.Query<COMM_USER>(sSql, p).First();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public void updateUserPwd(string sUserId, string sPwd)
        {
            
            try
            {
                sSql = @"UPDATE COMM_USER SET PWD = @PWD WHERE USERID = @USERID";
                p.Add("@PWD", sPwd);
                p.Add("@USERID", sUserId);
                cn.ExecuteScalar(sSql, p);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public List<COMM_USER_HAS_ROLES> getAllUserWithRole()
        {
            
            try
            {
                return cn.Query<COMM_USER_HAS_ROLES>("SELECT * FROM COMM_USER_HAS_ROLES ORDER BY ROLEID").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public List<COMM_ROLE> getAllRoles()
        {
            
            try
            {
                return cn.Query<COMM_ROLE>("SELECT * FROM COMM_ROLE").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public List<string> getUserRole(string sUserId)
        {
            
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@USERID", sUserId);
                return cn.Query<COMM_USER_HAS_ROLES>("SELECT * FROM COMM_USER_HAS_ROLES WHERE USERID = @USERID",p).ToList().Select(o => o.ROLEID).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }

        public bool getNodeIsReference(string sNodeId)
        {
            
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@NODEID", sNodeId);
                var q = cn.Query<COMM_FUNCTIONLIST>("SELECT * FROM COMM_FUNCTIONLIST WHERE NODEID =@NODEID", p).FirstOrDefault();
                if (q != null)
                {
                    return q.ISREFERENCE;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
        }
        public void updateFunctionList(string ComFacID)
        {
            try
            {
                List<COMM_FUNCTIONLIST> ltFunction = getFunctionList();
                foreach(var item in ltFunction)
                {
                    string[] parameter = item.NODEURL.Split('&');
                    for (int i = 0; i < parameter.Length; i++)
                    {
                        if(parameter[i].ToUpper().Contains("COMPANYID="))
                        {
                            parameter[i] = "CompanyID=" + ComFacID.Split('-')[0];
                        }
                        if (parameter[i].ToUpper().Contains("FACTORYID="))
                        {
                            parameter[i] = "FactoryID=" + ComFacID.Split('-')[1];
                        }
                    }
                    item.NODEURL = string.Join("&", parameter);
                }
                sSql = string.Format(@"UPDATE dbo.COMM_FUNCTIONLIST
                                    SET NODEURL = @NODEURL
                                    WHERE NODEID = @NODEID");
                cn.Execute(sSql, ltFunction);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
        }

        public string getUserPwd(string sUserId)
        {
            
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@sUserId", sUserId);
                return cn.Query<COMM_USER>("SELECT * FROM COMM_USER WHERE USERID = @USERID", p).First().PWD;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return "";
        }
        public string getFactoryName(string CompanyID,string FactoryID)
        {
            
            try
            {
                DynamicParameters p = new DynamicParameters();
                p.Add("@CompanyID", CompanyID);
                p.Add("@FactoryID", FactoryID);
                var q = cn.Query<Factory>("SELECT * FROM Factory WHERE CompanyID=@CompanyID and FactoryID=@FactoryID",p).FirstOrDefault();
                return q!=null?q.FactoryName:"";
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return "";
        }
    }
}