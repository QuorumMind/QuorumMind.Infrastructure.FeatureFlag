using Microsoft.AspNetCore.Mvc;
using QuorumMind.Infrastructure.FeatureFlag.Core.Interfaces;

namespace QuorumMind.Infrastructure.FeatureFlag.ExampleApp.Controllers;

public class FeatureFlagTestController : Controller
{
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureFlagTestController(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }
    
    [HttpGet("test-all-flags")]
    public async Task<IActionResult> TestAllFlags()
    {
        var context = new Dictionary<string, string>
        {
            ["region"] = "US",
            ["age"] = "21",
            ["role"] = "admin",
            ["temp"] = "10"
        };

        var userId = "user-1";
        var results = new Dictionary<string, bool>();

        foreach (var flag in new[]
                 {
                     "EnableFeature_RegionUS",
                     "EnableFeature_AdultsOnly",
                     "EnableFeature_Admins",
                     "EnableFeature_WhitelistUser",
                     "EnableFeature_ColdOnly"
                 })
        {
            results[flag] = await _featureFlagService.IsFeatureEnabledAsync(flag, userId, context);
        }

        return Ok(results);
    }

}