using System;
using System.Collections.Generic;

namespace Eols.EPiGraphQL.Core
{
    public interface IGraphQLEPiServerDependencyResolver : GraphQL.IDependencyResolver
    {
        IEnumerable<object> GetAllInstances(Type serviceType);
        IEnumerable<TService> GetAllInstances<TService>();
    }
}