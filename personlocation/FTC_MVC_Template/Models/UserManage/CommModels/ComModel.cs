using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FTC_MES_MVC.Models.ViewModels;

namespace FTC_MES_MVC.Models.UserManage
{
    /// <summary>
    /// 基本資訊
    /// </summary>
    public class BasicInfo
    {
        /// <summary>
        /// 節點代號
        /// </summary>
        public string NODEID { get; set; }
        /// <summary>
        /// 公司代號
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 工廠代號
        /// </summary>
        public string FactoryID { get; set; }
        /// <summary>
        /// FTCLOT ID
        /// </summary>
        public string LOTID { get; set; }
        /// <summary>
        /// 機台
        /// </summary>
        public string EQPID { get; set; }
        /// <summary>
        /// 更改使用者
        /// </summary>
        public string MODIFYUSER { get; set; }
    }

    public class COMM_NODEFUNCTION_P
    {
        public string AUTHID { get; set; }
        public string AUTHTEXT { get; set; }
        /// <summary>
        /// Jquery Selector Query
        /// </summary>
        public string OBJECTCLASS { get; set; }
    }

    /// <summary>
    /// 每個 連結有的資料
    /// </summary>
    public class ComFunAndAuth : BasicInfo
    {
        public COMM_FUNCTIONLIST Function { get; set; }

        public List<COMM_NODE_HAS_CONTROL> Auths { get; set; }
    }

    /// <summary>
    /// NODE有哪些額外權限 (標準)
    /// </summary>
    public class COMM_NODE_HAS_CONTROL
    {
        public string NODEID { get; set; }
        public string PARENTID { get; set; }
        public string AUTHID { get; set; }
        public string AUTHTEXT { get; set; }
        public string OBJECTCLASS { get; set; }
    }

    /// <summary>
    /// 權組有哪些額外權限 (實際)
    /// </summary>
    public class COMM_ROLENODE_HAS_AUTH
    {
        public string AUTHTEXT { get; }
        public string ROLEID { get; set; }
        public string NODEID { get; set; }
        public string AUTHID { get; set; }
        public string OBJECTCLASS { get; set; }
        public string DESCRIPTION { get; set; }

    }

    public class ViewModel_Role_P
    {
        /// <summary>
        /// 分群後的父節點
        /// </summary>
        public List<COMM_FUNCTIONLIST> PNodeList { get; set; }
        public List<COMM_FUNCTIONLIST> NodeList { get; set; }
        public List<string> ControlList { get; set; }
        public List<COMM_ROLENODE> AuthList { get; set; }
        public List<NodeWithAuth> NodeControl { get; set; }
    }
}