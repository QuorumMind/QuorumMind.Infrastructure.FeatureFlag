using System.Text.Json;
using Amazon;
using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Amazon.Runtime;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Aws.AppConfig;


public class AwsAppConfigFeatureFlagProvider : IFeatureFlagProvider
{
    private readonly IAmazonAppConfigData _client;
    private readonly string _appId, _envId, _profileId;
    private IEnumerable<Type> _knownTypes = Enumerable.Empty<Type>();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private string? _token;
    
    public AwsAppConfigFeatureFlagProvider( string appId,
        string envId,
        string profileId,
        string accessKey,
        string secretKey,
       RegionEndpoint? region = null)
    {
        var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
        var config = new AmazonAppConfigDataConfig();
        config.RegionEndpoint = region;

        _client = new AmazonAppConfigDataClient(awsCredentials, config);
        _appId = appId;
        _profileId = profileId;
        _envId = envId;
    }

    public void RegisterKnownTypes(IEnumerable<Type> knownTypes) => _knownTypes = knownTypes;

    public async Task<Dictionary<string, IFeatureFlagDefinition>> LoadAllAsync(string scope, CancellationToken cancellationToken = default)
    {
        if (!_knownTypes.Any())
            throw new InvalidOperationException("No known types registered.");

        var result = new Dictionary<string, IFeatureFlagDefinition>();

        if (_token == null)
        {
            var start = await _client.StartConfigurationSessionAsync(new()
            {
                ApplicationIdentifier = _appId,
                EnvironmentIdentifier = _envId,
                ConfigurationProfileIdentifier = _profileId
            }, cancellationToken);
            _token = start.InitialConfigurationToken;
        }

        
        var response = await _client.GetLatestConfigurationAsync(new GetLatestConfigurationRequest
        {
            ConfigurationToken = _token
        }, cancellationToken);

        using var reader = new StreamReader(response.Configuration);
        var jsonText = await reader.ReadToEndAsync(cancellationToken);
        var array = JsonDocument.Parse(jsonText).RootElement.EnumerateArray();

        foreach (var item in array)
        {
            var json = item.GetRawText();
            try
            {
                var typeName = item.GetProperty("type").GetString();
                var type = _knownTypes.FirstOrDefault(t => t.Name == typeName);
                if (type == null)
                    throw new InvalidOperationException($"Unknown type: {typeName}");

                var flag = JsonSerializer.Deserialize(json, type, _jsonOptions) as IFeatureFlagDefinition;
                if (flag != null && !string.IsNullOrWhiteSpace(flag.Name))
                    result[flag.Name] = flag;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AWS AppConfig] Failed to parse flag: {ex.Message}");
            }
        }

        return result;
    }
}