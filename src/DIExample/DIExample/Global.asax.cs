using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DIExample.Controllers;
using DIExample.Services;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace DIExample
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            StructureMapConfiguration.Initialize();

            var controllerFactory = new StructureMapControllerFactory(ObjectFactory.Container);

            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new NestedContainerViewEngine());
		}
    }
}