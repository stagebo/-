using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBlog.Filter
{
    public class SessionRefreshAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// 在Action开始执行之前  每次访问刷新session记录
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string id = filterContext.HttpContext.Session["id"]?.ToString();
            string uid = filterContext.HttpContext.Session["uid"]?.ToString();
            string logTime = filterContext.HttpContext.Session["logTime"]?.ToString();
            if (!string.IsNullOrWhiteSpace(id)) {
                filterContext.HttpContext.Session["id"] = id;
                filterContext.HttpContext.Session["uid"] = uid;
                filterContext.HttpContext.Session["logTime"] = logTime;
                filterContext.HttpContext.Session["refreshTime"] = DateTime.Now;
            }
        }
    }
}