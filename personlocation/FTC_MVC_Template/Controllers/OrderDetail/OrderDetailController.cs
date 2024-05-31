using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FTC_MES_MVC.Controllers
{
    public class OrderDetailController : BaseController
    {
        // ActionResult是動作方法的返回類型之一，用於將不同類型的回應返回給用戶
        //PersonData()方法可尋找View中對應的cshtml(PersonData.cshtml)呈現給使用者
        public ActionResult OrderHighcharts()
        {
            return View();
        }
    }
}