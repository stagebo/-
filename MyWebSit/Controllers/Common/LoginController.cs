using Common.Log4Net;
using Common.NHibernate;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers.Common
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View("Page_Login");
        }
        public ActionResult Test()
        {
            return View("View");
        }
    }
}