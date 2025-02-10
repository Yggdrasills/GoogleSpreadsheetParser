using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    public class GoogleSpreadsheetsParserEditorWindow : EditorWindow
    {
        private ScrollView _mainContainer;
        private GoogleSpreadsheetSettings _settings;
        private GoogleSpreadsheetApiClient _apiClient;
        private SpreadsheetImporter _importer;

        [MenuItem("Tools/Google Spreadsheets Parser")]
        public static void ShowWindow()
        {
            var window = CreateInstance<GoogleSpreadsheetsParserEditorWindow>();
            window.Show();
        }

        private void CreateGUI()
        {
            var stylePath = Path.Combine(GetContentFolderPath(), "SpreadsheetStyles.uss");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);

            rootVisualElement.styleSheets.Add(styleSheet);

            titleContent = new GUIContent("Google spreadsheet parser");

            _settings = GetOrCreateSettings();

            _importer = new SpreadsheetImporter(_settings, GetSheetSerializer());

            _mainContainer = new ScrollView();

            _apiClient = new GoogleSpreadsheetApiClient(_settings, new EditorProgressHandler());

            var header = new VisualElement();
            header.AddToClassList("box-container");

            CreateVerifyButton(header);
            CreateRefreshButton(header);

            BuildSpreadsheetListTree();

            rootVisualElement.Add(header);
            rootVisualElement.Add(_mainContainer);
        }

        private GoogleSpreadsheetSettings GetOrCreateSettings()
        {
            var settings = Resources.Load<GoogleSpreadsheetSettings>("GoogleSpreadsheetSettings");

            if (settings == null)
            {
                settings = CreateInstance<GoogleSpreadsheetSettings>();

                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                AssetDatabase.CreateAsset(settings, "Assets/Resources/GoogleSpreadsheetSettings.asset");
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private void CreateVerifyButton(VisualElement root)
        {
            var button = new Button();
            button.AddToClassList("verify-button");
            button.text = "Ping";
            var icon = new Image();
            button.Add(icon);
            button.clicked += () =>
            {
                _apiClient.Ping(response =>
                {
                    var result = JsonConvert.DeserializeObject<string>(response);
                    var success = result == "pong";
                    var sprite = success ? "check.png" : "exclamation.png";
                    var iconsPath = Path.Combine(GetContentFolderPath(), "Icons", sprite);

                    icon.image = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath);
                });
            };

            root.Add(button);
        }

        private void CreateRefreshButton(VisualElement root)
        {
            var button = new Button();
            button.AddToClassList("control-button");
            button.text = "Refresh";
            button.clicked += BuildSpreadsheetListTree;

            root.Add(button);
        }

        private void BuildSpreadsheetListTree()
        {
            _mainContainer.Clear();

            var tree = new SpreadsheetTree(_settings, _importer);
            tree.SetContentFolderPath(GetContentFolderPath());
            tree.Build();

            _mainContainer.Add(tree);
        }

        private string GetContentFolderPath()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var packageInfo = PackageInfo.FindForAssembly(assembly);

            if (packageInfo != null)
                return Path.Combine(packageInfo.assetPath, "Content");

            return Path.Combine("Assets", "Plugins", "GoogleSpreadsheetParser", "Content");
        }

        protected virtual ISheetSerializer GetSheetSerializer()
        {
            return new JsonSheetSerializer();
        }
    }
}