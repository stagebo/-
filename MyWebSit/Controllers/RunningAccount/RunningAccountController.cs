using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers.RunningAccount
{
    public class RunningAccountController : Controller
    {
        // GET: RunningAccount
        public ActionResult Index()
        {
            return View("Page_Running_Account");
        }
    }
}