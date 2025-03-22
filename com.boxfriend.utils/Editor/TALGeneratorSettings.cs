using System.IO;
using UnityEngine;

public class TALGeneratorSettings : ScriptableObject
{
    public string FilePath;
    public string FileName;
    public int Seconds = 5;

    private const string SettingsPath = @"ProjectSettings\TALGeneratorSettings.asset";
    private static TALGeneratorSettings _instance;
    
    public static TALGeneratorSettings GetOrCreate()
    {
        if (_instance == null)
            _instance = CreateInstance<TALGeneratorSettings>();
        
        var path = GetPath();
        if (!File.Exists(path)) return _instance;
        
        var json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, _instance);

        return _instance;
    }

    private static string GetPath()
    {
        var projectPath = Directory.GetParent(Application.dataPath).FullName;
        return Path.Combine(projectPath, SettingsPath);
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(this, true);
        var path = GetPath();
        File.WriteAllText(path, json);
    }
}