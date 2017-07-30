using BussinessLogicLayer;
using Common;
using Common.Log4Net;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebBlog.Filter;

namespace MyWebSit.Controllers
{
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// POST /UserManagement/GetUserInfo 
        /// </summary>
        /// <returns></returns>
        [Right]
        public ActionResult GetUserInfo()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string id = Session["id"].ToString();
            Guid idGuid;
            if (!Guid.TryParse(id, out idGuid))
            {
                Log4NetUtils.Error(this, "检测到非法入侵！！！！！！！！！！！！");
                return Content(errorJsonString);
            }
            User u = new UserBLL().SearchModelObjectByID<User>(idGuid);
            if (u == null)
            {
                Log4NetUtils.Error(this, "查询用户信息，无法检索到用户信息！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append("{\"result\":\"" + CommonEnum.AjaxResult.SUCCESS + "\",");
            re.Append(u.ToJsonString(false));
            re.Append("}");
            return Content(re.ToString());
        }
    }
}