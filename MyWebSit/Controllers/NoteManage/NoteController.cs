using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BussinessLogicLayer;
using DataAccessLayer;
using Model;

namespace MyWebSit.Controllers.NoteManage
{
    public class NoteController : Controller
    {
        /// <summary>
        /// GET /Note/Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<Message> l = new MessageBLL().SearchAllMessage();
            User u = new User();
            u.f_id = Guid.NewGuid();
            u.f_uid = "123";
            u.f_pwd = "123";
            u.f_exist = 1;
            u.f_reg_date = DateTime.Now;
            bool f = new UserBLL().AddModel<User>(u);
            return View("Page_Note");
        }
        /// <summary>
        /// POST /Note/SubmitMessage 提交留言信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitNote()
        {
            
            return Content("{\"result\":0}");
        }
        /// <summary>
        /// post /Note/SearchNoteList
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchCommonList()
        {
            
            return Content(null);
        }
        /// <summary>
        /// POST /Note/DeleteSingleNote
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteSingleNote()
        {
           
            return Content(null);
        }
    }
}