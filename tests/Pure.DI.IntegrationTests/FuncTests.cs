﻿namespace Pure.DI.IntegrationTests;

[Collection(nameof(NonParallelTestsCollectionDefinition))]
public class FuncTests
{
    [Fact]
    public async Task ShouldSupportFuncForTransientDependencies()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private Func<IDependency> _depFactory;
        public Service(Func<IDependency> depFactory)
        { 
            _depFactory = depFactory;           
        }

        public IDependency Dep => _depFactory();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep != service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncForSingletonDependencies()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private Func<IDependency> _depFactory;
        public Service(Func<IDependency> depFactory)
        { 
            _depFactory = depFactory;           
        }

        public IDependency Dep => _depFactory();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().As(Lifetime.Singleton).To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep == service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncForPerResolveDependencies()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private Func<IDependency> _depFactory;
        public Service(Func<IDependency> depFactory)
        { 
            _depFactory = depFactory;           
        }

        public IDependency Dep => _depFactory();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().As(Lifetime.PerResolve).To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep == service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncWithTag()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private Func<IDependency> _depFactory;
        public Service([Tag("Abc")] Func<IDependency> depFactory)
        { 
            _depFactory = depFactory;           
        }

        public IDependency Dep => _depFactory();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>("Abc").To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep != service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncOfFuncOfFunc()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private Func<IDependency> _depFactory;
        public Service(Func<Func<Func<IDependency>>> depFactory)
        { 
            _depFactory = depFactory()();           
        }

        public IDependency Dep => _depFactory();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep != service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncOfIEnumerable()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;
using System.Linq;
using System.Collections.Generic;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private System.Collections.Generic.IEnumerable<IDependency> _depFactory;
        public Service(Func<Func<System.Collections.Generic.IEnumerable<IDependency>>> depFactory)
        { 
            _depFactory = depFactory()();           
        }

        public IDependency Dep => _depFactory.First();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep != service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncOfArray()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;
using System.Linq;
using System.Collections.Generic;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private IDependency[] _depFactory;
        public Service(Func<Func<IDependency[]>> depFactory)
        { 
            _depFactory = depFactory()();           
        }

        public IDependency Dep => _depFactory[0];
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();                                                       
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
    }
    
    [Fact]
    public async Task ShouldSupportFuncOfTuple()
    {
        // Given

        // When
        var result = await """
using System;
using Pure.DI;
using System.Linq;
using System.Collections.Generic;

namespace Sample
{
    interface IDependency {}

    class Dependency: IDependency {}

    interface IService
    {
        IDependency Dep { get; }        
    }

    class Service: IService 
    {
        private System.Collections.Generic.IEnumerable<IDependency> _depFactory;
        public Service(Func<(System.Collections.Generic.IEnumerable<IDependency>, System.Collections.Generic.IEnumerable<IDependency>)> depFactory)
        { 
            _depFactory = depFactory().Item1;           
        }

        public IDependency Dep => _depFactory.First();
    }

    static class Setup
    {
        private static void SetupComposition()
        {
            DI.Setup("Composition")
                .Bind<IDependency>().To<Dependency>()
                .Bind<IService>().To<Service>()    
                .Root<IService>("Service");
        }
    }

    public class Program
    {
        public static void Main()
        {
            var composition = new Composition();
            var service = composition.Service;
            Console.WriteLine(service.Dep != service.Dep);                               
        }
    }                
}
""".RunAsync();

        // Then
        result.Success.ShouldBeTrue(result.GeneratedCode);
        result.StdOut.ShouldBe(ImmutableArray.Create("True"), result.GeneratedCode);
    }
}