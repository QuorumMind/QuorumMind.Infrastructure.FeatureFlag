using Amazon;
using Consul;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using QuorumMind.Infrastructure.FeatureFlag.Aws.AppConfig;
using QuorumMind.Infrastructure.FeatureFlag.Consul;
using QuorumMind.Infrastructure.FeatureFlag.Core.Common;
using QuorumMind.Infrastructure.FeatureFlag.Core.Extensions;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Rules;
using QuorumMind.Infrastructure.FeatureFlag.ExampleApp.App.Util;
using QuorumMind.Infrastructure.FeatureFlag.GCP.Firestore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FeatureFlagCache>();
builder.Services.AddSingleton<IFeatureFlagAuditLogger, ConsoleAuditLogger>();



SetupConsulFeatureFlagProvider();
//SetupAwsAppConfigFeatureFlagProvider();
//SetupGcpFirestoreFeatureFlagProvider();

void SetupConsulFeatureFlagProvider()
{
    builder.Services.AddConsulFeatureFlags(
        "http://localhost:8500",
        basePath: "FeatureFlags");
}
void SetupAwsAppConfigFeatureFlagProvider()
{
   
    string accessKey = "<PLACEHOLDER>";
    string secretKey = "<PLACEHOLDER>";
    builder.Services.AddAwsAppConfigFeatureFlags("<PLACEHOLDER>", "<PLACEHOLDER>", "<PLACEHOLDER>", accessKey, secretKey, RegionEndpoint.USEast2);
}
void SetupGcpFirestoreFeatureFlagProvider()
{
    var credential = GoogleCredential.FromFile("<PLACEHOLDER>");
    var fireStoreBuilder = new FirestoreClientBuilder
    {
        Credential = credential,
    };
    var firestoreDB = FirestoreDb.Create("<PLACEHOLDER>", fireStoreBuilder.Build());
    builder.Services.AddFirestoreFeatureFlags(firestoreDB, "feature-flags");
}

builder.Services.AddFeatureFlagsCore("<PLACEHOLDER>");

    // Register known types
builder.Services.BuildServiceProvider()
    .GetRequiredService<IFeatureFlagProvider>()
    .RegisterKnownTypes(new[]
    {
        typeof(RegionBasedFlag),
        typeof(AgeBasedFlag),
        typeof(RoleBasedFlag),
        typeof(UserTargetedFlag),
        typeof(WeatherFlag)
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
