using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers.NoteManage
{
    public class NoteController : Controller
    {
        /// <summary>
        /// GET /Not/Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
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