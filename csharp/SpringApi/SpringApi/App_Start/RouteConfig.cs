using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpringApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "App", action = "Index" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "App", action = "Search"}
            );

            routes.MapRoute(
                name: "Listing",
                url: "listing/{id}",
                defaults: new { controller = "App", action = "Listing" }
            );

            routes.MapRoute(
                name: "ListingCounters",
                url: "listingcounters/{id}",
                defaults: new { controller = "App", action = "ListingCounters" }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Auth", action = "Login" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Auth", action = "Logout" }
            );

            routes.MapRoute(
                name: "Oauth",
                url: "oauth",
                defaults: new { controller = "Auth", action = "Oauth" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}