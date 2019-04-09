using GraphQL.Types;
using System;

namespace Graphify.EPiServer.Core
{
    public interface ICustomGraphType : IGraphType
    {
        Type TargetType { get; }
    }
}