using RoslynMcp.ServiceLayer;

namespace RoslynMcp.Tests;

/// <summary>Изолированные тесты <see cref="MoveMembersToPartialFile"/>: временный каталог и минимальный SDK-проект, без ссылок на репозитории приложений.</summary>
public sealed class MoveMembersToPartialFileTests
{
    [Fact]
    public async Task MoveAsync_single_const_field_matches_by_name_preview()
    {
        var dir = Path.Combine(Path.GetTempPath(), "roslyn-mcp-move-const-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var csproj = Path.Combine(dir, "MoveConst.csproj");
        var sourceCs = Path.Combine(dir, "ConstHolder.cs");
        var outCs = Path.Combine(dir, "ConstHolder.Alpha.cs");

        File.WriteAllText(csproj, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <RootNamespace>MoveConst</RootNamespace>
              </PropertyGroup>
            </Project>
            """);

        File.WriteAllText(sourceCs, """
            namespace MoveConst;

            public static partial class ConstHolder
            {
                public const string Alpha = "a";
                public const string Beta = "b";
            }
            """);

        // Каретка на имени типа (строка с объявлением класса).
        var result = await MoveMembersToPartialFile.MoveAsync(
            csproj,
            sourceCs,
            line: 3,
            column: 29,
            memberNames: ["Alpha"],
            outputFilePath: outCs,
            apply: false,
            addDependentUpon: false);

        Assert.DoesNotContain("no members matched", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("# Members moved: 1", result, StringComparison.Ordinal);
        Assert.Contains("Alpha", result, StringComparison.Ordinal);
        Assert.Contains("## New file content", result, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MoveAsync_multi_variable_const_subset_returns_error()
    {
        var dir = Path.Combine(Path.GetTempPath(), "roslyn-mcp-move-const-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var csproj = Path.Combine(dir, "MoveConst.csproj");
        var sourceCs = Path.Combine(dir, "Multi.cs");
        var outCs = Path.Combine(dir, "Multi.Out.cs");

        File.WriteAllText(csproj, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
                <RootNamespace>MoveConst</RootNamespace>
              </PropertyGroup>
            </Project>
            """);

        File.WriteAllText(sourceCs, """
            namespace MoveConst;

            public static partial class Multi
            {
                public const string X = "x", Y = "y";
            }
            """);

        var result = await MoveMembersToPartialFile.MoveAsync(
            csproj,
            sourceCs,
            line: 3,
            column: 29,
            memberNames: ["X"],
            outputFilePath: outCs,
            apply: false,
            addDependentUpon: false);

        Assert.Contains("cannot move a subset", result, StringComparison.OrdinalIgnoreCase);
    }
}
