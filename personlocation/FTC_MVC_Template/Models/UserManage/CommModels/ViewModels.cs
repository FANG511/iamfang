using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.ViewModels
{
    public partial class V_TreeView
    {
        [Key]
        public string NODEID { get; set; }
        public string PARENTID { get; set; }
        public string NODETEXT { get; set; }
        public bool ISEXPANDED { get; set; }
    }

    public partial class V_TreeNode
    {
        [Key]
        [Column(Order = 1)]
        public string USERID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string NODEID { get; set; }
        public string PARENTID { get; set; }
        public string NODETEXT { get; set; }
        public string NODETARGET { get; set; }
        public int NODESEQUENCE { get; set; }
        public string NODEURL { get; set; }
        public string NODEIMAGE { get; set; }
        public bool CONTROLID { get; set; }
        public string IFRAMEHEIGHT { get; set; }
    }

    public class TreeViewModel
    {
        public List<V_TreeNode> RootNodes { get; set; }
        public List<V_TreeNode> TreeNodes { get; set; }
        //public ChangePassword ChangePassword { get; set; }
    }

    public partial class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "目前密碼")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認密碼不相符。")]
        public string ConfirmPassword { get; set; }
    }

    public class foreignKey
    {
        public string text { get; set; }
        public string value { get; set; }
    }
}