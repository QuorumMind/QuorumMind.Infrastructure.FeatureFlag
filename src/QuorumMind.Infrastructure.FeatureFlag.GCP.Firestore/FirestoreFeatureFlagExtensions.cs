using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

namespace QuorumMind.Infrastructure.FeatureFlag.GCP.Firestore;

public static class FirestoreFeatureFlagExtensions
{
    public static IServiceCollection AddFirestoreFeatureFlags(this IServiceCollection services, FirestoreDb db, string collection = "feature_flags")
    {
        services.AddSingleton<IFeatureFlagProvider>(new FirestoreFeatureFlagProvider(db, collection));
        return services;
    }
}