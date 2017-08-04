using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MyWebSit
{
    public class GlobalInfomationAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            new Thread(() => {
                string browser = request.Browser?.Browser;
                string filePath = request.FilePath;
                string url = request.Url.ToString();
                string referUrl = request.UrlReferrer?.ToString();
                string fullUrl=request.RawUrl?.ToString();

                string hostIP = request.UserHostAddress;
                string hostName = request.UserHostName;
                var param = request.Params;
                
                //request.SaveAs("C:\\Users\\WANYONGBO\\Desktop\\FileDownload\\request.txt",true);

            }).Start();
            int i = 0;
        }
    }
}