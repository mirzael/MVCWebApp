using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;
using OrderWebApplication.Controllers;

namespace OrderWebApplication.IoC
{
    public class InjectionControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType != typeof(AccountController))
            {
                return DependencyContainer.Container.GetInstance(controllerType) as Controller;
            }
            else
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }
            
        }
    }
}

