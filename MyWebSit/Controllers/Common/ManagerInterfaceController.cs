using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.NHibernate;
using NHibernate;

using System.Text;

namespace MyWebSit.Controllers.Common
{
    public class ManagerInterfaceController : Controller
    {
        // GET: ManagerInterface
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///  /ManagerInterface/ExcuteSql?r= &sql=
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcuteSql()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            string sql = Request.QueryString["sql"];
            
            string right = CommonFunction.MD5Encrypt(Request.QueryString["r"]?.ToLower());
            string aim = "1?e!晜\u0019?t厎\u001b窿";
            if (!aim.Equals(right)||string.IsNullOrWhiteSpace(sql)||sql.ToUpper().StartsWith("DELETE")||sql.ToUpper().StartsWith("DROP"))
            {
                return Content(errorJsonString);
            }
            ISession session = null;
            try
            {
                session = SessionManager.OpenSession();
                ISQLQuery iSQLQuery = session.CreateSQLQuery(sql);

                if (sql.ToUpper().StartsWith("SELECT"))
                {
                    var o = iSQLQuery.List<object[]>().ToList<object[]>();
                    StringBuilder re = new StringBuilder();
                    re.Append("[");
                    bool isStart = true;
                    foreach (object[] item in o)
                    {
                        re.Append(isStart ? "[" : ",[");
                        isStart = false;
                        bool start = true;
                        foreach (object x in item)
                        {
                            re.Append(start ? "" : ",");
                            start = false;
                            string xstr = x == null ? "null" : x?.ToString();
                            xstr = Uri.EscapeDataString(xstr);
                            re.Append("\""+xstr+"\"");
                        }
                        re.Append("]");
                    }
                    re.Append("]");

                    return Content(re.ToString());
                }
                else
                {
                    var r = iSQLQuery.ExecuteUpdate();
                    return Content($"result:【{r}】");
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message+"\r\n"+ex.StackTrace);
            }
            finally
            {
                SessionManager.CloseSession(session);
            }
        }
    }
}