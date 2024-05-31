using Dapper;
using FTC_MES_MVC.Controllers;
using FTC_MES_MVC.Filters;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.Services;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FTC_MES_MVC.Models.UserManage;
using FTC_MES_MVC.Models.Dals.UserManage;

namespace FTC_MES_MVC.Controllers
{
    public class UserManageController : BaseController
    {
        dalUserManageService oDal ;
        protected override void Dispose(bool disposing)
        {
            if (oDal != null)
            {
                oDal.Dispose();
            }
        }
        /// <summary>
        /// 管理使用者
        /// </summary>
        /// <returns></returns>
        public ActionResult CommUser()
        {
            return View();
        }

        /// <summary>
        /// 設定群組
        /// </summary>
        /// <returns></returns>
        public ActionResult UserHasRoles()
        {
            return View();
        }

        /// <summary>
        /// 新增頁面，以及設定額外權限
        /// </summary>
        /// <returns></returns>
        public ActionResult NodeAndAuth()
        {
            return View();
        }

        /// <summary>
        /// 額外權限最大列表
        /// </summary>
        /// <returns></returns>
        public ActionResult NodeFunction()
        {
            return View();
        }

        /// <summary>
        /// 設定群組
        /// </summary>
        /// <returns></returns>
        public ActionResult Roles()
        {
            oDal = new dalUserManageService();
            ViewModel_Role_P qData = new ViewModel_Role_P();
            try
            {
                qData=oDal.GetViewModel();
            }
            catch(Exception ex)
            {
                WriteLog_Error(ex.ToString());
            }
            return View(qData);
        }
    }
}