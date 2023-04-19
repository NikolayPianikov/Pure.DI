﻿/*
$v=true
$p=0
$d=Decorator
$h=_Decorator_ is a well known and useful design pattern. To build a chain of nested decorators, it is convenient to use tagged dependencies, as in the example below:
$f=Here the instance of type _Service_ is marked as _"base"_ is injected into the decorator _DecoratorService_.
*/

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Local
namespace Pure.DI.UsageTests.Interception.DecoratorScenario;

using Shouldly;
using Xunit;

// {
internal interface IDependency { }

internal class Dependency : IDependency { }

internal interface IService
{
    string GetMessage();
}

internal class Service : IService
{
    public Service(IDependency dependency)
    {
    }

    public string GetMessage() => "Hello World";
}

internal class DecoratorService : IService
{
    private readonly IService _baseService;

    public DecoratorService([Tag("base")] IService baseService) => _baseService = baseService;

    public string GetMessage() => $"{_baseService.GetMessage()} !!!";
}

// }

public class Scenario
{
    [Fact]
    public void Run()
    {
        // ToString = On
// {            
        DI.Setup("Composition")
            .Bind<IDependency>().To<Dependency>()
            .Bind<IService>("base").To<Service>()
            .Bind<IService>().To<DecoratorService>().Root<IService>("Root");

        var composition = new Composition();
        var service = composition.Root;
        service.GetMessage().ShouldBe("Hello World !!!");
// }
        TestTools.SaveClassDiagram(composition, nameof(DecoratorScenario));
    }
}