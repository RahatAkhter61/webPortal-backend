using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CORE_API.Areas.WPS
{
    public class WPSAreaRegistration : AreaRegistration
    {

        public override string AreaName
        {
            get
            {
                return "WPS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WPS_default",
                "WPS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}