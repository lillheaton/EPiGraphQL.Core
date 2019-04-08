[![Build status](https://ci.appveyor.com/api/projects/status/xmugcagkt9k1k5u9?svg=true)](https://ci.appveyor.com/project/lillheaton/eols-epigraphql-core)

Adding [Facebook's GraphQL](https://github.com/facebook/graphql) for [EPiServer's](https://www.episerver.com/) platform. This tool automates creation of Graphs based on the defined available content types.

This project builds on top of [GraphQL .NET](https://github.com/graphql-dotnet/graphql-dotnet) library written by [Joe McBride](https://github.com/joemcbride) (MIT licence)

## Installation
You can install the latest version via [NuGet](https://www.nuget.org/packages/EPiGraphQL.Core/).

`PM> Install-Package EPiGraphQL.Core`

## Basic Usage
This is the core library for the following projects
 * [EPiGraphQL.Cms](https://github.com/lillheaton/EPiGraphQL.Cms)

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

#### Hide or Change name on Content Types or Properties
You can use GraphTypeAttribute on class to change graph name or hide it and you can use GraphPropertyAttribute to change name on property or hide

```cs
[GraphType(GraphName = "MyStartPage", Hide = false))]
[ContentType(DisplayName = "Start page")]
public class StartPage : PageData
{
    [Display(Name = "Meta Title", Order = 5)]
    public virtual virtual string MetaTitle { get; set; }

    [Display(Name = "Meta Description", Order = 7)]
    public virtual string MetaDescription { get; set; }

    [GraphProperty(PropertyName = "MySettings", Hide = false)]
    [Display(Name = "Setting", Order = 10)]
    public virtual Setting Settings { get; set; }
}
```

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
