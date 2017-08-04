
using System.Web;
using System.Web.Mvc;
using MyWebSit;

namespace MyWebSit
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalHandleErrorAttribute());
            filters.Add(new GlobalInfomationAttribute());
        }
    }
}