using RoslynMcp.ServiceLayer;

namespace RoslynMcp.Tests;

public sealed class DependentUponCsprojTests
{
    [Fact]
    public void TryRemoveRedundantSdkCompileInclude_removes_matching_include()
    {
        var dir = Path.Combine(Path.GetTempPath(), "roslyn-mcp-sdk-compile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "Test.csproj");
        File.WriteAllText(path, """
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <Compile Include="ViewModels\Foo.cs" />
              </ItemGroup>
            </Project>
            """);

        var msg = DependentUponCsproj.TryRemoveRedundantSdkCompileInclude(path, "ViewModels/Foo.cs");

        Assert.Contains("removed", msg, StringComparison.Ordinal);
        var xml = File.ReadAllText(path);
        Assert.DoesNotContain("Include=\"ViewModels", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void TryRemoveRedundantSdkCompileInclude_skips_when_EnableDefaultCompileItems_false()
    {
        var dir = Path.Combine(Path.GetTempPath(), "roslyn-mcp-sdk-compile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "Test.csproj");
        File.WriteAllText(path, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
              </PropertyGroup>
              <ItemGroup>
                <Compile Include="ViewModels\Foo.cs" />
              </ItemGroup>
            </Project>
            """);

        var msg = DependentUponCsproj.TryRemoveRedundantSdkCompileInclude(path, "ViewModels\\Foo.cs");

        Assert.Contains("EnableDefaultCompileItems=false", msg, StringComparison.Ordinal);
        Assert.Contains("Compile Include", File.ReadAllText(path), StringComparison.Ordinal);
    }
}
