namespace GitHubRegistryNetworking.Scripts.Networking.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Networking;


    public class Requests
    {
        public void SendRequest<T>(string url, List<Header> headers, Action<T> callback = null)
        {
            SendRequest(url, headers, (text, bytes) =>
            {
                var parsedResult = ParseAndGetResult<T>(text);
                callback?.Invoke(parsedResult);
            });
        }
        
        public void SendRequestAndDownload(string url, List<Header> headers, Action<byte[]> callback = null)
        {
            SendRequest(url, headers, (text, bytes) =>
            {
                callback?.Invoke(bytes);
            });
        }
        
        public void SendRequest(string url, List<Header> headers, Action<string, byte[]> callback = null)
        {
            Debug.Log(url);
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            AddHeadersToRequest(ref uwr, headers);
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                Thread.Sleep(50);
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("ERROR: " + uwr.result);
            }
            else
            {
                callback?.Invoke(uwr.downloadHandler.text, uwr.downloadHandler.data);
            }
            
            uwr.Dispose();
        }

        private T ParseAndGetResult<T>(string textToParse)
        {
            var parsedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(textToParse);
            return parsedResult;
        }
        
        private void AddHeadersToRequest(ref UnityWebRequest uwr, List<Header> headers)
        {
            foreach (var header in headers)
            {
                uwr.SetRequestHeader(header.Name, header.Value);
            }
        }
    }

    public class Header
    {
        public string Name;
        public string Value;
    }
}