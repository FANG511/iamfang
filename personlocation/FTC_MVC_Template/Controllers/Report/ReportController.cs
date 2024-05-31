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
using System.IO;
namespace FTC_MES_MVC.Controllers
{
    
    public class ReportController : BaseController
    {
        public ActionResult LoadXls()
        {
            return View();
        }
        public ActionResult ReportForKeyValue()
        {
            return View();
        }
        public ActionResult ReportHistoryForKeyValue()
        {
            return View();
        }
    }
}
