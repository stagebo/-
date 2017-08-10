using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit.Controllers.Game
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            return View("Page_Game_GaoZiQi");
        }
    }
}