using System.Text.Json;
using Google.Cloud.Firestore;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.GCP.Firestore;


public class FirestoreFeatureFlagProvider : IFeatureFlagProvider
{
    private readonly FirestoreDb _db;
    private readonly string _collectionPrefix;
    private IEnumerable<Type> _knownTypes = Enumerable.Empty<Type>();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public FirestoreFeatureFlagProvider(FirestoreDb db, string collectionPrefix = "")
    {
        _db = db;
        _collectionPrefix = collectionPrefix.TrimEnd('/');
    }

    public void RegisterKnownTypes(IEnumerable<Type> knownTypes) => _knownTypes = knownTypes;

    public async Task<Dictionary<string, IFeatureFlagDefinition>> LoadAllAsync(string microserviceScope, CancellationToken cancellationToken = default)
    {
        if (!_knownTypes.Any())
            throw new InvalidOperationException("No known feature flag types registered. Use RegisterKnownTypes(...) before loading flags.");
        
        var result = new Dictionary<string, IFeatureFlagDefinition>();
        var snapshot = await _db.Collection(_collectionPrefix).Document(microserviceScope).GetSnapshotAsync(cancellationToken);

        var flags = snapshot.ToDictionary();
        foreach (var currentFlag in flags)
        {
            try
            {
                var json = JsonSerializer.Serialize(currentFlag.Value);
                var docRoot = JsonDocument.Parse(json).RootElement;
                if (!docRoot.TryGetProperty("type", out var typeProp))
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
                Console.WriteLine($"[Firestore] Failed to parse flag: {ex.Message}");
            }
        }

        return result;
    }
}
