using OrderWebApplication.Repository;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderWebApplication.IoC
{
    public class DependencyContainer
    {
        private static IContainer _container;

        public static IContainer Container { get{
            if(_container == null){
                _container = new Container(x => {
                    x.For<IUnitOfWork>().Use<UnitOfWork>();
                });
            }
            return _container;
        } }

    }
}