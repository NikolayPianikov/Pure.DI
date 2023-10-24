// ReSharper disable UnusedMember.Local

using Pure.DI;
using WebAPI.Controllers;
using static Pure.DI.Lifetime;

namespace WebAPI;

using Pure.DI.MS;
using WeatherForecast;

internal partial class Composition: ServiceProviderFactory<Composition>
{
    private static void Setup() =>
        DI.Setup(nameof(Composition))
            .DependsOn(Base)
            .Bind<IWeatherForecastService>()
                .As(Singleton)
                .To<WeatherForecastService>()
                .Root<WeatherForecastController>();
}