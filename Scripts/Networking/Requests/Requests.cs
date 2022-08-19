namespace GitHubRegistryNetworking.Scripts.Networking.Requests
{
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Networking;


    public class Requests
    {
        public void SendRequest(string url, List<Header> headers)
        {
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
            uwr.Dispose();
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