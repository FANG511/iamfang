using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_ROLE
    {
        [Key]
        [DisplayName("群組代號")]
        [Required]
        public string ROLEID { get; set; }
        [DisplayName("群組名稱")]
        [Required]
        public string ROLE_NAME { get; set; }
        [DisplayName("群組描述")]
        [Required]
        public string ROLE_DESC { get; set; }
    }
}