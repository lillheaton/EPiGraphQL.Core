[![Build status](https://ci.appveyor.com/api/projects/status/xmugcagkt9k1k5u9?svg=true)](https://ci.appveyor.com/project/lillheaton/eols-epigraphql-core)

Adding [Facebook's GraphQL](https://github.com/facebook/graphql) for [EPiServer's](https://www.episerver.com/) platform. This tool automates creation of Graphs based on the defined available content types.

This project builds on top of [GraphQL .NET](https://github.com/graphql-dotnet/graphql-dotnet) library written by [Joe McBride](https://github.com/joemcbride) (MIT licence)

## Installation
You can install the latest version via [NuGet](https://www.nuget.org/packages/Eols.EPiGraphQL.Core/).

`PM> Install-Package Eols.EPiGraphQL.Core`

## Basic Usage
This is the core library for the following projects
 * [Eols.EPiGraphQL.Cms](https://github.com/lillheaton/Eols.EPiGraphQL.Cms)

#### Resolve dependencies
You'll need to resolve GraphQL.IDependencyResolver with EPiServers ServiceLocator

```cs
services.AddSingleton<GraphQL.IDependencyResolver>(x =>
    new FuncDependencyResolver(type =>  x.GetInstance(type))
);
```

#### Custom GraphTypes
This project contains a default implementation/resolvers for some of EPiServers properties (Url, ContentArea, ContentReference etc). These
resolvers can be overwriten by creating your own GraphType.

```cs
[ServiceConfiguration(typeof(ICustomGraphType), Lifecycle = ServiceInstanceScope.Singleton)]
public class TestOverrideGraphType : ScalarGraphType, ICustomGraphType
{
    public Type TargetType => typeof(Url);
}
```

#### Hide Content Types or Properties
Set GraphHideAttribute to either Class or specific properties

#### Adding your own Graphs to Schema
This projects creates and sets up the Schema with a "root" query. You can extend this graph with more fields. The following show an example.

```cs
public class Droid
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class DroidType : ObjectGraphType<Droid>
{
    public DroidType()
    {
        Name = "Droid";
        Field(x => x.Id).Description("The Id of the Droid.");
        Field(x => x.Name).Description("The name of the Droid.");
    }
}

public class StarWarsQuery : ObjectGraphType
{
    public StarWarsQuery()
    {
        Name = "StarWars";
        Field<DroidType>(
          "hero",
          resolve: context =>
          {
              return new Droid { Id = "1", Name = "R2-D2" };
          }
        );
    }
}

[InitializableModule]
public class GraphsInitialization : IConfigurableModule
{
    public void Initialize(InitializationEngine context)
    {
        var rootQuery = context.Locate.Advanced.GetInstance<IRootQuery>();

        rootQuery.AddField(new GraphQL.Types.FieldType
        {
            Name = "MyOwnRootQuery",
            Type = typeof(StarWarsQuery),
            Resolver = new FuncFieldResolver<StarWarsQuery, object>(fieldContext => new { })
        });
    }
}
```
