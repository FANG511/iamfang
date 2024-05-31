using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_USER_HAS_ROLES
    {
        [Key]
        [Column(Order = 1)]
        [DisplayName("NotesID")]
        [Required]
        [UIHint("User_Select")]
        public string USERID { get; set; }
        /// <summary>
        /// Joint出來的使用者名稱
        /// </summary>
        public string USER_NAME { get; set; }
        [Key]
        [Column(Order = 2)]
        [DisplayName("權限ID")]
        [Required]
        [UIHint("comm_role_select")]
        public string ROLEID { get; set; }
        /// <summary>
        /// Joint出來的群組名稱
        /// </summary>

        public string ROLE_NAME { get; set; }
        ///<summary>
        ///
        ///</summary>	
        public string TXN_USERID { get; set; }
        ///<summary>
        ///
        ///</summary>	
        public string TXN_TIMESTAMP { get; set; }

        public string sJson { get; set; }
    }    
}