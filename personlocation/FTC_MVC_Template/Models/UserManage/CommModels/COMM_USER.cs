using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_USER
    {
        [Key]
        [DisplayName("NotesID")]
        [Required]
        public string USERID { get; set; }
        [DisplayName("密碼")]
        [DataType(DataType.Password)]
        public string PWD { get; set; }
        [DisplayName("姓名")]
        [Required]
        public string USER_NAME { get; set; }
        [DisplayName("部門")]
        [Required]
        public string USER_DEPT { get; set; }
        [DisplayName("描述")]
        public string USER_DESC { get; set; }
        [DisplayName("信箱")]
        public string MAIL { get; set; }
        [DisplayName("分機")]
        public string PHONE { get; set; }
        public string TXN_USERID { get; set; }
        public DateTime TXN_TIMESTAMP { get; set; }
        public string EXT01 { get; set; }
        public string EXT02 { get; set; }
        public string EXT03 { get; set; }
        public string EXT04 { get; set; }
        public string EXT05 { get; set; }
    }

    //public class Param
    //{
    //    public COMM_USER User { get; set; }
    //}
}