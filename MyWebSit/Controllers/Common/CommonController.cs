using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MyWebSit.Controllers.Common
{
    public class CommonController : Controller
    {
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// GET /Common/ErrorLogin 
        /// </summary>
        /// <returns></returns>
        public ActionResult ErrorLogin()
        {
            return View("Page_Login_Error");
        }
        /// <summary>
        /// GET /Common/NotFondPage 404页面
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFondPage()
        {
            return View("Page_Notfond");
        }
    }
}