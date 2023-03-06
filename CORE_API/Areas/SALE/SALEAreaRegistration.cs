using System.Web.Mvc;

namespace CORE_API.Areas.SALE
{
    public class SALEAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SALE";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SALE_default",
                "SALE/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}