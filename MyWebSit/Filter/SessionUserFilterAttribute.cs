using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBlog.Filter
{
    public class SessionUserFilterAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// 在Action开始执行之前，验证Session登录状态和用户类型是否允许访问Action
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string requestMethodString = filterContext.HttpContext.Request.HttpMethod.ToUpper();

            ContentResult contentResult = new ContentResult();
            contentResult.Content = "{\"result\":\"0\"}";
            /* 验证失败后的跳转页面 */
            RedirectResult loginRedirectResult = new RedirectResult("");


            /* 获取Session的登录状态 */
            //object loginState = filterContext.HttpContext.Session[SessionEnum.LoginState.KEY];
            ///* 获取Session的用户类型 */
            //object userType = filterContext.HttpContext.Session[SessionEnum.UserType.KEY];

            ///* 如果允许访问Action的LoginState数组不为null并且元素数量大于0时，表示需要验证当前用户的LoginState */
            //if (this.AllowedLoginStateArray != null && this.AllowedLoginStateArray.Length > 0)
            //{
            //    /* 如果当前Session上的loginState为null或不属于SessionEnum.LoginState.Value枚举类型，跳转至错误页 */
            //    /* 如果允许访问Action的LoginState数组中不包含当前Session上的loginState，跳转至错误页 */
            //    if (loginState == null || !(loginState is SessionEnum.LoginState.Value)
            //        || !this.AllowedLoginStateArray.Contains((SessionEnum.LoginState.Value)loginState))
            //    {
            //        if (requestMethodString.Equals("POST"))
            //        {
            //            filterContext.Result = contentResult;
            //        }
            //        else
            //        {
            //            filterContext.Result = loginRedirectResult;
            //        }
            //        return;
            //    }

            //    /* 如果允许访问Action的UserType数组不为null并且元素数量大于0时，表示需要验证当前用户的UserType */
            //    if (this.AllowedUserTypeArray != null && this.AllowedUserTypeArray.Length > 0)
            //    {
            //        if (userType == null || !(userType is SessionEnum.UserType.Value)
            //            || !this.AllowedUserTypeArray.Contains((SessionEnum.UserType.Value)userType))
            //        {
            //            if (requestMethodString.Equals("POST"))
            //            {
            //                filterContext.Result = contentResult;
            //            }
            //            else
            //            {
            //                filterContext.Result = rightRedirectResult;
            //            }
            //        }
            //    }
            //}
        }
    }
}