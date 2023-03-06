using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CORE_API.Areas.PCMS
{
    public class PCMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PCMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PCMS_default",
                "PCMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}