using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Web.Mvc;

namespace FTC_MES_MVC.Models
{
    // AccountController 動作所傳回的模型。
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "帳號")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "廠處")]
        public string ComFacID { get; set; }

        [Display(Name = "記住我?")]
        public bool RememberMe { get; set; }

        [Display(Name = "語系")]
        public string Language { get; set; }
    }

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
