using Dapper;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using FTC_MES_MVC.Models.Dals.UserManage;
using FTC_MES_MVC.Models.UserManage;
using System.Dynamic;

namespace FTC_MES_MVC.Controllers
{
    public class UserManageApiController : BaseApiController
    {
        dalUserManageService oDal = new dalUserManageService();
        protected string sSql { get; set; }
        DynamicParameters p = new DynamicParameters();
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }
        /// <summary>
        /// 取得使用者在頁面的權限列表 有表示說該頁面有下列權限；isvisable => 0 1 表示是否可以看見
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public List<ControlAuthForPage> R_PageControl(BasicInfo o)
        {
            try
            {
                return oDal.R_PageControl(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
                throw ex;
            }
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage addUser(COMM_USER o)
        {
            try
            {
                if (string.IsNullOrEmpty(o.PWD))
                { throw new Exception("NoPassword!"); }

                oApiReturnMessage = oDal.C_User(o, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                if (string.IsNullOrEmpty(ex.Message))
                    oApiReturnMessage.ReturnMessage = ex.ToString();
                else
                    oApiReturnMessage.ReturnMessage = ex.Message;
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 更新使用者
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage updateUser(COMM_USER o)
        {
            try
            {
                if (string.IsNullOrEmpty(o.PWD))
                { throw new Exception("NoPassword!"); }

                oApiReturnMessage = oDal.U_User(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                if (string.IsNullOrEmpty(ex.Message))
                    oApiReturnMessage.ReturnMessage = ex.ToString();
                else
                    oApiReturnMessage.ReturnMessage = ex.Message; ;
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage deleteUser(COMM_USER o)
        {
            try
            {
                oApiReturnMessage = oDal.D_User(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 取得網頁Node權限
        /// </summary>
        /// <param name="sRole"></param>
        /// <returns></returns>
        [HttpGet]
        public List<string> GetRoleNode(string sRole)
        {
            try
            {
                return oDal.R_RoleNode(sRole);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }
        /// <summary>
        /// 更新網頁Node權限
        /// </summary>
        /// <param name="sCheckedNode"></param>
        /// <param name="sRole"></param>
        /// <param name="sAddionalAuth"></param>
        /// <returns></returns>
        [HttpGet]
        public ApiReturnMessage UpdateRoleNode(string sCheckedNode, string sRole, string sAddionalAuth)
        {
            try
            {
                oApiReturnMessage = oDal.U_RoleNode(sCheckedNode, sRole, sAddionalAuth);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 取得人員對應的群組
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<COMM_ROLE> getRoles()
        {
            try
            {
                return oDal.R_Roles();
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }
        /// <summary>
        /// 更新人員對應的群組
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage updateRole(COMM_ROLE o)
        {
            try
            {
                oApiReturnMessage = oDal.U_Role(o, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 刪除人員對應的群組
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage deleteRole(COMM_ROLE o)
        {
            try
            {
                oApiReturnMessage = oDal.D_Role(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 新增人員對應的群組
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage addRole(COMM_ROLE o)
        {
            try
            {
                oApiReturnMessage = oDal.C_Role(o, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 取得人員對應的權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<COMM_USER_HAS_ROLES> getUserHasRoles(COMM_USER_HAS_ROLES o)
        {
            try
            {
                return oDal.R_UserHasRoles(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }
        /// <summary>
        /// 更新人員對應的權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage updateUserHasRoles(COMM_USER_HAS_ROLES o)
        {
            try
            {
                oApiReturnMessage = oDal.U_UserHasRoles(o, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }
        /// <summary>
        /// 刪除人員對應的權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage deleteUserHasRoles(COMM_USER_HAS_ROLES o)
        {
            try
            {
                oApiReturnMessage = oDal.D_UserHasRoles(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增人員對應的權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage addUserHasRoles(COMM_USER_HAS_ROLES o)
        {
            try
            {
                oApiReturnMessage = oDal.C_UserHasRoles(o, User.Identity.Name);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增人員對應的權限 (批次)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage UC_UserHasRoles(COMM_USER_HAS_ROLES oo)
        {
            try
            {
                List<COMM_USER_HAS_ROLES> lWant = JsonConvert.DeserializeObject<List<COMM_USER_HAS_ROLES>>(oo.sJson);

                var lAll = oDal.R_UserHasRoles(new COMM_USER_HAS_ROLES {ROLEID = replaceSQLChar(oo.ROLEID)});

                lWant.ForEach(x => x.ROLEID = oo.ROLEID); //設定ROLEID

                // 列出與lAll的差異
                List<COMM_USER_HAS_ROLES> lDelete = lAll.Where(x=> lWant.Where(z=>z.USERID ==x.USERID).Count()==0).ToList();

                // 列出與lWant的差異
                List<COMM_USER_HAS_ROLES> lInsert = lWant.Where(x => lAll.Where(z => z.USERID == x.USERID).Count() == 0).ToList();

                //找出差異 並做新增與刪除
                foreach (var o in lDelete)
                {
                    oApiReturnMessage = oDal.D_UserHasRoles(o);
                }

                foreach (var o in lInsert)
                {
                    oApiReturnMessage = oDal.C_UserHasRoles(o, User.Identity.Name);
                }                

            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得 額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<COMM_NODEFUNCTION_P> R_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                return oDal.R_COMM_NODEFUNCTION(o);
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
                throw ex;
            }
        }

        /// <summary>
        /// 更新額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage U_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                oDal.U_COMM_NODEFUNCTION(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage C_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                oDal.C_COMM_NODEFUNCTION(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage D_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                oDal.D_COMM_NODEFUNCTION(o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 檢查Comdb資料表是否齊全，若沒有會自動補建
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage CheckComdb()
        {
            try
            {
                oDal.CheckCOMM_ROLENODE_Exist();//檢查COMM_ROLENODE_HAS_AUTH
                oDal.CheckCOMM_NODEFUNCTION_TABLE();//調整欄位大小
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 取得 所有 Node
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> R_COMM_FUNCTIONLIST(ComFunAndAuth o)
        {
            try
            {
                List<dynamic> l = new List<dynamic>();
                l = oDal.R_COMM_FUNCTIONLIST(o.Function);

                //做父節點group 記得select new 要給 屬性名稱
                var pGroup = l
                    .GroupBy(x => x.PARENTID)
                    .Where(p => !string.IsNullOrEmpty(p.Key))
                    .Select(z => new { PARENTID = z.Key })
                    .ToList();

                //var test = pGroup.Where(z => z.PARENTID == "N100" || z.PARENTID == "N100");

                //l.ForEach(x => x.test = "test");

                //NodeID有 或者 PARENTID 在父節點範圍內，則給予在 PGROUP內                
                l.ForEach(x => x.PGROUP =
                    pGroup
                    .Where(z => z.PARENTID == x.NODEID || z.PARENTID == x.PARENTID)
                    .Any() ?
                    pGroup
                    .Where(z => z.PARENTID == x.NODEID || z.PARENTID == x.PARENTID)
                    .First().PARENTID : ""
                );

                List<COMM_NODE_HAS_CONTROL> lAuths = new List<COMM_NODE_HAS_CONTROL>();
                lAuths = oDal.R_COMM_NODE_HAS_CONTROL(new COMM_NODE_HAS_CONTROL { });

                //相同NODEID的放入
                l.ForEach(x => x.Auths = lAuths.Where(z => x.NODEID == z.NODEID));

                return l;
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
                throw ex;
            }
        }

        /// <summary>
        /// 取得 目前的父節點
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> R_COMM_FUNCTIONLIST_P(ComFunAndAuth o)
        {
            try
            {
                List<dynamic> lReturn = new List<dynamic>();
                List<dynamic> l = new List<dynamic>();
                l = oDal.R_COMM_FUNCTIONLIST(o.Function);

                //做父節點group 記得select new 要給 屬性名稱
                var pGroup = l
                    .GroupBy(x => x.PARENTID)
                    .Where(p => !string.IsNullOrEmpty(p.Key))
                    .Select(z => new { PARENTID = z.Key }).ToList();

                foreach (var oGroup in pGroup)
                {
                    dynamic od = new ExpandoObject();
                    od.PARENTID = oGroup.PARENTID;
                    od.NODETEXT = l.Where(x => x.NODEID == oGroup.PARENTID).First().NODETEXT;
                    lReturn.Add(od);
                }

                return lReturn;
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
                throw ex;
            }
        }

        /// <summary>
        /// 更新額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage U_COMM_FUNCTIONLIST(ComFunAndAuth o)
        {
            try
            {
                oDal.U_COMM_FUNCTIONLIST(o.Function);

                //先刪除全部
                oDal.D_COMM_NODE_HAS_CONTROL(new List<COMM_NODE_HAS_CONTROL> { new COMM_NODE_HAS_CONTROL { NODEID = o.Function.NODEID } });

                if (o.Auths != null && o.Auths.Any())
                {
                    o.Auths.ForEach(x => x.NODEID = o.Function.NODEID);
                    o.Auths.ForEach(x => x.PARENTID = o.Function.PARENTID);
                    //新增
                    oDal.C_COMM_NODE_HAS_CONTROL(o.Auths);
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 新增額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage C_COMM_FUNCTIONLIST(ComFunAndAuth o)
        {
            try
            {
                oDal.C_COMM_FUNCTIONLIST(o.Function);
                //先刪除全部
                oDal.D_COMM_NODE_HAS_CONTROL(new List<COMM_NODE_HAS_CONTROL> { new COMM_NODE_HAS_CONTROL { NODEID = o.Function.NODEID } });

                if (o.Auths != null && o.Auths.Any())
                {
                    o.Auths.ForEach(x => x.NODEID = o.Function.NODEID);
                    o.Auths.ForEach(x => x.PARENTID = o.Function.PARENTID);
                    //新增
                    oDal.C_COMM_NODE_HAS_CONTROL(o.Auths);
                }

            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 刪除額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiReturnMessage D_COMM_FUNCTIONLIST(ComFunAndAuth o)
        {
            try
            {
                oDal.D_COMM_FUNCTIONLIST(o.Function);
                //刪除D_RoleNode
                oDal.D_RoleNode(o.Function.NODEID);
                //刪除群組控制，有該NODEID
                oDal.D_COMM_NODE_HAS_CONTROL(new List<COMM_NODE_HAS_CONTROL> { new COMM_NODE_HAS_CONTROL { NODEID = o.Function.NODEID } });
                //刪除額外權限控制，有該NODEID
                oDal.D_COMM_ROLENODE_HAS_AUTH(new COMM_ROLENODE_HAS_AUTH { NODEID = o.Function.NODEID });
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }


        /// <summary>
        /// 取得 NODEID權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<COMM_NODE_HAS_CONTROL> R_COMM_NODE_HAS_CONTROL(COMM_NODE_HAS_CONTROL o)
        {
            try
            {
                return oDal.R_COMM_NODE_HAS_CONTROL(o);
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
                throw ex;
            }
        }

        /// <summary>
        /// 取得 群組權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<COMM_ROLENODE_HAS_AUTH> R_COMM_ROLENODE_HAS_AUTH(COMM_ROLENODE_HAS_AUTH o)
        {
            try
            {
                return oDal.R_COMM_ROLENODE_HAS_AUTH(o);
            }
            catch (Exception ex)
            {
                oLogMessage.Error = ex.ToString();
                WriteLog();
                throw ex;
            }
        }

        [HttpPost]
        public List<COMM_USER> getUser()
        {
            try
            {
                return oDal.getUser().ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }
        [HttpPost]
        public List<COMM_USER> getUserByDept(COMM_USER o)
        {
            List<COMM_USER> qData = new List<COMM_USER>();
            try
            {
                qData = oDal.getUserByDept(o.USER_DEPT).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return qData;
        }
        [HttpPost]
        public List<COMM_USER> getUserDetail(COMM_USER o)
        {
            try
            {
                return oDal.getUserDetail(o.TXN_USERID).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }                                                   

        [HttpGet]
        public ApiReturnMessage UpdateRoleNode(string sCheckedNode, string sRole)
        {
            try
            {
                oApiReturnMessage = oDal.UpdateRoleNode(sCheckedNode, sRole);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 依前綴字取得人員對應的群組
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<COMM_ROLE> getRolesByPrefix(string sRole)
        {
            try
            {
                return oDal.R_RoleByPrefix(sRole);
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        public List<ControlAuthForPage> getPageControl(string sNodeId)
        {
            try
            {
                return oDal.getPageControl(sNodeId, User.Identity.Name).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return null;
        }
    }
}