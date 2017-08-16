using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers
{
    public class QController : Controller
    {
        // GET: Q
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET/POST /Q/A 
        /// </summary>
        /// <returns></returns>
        public ActionResult A()
        {
            return View("Page_Quickly_Account");
        }
    }
}