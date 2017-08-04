using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers.Blog
{
    public class BlogController : Controller
    {
        /// <summary>
        /// GET /Blog/Index
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            return View("Page_Search_Blog_List");
        }
        /// <summary>
        /// GET /Blog/AddBlogPage
        /// </summary>
        /// <returns></returns>
        public ActionResult AddBlogPage()
        {
            return View("Page_Add_Blog");
        }
        /// <summary>
        /// GET /Blog/BlogManagePage 
        /// </summary>
        /// <returns></returns>
        public ActionResult BlogManagePage()
        {
            return View("Page_Blog_Manage");
        }


    }
}