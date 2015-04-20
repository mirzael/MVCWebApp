using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace OrderWebApplication.IoC
{
    public class InjectionControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return ObjectFactory.GetInstance(controllerType) as Controller;

        }
    }
}