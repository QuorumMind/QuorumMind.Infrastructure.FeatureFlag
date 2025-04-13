using Microsoft.Extensions.Hosting;
using QuorumMind.Infrastructure.FeatureFlag.Core.Common;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;
using QuorumMind.Infrastructure.FeatureFlag.Core.Models;

namespace QuorumMind.Infrastructure.FeatureFlag.Core.Services;

public class FeatureFlagRefreshService : IHostedService
{
    private readonly FeatureFlagCache _cache;
    private readonly IFeatureFlagProvider _provider;
    private readonly string _scope;
    private Timer? _timer;
    private readonly TimeSpan _interval;

    public FeatureFlagRefreshService(IFeatureFlagProvider provider, FeatureFlagCache cache, string scope, TimeSpan? interval = null)
    {
        _provider = provider;
        _cache = cache;
        _scope = scope;
        _interval = interval ?? TimeSpan.FromMinutes(1);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Refresh(cancellationToken);
        _timer = new Timer(async _ => await Refresh(CancellationToken.None), null, _interval, _interval);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    private async Task Refresh(CancellationToken cancellationToken)
    {
        try
        {
            var flags = await _provider.LoadAllAsync(_scope, cancellationToken);
            _cache.Set(flags);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FeatureFlags] Failed to refresh flags: {ex.Message}");
        }
    }
}