using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using StructureMap.Web;
using System.Web.Http.Dispatcher;
using System.Web.Http.Controllers;
using System.Net.Http;
using StructureMap;

namespace OrderWebApplication.IoC
{
    public class ServiceActivator : IHttpControllerActivator
    {
        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = ObjectFactory.GetInstance(controllerType) as IHttpController;
            return controller;
        }
    }
}