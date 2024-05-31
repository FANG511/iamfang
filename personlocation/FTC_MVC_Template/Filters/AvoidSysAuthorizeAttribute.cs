using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Filters
{
    /// <summary>
    /// 在BaseController都會繼承SysAuthorize，但有些Action不是需要Authorize，就可以加入這個filter屬性
    /// 避免因為無法Authorize而造成系統異常
    /// </summary>
    public class AvoidSysAuthorizeAttribute : Attribute
    {
    }
}