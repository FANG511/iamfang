using Dapper;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Models;
using System.Text;
using System.Security.Cryptography;
using FTC_MES_MVC.Models.UserManage;

namespace FTC_MES_MVC.Models.Dals.UserManage
{
    /// <summary>
    /// Com DAL，放SQL CMD
    /// </summary>
    public class dalUserManageService : dalBase
    {
        string sSql = "";
        DynamicParameters p = new DynamicParameters();

        /// <summary>
        /// 釋放全部API的資源
        /// </summary>
        protected void AllDispose()
        {
            Dispose();
        }
        public List<ControlAuthForPage> R_PageControl(BasicInfo o)
        {
            List<ControlAuthForPage> listControlAuth = new List<ControlAuthForPage>();
            try
            {
                sSql = @"-- 找該使用者在NODE是否有對應的 AUTHID
                WITH RoleAuthNode AS (
                SELECT a.USERID,a.ROLEID,b.NODEID,b.AUTHID AS oAUTHID
                FROM COMM_USER_HAS_ROLES a -- 使用者歸屬於那些Group
                LEFT JOIN COMM_ROLENODE_HAS_AUTH b ON a.ROLEID = b.ROLEID  --定義每個NODE 對應 ROLE 有哪些額外控制功能列表 
                --LEFT JOIN COMM_NODE_HAS_CONTROL c ON b.NODEID = c.NODEID  --每個NODE 有哪些 權限功能 (設定)

                WHERE a.USERID = @UserId AND b.NODEID = @NodeId
                )
                --將有的ID對應 該頁面的額外功能列表 沒有的 IsVisible 設定 0
                SELECT A.*,C.USERID,C.ROLEID,C.oAUTHID,d.AUTHTEXT,d.OBJECTCLASS,
                CASE WHEN c.oAUTHID IS NULL THEN 0 ELSE 1 END AS IsVisible 
                FROM COMM_NODE_HAS_CONTROL a -- 以NODE為主
                LEFT JOIN RoleAuthNode c ON a.AUTHID = c.oAUTHID 
                LEFT JOIN COMM_NODEFUNCTION d ON a.AUTHID = d.AUTHID --額外控制的功能列表
                WHERE a.NODEID = @NodeId";

                p.Add("@NodeId", o.NODEID);
                p.Add("@UserId", o.MODIFYUSER);

                listControlAuth = cn.Query<ControlAuthForPage>(sSql, p).ToList();

                return listControlAuth;

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>        
        public List<COMM_USER> R_User()
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                return cn.Query<COMM_USER>("SELECT * FROM COMM_USER").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        public ApiReturnMessage C_User(COMM_USER o,string sIdentity_Name)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
               
                string hashPassword = AesEncrypt(o.PWD);

                sSql = string.Format(@"INSERT INTO dbo.COMM_USER (USERID, USER_NAME, USER_DEPT, USER_DESC, MAIL, PHONE, PWD, TXN_USERID, TXN_TIMESTAMP,EXT01,EXT02,EXT03,EXT04,EXT05)
                        VALUES (@USERID, @USER_NAME, @USER_DEPT, @USER_DESC, @MAIL, @PHONE, '{0}', '{1}', '{2}',@EXT01,@EXT02,@EXT03,@EXT04,@EXT05)", hashPassword, sIdentity_Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage U_User(COMM_USER o)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = @"SELECT * FROM COMM_USER where USERID=@USERID";
                var oCOMM_USER = cn.Query<COMM_USER>(sSql, o).FirstOrDefault();
                if (oCOMM_USER != null)
                {
                    if (oCOMM_USER.PWD != o.PWD)
                    {
                        o.PWD = AesEncrypt(o.PWD);
                    }
                }
                sSql = @"UPDATE dbo.COMM_USER
                        SET USER_NAME = @USER_NAME
	                        ,USER_DEPT = @USER_DEPT
	                        ,USER_DESC = @USER_DESC
	                        ,MAIL = @MAIL
	                        ,PHONE = @PHONE
	                        ,PWD = @PWD
                            ,EXT01 = @EXT01
                            ,EXT02 = @EXT02
                            ,EXT03 = @EXT03
                            ,EXT04 = @EXT04
                            ,EXT05 = @EXT05
                        WHERE USERID = @USERID";
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }


        public ApiReturnMessage D_User(COMM_USER o)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = "DELETE FROM dbo.COMM_USER WHERE USERID = @USERID";
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public List<string> R_RoleNode(string sRole)
        {
            List<string> slistRoleNode = new List<string>();
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = @"SELECT * FROM COMM_ROLENODE WHERE ROLEID = @ROLEID";
                p.Add("@ROLEID", sRole);
                slistRoleNode = cn.Query<COMM_ROLENODE>(sSql, p).ToList().Select(o => o.NODEID).ToList();
                return slistRoleNode;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        public ApiReturnMessage U_RoleNode(string sCheckedNode, string sRole, string sAddionalAuth)
        {
            List<COMM_ROLENODE> listRoleNode = new List<COMM_ROLENODE>();
            List<string> slistCheckedNode = sCheckedNode.Split(',').ToList();
            List<COMM_ROLENODE_HAS_AUTH> lRoleAuth = new List<COMM_ROLENODE_HAS_AUTH> { };
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                //還原，先移除最末,
                string txtRemove = ",";

                if (!string.IsNullOrEmpty(sAddionalAuth))
                {
                    sAddionalAuth = sAddionalAuth.Remove(sAddionalAuth.LastIndexOf(txtRemove), txtRemove.Length);

                    string[] aAddAuth = sAddionalAuth.Split(',');

                    foreach (string sAddAuth in aAddAuth)
                    {
                        string[] aDetail = sAddAuth.Split('_');
                        lRoleAuth.Add(new COMM_ROLENODE_HAS_AUTH
                        {
                            ROLEID = sRole,
                            NODEID = aDetail[0],
                            AUTHID = aDetail[1]
                        });
                    }
                }

                //先移除所有的Node
                sSql = @"DELETE FROM COMM_ROLENODE WHERE ROLEID = @ROLEID";
                p.Add("@ROLEID", sRole);
                cn.ExecuteScalar(sSql, p);

                //再重新加入Node
                foreach (string sNode in slistCheckedNode)
                    listRoleNode.Add(new COMM_ROLENODE() { ROLEID = replaceSQLChar(sRole), NODEID = replaceSQLChar(sNode), CONTROLID = false });

                sSql = @"INSERT INTO dbo.COMM_ROLENODE (ROLEID, NODEID, CONTROLID, DESCRIPTION)
                        VALUES (@ROLEID, @NODEID, @CONTROLID, NULL)";
                cn.Execute(sSql, listRoleNode);

                //移除全部額外權限
                D_COMM_ROLENODE_HAS_AUTH(new COMM_ROLENODE_HAS_AUTH { ROLEID = sRole });

                //新增全部額外權限
                if (lRoleAuth.Any())
                {
                    C_COMM_ROLENODE_HAS_AUTH(lRoleAuth);
                }

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage D_RoleNode(string sNODEID)
        {
            try
            {
                sSql = @"DELETE FROM dbo.COMM_ROLENODE
                WHERE NODEID = @NODEID";
                cn.Execute(sSql, new { @NODEID = sNODEID });
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public List<COMM_ROLE> R_Roles()
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                return cn.Query<COMM_ROLE>("SELECT * FROM COMM_ROLE order by TXN_TIMESTAMP").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return null;
        }

        public ApiReturnMessage U_Role(COMM_ROLE o, string sIdentity_Name)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"UPDATE dbo.COMM_ROLE
                                    SET ROLEID = @ROLEID
	                                    ,ROLE_NAME = @ROLE_NAME
	                                    ,ROLE_DESC = @ROLE_DESC
	                                    ,TXN_USERID = '{0}'
	                                    ,TXN_TIMESTAMP = '{1}'
                                    WHERE ROLEID = @ROLEID", sIdentity_Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage D_Role(COMM_ROLE o)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"DELETE FROM dbo.COMM_ROLE WHERE ROLEID = @ROLEID");
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage C_Role(COMM_ROLE o, string sIdentity_Name)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"INSERT INTO dbo.COMM_ROLE (ROLEID, ROLE_NAME, ROLE_DESC, TXN_USERID, TXN_TIMESTAMP)
                                    VALUES (@ROLEID, @ROLE_NAME, @ROLE_DESC, '{0}', '{1}')",
                                    sIdentity_Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cn.Execute(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public List<COMM_USER_HAS_ROLES> R_UserHasRoles(COMM_USER_HAS_ROLES o)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            DynamicParameters dp = new DynamicParameters();
            string sql = "";
            try
            {
                sql = @"SELECT A.* ,B.USER_NAME,C.ROLE_NAME
                FROM COMM_USER_HAS_ROLES A
                LEFT JOIN COMM_USER B ON A.USERID = B.USERID
                LEFT JOIN COMM_ROLE C ON A.ROLEID = C.ROLEID
                WHERE 1=1 ";

                if (!string.IsNullOrEmpty(o.ROLEID))
                {
                    sql += " AND A.ROLEID =@ROLEID";
                    dp.Add("@ROLEID", o.ROLEID);
                }
                //else
                //{
                //    sql += "AND ROLEID =@ROLEID";
                //    dp.Add("@ROLEID", "SYS_ADM_GRP");
                //}

                if (!string.IsNullOrEmpty(o.USERID))
                {
                    sql += " AND A.USERID =@USERID";
                    dp.Add("@USERID", o.USERID);
                }

                return cn.Query<COMM_USER_HAS_ROLES>(sql, dp).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }

        public ApiReturnMessage U_UserHasRoles(COMM_USER_HAS_ROLES o, string sIdentity_Name)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"UPDATE dbo.COMM_USER_HAS_ROLES
                                    SET USERID = @USERID
	                                    ,ROLEID = @ROLEID
	                                    ,TXN_USERID = '{0}'
	                                    ,TXN_TIMESTAMP = '{1}'
                                    WHERE USERID = @USERID AND ROLEID = @ROLEID", sIdentity_Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage D_UserHasRoles(COMM_USER_HAS_ROLES o)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"DELETE FROM dbo.COMM_USER_HAS_ROLES WHERE USERID = @USERID AND ROLEID = @ROLEID");
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        public ApiReturnMessage C_UserHasRoles(COMM_USER_HAS_ROLES o, string sIdentity_Name)
        {
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = string.Format(@"INSERT INTO dbo.COMM_USER_HAS_ROLES (USERID, ROLEID, TXN_USERID, TXN_TIMESTAMP)
                                    VALUES ( @USERID, @ROLEID, '{0}', '{1}')",
                                    sIdentity_Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cn.ExecuteScalar(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }

        /// <summary>
        /// 檢查公用COMM_ROLENODE_HAS_AUTH是否存在，不存在的話自動建立
        /// </summary>
        public void CheckCOMM_ROLENODE_Exist()
        {
            try
            {
                //COMM_ROLENODE_HAS_AUTH
                sSql += @"
                    --檢查 COMM_ROLENODE_HAS_AUTH
                    IF (EXISTS (SELECT * 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND  TABLE_NAME = 'COMM_ROLENODE_HAS_AUTH'))
                    --選擇1表示有
                    SELECT 1 AS TableExist
                    --創建Table
                    ELSE
                    BEGIN		
	                    CREATE TABLE dbo.COMM_ROLENODE_HAS_AUTH
		                    (
		                    ROLEID      NVARCHAR (20) NOT NULL,
		                    NODEID      NVARCHAR (50) NOT NULL,
		                    AUTHID      INT NOT NULL,
		                    DESCRIPTION NVARCHAR (50) NULL,
		                    CONSTRAINT PK_COMM_ROLENODE_HAS_AUTH PRIMARY KEY (ROLEID,NODEID,AUTHID)
		                    )	
                        --選擇2表示補建
                        SELECT 2 AS TableExist
                    END" + "\r\n";

                cn.Execute(sSql);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 取得 額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<COMM_NODEFUNCTION_P> R_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                sSql = @"SELECT * FROM COMM_NODEFUNCTION ";

                return cn.Query<COMM_NODEFUNCTION_P>(sSql, o).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 更新額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void U_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                sSql = @"
                    UPDATE dbo.COMM_NODEFUNCTION
	                SET  AUTHTEXT = @authtext,
	                    OBJECTCLASS = @objectclass
                    WHERE AUTHID = @authid
                    ";

                cn.Execute(sSql, o);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void C_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                var l = R_COMM_NODEFUNCTION(o);

                if (string.IsNullOrEmpty(o.AUTHID))
                    o.AUTHID = (l.Any() ? l.Count + 1 : 1).ToString();

                sSql = @"
                    INSERT INTO dbo.COMM_NODEFUNCTION (AUTHID, AUTHTEXT, OBJECTCLASS)
                    VALUES (@authid, @authtext, @objectclass)
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 刪除額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void D_COMM_NODEFUNCTION(COMM_NODEFUNCTION_P o)
        {
            try
            {
                sSql = @"
                    DELETE FROM COMM_NODEFUNCTION WHERE AUTHID = @AUTHID
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 更新COMM_NODEFUNCTION資料結構成最新 20210622
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void CheckCOMM_NODEFUNCTION_TABLE()
        {
            try
            {
                sSql = @"
                    ALTER TABLE COMM_NODEFUNCTION ALTER COLUMN  OBJECTCLASS NVARCHAR(600)
                    ";

                cn.Execute(sSql);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 取得 所有 Node
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<dynamic> R_COMM_FUNCTIONLIST(COMM_FUNCTIONLIST o)
        {
            try
            {
                sSql = @"SELECT * FROM COMM_FUNCTIONLIST 
                        WHERE 1=1 
                ";

                if (!string.IsNullOrEmpty(o.PARENTID))
                {
                    sSql += " AND PARENTID = @PARENTID";
                }

                return cn.Query<dynamic>(sSql, o).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 更新額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void U_COMM_FUNCTIONLIST(COMM_FUNCTIONLIST o)
        {
            try
            {
                sSql = @"                    
                UPDATE dbo.COMM_FUNCTIONLIST
                SET 
	                PARENTID = @parentid,
	                NODETEXT = @nodetext,
	                NODESEQUENCE = @nodesequence,
	                NODEURL = @nodeurl,
	                NODETARGET = @nodetarget,
	                NODEIMAGE = @nodeimage,
	                NODEEXPANDIMAGE = @nodeexpandimage,
	                FUNCTIONID = @functionid,
	                DESCRIPTION = @description,
	                ISLEAFNODE = @isleafnode,
	                ISEXPANDED = @isexpanded,
	                IFRAMEHEIGHT = @iframeheight,
	                ISREFERENCE = @isreference
                WHERE NODEID = @nodeid
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void C_COMM_FUNCTIONLIST(COMM_FUNCTIONLIST o)
        {
            try
            {
                sSql = @"
                    INSERT INTO dbo.COMM_FUNCTIONLIST (NODEID, PARENTID, NODETEXT, NODESEQUENCE, NODEURL, NODETARGET, NODEIMAGE, NODEEXPANDIMAGE, FUNCTIONID, DESCRIPTION, ISLEAFNODE, ISEXPANDED, IFRAMEHEIGHT, ISREFERENCE)
                    VALUES (@nodeid, @parentid, @nodetext, @nodesequence, @nodeurl, @nodetarget, @nodeimage, @nodeexpandimage, @functionid, @description, @isleafnode, @isexpanded, @iframeheight, @isreference)
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 刪除額外權控全部列表
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void D_COMM_FUNCTIONLIST(COMM_FUNCTIONLIST o)
        {
            try
            {
                sSql = @"
                    DELETE FROM COMM_FUNCTIONLIST WHERE NODEID = @NODEID
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 取得群組有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<COMM_ROLENODE_HAS_AUTH> R_COMM_ROLENODE_HAS_AUTH(COMM_ROLENODE_HAS_AUTH o)
        {
            try
            {
                sSql = @"SELECT A.*, B.AUTHTEXT,B.OBJECTCLASS
                FROM COMM_ROLENODE_HAS_AUTH A
                LEFT JOIN COMM_NODEFUNCTION B ON A.AUTHID = B.AUTHID
                WHERE 1=1  
                        ";

                if (!string.IsNullOrEmpty(o.NODEID))
                {
                    sSql += " AND A.NODEID = @NODEID";
                }

                if (!string.IsNullOrEmpty(o.ROLEID))
                {
                    sSql += " AND A.ROLEID = @ROLEID";
                }

                if (!string.IsNullOrEmpty(o.AUTHID))
                {
                    sSql += " AND A.AUTHID = @AUTHID";
                }

                return cn.Query<COMM_ROLENODE_HAS_AUTH>(sSql, o).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增群組有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void C_COMM_ROLENODE_HAS_AUTH(List<COMM_ROLENODE_HAS_AUTH> l)
        {
            try
            {
                sSql = @"
                    INSERT INTO dbo.COMM_ROLENODE_HAS_AUTH (ROLEID, NODEID, AUTHID, DESCRIPTION)
                    VALUES (@roleid, @nodeid, @authid, @description)
                    ";

                cn.Execute(sSql, l);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 刪除群組有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void D_COMM_ROLENODE_HAS_AUTH(COMM_ROLENODE_HAS_AUTH o)
        {
            try
            {
                sSql = @"
                    DELETE FROM COMM_ROLENODE_HAS_AUTH WHERE 1=1
                    ";

                if (!string.IsNullOrEmpty(o.NODEID))
                {
                    sSql += " AND NODEID = @NODEID";
                }

                if (!string.IsNullOrEmpty(o.ROLEID))
                {
                    sSql += " AND ROLEID = @ROLEID";
                }

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 取得NODE有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<COMM_NODE_HAS_CONTROL> R_COMM_NODE_HAS_CONTROL(COMM_NODE_HAS_CONTROL o)
        {
            try
            {
                sSql = @"SELECT A.* ,B.AUTHTEXT,B.OBJECTCLASS
                FROM COMM_NODE_HAS_CONTROL A
                LEFT JOIN COMM_NODEFUNCTION B ON A.AUTHID = B.AUTHID";

                if (!string.IsNullOrEmpty(o.NODEID))
                {
                    sSql += " AND A.NODEID = @NODEID";
                }

                if (!string.IsNullOrEmpty(o.PARENTID))
                {
                    sSql += " AND A.PARENTID = @PARENTID";
                }

                if (!string.IsNullOrEmpty(o.AUTHID))
                {
                    sSql += " AND A.AUTHID = @AUTHID";
                }

                return cn.Query<COMM_NODE_HAS_CONTROL>(sSql, o).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增NODE有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void C_COMM_NODE_HAS_CONTROL(List<COMM_NODE_HAS_CONTROL> l)
        {
            try
            {
                sSql = @"
                    INSERT INTO dbo.COMM_NODE_HAS_CONTROL (NODEID, PARENTID, AUTHID)
                    VALUES (@nodeid, @parentid, @authid)
                    ";

                cn.Execute(sSql, l);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 刪除NODE有哪些額外權限
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void D_COMM_NODE_HAS_CONTROL(List<COMM_NODE_HAS_CONTROL> l)
        {
            try
            {
                sSql = @"
                    DELETE FROM COMM_NODE_HAS_CONTROL WHERE NODEID = @NODEID
                    ";

                cn.Execute(sSql, l);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 取得 COMM_ItemList
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<COMM_ItemList> R_COMM_ItemList(COMM_ItemList o)
        {
            try
            {
                sSql = @"SELECT * FROM COMM_ItemList WHERE 1=1";

                if (!string.IsNullOrEmpty(o.ListName))
                {
                    sSql += " AND ListName = @ListName";
                }

                if (!string.IsNullOrEmpty(o.ItemName))
                {
                    sSql += " AND ItemName = @ItemName";
                }

                if (!string.IsNullOrEmpty(o.ItemValue))
                {
                    sSql += " AND ItemValue = @ItemValue";
                }

                if (!string.IsNullOrEmpty(o.ItemMemo))
                {
                    sSql += " AND ItemMemo = @ItemMemo";
                }

                return cn.Query<COMM_ItemList>(sSql, o).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 更新 COMM_ItemList
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void U_COMM_ItemList(COMM_ItemList o)
        {
            try
            {
                sSql = @"                    
                UPDATE dbo.COMM_ItemList
                SET ListName = @listname,
	                ItemIndex = @itemindex,
	                ItemName = @itemname,
	                ItemValue = @itemvalue,
	                ValueType = @valuetype,
	                ItemMemo = @itemmemo,
	                Description = @description
                 ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 新增 COMM_ItemList
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void C_COMM_ItemList(COMM_ItemList o)
        {
            try
            {
                sSql = @"
                    INSERT INTO dbo.COMM_ItemList (ListName, ItemIndex, ItemName, ItemValue, ValueType, ItemMemo, Description)
                    VALUES (@listname, @itemindex, @itemname, @itemvalue, @valuetype, @itemmemo, @description)
                    ";

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }

        /// <summary>
        /// 刪除 COMM_ItemList
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void D_COMM_ItemList(COMM_ItemList o)
        {
            try
            {
                sSql = @"
                    DELETE FROM COMM_ItemList WHERE 1=1
                    ";

                if (!string.IsNullOrEmpty(o.ListName))
                {
                    sSql += " AND ListName = @ListName";
                }

                if (!string.IsNullOrEmpty(o.ItemName))
                {
                    sSql += " AND ItemName = @ItemName";
                }

                if (!string.IsNullOrEmpty(o.ItemValue))
                {
                    sSql += " AND ItemValue = @ItemValue";
                }

                if (!string.IsNullOrEmpty(o.ItemMemo))
                {
                    sSql += " AND ItemMemo = @ItemMemo";
                }

                cn.Execute(sSql, o);

            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw ex;
            }
            finally
            {
                AllDispose();
            }
        }
        public List<COMM_ROLE> R_RoleByPrefix(string sRole)
        {
            List<COMM_ROLE> slistRole = new List<COMM_ROLE>();
            //_cn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString());
            try
            {
                sSql = @"SELECT * FROM COMM_ROLE WHERE ROLEID LIKE @ROLEID";
                p.Add("@ROLEID", sRole);

                slistRole = cn.Query<COMM_ROLE>(sSql, p).ToList();
                return slistRole;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }
        public List<V_TreeNode> GetNodes(string sIdentity_Name, string CompanyID, string FactoryID, string Language)
        {
            List<COMM_ROLE> slistRole = new List<COMM_ROLE>();
            try
            {
                sSql = @"SELECT * FROM V_TreeNode WHERE USERID = @USERID AND CONTROLID = 0";
                p.Add("@USERID", sIdentity_Name);
                //預設的iframe高是1000px
                List<V_TreeNode> Org = cn.Query<V_TreeNode>(sSql, p).ToList();
                foreach (var a in Org)
                {
                    if (a.IFRAMEHEIGHT == "" || a.IFRAMEHEIGHT == null)
                    {
                        a.IFRAMEHEIGHT = "1000px";
                    }
                }
                foreach (var oV_TreeNode in Org)
                {
                    if (!string.IsNullOrEmpty(oV_TreeNode.NODEURL))
                    {
                        string URLLink = "?";
                        if (oV_TreeNode.NODEURL.Contains("?"))
                        {
                            URLLink = "&";
                        }
                        if (!oV_TreeNode.NODEURL.Contains("CompanyID"))
                        {
                            if (CompanyID != null && FactoryID != null)
                            {
                                oV_TreeNode.NODEURL = oV_TreeNode.NODEURL + $"{URLLink}CompanyID={CompanyID}&FactoryID={FactoryID}";
                            }
                        }
                        // 可能CompanyID && FactoryID 自動產生
                        if (oV_TreeNode.NODEURL.Contains("?"))
                        {
                            URLLink = "&";
                        }

                        if (!oV_TreeNode.NODEURL.Contains("NodeId"))
                        {
                            oV_TreeNode.NODEURL = oV_TreeNode.NODEURL + $"{URLLink}NodeId={oV_TreeNode.NODEID}";
                        }
                        if (sIdentity_Name != null)
                        {
                            oV_TreeNode.NODEURL = oV_TreeNode.NODEURL + $"&MODIFYUSER={sIdentity_Name}";
                        }
                        if (Language != null)
                        {
                            oV_TreeNode.NODEURL = oV_TreeNode.NODEURL + $"&Lng={Language}";
                        }
                        else
                        {
                            oV_TreeNode.NODEURL = oV_TreeNode.NODEURL + $"&Lng=zh-TW";
                        }
                    }

                }
                return Org;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return null;
        }
        public List<COMM_USER> getUserByDept(string USER_DEPT)
        {
            List<COMM_USER> qData = new List<COMM_USER>();
            try
            {
                String sSql = "SELECT * FROM COMM_USER where USER_DEPT like @USER_DEPT";
                DynamicParameters p = new DynamicParameters();
                p.Add("@USER_DEPT", USER_DEPT + "%");
                qData = cn.Query<COMM_USER>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }
        public List<COMM_USER> getUserDetail(string TXN_USERID)
        {
            List<COMM_USER> qData = new List<COMM_USER>();
            try
            {
                DynamicParameters p = new DynamicParameters();
                String sSql = "SELECT * FROM COMM_USER WHERE 1=1";
                if (!string.IsNullOrEmpty(TXN_USERID))
                {
                    sSql += "AND USERID = @USERID";
                    p.Add("@USERID", TXN_USERID);
                }
                return cn.Query<COMM_USER>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }
        public List<COMM_USER> getUser()
        {
            List<COMM_USER> qData = new List<COMM_USER>();
            try
            {
               
                return cn.Query<COMM_USER>("SELECT * FROM COMM_USER").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }
        public List<ControlAuthForPage> getPageControl(string sNodeId, string sIdentity_Name)
        {
            List<ControlAuthForPage> qData = new List<ControlAuthForPage>();
            try
            {

                sSql = @"WITH AuthNode AS (
                        SELECT a.USERID,a.ROLEID,b.NODEID,c.AUTHID FROM COMM_USER_HAS_ROLES a
                        LEFT JOIN COMM_ROLENODE b ON a.ROLEID = b.ROLEID
                        LEFT JOIN COMM_NODE_HAS_CONTROL c ON b.NODEID = c.NODEID
                        WHERE a.USERID = @UserId AND c.PARENTID = @NodeId
                        )
                        SELECT *, CASE WHEN b.AUTHID IS NULL THEN 0 ELSE 1 END AS IsVisible FROM COMM_NODEFUNCTION a
                        LEFT JOIN AuthNode b ON a.AUTHID = b.AUTHID";

                p.Add("@NodeId", sNodeId);
                p.Add("@UserId", sIdentity_Name);

                return cn.Query<ControlAuthForPage>(sSql, p).ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }
        public ViewModel_Role_P GetViewModel()
        {
            ViewModel_Role_P qData = new ViewModel_Role_P();
            try
            {
                string sSql = @"SELECT a.NODEID,a.PARENTID,b.AUTHTEXT,b.OBJECTCLASS FROM COMM_NODE_HAS_CONTROL a 
                            LEFT JOIN COMM_NODEFUNCTION b ON a.AUTHID = b.AUTHID";
                //依照有父節點的排序
                //model.PNodeList = cn.Query<COMM_FUNCTIONLIST>("SELECT * FROM COMM_FUNCTIONLIST").Where(a=> !string.IsNullOrEmpty(a.PARENTID)).GroupBy(x=>x.PARENTID).Select(z=>z.Key).ToList();
                qData.NodeList = cn.Query<COMM_FUNCTIONLIST>("SELECT * FROM COMM_FUNCTIONLIST").ToList();
                qData.ControlList = cn.Query<COMM_NODEFUNCTION>("SELECT * FROM COMM_NODEFUNCTION").ToList().Select(o => o.AUTHTEXT).ToList();
                qData.AuthList = cn.Query<COMM_ROLENODE>("SELECT * FROM COMM_ROLENODE").ToList();
                qData.NodeControl = cn.Query<NodeWithAuth>(sSql).ToList();

                //做父節點group 
                qData.PNodeList = qData.NodeList
                    .GroupBy(x => x.PARENTID)
                    .Where(p => !string.IsNullOrEmpty(p.Key))
                    .Select(z => new COMM_FUNCTIONLIST { NODEID = z.Key, PARENTID = z.Key }).ToList();

                qData.PNodeList.ForEach(x => x.NODETEXT = qData.NodeList.Where(z => z.NODEID == x.NODEID).First().NODETEXT);

                return qData;
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }
        public ApiReturnMessage UpdateRoleNode(string sCheckedNode, string sRole)
        {
           
            try
            {
                List<COMM_ROLENODE> listRoleNode = new List<COMM_ROLENODE>();
                List<string> slistCheckedNode = sCheckedNode.Split(',').ToList();
                //先移除所有的Node
                sSql = @"DELETE FROM COMM_ROLENODE WHERE ROLEID = @ROLEID";
                p.Add("@ROLEID", sRole);
                cn.ExecuteScalar(sSql, p);

                //再重新加入Node
                foreach (string sNode in slistCheckedNode)
                    listRoleNode.Add(new COMM_ROLENODE() { ROLEID = replaceSQLChar(sRole), NODEID = replaceSQLChar(sNode), CONTROLID = replaceSQLChar(sNode).Length == 5 ? true : false });

                sSql = @"INSERT INTO dbo.COMM_ROLENODE (ROLEID, NODEID, CONTROLID, DESCRIPTION)
                        VALUES (@ROLEID, @NODEID, @CONTROLID, NULL)";
                cn.Execute(sSql, listRoleNode);
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                AllDispose();
            }
            return oApiReturnMessage;
        }
    }
}