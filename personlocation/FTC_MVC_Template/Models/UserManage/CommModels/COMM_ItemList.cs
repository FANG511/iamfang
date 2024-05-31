using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class COMM_ItemList
    {

        [Key]
        [Required]
        [Column(Order = 1)]
        [DisplayName("清單名稱")]
        [UIHint("ItemList_ListName")]
        public string ListName { get; set; }
        [Key]
        [Required]
        [Column(Order = 2)]
        [DisplayName("項目編號")]
        public short ItemIndex { get; set; }
        [Required]
        [DisplayName("項目名稱")]
        public string ItemName { get; set; }
        [Required]
        [DisplayName("項目代碼")]
        public string ItemValue { get; set; }
        //public string ValueType { get; set; }
        [DisplayName("備註")]
        public string ItemMemo { get; set; }
        [DisplayName("類別")]
        public string ValueType { get; set; }
        [DisplayName("說明")]
        public string Description { get; set; }
    }
}