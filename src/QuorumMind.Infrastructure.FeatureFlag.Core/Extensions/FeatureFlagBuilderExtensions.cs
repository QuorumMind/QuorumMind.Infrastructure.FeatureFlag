using Microsoft.Extensions.DependencyInjection;
using QuorumMind.Infrastructure.FeatureFlag.Core.Common;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Services;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Extensions;

public static class FeatureFlagBuilderExtensions
{
    public static IServiceCollection AddFeatureFlagsCore(this IServiceCollection services, string scope, TimeSpan? interval = null)
    {
    
        services.AddSingleton<IFeatureFlagService, FeatureFlagService>();
        services.AddHostedService<FeatureFlagRefreshService>(sp =>
            new FeatureFlagRefreshService(
                sp.GetRequiredService<IFeatureFlagProvider>(),
                sp.GetRequiredService<FeatureFlagCache>(),
                scope: scope,
                interval: interval)
        );
        return services;
    }
}