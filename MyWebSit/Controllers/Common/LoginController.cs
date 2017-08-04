using Common.Log4Net;
using Common.NHibernate;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Common;
using BussinessLogicLayer;

namespace MyWebSit.Controllers.Common
{

    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View("Page_Login");
        }
        /// <summary>
        /// GET /Login/RegisterPage
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterPage()
        {
            return View("Page_Register");
        }

        /// <summary>
        /// POST /Login/Register 用户注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string successString = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
            string uid = Request.Form["uid"];
            string pwd = Request.Form["pwd"];
            if (string.IsNullOrWhiteSpace(uid))
            {
                Log4NetUtils.Error(this, "注册用户，接收前端用户名失败！");
                return Content(errorJsonString);
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                Log4NetUtils.Error(this, "注册用户，接收前端用户密码失败！");
                return Content(errorJsonString);
            }
            UserBLL userBLL=new UserBLL();
            int? uCount = userBLL.SearchModelObjectCountByCondition<User>(
                new Dictionary<string, object>() {
                    { "f_uid,Eq",uid}
                });
            if (uCount == null) {
                Log4NetUtils.Error(this,"注册用户，查询已存在该用户名失败！");
                return Content(errorJsonString);
            }
            if (uCount > 0)
            {
                Log4NetUtils.Error(this,"注册用户，该用户名已存在~");
                return Content(errorJsonString);
            }
            User u = new User();
            u.f_id = Guid.NewGuid();
            u.f_uid = uid;
            u.f_pwd = CommonFunction.MD5Encrypt(pwd);
            u.f_exist = CommonEnum.DataExist.EXIST;
            u.f_reg_date = DateTime.Now;
            if (!userBLL.AddModel<User>(u))
            {
                Log4NetUtils.Error(this, "注册用户，添加User实体失败！");
                return Content(errorJsonString);
            }
            return Content(successString);
        }
        /// <summary>
        /// 检验用户名是否合法
        /// </summary>
        /// <returns></returns>
        public ActionResult UserIsExist()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string uid = Request.Form["uid"];
            int? uCount =new UserBLL().SearchModelObjectCountByCondition<User>(
               new Dictionary<string, object>() {
                    { "f_uid,Eq",uid}
               });
            if (uCount == null)
            {
                Log4NetUtils.Error(this, "注册用户，查询已存在该用户名失败！");
                return Content(errorJsonString);
            }
            string result;
            if (uCount > 0)
            {
                result = "{\"result\":\"" + CommonEnum.AjaxResult.SUCCESS + "\",\"state\":\""+CommonEnum.UserIDState.EXIST+"\"}";
                return Content(result);
            }
            result = "{\"result\":\""+CommonEnum.AjaxResult.SUCCESS+"\",\"state\":\""+CommonEnum.UserIDState.LEGAL+"\"}";
            return Content(result);
        }
        /// <summary>
        /// /Login/Validate 用户验证登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Validate()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string successString = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";

            string uid = Request.Form["uid"];
            string pwd = Request.Form["pwd"];
            if (string.IsNullOrWhiteSpace(uid))
            {
                Log4NetUtils.Error(this, "验证用户，接收前端用户名失败！");
                return Content(errorJsonString);
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                Log4NetUtils.Error(this, "验证用户，接收前端用户密码失败！");
                return Content(errorJsonString);
            }
            Dictionary<string, object> condition = new Dictionary<string, object>() {
                {"f_uid,Eq",uid },
                { "f_pwd,Eq",CommonFunction.MD5Encrypt(pwd)}
            };
            User u = new UserBLL().SearchUniqueModelObjectByCondition<User>(condition);
            if (u == null) {
                Log4NetUtils.Error(this,"验证用户，用户名或密码错误，用户名："+uid);
                return Content(errorJsonString);
            }
            /*登记登录信息*/
            Session["uid"] = u.f_uid;
            Session["id"] = u.f_id;
            Session["logTime"] = DateTime.Now;

            return Content(successString);
        }
        /// <summary>
        /// GET /Login/LogOut 用户登出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session["uid"] = "";
            Session["id"] = "";
            Session["logTime"] = "";
            return View("Page_Login");
        }

    }
}