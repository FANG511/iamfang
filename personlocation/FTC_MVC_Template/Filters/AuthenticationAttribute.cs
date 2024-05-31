using FTC_MES_MVC.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using System.Web.Security;

namespace FTC_MES_MVC.Filters
{
    public class AuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
       

        /// <summary>
        /// 驗證使用者的授權是否正確或到期
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["FtcMesAuthentication"] != "true")
            {
                return;
            }
            string sNodeId = filterContext.HttpContext.Request.QueryString["NodeId"];
           
            //只要Action或Contorller有指定AllowAnonymousAttribute或是AvoidSysAuthorize，就不會進行權限驗證
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                || filterContext.ActionDescriptor.IsDefined(typeof(AvoidSysAuthorizeAttribute), true)
                //如果該URL可以被外部直接連結，則不判斷權限，但如果URL沒有帶Ref參數，則視同無權限，導向Login畫面
                || filterContext.HttpContext.Request.QueryString["Ref"] != null)
            {
                return;
            }
            //判定COMM_FUNCTIONLIST.ISREFERENCE 欄位設定如果是True表示，該URL可以被外部直接連結
            if (!string.IsNullOrEmpty(sNodeId))
            {
                dalCommService svc = new dalCommService();
                Boolean IsReference = svc.getNodeIsReference(sNodeId);
                svc.Dispose();
                if (IsReference)
                {
                    return;
                }
            }
            string sRoles = "";
            if (filterContext.Principal.Identity.IsAuthenticated && filterContext.Principal.Identity is FormsIdentity)
            {
                var identity = (FormsIdentity)filterContext.Principal.Identity;
                var ticket = identity.Ticket;
                
                if (!string.IsNullOrEmpty(ticket.UserData))
                {
                    sRoles = ticket.UserData;
                    var roles = sRoles.Split(',').ToArray();
                    if (roles.Any())
                        filterContext.Principal = new GenericPrincipal(identity, roles);
                }
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

            //-------檢查使用者有無存取頁面的權限-------
            if (!string.IsNullOrEmpty(filterContext.HttpContext.User.Identity.Name))
            {
                //HOME不需要驗證權限，因為不在FunctionList中
                if (filterContext.RouteData.Values["controller"].ToString().ToUpper() == "HOME")
                {
                    return;
                }
                if (string.IsNullOrEmpty(sNodeId))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"controller","Home"},
                        {"action","NoAccess"},
                        {"area",""}
                    });
                }
                else
                {
                    dalCommService svc = new dalCommService();
                    List<string> slistAuthorizeNode = svc.getAuthorizedNode(sRoles.Split(',').ToArray());
                    svc.Dispose();
                    if (!slistAuthorizeNode.Contains(sNodeId))
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"controller","Home"},
                        {"action","NoAccess"},
                        {"area",""}
                    });
                }
            }
        }

        /// <summary>
        /// 未授權或是授權過期，自動導向登入畫面
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["FtcMesAuthentication"] != "true")
            {
                return;
            }
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"controller","Account"},
                    {"action","Login"},
                    {"returnUrl",filterContext.HttpContext.Request.RawUrl },
                    {"status", "LogOff" },
                    {"area", "" },
                });
            }
            //or do something , add challenge to response 
        }
    }
    /// <summary>
    /// 防止ASP.NET MVC 的 System.Web.Mvc.HttpAntiForgeryException 錯誤
    /// </summary>
    public class AntiForgeryErrorHandlerAttribute : HandleErrorAttribute
    {
        //用來指定 redirect 的目標 controller
        public string Controller { get; set; }
        //用來儲存想要顯示的訊息
        public string ErrorMessage { get; set; }
        //覆寫預設發生 exception 時的動作
        public override void OnException(ExceptionContext filterContext)
        {
            //如果發生的 exception 是 HttpAntiForgeryException 就轉導至設定的 controller、action (action 在 base HandleErrorAttribute已宣告)
            if (filterContext.Exception is HttpAntiForgeryException)
            {
                //這個屬性要設定為 true 才能接手處理 exception 也才可以 redirect
                filterContext.ExceptionHandled = true;
                //將 errormsg 使用 TempData 暫存 (ViewData 與 ViewBag 因為生命週期的關係都無法正確傳遞)
                //filterContext.Controller.TempData.Add("Timeout", ErrorMessage);
                //指定 redirect 的 controller 偶 action
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectToRouteResult(
                     new RouteValueDictionary
                     {
                        { "action", "index" },
                        { "controller", "Home"},
                     });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary
                      {
                        { "action", View },
                        { "controller", Controller},
                      });
                }
            }
            else
                base.OnException(filterContext);// exception 不是 HttpAntiForgeryException 就照 mvc 預設流程
           
           
        }
    }
}