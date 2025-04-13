using System.Text;
using System.Text.Json;
using Consul;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Consul;

public class ConsulFeatureFlagProvider : IFeatureFlagProvider
{
    private readonly ConsulClient _client;
    private readonly string _basePath;
    private IEnumerable<Type> _knownTypes = Enumerable.Empty<Type>();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public ConsulFeatureFlagProvider(string consulUrl, string basePath = "feature-flags")
    {
        _client = new ConsulClient(cfg => cfg.Address = new Uri(consulUrl));
        _basePath = basePath.TrimEnd('/');
    }

    public void RegisterKnownTypes(IEnumerable<Type> knownTypes) => _knownTypes = knownTypes;

    public async Task<Dictionary<string, IFeatureFlagDefinition>> LoadAllAsync(string microserviceScope, CancellationToken cancellationToken = default)
    {
        if (!_knownTypes.Any())
            throw new InvalidOperationException("No known feature flag types registered. Use RegisterKnownTypes(...) before loading flags.");
        
        var result = new Dictionary<string, IFeatureFlagDefinition>();
        var prefix = $"{_basePath}/{microserviceScope}/";
        var query = await _client.KV.List(prefix, cancellationToken);

        if (query.Response != null)
        {
            foreach (var item in query.Response)
            {
                if (item.Value == null) continue;
                var json = Encoding.UTF8.GetString(item.Value);

                try
                {
                    var doc = JsonDocument.Parse(json);
                    if (!doc.RootElement.TryGetProperty("type", out var typeProp))
                        throw new InvalidOperationException("Missing 'type' field");

                    var typeName = typeProp.GetString();
                    var type = _knownTypes.FirstOrDefault(t => t.Name == typeName);
                    if (type == null)
                        throw new InvalidOperationException($"Unknown type: {typeName}");

                    var flag = JsonSerializer.Deserialize(json, type, _jsonOptions) as IFeatureFlagDefinition;
                    if (flag != null && !string.IsNullOrWhiteSpace(flag.Name))
                        result[flag.Name] = flag;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Consul] Failed to parse flag: {ex.Message}");
                }
            }
        }

        return result;
    }
}
