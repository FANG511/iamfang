using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FTC_MES_MVC.Controllers
{
    public class PersonLocationController : BaseController
    {
        /// <summary>
        /// 人員資料
        /// </summary>
        public ActionResult PersonData()
        {
            return View();
        }

        public ActionResult DeviceInfo()
        {
            return View();
        }

        public ActionResult BindDevice()
        {
            return View();
        }

        public ActionResult UnBindDevice()
        {
            return View();
        }

        public ActionResult GeoPolygon()
        {
            return View();
        }

        public ActionResult AreaInfo()
        {
            return View();
        }

        public ActionResult BLEBeacon()
        {
            return View();
        }

        public ActionResult PersonCurrent()
        {
            return View();
        }

        public ActionResult PersonMonitor()
        {
            return View();
        }

        public ActionResult AreaMonitor()
        {
            return View();
        }

        public ActionResult HistoryMonitor()
        {
            return View();
        }

        public ActionResult CurrentMonitor()
        {
            return View();
        }

        public ActionResult AlarmSetting()
        {
            return View();
        }

        public ActionResult AlarmRecord()
        {
            return View();
        }
        public ActionResult AlarmMessage()
        {
            return View();
        }

    }
}