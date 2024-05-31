using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Description;

namespace FTC_MES_MVC.App_Start
{
    /// <summary>
    /// API描述器扩展
    /// </summary>
    public static class ApiDescriptionExtension
    {
        /// <summary>
        /// 获取区域名称
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string GetAreaName(this ApiDescription description)
        {
            string controllerFullName = description.ActionDescriptor.ControllerDescriptor.ControllerType.FullName;
            //匹配areaName
            string areaName = Regex.Match(controllerFullName, @"Area.([^,]+)\.C").Groups[1].ToString().Replace(".", "");
            if (string.IsNullOrEmpty(areaName))
            {
                //若不是areas下的controller,将路由格式中的{area}去掉
                description.RelativePath = description.RelativePath.Replace("{area}/", "");
            }
            else
            {
                //若是areas下的controller,将路由格式中的{area}替换为真实areaname
                description.RelativePath = description.RelativePath.Replace("{area}", areaName);
                var findResult = description.ParameterDescriptions.Where(t => t.Name == "area");
                if (findResult != null && findResult.Count() > 0)
                {
                    description.ParameterDescriptions.Remove(findResult.First());
                }
            }
            return areaName==""?"Root": areaName;
        }
    }
}