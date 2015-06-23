using SimpleBlog.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            //get namespace of postcontroller in main controller folder
            var namespaces = new[] { typeof(PostsController).Namespace };
            //routes to ignore. this is always there. dont delete
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //remove default. hand write all my routes
            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );*/

            routes.MapRoute("Login", "login", new { controller = "Auth", action = "Login" }, namespaces);
            //Home - name of page
            //"" - home page - dosn't need any slashes
            // controller called is PostsController
            // method in PostsController called is Index
            routes.MapRoute("Home", "", new { controller = "Posts", action = "Index" }, namespaces);
        }
    }
}