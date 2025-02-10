using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class SpreadsheetTree : VisualElement
    {
        private readonly SpreadsheetImporter _importer;
        private readonly GoogleSpreadsheetApiClient _apiClient;
        private readonly Dictionary<Sheet, Toggle> _sheetToggles = new();
        private readonly Dictionary<Spreadsheet, Toggle> _spreadsheetToggles = new();
        private string _contentFolderPath;

        internal SpreadsheetTree(GoogleSpreadsheetSettings settings, SpreadsheetImporter importer)
        {
            _importer = importer;

            _apiClient = new GoogleSpreadsheetApiClient(settings, new EditorProgressHandler());

            AddToClassList("box-container");
        }

        public void SetContentFolderPath(string path)
        {
            _contentFolderPath = path;
        }

        public void Build()
        {
            if (!_importer.IsValid)
                return;

            var downloadSelectedButton = new Button(DownloadSelectedSheets) { text = "Download Selected" };
            downloadSelectedButton.AddToClassList("control-button");
            Add(downloadSelectedButton);

            var spreadsheetCollection = _importer.GetSpreadsheetCollection();

            foreach (var spreadsheet in spreadsheetCollection.Spreadsheets)
            {
                var spreadsheetElement = new VisualElement();
                spreadsheetElement.AddToClassList("spreadsheet-element");

                var header = new VisualElement();
                header.AddToClassList("spreadsheet-header");

                var openButton = CreateOpenSpreadsheetButton(spreadsheet);
                var downloadButton = CreateDownloadSpreadsheetButton(spreadsheet);
                var spreadsheetToggle = new Toggle { value = false };
                spreadsheetToggle.AddToClassList("spreadsheet-toggle");
                _spreadsheetToggles[spreadsheet] = spreadsheetToggle;

                spreadsheetToggle.RegisterValueChangedCallback(evt =>
                {
                    foreach (var sheet in spreadsheet.SheetList)
                    {
                        if (_sheetToggles.TryGetValue(sheet, out var sheetToggle))
                        {
                            sheetToggle.value = evt.newValue;
                        }
                    }
                });

                header.Add(openButton);
                header.Add(spreadsheetToggle);
                header.Add(downloadButton);
                spreadsheetElement.Add(header);

                var sheetsContainer = new VisualElement();

                foreach (var sheet in spreadsheet.SheetList)
                {
                    var sheetElement = new VisualElement();
                    sheetElement.AddToClassList("sheet-element");

                    var sheetOpenButton = CreateOpenSheetButton(sheet);
                    var toggle = new Toggle { value = false };
                    toggle.AddToClassList("sheet-toggle");
                    _sheetToggles[sheet] = toggle;

                    toggle.RegisterValueChangedCallback(evt =>
                    {
                        if (!evt.newValue)
                        {
                            _spreadsheetToggles[spreadsheet].SetValueWithoutNotify(false);
                        }
                        else
                        {
                            UpdateSpreadsheetToggle(spreadsheet);
                        }
                    });

                    var sheetDownloadButton = CreateDownloadSheetButton(sheet);

                    sheetElement.Add(sheetOpenButton);
                    sheetElement.Add(toggle);
                    sheetElement.Add(sheetDownloadButton);

                    sheetsContainer.Add(sheetElement);
                }

                spreadsheetElement.Add(sheetsContainer);
                Add(spreadsheetElement);
            }
        }

        private void UpdateSpreadsheetToggle(Spreadsheet spreadsheet)
        {
            if (_spreadsheetToggles.TryGetValue(spreadsheet, out var spreadsheetToggle))
            {
                var allSheetToggles = spreadsheet.SheetList
                    .Select(sheet => _sheetToggles[sheet])
                    .ToList();

                spreadsheetToggle.value = allSheetToggles.All(toggle => toggle.value);
            }
        }

        private Button CreateDownloadSpreadsheetButton(Spreadsheet spreadsheet)
        {
            var sheetNames = spreadsheet.Sheets.Keys.ToList();
            var button = new Button(() => DownloadSheets(spreadsheet, sheetNames)) { text = spreadsheet.Name };
            button.AddToClassList("download-button");
            return button;
        }

        private Button CreateDownloadSheetButton(Sheet sheet)
        {
            var button = new Button(() => DownloadSheet(sheet)) { text = sheet.Name };
            button.AddToClassList("download-button");
            return button;
        }

        private Button CreateOpenSpreadsheetButton(Spreadsheet spreadsheet)
        {
            return CreateGoToButton(() => OpenSpreadsheet(spreadsheet));
        }

        private Button CreateOpenSheetButton(Sheet sheet)
        {
            return CreateGoToButton(() => OpenSheet(sheet));
        }

        private Button CreateGoToButton(Action onClick)
        {
            var button = new Button(onClick);
            button.AddToClassList("icon-button");

            var iconsPath = Path.Combine(_contentFolderPath, "Icons", "share.png");
            var icon = new Image();
            icon.image = AssetDatabase.LoadAssetAtPath<Texture2D>(iconsPath);

            button.Add(icon);
            return button;
        }

        private void DownloadSelectedSheets()
        {
            var selectedSheets = _sheetToggles
                .Where(kvp => kvp.Value.value)
                .Select(kvp => kvp.Key)
                .GroupBy(x => x.Spreadsheet)
                .ToDictionary(x => x.Key, x => x.ToList());

            if (!selectedSheets.Any())
            {
                EditorUtility.DisplayDialog("Warning", "No sheets selected", "OK");
                return;
            }

            foreach (var (spreadsheet, sheets) in selectedSheets)
            {
                DownloadSheets(spreadsheet, sheets.Select(sheet => sheet.Name).ToList());
            }
        }

        private void DownloadSheets(Spreadsheet spreadsheet, List<string> sheetNames)
        {
            _apiClient.GetSheets(spreadsheet.SpreadsheetId, sheetNames, response =>
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, List<List<object>>>>(response);

                foreach (var (key, data) in result)
                {
                    var sheet = spreadsheet.Sheets[key];

                    _importer.Import(sheet, data);

                    Debug.Log($"Spreadsheet {sheet.Name} imported");
                }

                AssetDatabase.Refresh();
            });
        }

        private void DownloadSheet(Sheet sheet)
        {
            _apiClient.GetSheet(sheet.Spreadsheet.SpreadsheetId, sheet.Name, response =>
            {
                var result = JsonConvert.DeserializeObject<List<List<object>>>(response);

                _importer.Import(sheet, result);

                Debug.Log($"Spreadsheet {sheet.Name} imported");

                AssetDatabase.Refresh();
            });
        }

        private void OpenSpreadsheet(Spreadsheet spreadsheet)
        {
            var url = $"https://docs.google.com/spreadsheets/d/{spreadsheet.SpreadsheetId}";
            Application.OpenURL(url);
        }

        private void OpenSheet(Sheet sheet)
        {
            _apiClient.GetSheetId(sheet.Spreadsheet.SpreadsheetId, sheet.Name, response =>
            {
                var sheetId = JsonConvert.DeserializeObject<string>(response);
                var url = $"https://docs.google.com/spreadsheets/d/{sheet.Spreadsheet.SpreadsheetId}/edit#gid={sheetId}";
                Application.OpenURL(url);
            });
        }
    }
}