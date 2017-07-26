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
            string sql = "select * from t_message";
            ISession session = SessionManager.OpenSession();
            session = SessionManager.OpenSession();
            //加载SQL语句
            ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);

            //查询结果并返回
            var t = iSQLQuery.List<object>().ToList<object>();
            int[] a = { 1, 2, 3 };
            Log4NetUtils.Error(this,"errr");
            string s = string.Join("#", a);
            return null;
        }
    }
}