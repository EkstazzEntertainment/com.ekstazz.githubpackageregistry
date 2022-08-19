namespace GitHubRegistryNetworking.Scripts.Networking.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataTypes;
    using UnityEngine;
    using UnityEngine.Networking;


    public class Requests
    {
        public void SendRequest<T>(string url, List<Header> headers, Action<T> callback = null)
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
            var parsedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
            
            uwr.Dispose();
            
            callback?.Invoke(parsedResult);
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