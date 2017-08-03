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
        /// GET /UserManagement/ModifyUserPage  修改个人信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyUserPage()
        {
            return View("Page_Modify_User_Info");
        }
        /// <summary>
        /// GET /UserManagement/CheckUserPage  查看个人信息页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckUserPage()
        {
            return View("Page_Check_User_Info");
        }
        /// <summary>
        /// GET /UserManagement/PageModifyPassword 修改密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PageModifyPassword()
        {
            return View("Page_ModifyPassword");
        }
        /// <summary>
        /// POST /UserManagement/GetUserInfo 
        /// </summary>
        /// <returns></returns>
        [Right]
        [HttpPost]
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
        /// <summary>
        /// POST /UserManagement/  根据Session获取当前用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchLoginingUserInfo()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string id = Session["id"].ToString();
            if (string.IsNullOrWhiteSpace(id))
            {
                Log4NetUtils.Warn(this, "查询用户信息，非法入侵！来自uil：" + Request.UrlReferrer);
                return Content(errorJsonString);
            }
            Guid idGuid;
            if (!Guid.TryParse(id, out idGuid))
            {
                Log4NetUtils.Warn(this, "查询用户信息，非法入侵！来自uil：" + Request.UrlReferrer);
                return Content(errorJsonString);
            }

            User user = new UserBLL().SearchModelObjectByID<User>(idGuid);
            if (user == null)
            {
                Log4NetUtils.Error(this, "查询用户信息，根据uid查询用户信息失败！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append($"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\",");
            re.Append("\"user\":");
            re.Append(user.ToJsonString(true));
            re.Append("}");
            return Content(re.ToString());


        }
        /// <summary>
        ///  POST /UserManagement/  修改用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifySingleUserInfo()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string idString = Request.Form["id"];
            string uidString = Request.Form["uid"];
            string emailString = Request.Form["email"];
            string phoneString = Request.Form["phone"];
            string ageString = Request.Form["age"];
            string genderString = Request.Form["gender"];
            string addressString = Request.Form["address"];

            /*数据验证*/
            if (string.IsNullOrWhiteSpace(uidString))
            {
                Log4NetUtils.Error(this, "修改个人信息，接收前台参数uid失败！");
                return Content(errorJsonString);
            }
            Guid idGuid;
            int ageInt, genderInt;
            if (!Guid.TryParse(idString, out idGuid)) {
                Log4NetUtils.Error(this, "修改个人信息，接收用户id失败");
                return Content(errorJsonString);
            }
            if (!int.TryParse(ageString, out ageInt)) {
                Log4NetUtils.Error(this, "修改个人信息，接收用户age失败");
                ageInt = -1;
            }
            if (!int.TryParse(genderString, out genderInt))
            {
                Log4NetUtils.Error(this, "修改个人信息，接收用户gender失败");
                genderInt = -1;
            }
            UserBLL userBll = new UserBLL();
            User user = userBll.SearchModelObjectByID<User>(idGuid);
            if (user == null)
            {
                Log4NetUtils.Error(this, "修改个人信息，查询用户模型失败！");
                return Content(errorJsonString);
            }
            user.f_uid = uidString;
            user.f_phone = phoneString;
            user.f_address = addressString;
            user.f_age = ageInt;
            user.f_gender = genderInt;
            user.f_email = emailString;
            if (!userBll.ModifyModel<User>(user))
            {
                Log4NetUtils.Error(this, "修改个人信息，修改用户模型失败！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append($"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}");
            return Content(re.ToString());
        }
        /// <summary>
        /// POST /UserManagement/ValidateUid  验证用户名状态 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidateUid()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string id = Request.Form["id"];
            string uid = Request.Form["uid"];
            Guid idGuid;
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(uid)||!Guid.TryParse(id,out idGuid))
            {
                Log4NetUtils.Error(this,"验证失败！");
                return Content(errorJsonString);
            }
            Dictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "f_uid,Eq",uid},
                { "f_exist,Eq",CommonEnum.DataExist.EXIST}
            };
            List<User> userList = new UserBLL().SearchModelObjectListByCondition<User>(condition);
            if (userList == null)
            {
                Log4NetUtils.Error(this,"验证用户名，查询失败！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append("{\"result\":\"" + CommonEnum.AjaxResult.SUCCESS + "\",");
            re.Append("\"state\":\"");
            if (userList.Count > 1)
            {
                re.Append(CommonEnum.UserIDState.EXIST);
            }
            else if (userList.Count == 1)
            {
                User u = userList[0];
                if (u.f_id.Equals(idGuid))
                {
                    re.Append(CommonEnum.UserIDState.LEGAL);
                }
                else
                {
                    re.Append(CommonEnum.UserIDState.UNLEGAL);
                }
            }
            else {
                re.Append(CommonEnum.UserIDState.LEGAL);
            }
            re.Append("\"}");
            return Content(re.ToString());
        }
        /// <summary>
        /// POST /UserManagement/ResetPassword 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string idString = Request.Form["id"];
            if (string.IsNullOrWhiteSpace(idString))
            {
                Log4NetUtils.Error(this, "重置密码，接收前台参数失败！");
                return Content(errorJsonString);
            }
            Guid idGuid;
            if (!Guid.TryParse(idString, out idGuid))
            {
                Log4NetUtils.Error(this, "重置密码，转换用户id失败！");
                return Content(errorJsonString);
            }
            User user = new UserBLL().SearchModelObjectByID<User>(idGuid);
            if (user == null)
            {
                Log4NetUtils.Error(this, "重置密码，查询用户失败！");
                return Content(errorJsonString);
            }
            user.f_pwd = CommonFunction.MD5Encrypt(CommonEnum.UserDefaultParas.DEFAULT_PASSWORD);
            if (!new UserBLL().ModifyModel<User>(user))
            {
                Log4NetUtils.Error(this, "重置密码，重置密码失败！");
                return Content(errorJsonString);
            }
            string re = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
            return Content(re);
        }
        /// <summary>
        /// POST /UserManagement/ModifyPassword 修改用户密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyPassword()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string idString = Request.Form["id"];
            string pwdOldString = Request.Form["pwd_old"];
            string pwdNewString = Request.Form["pwd_new"];
            if (string.IsNullOrWhiteSpace(idString))
            {
                Log4NetUtils.Error(this, "修改密码，接收前台参数失败！");
                return Content(errorJsonString);
            }
            Guid idGuid;
            if (!Guid.TryParse(idString, out idGuid))
            {
                Log4NetUtils.Error(this, "修改密码，转换用户id失败！");
                return Content(errorJsonString);
            }
            if (string.IsNullOrWhiteSpace(pwdOldString))
            {
                Log4NetUtils.Error(this,"修改密码，接收前台旧密码失败！");
                return Content(errorJsonString);
            }
            if (string.IsNullOrWhiteSpace(pwdNewString))
            {
                Log4NetUtils.Error(this,"修改密码，接收前台新密码失败！");
                return Content(errorJsonString);
            }
          
            User user = new UserBLL().SearchModelObjectByID<User>(idGuid);
            if (user == null)
            {
                Log4NetUtils.Error(this, "修改密码，查询用户失败！");
                return Content(errorJsonString);
            }
            if (!user.f_pwd.Equals(CommonFunction.MD5Encrypt(pwdOldString)))
            {
                return Content(errorJsonString);
            }
            user.f_pwd = CommonFunction.MD5Encrypt(pwdNewString);
            if (!new UserBLL().ModifyModel<User>(user))
            {
                Log4NetUtils.Error(this, "修改密码，修改密码失败！");
                return Content(errorJsonString);
            }

            string re = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
            return Content(re);
        }
    }
}