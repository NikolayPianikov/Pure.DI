// <auto-generated/>
// #pragma warning disable

namespace Pure.DI.MS;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal class ServiceCollectionFactory<TComposition>
{
    private readonly List<(Type ServiceType, IResolver<TComposition, object> Resolver)> _resolvers = new();
    
    /// <summary>
    /// Registers a resolver of the composition for use in a service collection.
    /// </summary>
    /// <param name="resolver">Composition resolver.</param>
    /// <typeparam name="TContract">The type of object that the resolver returns.</typeparam>
    [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
    public void AddResolver<TContract>(IResolver<TComposition, TContract> resolver) => 
        _resolvers.Add((typeof(TContract), (IResolver<TComposition, object>)resolver));

    /// <summary>
    /// Creates a service collection.
    /// </summary>
    /// <param name="composition">An instance of composition.</param>
    /// <returns><see cref="ServiceCollection"/></returns>
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP || NET40_OR_GREATER
    [global::System.Diagnostics.Contracts.Pure]
#endif
    [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
    public IServiceCollection CreateServiceCollection(TComposition composition) => 
        new ServiceCollection().Add(CreateDescriptors(composition));

    [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
    private IEnumerable<ServiceDescriptor> CreateDescriptors(TComposition composition) =>
        _resolvers.Select(resolver => CreateDescriptor(composition, resolver.ServiceType, resolver.Resolver));

    [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x300)]
    private static ServiceDescriptor CreateDescriptor(TComposition composition, Type serviceType, IResolver<TComposition, object> resolver) =>
        new(serviceType, _ => resolver.Resolve(composition), ServiceLifetime.Transient);
}

// #pragma warning restore