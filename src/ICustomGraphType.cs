using GraphQL.Types;
using System;

namespace Eols.EPiGraphQL.Core
{
    public interface ICustomGraphType : IGraphType
    {
        Type TargetType { get; }
    }
}