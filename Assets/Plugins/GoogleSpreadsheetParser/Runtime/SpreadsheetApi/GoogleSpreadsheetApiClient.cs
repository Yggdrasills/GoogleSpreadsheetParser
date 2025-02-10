using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class GoogleSpreadsheetApiClient
    {
        private readonly GoogleSpreadsheetSettings _settings;
        private readonly IProgressHandler _progressHandler;

        public GoogleSpreadsheetApiClient(GoogleSpreadsheetSettings settings, IProgressHandler progressHandler)
        {
            _settings = settings;
            _progressHandler = progressHandler;
        }

        internal void GetSheet(string spreadsheetId, string sheetName, Action<string> completed)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", "getSheet" },
                { "spreadsheetId", spreadsheetId },
                { "sheetName", sheetName }
            };

            SendGetRequest(parameters, completed);
        }

        internal void GetSheets(string spreadsheetId, List<string> sheetNames, Action<string> completed)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", "getSheets" },
                { "spreadsheetId", spreadsheetId },
                { "sheetNames", JsonConvert.SerializeObject(sheetNames) }
            };

            SendGetRequest(parameters, completed);
        }

        internal void GetSheetId(string spreadsheetId, string sheetName, Action<string> completed)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", "getSheetId" },
                { "spreadsheetId", spreadsheetId },
                { "sheetName", sheetName }
            };

            SendGetRequest(parameters, completed);
        }

        internal void Ping(Action<string> completed)
        {
            SendGetRequest(null, completed);
        }

        private void SendGetRequest(Dictionary<string, string> parameters, Action<string> completed)
        {
            UnityWebRequest request;
            var uri = BuildUri(parameters);
            try
            {
                _progressHandler.Show("Requesting", "Please wait...");

                request = UnityWebRequest.Get(uri);
                var operation = request.SendWebRequest();

                operation.completed += _ =>
                {
                    _progressHandler.Hide();
                    if (request.result != UnityWebRequest.Result.Success)
                        throw new SpreadsheetRequestException(request.error);

                    completed?.Invoke(request.downloadHandler.text);
                    request.Dispose();
                };
            }
            catch (Exception ex)
            {
                _progressHandler.Hide();
                throw new SpreadsheetRequestException($"Failed to complete request: {uri}", ex);
            }
        }

        private Uri BuildUri(Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return new Uri(_settings.WebServiceUrl);

            var queryString = new StringBuilder();

            foreach (var param in parameters)
            {
                queryString.Append(Uri.EscapeDataString(param.Key));
                queryString.Append("=");
                queryString.Append(Uri.EscapeDataString(param.Value));
                queryString.Append("&");
            }

            queryString.Length--; // note: Remove the trailing '&'

            var uriBuilder = new UriBuilder(_settings.WebServiceUrl)
            {
                Query = queryString.ToString()
            };

            return uriBuilder.Uri;
        }
    }
}