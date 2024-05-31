using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FTC_MES_MVC.Startup))]

namespace FTC_MES_MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //HttpConfiguration config = new HttpConfiguration();
            // Handle registration of Swagger API docs
            //SwaggerConfig.Register();
            // 如需如何設定應用程式的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
