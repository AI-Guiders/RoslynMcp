using McpToolManifest;
using RoslynMcp;

var tools = ToolCatalog.Build().Select(t => (t.Name!, (string?)t.Description)).ToList();
return McpToolManifestExporter.Run(
    args,
    tools,
    new McpToolManifestExportOptions
    {
        McpId = "roslyn-mcp",
        Title = "Roslyn MCP",
        RepoFolderName = "roslyn-mcp",
    });
