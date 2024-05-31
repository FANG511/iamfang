using Dapper;
using FTC_MES_MVC.Filters;
using FTC_MES_MVC.Models.Dals.UserManage;
using FTC_MES_MVC.Models.Services;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FTC_MES_MVC.Controllers
{
    
    public class HomeController : BaseController
    {
        dalCommService svc ;
        dalUserManageService oDal;
        protected override void Dispose(bool disposing)
        {
            if (svc != null)
            {
                svc.Dispose();
            }
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }
        public ActionResult Index()
        {
            try
            {
                
                ViewBag.Title = System.Configuration.ConfigurationManager.AppSettings["System_Name"];

                ViewBag.HomePage = "";
                string HomePage = System.Configuration.ConfigurationManager.AppSettings["HomePage"];
                string Language = "&Lng=zh-TW";


                if (Session != null && Session["CompanyID"] != null && Session["FactoryID"] != null)
                {
                    svc = new dalCommService();
                    ViewBag.FactoryName = svc.getFactoryName(Session["CompanyID"].ToString(), Session["FactoryID"].ToString());
                    if (!string.IsNullOrEmpty(HomePage))
                    {
                        if (Session["Language"] != null)
                        {
                            Language = $"&Lng={Session["Language"].ToString()}";
                        }
                        string URLLink = "?";
                        if (HomePage.Contains("?"))
                        {
                            URLLink = "&";
                        }
                        ViewBag.HomePage = $"{HomePage}{URLLink}CompanyID={Session["CompanyID"].ToString()}&FactoryID={Session["FactoryID"].ToString()}&MODIFYUSER={User.Identity.Name}{Language}";
                    }
                }
                return View();
            }
            catch(Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return View();

        }

        [AllowAnonymous]
        public ActionResult NoAccess()
        {
            return View();
        }
        /// <summary>
        /// 讓每一頁可以取得Menu、ChangePassword的PartialView
        /// </summary>
        /// <returns></returns>
        [AvoidSysAuthorize]
        public ActionResult _ChangePassword()
        {
            return PartialView();
        }

        [AvoidSysAuthorize]
        public ActionResult _Menu()
        {
            oDal = new dalUserManageService();
            string CompanyID =Session["CompanyID"]!=null? Session["CompanyID"] .ToString(): "";
            string FactoryID = Session["FactoryID"] != null ? Session["FactoryID"].ToString() : "";
            string Language = Session["Language"] != null ? Session["Language"].ToString() : "";
            var menuList = oDal.GetNodes(User.Identity.Name, CompanyID, FactoryID, Language); 
            TreeViewModel model = new TreeViewModel()
            {
                RootNodes = menuList.Where(x => x.PARENTID == null).ToList(),
                TreeNodes = menuList.ToList()
            };
            return PartialView(model);
        }
    }
}
