using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Base;
using BussinessLogicLayer;
using Common;
using Common.Log4Net;
using DataAccessLayer;
using Model;
using WebBlog.Filter;

namespace MyWebSit.Controllers.NoteManage
{
    [Right]

    public class NoteController : Controller
    {
        /// <summary>
        /// GET /Note/Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Page_Show_Note");
        }
        /// <summary>
        /// POST /Note/SubmitNote 提交留言信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitNote()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string successString = $"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
            
            string content = Request.Form["content"];
            content = Server.UrlDecode(content);
            if (string.IsNullOrWhiteSpace(content))
            {
                Log4NetUtils.Error(this, "提交留言，接收前端留言内容失败!");
                return Content(errorJsonString);
            }
            
            Message m = new Message();
            m.f_message_id = Guid.NewGuid();
            m.f_message_exist = CommonEnum.DataExist.EXIST;
            m.f_writer_name = Session["uid"].ToString();
            Guid writerIDGuid;
            Guid.TryParse(Session["id"].ToString(),out writerIDGuid);
            m.f_writer_id = writerIDGuid;
            m.f_common_date = DateTime.Now;
            m.f_content = content;
            if (!new MessageBLL().AddModel<Message>(m))
            {
                Log4NetUtils.Error(this,"提交留言，添加Message实体失败！");
                return Content(errorJsonString);
            }
            return Content(successString);
        }
        /// <summary>
        /// post /Note/SearchNoteList
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchNoteList()
        {
            string errorJsonString = $"{{\"result\":\"{CommonEnum.AjaxResult.ERROR}\"}}";
            string pageIndexString = Request.Form["pageIndex"];
            string pageSizeString = Request.Form["pageSize"];
            int pageIndex, pageSize;
            if (!int.TryParse(pageIndexString, out pageIndex))
            {
                pageIndex = 1;
            }
            if (!int.TryParse(pageSizeString, out pageSize))
            {
                pageSize = 15;
            }
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 15 ? 15 : pageSize;
            pageSize = pageSize > 50 ? 50 : pageSize;
            Dictionary<string, object> condition = new Dictionary<string, object>()
            {
                { "f_message_exist,Eq",CommonEnum.DataExist.EXIST}
            };
            int? totalCount = new MessageBLL().SearchModelObjectCountByCondition<Message>(condition);
            if (totalCount == null)
            {
                Log4NetUtils.Error(this,"查询留言，查询留言总数失败！");
                return Content(errorJsonString);
            }
            int totalPage=((int)totalCount-1)/ pageSize + 1;
            List<string[]> orderList = new List<string[]>();
            orderList.Add(new string[2] { "f_common_date", "desc"});
            List<Message> messageList = new MessageBLL().SearchModelObjectListByPage<Message>(condition,orderList,pageIndex,pageSize);
            if (messageList == null)
            {
                Log4NetUtils.Error(this,"查询留言，查询MessageList失败！");
                return Content(errorJsonString);
            }
            StringBuilder successStringBuilder = new StringBuilder();
            successStringBuilder.Append("{\"result\":\""+CommonEnum.AjaxResult.SUCCESS+"\",");
            successStringBuilder.Append($"\"totalPages\":\"{totalPage}\",");
            successStringBuilder.Append($"\"uid\":\"{Session["uid"]?.ToString()}\",");
            successStringBuilder.Append("\"data\":");
            successStringBuilder.Append(BaseModel.ModelListToJsonString(messageList));
            successStringBuilder.Append("}");
            return Content(successStringBuilder.ToString());
        }
        /// <summary>
        /// POST /Note/DeleteSingleNote
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteSingleNote()
        {
            string errorJsonString = "{\"result\":\""+CommonEnum.AjaxResult.ERROR+"\",\"state\":\"0\"}";
            string f_id = Request.Form["f_id"];
            if (string.IsNullOrWhiteSpace(f_id))
            {
                Log4NetUtils.Error(this,"删除留言，接收前端参数失败~");
                return Content(errorJsonString);
            }
            Guid f_idGuid;
            if (!Guid.TryParse(f_id,out f_idGuid))
            {
                Log4NetUtils.Error(this,"删除留言，前端参数不是Guid类型！");
                return Content(errorJsonString);
            }
            MessageBLL messageBLL = new MessageBLL();
            Message message = messageBLL.SearchModelObjectByID<Message>(f_idGuid);
            if (message == null) {
                Log4NetUtils.Error(this,$"删除留言，查询留言实体失败,实体id：{f_idGuid}");
                return Content(errorJsonString);
            }
            string uid = Session["uid"].ToString();
            if (!message.f_writer_name.Equals(uid)) {
                string err = "{\"result\":\"" + CommonEnum.AjaxResult.ERROR + "\",\"state\":\"1\"}";
                return Content(err);
            }

            message.f_message_exist = CommonEnum.DataExist.NOT_EXIST;
            if (!messageBLL.ModifyModel<Message>(message))
            {
                errorJsonString= "{\"result\":\"" + CommonEnum.AjaxResult.ERROR + "\",\"state\":\"2\"}"; ;
                Log4NetUtils.Error(this,$"删除留言，修改留言逻辑列失败！");
            }
            string successString =$"{{\"result\":\"{CommonEnum.AjaxResult.SUCCESS}\"}}";
             return Content(successString);
        }
       
    }
}