using AIGuiders.Platform.Navigation.Policy;

namespace RoslynMcp.Tests;

public sealed class WorkspaceNavigationPresetMergeTests
{
    [Fact]
    public void Merge_peers_only_yields_include_partial_and_project_peer()
    {
        var (inc, exc, err) = NavigationPresetMerge.Merge(
            "peers_only",
            requestInclude: null,
            requestExclude: null);
        Assert.Null(err);
        Assert.NotNull(inc);
        Assert.Contains(NavigationRelatedKinds.PartialPeer, inc);
        Assert.Contains(NavigationRelatedKinds.ProjectPeer, inc);
        Assert.Equal(2, inc!.Count);
        Assert.Empty(exc!);
    }

    [Fact]
    public void Merge_unknown_preset_returns_error()
    {
        var (_, _, err) = NavigationPresetMerge.Merge(
            "no_such_preset",
            null,
            null);
        Assert.NotNull(err);
        Assert.Contains("Неизвестный пресет", err, StringComparison.Ordinal);
    }
}
