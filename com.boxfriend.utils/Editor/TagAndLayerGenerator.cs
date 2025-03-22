using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TagAndLayerGenerator
{
    private static readonly FileSystemWatcher _fileSystemWatcher;

    static TagAndLayerGenerator()
    {
        if (_fileSystemWatcher != null) return;
        
        var projectPath = Directory.GetParent(Application.dataPath).FullName;
        var path = Path.Combine(projectPath, "ProjectSettings");
        _fileSystemWatcher = new FileSystemWatcher(path, "TagManager.asset");
        _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;

        Generate();
    }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    private static void OnChanged(object sender, FileSystemEventArgs e) => OnChanged();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    private static async Awaitable OnChanged()
    {
        await Awaitable.MainThreadAsync();
        Generate();
    }

    public static void Generate()
    {
        var settings = TALGeneratorSettings.GetOrCreate();
        if (string.IsNullOrWhiteSpace(settings.FileName) || string.IsNullOrWhiteSpace(settings.FilePath))
            return;

        var relativeDir = Path.GetRelativePath(Application.dataPath, settings.FilePath);
        var path = Path.Combine("Assets", relativeDir, settings.FileName + ".cs");
        if (File.Exists(path))
        {
            var lastEdit = File.GetLastWriteTime(path);
            var difference = DateTime.Now.Subtract(lastEdit).TotalSeconds;
            if (difference < Mathf.Max(settings.Seconds, 5))
                return;
        }
        AssetDatabase.StartAssetEditing();
        var builder = new StringBuilder();
        builder.AppendLine("namespace Boxfriend.Generated \n{");
        builder.AppendLine(GenerateTags());
        builder.AppendLine(GenerateLayers());
        builder.AppendLine("}");
        File.WriteAllText(path, builder.ToString());
        AssetDatabase.StopAssetEditing();
        AssetDatabase.ImportAsset(path);
    }

    private static string GenerateTags()
    {
        var builder = new StringBuilder();
        builder.AppendLine("public static class Tags \n{");
        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        foreach (var tag in tags)
        {
            builder.AppendLine($"public const string {tag} = \"{tag}\";");
            builder.AppendLine($"public static readonly UnityEngine.TagHandle {tag}Handle = UnityEngine.TagHandle.GetExistingTag({tag});");
        }
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string GenerateLayers()
    {
        var layerBuilder = new StringBuilder();
        layerBuilder.AppendLine("public static class Layers \n{");

        var maskBuilder = new StringBuilder();
        maskBuilder.AppendLine("public static class Mask \n{");
        maskBuilder.AppendLine("public const int All = int.MaxValue;\npublic const int None = 0;");
        for (var i = 0; i < 32; i++)
        {
            var name = LayerMask.LayerToName(i);
            if (string.IsNullOrWhiteSpace(name))
                continue;
            name = name.Replace(" ", "");
            layerBuilder.AppendLine($"public const string {name} = \"{name}\";");
            maskBuilder.AppendLine($"public const int {name} = 1 << {i};");
        }

        maskBuilder.AppendLine("}");
        layerBuilder.Append(maskBuilder);
        layerBuilder.AppendLine("}");
        return layerBuilder.ToString();
    }
}