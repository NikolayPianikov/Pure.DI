﻿/*
$v=true
$p=3
$d=Injection
$h=This example shows how to create and initialize an instance manually injecting required dependencies. 
*/

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable CheckNamespace
// ReSharper disable UnusedVariable
namespace Pure.DI.UsageTests.Basics.InjectScenario;

using Xunit;

// {
internal interface IDependency { }

internal class Dependency : IDependency
{
}

internal interface IService
{
    IDependency Dependency { get; }
}

internal class Service : IService
{
    public Service(IDependency dependency)
    {
        Dependency = dependency;
    }

    public IDependency Dependency { get; }
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
            .Bind<IService>().To(ctx =>
            {
                ctx.Inject<IDependency>(out var dependency);
                return new Service(dependency);
            })
            .Root<IService>("Root");

        var composition = new Composition();
        var service = composition.Root;
// }            
        TestTools.SaveClassDiagram(composition, nameof(InjectScenario));
    }
}