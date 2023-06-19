using BILMS.App_Start;
using System.Web;
using System.Web.Mvc;

namespace BILMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
       
        }
    }
}
