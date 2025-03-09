using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class SpreadsheetImporter
    {
        private readonly GoogleSpreadsheetSettings _settings;
        private readonly ISheetSerializer _serializer;

        private readonly Dictionary<string, ISpreadsheetParser> _parsers;

        internal SpreadsheetImporter(GoogleSpreadsheetSettings settings, ISheetSerializer serializer)
        {
            _settings = settings;
            _serializer = serializer;
            _parsers = GetAssemblyParsers();
        }

        internal bool IsValid => !string.IsNullOrEmpty(_settings.IndexFilePath) && !string.IsNullOrEmpty(_settings.DownloadsFolderPath);

        internal SpreadsheetCollection GetSpreadsheetCollection()
        {
            var index = GetIndexFileContent();
            var spreadsheets = SpreadsheetCollection.FromJson(index);
            return spreadsheets;
        }

        internal void Import(Sheet sheet, List<List<object>> data)
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.DownloadsFolderPath))
                    throw new Exception("Downloads folder path is not set");

                if (!Directory.Exists(_settings.DownloadsFolderPath))
                    Directory.CreateDirectory(_settings.DownloadsFolderPath);

                var parser = _parsers.GetValueOrDefault(sheet.Type);

                if (parser == null)
                    throw new Exception($"Not found parser type {sheet.Type} for sheet {sheet.Name}");

                var sheetData = parser.Parse(data);
                var serialized = _serializer.Serialize(sheetData);

                var filePath = Path.Combine(_settings.DownloadsFolderPath, $"{sheet.Name}.json");

                File.WriteAllText(filePath, serialized);
            }
            catch (Exception)
            {
                Debug.LogError("Failed to import sheet " + sheet.Name);
                throw;
            }
        }

        private Dictionary<string, ISpreadsheetParser> GetAssemblyParsers()
        {
            var parsers = new Dictionary<string, ISpreadsheetParser>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var assemblyParsers = assembly
                    .GetTypes().Where(x =>
                        typeof(ISpreadsheetParser).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract
                        && x.GetCustomAttribute<SpreadsheetParserAttribute>() != null);

                foreach (var parser in assemblyParsers)
                {
                    parsers[parser.GetCustomAttribute<SpreadsheetParserAttribute>().Id] =
                        (ISpreadsheetParser)Activator.CreateInstance(parser);
                }
            }

            return parsers;
        }

        private string GetIndexFileContent()
        {
            if (!File.Exists(_settings.IndexFilePath))
                throw new FileNotFoundException("Index file not found");

            return File.ReadAllText(_settings.IndexFilePath);
        }
    }
}