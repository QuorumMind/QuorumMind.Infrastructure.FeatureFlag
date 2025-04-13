using Amazon;
using Amazon.AppConfigData;
using Microsoft.Extensions.DependencyInjection;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

namespace QuorumMind.Infrastructure.FeatureFlag.Aws.AppConfig;

public static class AwsAppConfigFeatureFlagExtensions
{
    public static IServiceCollection AddAwsAppConfigFeatureFlags(
        this IServiceCollection services,
        string appId,
        string envId,
        string profileId,
        string accessKey,
        string secretKey,
        RegionEndpoint? region = null)
    {
        services.AddSingleton<IFeatureFlagProvider>(new AwsAppConfigFeatureFlagProvider(appId, envId, profileId, accessKey, secretKey, region));
        return services;
    }
}