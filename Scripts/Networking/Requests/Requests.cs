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
            SendRequest(url, headers, responseText =>
            {
                var parsedResult = ParseAndGetResult<T>(responseText);
                callback?.Invoke(parsedResult);
            });
        }
        
        public void SendRequest(string url, List<Header> headers, Action<string> callback = null)
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

            Debug.Log(uwr.downloadHandler.text);
            callback?.Invoke(uwr.downloadHandler.text);
            
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