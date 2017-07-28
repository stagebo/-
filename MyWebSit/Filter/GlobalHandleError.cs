using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBlog.Filter
{
    public class GlobalHandleError : HandleErrorAttribute
    {
        /// <summary>
        /// 全局过滤器，触发异常时调用，保存异常Log日志
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            { 
                ContentResult contentResult = new ContentResult();
                contentResult.Content = "{\"result\":\"0\"}";
                RedirectResult redirectResult = new RedirectResult("/Common/ErrorException");
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