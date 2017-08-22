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
        [Right]
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
            /*判断字段是否存在TODO*/
            string sortField = string.IsNullOrWhiteSpace(Request.Form["sortField"]) ? "f_time" : Request.Form["sortField"];
            string sortOrder = "asc".Equals(Request.Form["sortOrder"]) ? "acs" : "desc";

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
            pageIndex = pageIndex > totalPage ? totalPage : pageIndex;


            Dictionary<string, object> condition = new Dictionary<string, object>()
            {
                  { "userID,Eq"     ,   userIDGuid                 }
                , { "f_exist,Eq"    ,   CommonEnum.DataExist.EXIST }
            };
            List<string[]> orderList = new List<string[]>()
            {
                new string[] {
                    sortField , sortOrder
                }
            };

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
                string p_id = o[1].ToString();
                string f_name = o[2].ToString();
                if (ra == null || f_name == null || p_id == null)
                {
                    continue;
                }

                re.Append(isStart ? "{" : ",{");
                isStart = false;
                re.Append("\"f_id\":\"" + ra.f_id + "\",");
                re.Append("\"f_type\":\"" + ra.f_type + "\",");
                re.Append("\"f_time\":\"" + ra.f_time + "\",");
                re.Append("\"f_money\":\"" + ra.f_money + "\",");
                re.Append("\"f_purpose_name\":\"" + f_name + "\",");
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

        /// <summary>
        /// POST /RunningAccount/InsertAccountPurpose  
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertAccountPurpose()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string name = Request.Form["name"];
            string descript = Request.Form["descript"];
            if (string.IsNullOrWhiteSpace(name))
            {
                Log4NetUtils.Error(this, "插入类型，接收参数失败！");
                return Content(errorJsonString);
            }
            AccountPurpose ap = new AccountPurpose();
            ap.f_name = name;
            ap.f_descript = descript;
            ap.f_id = Guid.NewGuid();
            ap.f_type = 1;

            if (!new AccountPurposeBLL().AddModel<AccountPurpose>(ap))
            {
                Log4NetUtils.Error(this, "插入类型，插入模型失败！");
                return Content(errorJsonString);
            }

            string result = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
            return Content(result);
        }

        /// <summary>
        /// POST  /RunningAccount/GetRunningAccountDataByCondition
        /// </summary>
        /// <returns></returns>
        [Right]
        [HttpPost]
        public ActionResult GetRunningAccountDataByCondition()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            /*参数接收以及验证*/
            string methodString = Request.Form["method"];
            string countString = Request.Form["count"];
            string typeString = Request.Form["type"];

            /*获取当前登录用户信息*/
            string userIDString = Session["id"].ToString();
            Guid userIDGuid;
            if (!Guid.TryParse(userIDString, out userIDGuid))
            {
                Log4NetUtils.Error(this, "查询流水账信息，获取登陆用户信息失败！");
            }
            /*测试*/
            //methodString = "day";
            //countString = "7";
            //typeString = "1";

            int type, count;
            string field = null;
            DateTime time;
            if (string.IsNullOrWhiteSpace(methodString) || string.IsNullOrWhiteSpace(countString) || string.IsNullOrWhiteSpace(typeString)
                || !int.TryParse(typeString, out type) || !int.TryParse(countString, out count))
            {
                Log4NetUtils.Error(this, "查询流水账数据集，接收参数失败！");
                return Content(errorJsonString);
            }
            count = count - 1;
            List<object[]> fieldNameInfo = new List<object[]>();
            string timeString;
            switch (methodString)
            {
                case "day":
                    field = "f_day";
                    time = DateTime.Now.AddDays(-count);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddDays(-i).Day,DateTime.Now.AddDays(-i)
                        });
                    }
                    timeString = $"{time.Year}-{time.Month}-{time.Day}";
                    break;
                case "month":
                    field = "f_month";
                    time = DateTime.Now.AddMonths(-count);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddMonths(-i).Month,DateTime.Now.AddMonths(-i)
                        });
                    }
                    timeString = $"{time.Year}-{time.Month}-{1}";
                    break;
                case "year":
                    field = "f_year";
                    time = DateTime.Now.AddYears(-count + 1);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddYears(-i).Year,DateTime.Now.AddYears(-i)
                        });
                    }
                    timeString = $"{time.Year}-{1}-{1}";
                    break;
                default:
                    Log4NetUtils.Error(this, "查询流水账数据集，接收参数存在问题！");
                    return Content(errorJsonString);
            }
            count += 1;
            Dictionary<string, object> condition = new Dictionary<string, object>();
            condition.Add("field,Eq", field);
            condition.Add("type,Eq", type);
            condition.Add("datetime,Gt", timeString);
            condition.Add("userID,Eq", userIDGuid);
            Dictionary<int, decimal> raList = new RunningAccountBLL().SearchRunningAccountDataInfoListByCondition(condition);
            //var rraList = SessionManager.OpenSession().Query<RunningAccount>().Where<RunningAccount>(r =>
            //r.f_type == 1).ToList<RunningAccount>();
            if (raList == null)
            {
                Log4NetUtils.Error(this, "查询流水账数据集，查询Dictionary失败！");
                return Content(errorJsonString);
            }

            StringBuilder result = new StringBuilder();
            result.Append($"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\",");
            result.Append("\"dataList\":[");
            bool isStart = true;
            StringBuilder dL = new StringBuilder("[");
            StringBuilder tL = new StringBuilder("[");
            for (int i = 0; i < count; i++)
            {
                result.Append(isStart ? "{" : ",{");
                tL.Append(isStart ? "" : ",");
                dL.Append(isStart ? "" : ",");
                isStart = false;
                decimal resultMoney = -1;
                if (raList.ContainsKey((int)fieldNameInfo[i][0]))
                {
                    resultMoney = raList[(int)fieldNameInfo[i][0]];
                }

                result.Append("\"dateTime\":\"" + fieldNameInfo[i][1] + "\",");
                result.Append("\"money\":\"" + resultMoney + "\"");
                tL.Append($"\"{fieldNameInfo[i][1]}\"");
                dL.Append($"\"{resultMoney}\"");
                result.Append("}");
            }
            tL.Append("]");
            dL.Append("]");
            result.Append("],");
            result.Append("\"dL\":" + dL.ToString() + ",");
            result.Append("\"tL\":" + tL.ToString() + "");
            result.Append("}");
            return Content(result.ToString());

        }
        /// <summary>
        /// POST  /RunningAccount/GetAllRunningAccountDataByCondition
        /// </summary>
        /// <returns></returns>
        //[Right]
        //[HttpPost]
        public ActionResult GetAllRunningAccountDataByCondition()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";

            /*参数接收以及验证*/
            string methodString = Request.Form["method"];
            string countString = Request.Form["count"];
            /*测试*/
            //User u = SessionManager.OpenSession().Query<User>().SingleOrDefault<User>
            //    (us => us.f_uid == "c");
            //Session["id"] = u.f_id;
            /*获取当前登录用户信息*/
            string userIDString = Session["id"].ToString();
            Guid userIDGuid;
            if (!Guid.TryParse(userIDString, out userIDGuid))
            {
                Log4NetUtils.Error(this, "查询流水账信息，获取登陆用户信息失败！");
            }
            /*测试*/
            //methodString = "day";
            //countString = "10";

            int count;
            string field = null;
            DateTime time;
            if (string.IsNullOrWhiteSpace(methodString) || string.IsNullOrWhiteSpace(countString) || !int.TryParse(countString, out count))
            {
                Log4NetUtils.Error(this, "查询流水账数据集，接收参数失败！");
                return Content(errorJsonString);
            }
            count = count - 1;
            List<object[]> fieldNameInfo = new List<object[]>();
            string timeString;
            switch (methodString)
            {
                case "day":
                    field = "f_day";
                    time = DateTime.Now.AddDays(-count);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddDays(-i).Day,DateTime.Now.AddDays(-i)
                        });
                    }
                    timeString = $"{time.Year}-{time.Month}-{time.Day}";
                    break;
                case "month":
                    field = "f_month";
                    time = DateTime.Now.AddMonths(-count);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddMonths(-i).Month,DateTime.Now.AddMonths(-i)
                        });
                    }
                    timeString = $"{time.Year}-{time.Month}-{1}";
                    break;
                case "year":
                    field = "f_year";
                    time = DateTime.Now.AddYears(-count + 1);
                    for (int i = count; i >= 0; i--)
                    {
                        fieldNameInfo.Add(new object[] {
                            DateTime.Now.AddYears(-i).Year,DateTime.Now.AddYears(-i)
                        });
                    }
                    timeString = $"{time.Year}-{1}-{1}";
                    break;
                default:
                    Log4NetUtils.Error(this, "查询流水账数据集，接收参数存在问题！");
                    return Content(errorJsonString);
            }
            count += 1;
            Dictionary<string, object> condition = new Dictionary<string, object>();
            condition.Add("field,Eq", field);
            condition.Add("type,Eq", CommonEnum.RunningAccountType.INCOME);
            condition.Add("datetime,Gt", timeString);
            condition.Add("userID,Eq", userIDGuid);
            Dictionary<int, decimal> raInList = new RunningAccountBLL().SearchRunningAccountDataInfoListByCondition(condition);
            //var rraList = SessionManager.OpenSession().Query<RunningAccount>().Where<RunningAccount>(r =>
            //r.f_type == 1).ToList<RunningAccount>();
            if (raInList == null)
            {
                Log4NetUtils.Error(this, "查询流水账数据集，查询Dictionary失败！");
                return Content(errorJsonString);
            }
            condition["type,Eq"] = CommonEnum.RunningAccountType.OUTCOME;
            Dictionary<int, decimal> raOutList = new RunningAccountBLL().SearchRunningAccountDataInfoListByCondition(condition);
            if (raOutList == null)
            {
                Log4NetUtils.Error(this, "查询流水账数据集，查询Dictionary失败！");
                return Content(errorJsonString);
            }

            StringBuilder result = new StringBuilder();
            result.Append($"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\",");
            /*收入数据*/
            result.Append("\"allInDataList\":[");
            bool isStart = true;
            StringBuilder inDataList = new StringBuilder("[");
            StringBuilder inTextList = new StringBuilder("[");
            StringBuilder outDataList = new StringBuilder("[");
            StringBuilder outTextList = new StringBuilder("[");
            for (int i = 0; i < count; i++)
            {
                result.Append(isStart ? "{" : ",{");
                inDataList.Append(isStart ? "" : ",");
                inTextList.Append(isStart ? "" : ",");
                isStart = false;
                decimal resultInMoney = -1;
                if (raInList.ContainsKey((int)fieldNameInfo[i][0]))
                {
                    resultInMoney = raInList[(int)fieldNameInfo[i][0]];
                }


                result.Append("\"dateTime\":\"" + fieldNameInfo[i][1] + "\",");
                result.Append("\"money\":\"" + resultInMoney + "\"");
                inTextList.Append($"\"{fieldNameInfo[i][1]}\"");
                inDataList.Append($"\"{resultInMoney}\"");
                result.Append("}");
            }
            result.Append("],");
            inTextList.Append("]");
            inDataList.Append("]");

            /*支出数据*/
            result.Append("\"allOutDataList\":[");
            isStart = true;
            for (int i = 0; i < count; i++)
            {
                result.Append(isStart ? "{" : ",{");
                outDataList.Append(isStart ? "" : ",");
                outTextList.Append(isStart ? "" : ",");
                isStart = false;
                decimal resultOutMoney = -1;
                if (raOutList.ContainsKey((int)fieldNameInfo[i][0]))
                {
                    resultOutMoney = raOutList[(int)fieldNameInfo[i][0]];
                }
                result.Append("\"dateTime\":\"" + fieldNameInfo[i][1] + "\",");
                result.Append("\"money\":\"" + resultOutMoney + "\"");
                outTextList.Append($"\"{fieldNameInfo[i][1]}\"");
                outDataList.Append($"\"{resultOutMoney}\"");
                result.Append("}");
            }
            result.Append("],");
            outTextList.Append("]");
            outDataList.Append("]");

            
            result.Append("\"inDataList\":" + inDataList.ToString() + ",");
            result.Append("\"inTextList\":" + inTextList.ToString() + ",");
            result.Append("\"outDataList\":" + outDataList.ToString() + ",");
            result.Append("\"outTextList\":" + outTextList.ToString() + "");

            result.Append("}");
            return Content(result.ToString());

        }

    }
}