using Autofac;
using Autofac.Integration.Mvc;
using SerkoTest.BusinessRepository.Class;
using SerkoTest.BusinessRepository.Interface;
using SerkoTestRepository.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SerkoTestRepository
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            SetAutofacContainer();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();

            // Register individual components
            builder.RegisterInstance(new ParseTextRepository())
                   .As<IParseTextRepository>();
            builder.RegisterType<HomeController>();        
                  
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }        
    }
}
