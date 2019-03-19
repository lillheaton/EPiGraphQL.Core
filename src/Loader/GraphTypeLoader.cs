using EPiServer.ServiceLocation;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiGraphQL.Core.Loader
{
    public class GraphTypeLoader
    {
        private static string LocalAssemblyName => 
            string.Join(".", typeof(GraphTypeLoader).Assembly.GetName().Name.Split('.').Take(2));

        private static bool IsLocalAssemblyGraph(object target) =>
            target.GetType().Assembly.FullName.Contains(LocalAssemblyName);

        private static bool HasGenericArgument<TArgument>(IInterfaceGraphType target) =>
            target.GetType().BaseType.GetGenericArguments().Any(x => x == typeof(TArgument));

        private static bool HasSameGenericArgument(IInterfaceGraphType a, IInterfaceGraphType b) =>
            a.GetType().BaseType.GetGenericArguments()[0].Equals(b.GetType().BaseType.GetGenericArguments()[0]);


        public static IEnumerable<ICustomGraphType> GetCustomGraphTypes(IServiceLocator serviceLocator)
        {
            // Get all registered ICustomGraphTypes
            var allCustomGraphTypes = serviceLocator.GetAllInstances<ICustomGraphType>();

            // Filter out the types based on assembly
            var localAssemblyGraphs = allCustomGraphTypes.Where(IsLocalAssemblyGraph);
            var otherAssemblyGraphs = allCustomGraphTypes.Where(x => IsLocalAssemblyGraph(x) == false);

            // Allow other assemblies to override local definitions
            return otherAssemblyGraphs.Concat(
                localAssemblyGraphs.Where(x => otherAssemblyGraphs.Any(y => y.TargetType.Equals(x.TargetType)) == false)
            );
        }

        public static IInterfaceGraphType GetGraphInterface<TSource>(IServiceLocator serviceLocator)
        {
            var allGraphInterfaces = serviceLocator.GetAllInstances<IInterfaceGraphType>();

            // First check if there is an interface from another aseembly
            var otherAssemblyInterface = allGraphInterfaces.FirstOrDefault(x => IsLocalAssemblyGraph(x) == false && HasGenericArgument<TSource>(x));
            if (otherAssemblyInterface != null)
                return otherAssemblyInterface;

            // If not try load from current assembly
            return allGraphInterfaces.FirstOrDefault(x => IsLocalAssemblyGraph(x) && HasGenericArgument<TSource>(x));
        }

        public static IEnumerable<IInterfaceGraphType> GetGraphInterfaces(IServiceLocator serviceLocator)
        {
            var allGraphInterfaces = serviceLocator.GetAllInstances<IInterfaceGraphType>();

            // Filter out the types based on assembly
            var localAssemblyGraphs = allGraphInterfaces.Where(IsLocalAssemblyGraph);
            var otherAssemblyGraphs = allGraphInterfaces.Where(x => IsLocalAssemblyGraph(x) == false);

            // Allow other assemblies to override local definitions
            return otherAssemblyGraphs.Concat(
                localAssemblyGraphs.Where(x => otherAssemblyGraphs.Any(y => HasSameGenericArgument(y, x)) == false)
            );
        }

        public static IEnumerable<IEPiServerGraphUnion> GetGraphUnions(IServiceLocator serviceLocator)
        {
            var allGraphUnions = serviceLocator.GetAllInstances<IEPiServerGraphUnion>();

            // Filter out the types based on assembly
            var localAssemblyUnions = allGraphUnions.Where(IsLocalAssemblyGraph);
            var otherAssemblyUnions = allGraphUnions.Where(x => IsLocalAssemblyGraph(x) == false);

            // Allow other assemblies to override local definitions
            return otherAssemblyUnions.Concat(
                localAssemblyUnions.Where(x => otherAssemblyUnions.Any(y => y.Name != x.Name) == false)
            );
        }
    }
}
