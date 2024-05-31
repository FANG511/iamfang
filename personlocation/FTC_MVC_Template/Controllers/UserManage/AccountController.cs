using FTC_MES_MVC.Filters;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.Services;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FTC_MES_MVC.Controllers
{
    public class AccountController : BaseController
    {
        dalCommService svc = new dalCommService();
        protected override void Dispose(bool disposing)
        {
            if (svc!=null)
            {
                svc.Dispose();
            }
        }
        // GET: Account
        public ActionResult Index()
        {

            return View();
        }

        public string hashPassword(string Password)
        {
            return AesEncrypt(Password);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string status)
        {
            try
            {
                ViewBag.ReturnUrl = returnUrl;
                var ComFacList = svc.getMesApHostDb();
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var list in ComFacList)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = list.CompanyID + "-" + list.FactoryID,
                        Value = list.CompanyID + "-" + list.FactoryID + "-" + list.CssUrl,
                    });
                }
                ViewBag.SelectList = items;

                var LanguageList = svc.getLanguageDefine();
                List<SelectListItem> LanguageItems = new List<SelectListItem>();
                foreach (var list in LanguageList)
                {
                    LanguageItems.Add(new SelectListItem()
                    {
                        Text = list.LanguageName,
                        Value = list.LanguageValue,
                        Selected = list.Defalut
                    });
                }
                ViewBag.LanguageList = LanguageItems;
                ViewBag.ShowComFacID = WebConfigurationManager.AppSettings["FtcMesAuthenticationVerifyByUserDept"] == "true" ? "none" : "block";

                string AdAuthentication = WebConfigurationManager.AppSettings["AdAuthentication"];
                if (status == "success")
                    Response.Write("<script language=javascript>alert('密碼修改成功!!');</" + "script>");
               
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
          
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "Login")]
        [AntiForgeryErrorHandlerAttribute(ExceptionType = typeof(HttpAntiForgeryException), View = "Login", Controller = "Account", ErrorMessage = "Session Timeout")]
        public async Task<ActionResult> Login([Bind(Include = "UserId,Password,ComFacID")] LoginViewModel model, string returnUrl)
        {
            bool bAuthticated = false;
            COMM_USER oCOMM_USER = new COMM_USER();
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                string AdAuthentication = WebConfigurationManager.AppSettings["AdAuthentication"];
              
                TempData["AdAuthentication"] = AdAuthentication;
                string sRole = string.Join(",", svc.getRoleByUserId(model.UserId.ToUpper()));
               
                Session["Language"] = model.Language;
                var ticket = new FormsAuthenticationTicket(
                    version: 1,
                    name: model.UserId.ToUpper(), //這邊看個人，你想放使用者名稱也可以，自行更改
                    issueDate: DateTime.Now,//現在時間
                    expiration: DateTime.Now.AddMinutes(Convert.ToInt16(WebConfigurationManager.AppSettings["CookieTimeOut"])),//Cookie有效時間取決於WebConfig設定的CookieTimeOut時間
                    isPersistent: model.RememberMe,//記住我 true or false
                    userData: sRole, //這邊可以放使用者名稱，而我這邊是放使用者的群組代號
                    cookiePath: FormsAuthentication.FormsCookiePath);
                
                switch (AdAuthentication)
                {
                    //AD驗證
                    case "Y":
                        bAuthticated = false;
                        //----------------暫不提供AD驗證功能---------------
                        //string domain = ConfigurationManager.ConnectionStrings["AD_FPG"].ToString();
                        //string charAllowList = @"^[a - zA - Z0 - 9] *$"; //只允許大小寫英數字
                        //Regex pattern = new System.Text.RegularExpressions.Regex(charAllowList);
                        //if (!pattern.IsMatch(model.UserId) | !pattern.IsMatch(model.Password))
                        //{
                        //    bAuthticated = false;
                        //}
                        //using (DirectoryEntry entry = new DirectoryEntry(domain, model.UserId.ToUpper(), model.Password.ToString()))
                        //{
                        //    string encodedFilter =$"(SAMAccountName={model.UserId.ToUpper()})";
                        //    DirectorySearcher search = new DirectorySearcher();
                        //    search.Filter = Microsoft.Security.Application.Encoder.LdapFilterEncode(encodedFilter);
                        //    try
                        //    {   //AD驗證成功
                        //        SearchResult result = search.FindOne();
                        //        if (result != null)
                        //            bAuthticated = true;
                        //    }
                        //    catch
                        //    {   //AD驗證失敗
                        //        bAuthticated = false;
                        //    }
                        //}
                        ////bAuthticated = Membership.ValidateUser(model.UserId.ToUpper(), model.Password);
                        break;

                    //表單驗證
                    default:
                        string hashPwd = hashPassword(model.Password);
                        
                        oCOMM_USER = svc.chkUserPwd(model.UserId.ToUpper(), hashPwd);
                        bAuthticated = oCOMM_USER != null ? true : false;
                        break;
                }

                //----2020/04/17 如果FtcMesAuthenticationVerifyByUserDept=true 
                //表示使用登入帳號的部門設定來決定使用的FactoryID(都是用4碼)
                //，CompanyID則使用config檔CompanyID參數設定，= false表示不啟用--
                string CompanyID = WebConfigurationManager.AppSettings["CompanyID"];
                string FtcMesAuthenticationVerifyByUserDept = WebConfigurationManager.AppSettings["FtcMesAuthenticationVerifyByUserDept"];
                if (oCOMM_USER!=null && FtcMesAuthenticationVerifyByUserDept== "true")
                {
                    string sDept = oCOMM_USER.USER_DEPT.Length>4? oCOMM_USER.USER_DEPT.Substring(0,4): oCOMM_USER.USER_DEPT;
                    model.ComFacID = CompanyID + "-" + sDept;
                    Session["CompanyID"] = CompanyID;
                    Session["FactoryID"] = sDept;
                    
                }
                if (!string.IsNullOrEmpty(model.ComFacID) && model.ComFacID.Split('-').Length > 1)
                {
                    Session["CompanyID"] = model.ComFacID.Split('-')[0];
                    Session["FactoryID"] = model.ComFacID.Split('-')[1];
                    Session["Style"] = model.ComFacID.Replace(Session["CompanyID"] + "-" + Session["FactoryID"] + "-", "");
                }
                if (bAuthticated)   //驗證成功
                {
                    FormsAuthentication.SetAuthCookie(model.UserId.ToUpper(), true);
                    var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的ticket加密
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(cookie);
                    //登入成功後重新導向指定網頁
                    if (returnUrl != null && returnUrl.Contains("ByMail=Y"))
                    {
                        var urlDetail = GetUrlDetail(returnUrl, model.UserId.ToUpper());
                        return this.RedirectToAction(urlDetail.Item1, urlDetail.Item2, urlDetail.Item3);
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Home");
                    }
                }
                else    //驗證失敗
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    var ComFacList = svc.getMesApHostDb();
                    foreach (var list in ComFacList)
                    {
                        items.Add(new SelectListItem()
                        {
                            Text = list.CompanyID + "-" + list.FactoryID,
                            Value = list.CompanyID + "-" + list.FactoryID+"-"+list.CssUrl,
                        });
                    }
                    ViewBag.SelectList = items;

                    var LanguageList = svc.getLanguageDefine();
                    List<SelectListItem> LanguageItems = new List<SelectListItem>();
                    foreach (var list in LanguageList)
                    {
                        LanguageItems.Add(new SelectListItem()
                        {
                            Text = list.LanguageName,
                            Value = list.LanguageValue,
                            Selected = list.Defalut
                        });
                    }
                    ViewBag.LanguageList = LanguageItems;
                    ViewBag.ShowComFacID = WebConfigurationManager.AppSettings["FtcMesAuthenticationVerifyByUserDept"] == "true" ? "none" : "block";
                    ModelState.AddModelError(string.Empty, "帳號或密碼輸入錯誤，請重新輸入！");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[HandleError(ExceptionType = typeof(HttpAntiForgeryException), View = "Login")]
        [AntiForgeryErrorHandlerAttribute(ExceptionType =typeof(HttpAntiForgeryException),View ="Login", Controller = "Account", ErrorMessage = "Session Timeout")]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult OldPassword(string oldpassword)
        {
            string hash = hashPassword(oldpassword);
            return Json(new { result = svc.chkOldPassword(User.Identity.Name, hash) });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult ChangePassword([Bind(Include = "OldPassword,NewPassword,ConfirmPassword")]  ChangePassword model)
        {
            if (model.NewPassword != null)
            {
                //var user = svc.getUser(User.Identity.Name);
                svc.updateUserPwd(User.Identity.Name, hashPassword(model.NewPassword));

                LogOff();
                Response.Write("<script language=javascript>alert('密碼修改成功!!');</" + "script>");
                return RedirectToAction("Login", "Account", new { status = "success" });
            }
            else
            {
                Response.Write("<script language=javascript>alert('密碼修改失敗，請重新登入。');</" + "script>");
                return RedirectToAction("Login", "Account", new { status = "error" });
            }
        }
        public Tuple<string, string, RouteValueDictionary> GetUrlDetail(string url, string userName)
        {
            string Action = "";
            string Controller = "";
            RouteValueDictionary Parameter = new RouteValueDictionary();

            var step1 = url.Split('?');
            var step2 = step1[0].Split('/');
            //取得Action
            Action = step2[step2.Length - 1];

            //取得Controller
            if (step2.Length == 3)
            {
                Controller = step2[step2.Length - 2];
            }
            else if (step2.Length == 4)
            {
                Controller = step2[step2.Length - 3] + "/" + step2[step2.Length - 4];
            }

            //取得Parameter

            if (step1.Length > 1)
            {
                var step3 = step1[1].Split('&');
                foreach (var item in step3)
                {
                    var step4 = item.Split('=');
                    if (step4[0] != "ByMail")
                    {
                        Parameter.Add(step4[0].ToString(), step4[1].ToString());
                    }
                }
            }

            Parameter.Add("CompanyID", Session["CompanyID"].ToString());
            Parameter.Add("FactoryID", Session["FactoryID"].ToString());
            Parameter.Add("Lng", Session["Language"] == null ? "zh-TW" : Session["Language"].ToString());
            Parameter.Add("MODIFYUSER", userName);
            Parameter.Add("ref", "1");

            Tuple<string, string, RouteValueDictionary> returnList = new Tuple<string, string, RouteValueDictionary>(Action, Controller, Parameter);

            return returnList;
        }
        public ActionResult Index2()
        {
            return View();
        }
    }

}