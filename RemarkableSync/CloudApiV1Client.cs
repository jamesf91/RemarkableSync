﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RemarkableSync
{
    class CloudApiV1Client : ICloudApiClient
    {
        private static string CustomBaseUrlName = "CustomBaseUrl";
        private static string DefaultBaseUrl = "https://document-storage-production-dot-remarkable-production.appspot.com";

        private HttpClient _client;
        private string _baseUrl;

        public CloudApiV1Client(HttpClient client, IConfigStore hiddenConfigStore = null)
        {
            _client = client;
            _baseUrl = hiddenConfigStore?.GetConfig(CustomBaseUrlName) ?? DefaultBaseUrl;
        }

        public void Dispose()
        { 
        }

        public async Task<RmDownloadedDoc> DownloadDocument(RmItem item, CancellationToken cancellationToken, IProgress<string> progress)
        {
            if (item.Type != RmItem.DocumentType)
            {
                Logger.LogMessage($"item with id {item.ID} is not document type");
                return null;

            }

            try
            {
                // first get the blob url
                string url = $"/document-storage/json/2/docs?doc={WebUtility.UrlEncode(item.ID)}&withBlob=true";
                HttpResponseMessage response = await Request(HttpMethod.Get, url, null, null);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogMessage("request failed with status code " + response.StatusCode.ToString());
                    return null;
                }
                List<RmItem> items = JsonSerializer.Deserialize<List<RmItem>>(response.Content.ReadAsStringAsync().Result);
                if (items.Count == 0)
                {
                    Logger.LogMessage("Failed to find document with id: " + item.ID);
                    return null;
                }
                string blobUrl = items[0].BlobURLGet;
                Stream stream = await _client.GetStreamAsync(blobUrl);
                ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);

                return new RmCloudV1DownloadedDoc(archive, item.ID);
            }
            catch (Exception err)
            {
                Logger.LogMessage($"failed for id {item.ID}. Error: {err.Message}");
                return null;
            }
        }

        public async Task<List<RmItem>> GetAllItems(CancellationToken cancellationToken, IProgress<string> progress)
        {
            HttpResponseMessage response = await Request(
                HttpMethod.Get,
                "/document-storage/json/2/docs",
                null,
                null);

            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string errMsg = "GetAllItems request failed with status code " + response.StatusCode.ToString();
                Logger.LogMessage($"Request failed with status code: {response.StatusCode.ToString()} and content: {responseContent}");
                throw new Exception(errMsg);
            }

            List<RmItem> collection = JsonSerializer.Deserialize<List<RmItem>>(responseContent);
            return collection;
        }

        private async Task<HttpResponseMessage> Request(HttpMethod method, string url, Dictionary<string, string> header, HttpContent content)
        {
            if (!url.StartsWith("http"))
            {
                if (!url.StartsWith("/"))
                    url = "/" + url;
                url = _baseUrl + url;
            }

            Logger.LogMessage($"url is: {url}");
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = method;
            if (content != null)
            {
                request.Content = content;
            }

            // add/replace the supplied headers
            if (header != null)
            {
                foreach (var key in header.Keys)
                {
                    request.Headers.Add(key, header[key]);
                }
            }

            HttpResponseMessage response = await _client.SendAsync(request);
            return response;
        }
    }
}
