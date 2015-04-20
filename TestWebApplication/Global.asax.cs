﻿using OrderWebApplication.IoC;
using OrderWebApplication.Repository;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OrderWebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Setting up MVC config
            ControllerBuilder.Current.SetControllerFactory(new InjectionControllerFactory());

            //Setting up Web Api Config
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Services
                  .Replace(typeof(IHttpControllerActivator), new ServiceActivator());
            
            //Setting up structure map IOC Container
            ObjectFactory.Initialize(registry => registry
                .For<IUnitOfWork>()
                .Use<UnitOfWork>());
        }
    }
}
