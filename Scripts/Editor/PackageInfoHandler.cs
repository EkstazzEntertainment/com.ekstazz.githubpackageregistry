using System;

namespace GitHubRegistryNetworking.Scripts.Editor
{
    using System.IO;
    using UnityEditor.PackageManager.Requests;

    public class PackageInfoHandler
    {
        private static ListRequest request;
  
        public bool CheckIfPackageIsInstalled(string packageName)
        {
            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains(packageName);
        } 
 
        public string GetInstalledVersion(string packageName)
        {
            string jsonText = File.ReadAllText($"Packages/{packageName}/package.json");
            var parsedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageJson>(jsonText);
            return parsedResult.version;
        } 
    }
}

[Serializable]
public class PackageJson
{
    public string version;
}
