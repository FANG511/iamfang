using System.Web;
using System.Web.Optimization;

namespace FTC_MES_MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Content/dist/js/app.min.js",
                        "~/Scripts/jquery.mask.min.js",
                        "~/Scripts/icheck.min.js",
                        "~/Scripts/toast/js/lobibox.min.js",
                        "~/Scripts/toast/js/notifications.min.js",
                        "~/Scripts/md5.js",
                        "~/Scripts/enc-base64.js"
                        ));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/i18next-1.8.0.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                        "~/Content/Font_Awesome_6/css/all.min.css",
                        "~/Content/ionicons.min.css",
                        "~/Content/toast/lobibox.min.css",
                        "~/Content/dist/css/AdminLTE.min.css",
                        "~/Content/dist/css/skins/_all-skins.min.css",
                        "~/Content/chk_style.css",
                        "~/Content/icheck_skins/all.css",
                        "~/Content/Iframe.css",
                        "~/Content/dist/css/skins/uxui-blue.css" //配合公司UXUI
                      ));

            //bundles.Add(new StyleBundle("~/Content/sitecss").Include(
            //            "~/Content/site.css"
            //         ));

            bundles.Add(new StyleBundle("~/Content/kendo/Css").Include(
                      "~/Content/kendo/kendo.common.min.css",
                      "~/Content/kendo/kendo.default.min.css",//配合公司UXUI由blueopal改成default
                      "~/Content/kendo/kendo.ftc.min.css"//配合公司UXUI由kendo要加上公司提供的css
                      ));
            //bundles.Add(new StyleBundle("~/Content/kendo/Css").Include(
            //      "~/Content/kendo/kendo.common.min.css",
            //      "~/Content/kendo/kendo.blueopal.min.css"
            //      ));
            bundles.Add(new ScriptBundle("~/bundles/kendo/Script").Include(
                     "~/Scripts/kendo/jszip.min.js",
                     "~/Scripts/kendo/kendo.all.min.v3.js"
                     //"~/Scripts/kendo/cultures/kendo.culture.zh-TW.min.js",
                     //"~/Scripts/kendo/cultures/kendo.culture.zh-CN.min.js",
                     //"~/Scripts/kendo/messages/kendo.messages.zh-TW.min.js",
                     //"~/Scripts/kendo/messages/kendo.messages.zh-CN.min.js",
                     // "~/Scripts/kendo/messages/kendo.messages.en-US.min.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/Highcharts/Script").Include(
                       "~/Scripts/Highcharts/highcharts.js",
                    "~/Scripts/Highcharts/highcharts-3d.js",
                    "~/Scripts/Highcharts/highcharts-more.js",
                    "~/Scripts/Highcharts/modules/exporting.js",
                    "~/Scripts/Highcharts/modules/offline-exporting.js",
                      "~/Scripts/html2canvas.min.js"));

            bundles.Add(new StyleBundle("~/Content/UIUX/Login/css").Include(
              "~/Content/UIUX/Login/css/Login.css"
              ));
        }
    }
}
