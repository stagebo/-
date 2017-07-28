using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBlog.Filter
{
    public class RightAttribute: ActionFilterAttribute
    {

        //action执行之前先执行此方法  
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            bool f = true;
            
            string requestMethodString = filterContext.HttpContext.Request.HttpMethod.ToUpper();
            ContentResult contentResult = new ContentResult();
            contentResult.Content = "{\"result\":\"0\"}";
            /* 验证失败后的跳转页面 */
            RedirectResult loginRedirectResult = new RedirectResult("/Common/ErrorLogin");
            string uid = filterContext.HttpContext.Session["uid"]?.ToString();
            if (uid==null||string.IsNullOrWhiteSpace(uid)) {
                if (requestMethodString.Equals("POST"))
                {
                    filterContext.Result = contentResult;
                }
                else
                {
                    filterContext.Result = loginRedirectResult;
                }
                return;
            }
            
        }
    }
}