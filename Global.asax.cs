using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

namespace FaceRecognitionSystem
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            // Append the PATH environment variable
            String _path = System.Environment.GetEnvironmentVariable("PATH");
            String _additionalPath = String.Concat(HttpRuntime.AppDomainAppPath, "");
            _path = String.Concat(_additionalPath, ";", _path);
            
            // Set the environment
            System.Environment.SetEnvironmentVariable("PATH", _path, EnvironmentVariableTarget.Process);

            RegisterRoutes(RouteTable.Routes);
        }
    }
}