using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BILMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "Pending",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Pending", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BILMS.Controllers" } 
            );
            
        }
    }
}
