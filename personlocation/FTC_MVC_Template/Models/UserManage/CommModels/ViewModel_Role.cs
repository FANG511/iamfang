using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.ViewModels
{
    public class ViewModel_Role
    {
        public List<COMM_FUNCTIONLIST> NodeList { get; set; }
        public List<string> ControlList { get; set; }
        public List<COMM_ROLENODE> AuthList { get; set; }
        public List<NodeWithAuth> NodeControl { get; set; }
    }

    public class NodeWithAuth
    {
        public string NODEID { get; set; }

        public string PARENTID { get; set; }

        public string AUTHTEXT { get; set; }

        public string OBJECTCLASS { get; set; }
    }

    public class ControlAuthForPage
    {
        public string NODEID { get; set; }
        public string PARENTID { get; set; }
        public string OBJECTCLASS { get; set; }
        public string AUTHTEXT { get; set; }
        public bool IsVisible { get; set; }
    }
}