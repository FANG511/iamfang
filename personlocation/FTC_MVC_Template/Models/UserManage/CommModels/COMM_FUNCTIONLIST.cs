using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_FUNCTIONLIST
    {
        [Key]
        //[Required]
        public string NODEID { get; set; }
        [DisplayName("功能名稱")]
        public string NODETEXT { get; set; }
        public string NODESEQUENCE { get; set; }
        public string NODEURL { get; set; }
        public string NODETARGET { get; set; }
        public string NODEIMAGE { get; set; }
        public string NODEEXPANDIMAGE { get; set; }
        public string FUNCTIONID { get; set; }
        public string DESCRIPTION { get; set; }
        public string PARENTID { get; set; }
        public bool ISEXPANDED { get; set; }
        public bool ISLEAFNODE { get; set; }
        public string IFRAMEHEIGHT { get; set; }
        public bool ISREFERENCE { get; set; }
    }
}