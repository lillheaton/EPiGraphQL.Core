using GraphQL.Types;
using System;

namespace EPiGraphQL.Core
{
    public interface ICustomGraphType : IGraphType
    {
        Type TargetType { get; }
    }
}