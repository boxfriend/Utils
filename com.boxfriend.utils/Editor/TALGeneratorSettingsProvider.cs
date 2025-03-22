using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TALGeneratorSettingsProvider : SettingsProvider
{
    private TALGeneratorSettings _settings;
    
    public TALGeneratorSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }
    
    [SettingsProvider]
    public static SettingsProvider CreateCustomSettingsProvider()
    {
        return new TALGeneratorSettingsProvider("Project/Tags and Layers Generator", SettingsScope.Project);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        _settings = TALGeneratorSettings.GetOrCreate();
        
        var container = new VisualElement
        {
            style =
            {
                paddingTop = 10,
                paddingLeft = 10,
                paddingRight = 10
            }
        };

        var titleLabel = new Label("Tags And Layers Generator Settings")
        {
            style =
            {
                fontSize = 14,
                unityFontStyleAndWeight = FontStyle.Bold
            }
        };
        container.Add(titleLabel);

        var folderPickerRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                marginBottom = 5
            }
        };

        var folderPathField = new TextField("Selected Path")
        {
            value = _settings.FilePath,
            tooltip = "Select a folder that the generated class will be placed into",
            isReadOnly = true,
            style =
            {
                flexGrow = 1
            }
        };
        folderPickerRow.Add(folderPathField);
        
        var button = new Button(() =>
        {
            var path = EditorUtility.OpenFolderPanel("Select a Folder", Application.dataPath, "");
            if (string.IsNullOrWhiteSpace(path) || !IsInAssetFolder(path)) return;
            
            _settings.FilePath = path;
            _settings.Save();
            folderPathField.value = path;
        })
        {
            text = "Browse",
            style =
            {
                marginLeft = 5
            }
        };
        folderPickerRow.Add(button);

        container.Add(folderPickerRow);
        
        var fileNameRow = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                marginBottom = 5
            }
        };

        var fileNameField = new TextField("File Name")
        {
            value = _settings.FileName,
            tooltip = "Select a name for the generated file",
            style = { flexGrow = 1 }
        };
        fileNameField.RegisterValueChangedCallback(evt =>
        {
            _settings.FileName = evt.newValue;
            _settings.Save();
        });
        fileNameRow.Add(fileNameField);

        var csLabel = new Label(".cs")
        {
            style = { marginRight = 10 }
        };
        fileNameRow.Add(csLabel);
        container.Add(fileNameRow);

        var buttonRow = new VisualElement()
        {
            style =
            {
                flexGrow = 1,
                flexDirection = FlexDirection.RowReverse,
                justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween),
                marginBottom = 5
            }
        };
        var generateButton = new Button(TagAndLayerGenerator.Generate)
        {
            text = "Manually Generate",
            style =
            {
                flexGrow = 0,
                marginRight = 10
            }
        };
        buttonRow.Add(generateButton);
        var secondsField = new IntegerField("AutoGenerate Threshold")
        {
            value = _settings.Seconds,
            tooltip = "Minimum number of seconds to wait before allowing auto generation again. Minimum of 5 seconds",
            style =
            {
                minWidth = 30
            }
        };
        secondsField.RegisterValueChangedCallback(evt =>
        {
            _settings.Seconds = Mathf.Max(evt.newValue, 5);
            _settings.Save();
        });
        buttonRow.Add(secondsField);
        container.Add(buttonRow);
        rootElement.Add(container);
    }

    private bool IsInAssetFolder(string path)
    {
        var fullName = Path.GetFullPath(path);
        return fullName.StartsWith(Application.dataPath);
    }
}