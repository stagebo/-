using Common;
using Common.Log4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit
{
    public class GlobalHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 全局过滤器，触发异常时调用，保存异常Log日志
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                Log4NetUtils.Error(filterContext.Controller, "GlobalHandleErrorAttribute捕获异常信息", filterContext.Exception);
                ContentResult contentResult = new ContentResult();
                contentResult.Content = "{\"result\":\"" + CommonEnum.AjaxResult.ERROR+ "\"}";
                RedirectResult redirectResult = new RedirectResult(CommonEnum.ErrorPageUrl.DEFAULT_URL);
                if (filterContext.HttpContext.Request.HttpMethod.ToUpper().Equals("POST"))
                {
                    filterContext.Result = contentResult;
                }
                else
                {
                    filterContext.Result = redirectResult;
                }
                filterContext.ExceptionHandled = true;
            }
        }
    }
}