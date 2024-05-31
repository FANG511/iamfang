using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_ROLENODE
    {
        [Key]
        [Column(Order = 1)]
        [DisplayName("群組代號")]
        [UIHint("comm_role_select")]
        [Required]
        public string ROLEID { get; set; }
        [Column(Order = 2)]
        [Key]
        [DisplayName("節點名稱")]
        [UIHint("comm_node_select")]
        [Required]
        public string NODEID { get; set; }
        [DisplayName("是否有CUD權限")]
        [Required]
        public bool CONTROLID { get; set; }
    }
}