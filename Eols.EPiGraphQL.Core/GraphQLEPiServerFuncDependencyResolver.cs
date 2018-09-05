using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eols.EPiGraphQL.Core
{
    public class GraphQLEPiServerFuncDependencyResolver : IGraphQLEPiServerDependencyResolver
    {
        private readonly IServiceLocator _serviceLocator;

        public GraphQLEPiServerFuncDependencyResolver(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _serviceLocator.GetAllInstances(serviceType);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetAllInstances(typeof(TService)).OfType<TService>();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            return _serviceLocator.GetInstance(type);
        }
    }
}