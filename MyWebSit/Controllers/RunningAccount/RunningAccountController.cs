using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BussinessLogicLayer;
using Common;
using Common.Log4Net;
using Common.NHibernate;
using Model;
using NHibernate;
using NHibernate.Linq;
using WebBlog.Filter;

namespace MyWebSit.Controllers
{
    public class RunningAccountController : Controller
    {
        // GET: RunningAccount
        [HttpGet]
        public ActionResult Index()
        {
            return View("Page_Running_Account");
        }

        /// <summary>
        /// POST /RunningAccount/SearchUserNameList 
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchUserNameList()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            string uid = Request.Form[""];

            Dictionary<string, object> condition = new Dictionary<string, object>() {
                { "f_uid,Like",$"%{uid}%"}
            };
            List<User> uList = new UserBLL().SearchModelObjectListByCondition<User>(condition);
            if (uList == null)
            {
                Log4NetUtils.Error(this, "查询用户名列表：查询userList失败！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append($@"
                    {{'result':'{CommonEnum.AjaxResult.SUCCESS}',
                    'data':[                                    ");
            bool isStart = true;
            foreach (User u in uList)
            {
                re.Append(isStart ? "{" : ",{");
                isStart = false;
                re.Append($@"
                        'id':'{u.f_id}',
                        'text':'{u.f_uid}'
                        ");
                re.Append("}");
            }

            re.Append("]}");

            return Content(re.ToString());
        }

        /// <summary>
        /// POST /RunningAccount/SearchAccountPurposeList 
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchAccountPurposeList()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            List<AccountPurpose> apList = new UserBLL().SearchModelObjectListByCondition<AccountPurpose>();
            if (apList == null)
            {
                Log4NetUtils.Error(this, "查询用途类型，查询失败！");
                return Content(errorJsonString);
            }
            StringBuilder re = new StringBuilder();
            re.Append($@"
                    {{'result':'{CommonEnum.AjaxResult.SUCCESS}',
                    'data':[                                    ");
            bool isStart = true;
            foreach (AccountPurpose ap in apList)
            {
                re.Append(isStart ? "{" : ",{");
                isStart = false;
                re.Append($@"
                        'id':'{ap.f_id}',
                        'text':'{ap.f_name}'
                        ");
                re.Append("}");
            }

            re.Append("]}");

            return Content(re.ToString());
        }

        /// <summary>
        /// POST  /RunningAccount/InsertAccount
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertAccount()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            ISession session = SessionManager.OpenSession();
            /*接收数据*/
            string uidString = Request.Form["uid"];
            string purposeString = Request.Form["purpose"];
            string numString = Request.Form["num"];
            string desString = Request.Form["descript"];
            string typeString = Request.Form["type"];

            /*数据验证*/
            Guid purposeGuid;
            Decimal numDecimal;
            int type;
            bool purposeValidate = Guid.TryParse(purposeString, out purposeGuid);
            User user = session.Query<User>().SingleOrDefault<User>(u => u.f_uid == uidString
            && u.f_exist == CommonEnum.DataExist.EXIST);
            AccountPurpose ap = SessionManager.OpenSession().
                Query<AccountPurpose>().SingleOrDefault<AccountPurpose>(u => u.f_id == purposeGuid);

            if (!purposeValidate || !Decimal.TryParse(numString, out numDecimal) || user == null || ap == null
                || !int.TryParse(typeString, out type))
            {
                Log4NetUtils.Error(this, "插入流水账，接收参数失败！");
                return Content(errorJsonString);
            }

            /*数据插入*/
            RunningAccount ra = new RunningAccount();

            ra.f_id = Guid.NewGuid();
            ra.f_time = DateTime.Now;
            ra.f_year = DateTime.Now.Year;

            ra.f_month = DateTime.Now.Month;
            ra.f_day = DateTime.Now.Day;
            ra.f_exist = 1;

            ra.f_address = "";//TODO
            ra.f_money = numDecimal;
            ra.f_purpose_id = ap.f_id;

            ra.f_remark = desString;
            ra.f_user_id = user.f_id;
            ra.f_type = type;

            if (!new UserBLL().AddModel<RunningAccount>(ra))
            {
                Log4NetUtils.Error(this, "插入流水账，插入失败！");
                return Content(errorJsonString);
            }

            /*数据返回*/
            string re = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}"; ;
            return Content(re);

        }

        /// <summary>
        /// GET /RunningAccount/GetAccountType
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAccountType()
        {
            string result = $@"
                            [
                            {{'id':'{CommonEnum.RunningAccountType.INCOME}','text':'收入'}}
                            ,{{'id':'{CommonEnum.RunningAccountType.OUTCOME}','text':'支出'}}
                            ]
                            ";
            return Content(result);
        }

        /// <summary>
        /// POST /RunningAccount/SearchAccountListByUser
        /// </summary>
        /// <returns></returns>
        [Right]
        [HttpPost]
        public ActionResult SearchAccountListByUser()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            Guid userIDGuid;
            if (!Guid.TryParse(Session["id"]?.ToString(), out userIDGuid))
            {
                Log4NetUtils.Error(this, "查询流水账，无登陆信息。");
                return Content(errorJsonString);
            }

            string pageSizeString = string.IsNullOrWhiteSpace(Request.Form["pageSize"]) ? "20" : Request.Form["pageSize"];
            string pageIndexString = string.IsNullOrWhiteSpace(Request.Form["pageIndex"]) ? "0" : Request.Form["pageIndex"];

            /*页码处理，前端传过来从0开始，数据库从1开始*/
            int pageSize, pageIndex;
            if (!int.TryParse(pageSizeString, out pageSize) || !int.TryParse(pageIndexString, out pageIndex))
            {
                Log4NetUtils.Error(this, "查询日常记账，接收参数失败！");
                return Content(errorJsonString);
            }
            int totalCount = SessionManager.OpenSession().Query<RunningAccount>().Where<RunningAccount>(
                a => a.f_user_id == userIDGuid && a.f_exist == CommonEnum.DataExist.EXIST).Count<RunningAccount>();
            pageSize = pageSize > 100 ? 100 : pageSize;
            pageSize = pageSize < 1 ? 20 : pageSize;
            int totalPage = (totalCount - 1) / pageSize + 1;
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            pageIndex = pageIndex > totalPage - 1 ? (totalPage - 1) : pageIndex;
            pageIndex = pageIndex + 1;


            Dictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "userID,Eq",userIDGuid}
                , { "f_exist,Eq",CommonEnum.DataExist.EXIST}
            };
            List<string[]> orderList = new List<string[]>()
            {
                new string[] {
                    "R.f_type","asc"
                }
            };
            pageSize = 1;

            List<object[]> accountInfoList = new RunningAccountBLL().SearchAccountInfoListByCondition(condition, orderList, pageIndex, pageSize);
            if (accountInfoList == null)
            {
                Log4NetUtils.Error(this, "查询流水账，列表为空。");
                return Content(errorJsonString);
            }

            StringBuilder re = new StringBuilder();
            re.Append("{\"result\":\"" + CommonEnum.AjaxResult.SUCCESS + "\",");
            re.Append("\"totalCount\":\"" + totalCount + "\",");
            re.Append("\"totalPage\":\"" + totalPage + "\",");
            re.Append("\"data\":[");
            bool isStart = true;
            foreach (object[] o in accountInfoList)
            {
                RunningAccount ra = o[0] as RunningAccount;
                AccountPurpose ap = o[1] as AccountPurpose;
                User u = o[2] as User;
                if (ra == null || ap == null || User == null)
                {
                    continue;
                }

                re.Append(isStart ? "{" : ",{");
                isStart = false;
                re.Append("\"f_id\":\"" + ra.f_id + "\",");
                re.Append("\"f_type\":\"" + ra.f_type + "\",");
                re.Append("\"f_time\":\"" + ra.f_time + "\",");
                re.Append("\"f_money\":\"" + ra.f_money + "\",");
                re.Append("\"f_purpose_name\":\"" + ap.f_name + "\",");
                re.Append("\"f_remark\":\"" + ra.f_remark + "\"");
                re.Append("}");


            }
            re.Append("]}");
            return Content(re.ToString());
        }
        /// <summary>
        /// POST /RunningAccount/GetBalanceByUser
        /// </summary>
        /// <returns></returns>
        [Right]
        [HttpPost]
        public ActionResult GetBalanceByUser()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            Guid userIDGuid;
            if (!Guid.TryParse(Session["id"]?.ToString(), out userIDGuid))
            {
                Log4NetUtils.Error(this, "查询余额，无登陆信息。");
                return Content(errorJsonString);
            }

            Dictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "userID,Eq",userIDGuid}
            };
            Dictionary<string, object> balanceInfo = new RunningAccountBLL().SearchBalanceInfoByCondition(condition);
            if (balanceInfo == null || !balanceInfo.ContainsKey("money"))
            {
                Log4NetUtils.Error(this, "查询余额，查询余额信息失败！");
                return Content(errorJsonString);
            }

            StringBuilder re = new StringBuilder();
            re.Append($"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\",");
            re.Append($"\"money\":\"{balanceInfo["money"]}\"}}");
            return Content(re.ToString());

        }


    }
}