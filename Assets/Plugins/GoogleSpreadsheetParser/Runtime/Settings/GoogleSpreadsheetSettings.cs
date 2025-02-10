using System.IO;
using UnityEngine;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class GoogleSpreadsheetSettings : ScriptableObject
    {
        [SerializeField] private string _webServiceUrl;
        [SerializeField] private string _indexFileFolderPath;
        [SerializeField] private string _downloadsFolderPath;

        public string WebServiceUrl => _webServiceUrl;
        public string IndexFilePath => GetIndexFilePath();
        public string DownloadsFolderPath => GetAssetFilePath(_downloadsFolderPath);

        private string GetIndexFilePath()
        {
            var folder = GetAssetFilePath(_indexFileFolderPath);

            if (string.IsNullOrEmpty(folder))
                return string.Empty;

            return Path.Combine(folder, "index.json");
        }

        private string GetAssetFilePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;

            return Path.Combine(Application.dataPath, relativePath);
        }
    }
}