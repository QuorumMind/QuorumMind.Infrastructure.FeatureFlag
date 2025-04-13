using Consul;
using Microsoft.Extensions.DependencyInjection;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

namespace QuorumMind.Infrastructure.FeatureFlag.Consul;

public static class ConsulFeatureFlagExtensions
{
    public static IServiceCollection AddConsulFeatureFlags(this IServiceCollection services, string consulUrl, string basePath = "feature-flags/")
    {
        services.AddSingleton<IFeatureFlagProvider>(new ConsulFeatureFlagProvider(consulUrl, basePath));
        return services;
    }
}