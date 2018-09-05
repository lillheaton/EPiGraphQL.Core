using GraphQL;
using GraphQL.Types;

namespace Eols.EPiGraphQL.Core
{
    public class EPiServerSchema : Schema
    {
        public EPiServerSchema(IDependencyResolver resolver)
        {
            Query = resolver.Resolve<IRootQuery>();
        }
    }
}