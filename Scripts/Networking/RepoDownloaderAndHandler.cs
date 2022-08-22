namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using DataTypes;
    using GitHubAPI;
    using Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;
    using PackageInfo = DataTypes.PackageInfo;
    using RegistryInfo = Registries.RegistryInfo;
    using Task = System.Threading.Tasks.Task;


    public class RepoDownloaderAndHandler
    {
        private const string CustomPackagesFolder = "CustomPackages";
        
        
        public void DownloadPackageVersion(RegistryInfo registryInfo, PackageInfo packageInfo, ReleaseInfo releaseInfo, Action callback = null)
        {
            GitHubRequests.DownloadPackageVersion(
                            registryInfo.Token, 
                            registryInfo.AuthorName, 
                            packageInfo.name,
                            releaseInfo.tag_name,
                            (bytes) =>
                            {
                                HandleDownloadedPackage(bytes, packageInfo, ".zip", releaseInfo.tag_name);
                            });
        }

        private void HandleDownloadedPackage(byte[] bytes, PackageInfo packageInfo, string format, string version)
        {
            SaveToDisk(bytes, packageInfo, format);
            DeCompressDownloadedZipPackage(packageInfo.name, format);
            ExportExtractedPackageToUnityPackage(packageInfo.name, () =>
            {
                ImportUnityPackage(packageInfo.name, version);
            }); 
        }

        private void SaveToDisk(byte[] bytes, PackageInfo packageInfo, string format)
        {
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
            DeleteDirectory("Assets/" + packageInfo.name);
            DeleteFile("Assets/" + packageInfo.name + ".meta");
            
            CreatePackagesFolder();
            File.WriteAllBytes(BuildPackageSavePath(packageInfo.name) + format, bytes);
        }

        private void DeCompressDownloadedZipPackage(string packageName, string format)
        {
            ZipFile.ExtractToDirectory(
                BuildPackageSavePath(packageName) + format, 
                BuildPackageSavePath(packageName));
            RenameExtractedFile(packageName);
        }
 
        private string BuildPackageSavePath(string packageName)
        {
            return Application.persistentDataPath + "/" + CustomPackagesFolder + "/" + packageName;
        }

        private void RenameExtractedFile(string packageName)
        {
            var files = Directory.EnumerateDirectories(BuildPackageSavePath(packageName)).ToList();
            Directory.Move(files.First(), BuildPackageSavePath(packageName) + "/" + packageName);
        }
  
        private async void ExportExtractedPackageToUnityPackage(string packageName, Action callback)
        {  
            MovePackageTemporarilyToAssets(packageName);
            AssetDatabase.Refresh();
            while (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                await Task.Delay(100);
            }
            
            AssetDatabase.ExportPackage(
                $"Assets/{packageName}", 
                BuildPackageSavePath(packageName) + $"/{packageName}.unitypackage", 
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

            DeleteDirectory("Assets/" + packageName);
            DeleteFile("Assets/" + packageName + ".meta");

            callback.Invoke();
        }

        private async void ImportUnityPackage(string packageName, string version)
        {
            await Task.Delay(1);
            AssetDatabase.ImportPackage(BuildPackageSavePath(packageName) + $"/{packageName}.unitypackage", false);
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
            
            AssetDatabase.Refresh();
            while (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                await Task.Delay(100);
            }
            
            MoveToPackagesFolder(packageName);

            AddToManifest(packageName, version);

            AssetDatabase.Refresh();
            while (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                await Task.Delay(100);
            }
        }

        private void AddToManifest(string packageName, string version)
        {
            string jsonText = File.ReadAllText($"Packages/manifest.json");
            var parsedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ManifestJson>(jsonText);
            parsedResult?.dependencies.Add(packageName, version);
            var serializedText = Newtonsoft.Json.JsonConvert.SerializeObject(parsedResult, Formatting.Indented);
            File.WriteAllText("Packages/manifest.json", serializedText);
        }
   
        private void MovePackageTemporarilyToAssets(string packageName)
        {
            var link = BuildPackageSavePath(packageName) + "/" + packageName;
            Directory.Move(link, "Assets/" + packageName);
        }

        private void MoveToPackagesFolder(string packageName)
        {
            Directory.Move("Assets/" + packageName, "Packages/" + packageName);
            File.Move("Assets/" + packageName + ".meta", "Packages/" + packageName + ".meta");
        }

        private void CreatePackagesFolder()
        {
            var path = Application.persistentDataPath + "/" + CustomPackagesFolder;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void DeleteDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        private void DeleteFile(string directory)
        {
            if (File.Exists(directory))
            {
                File.Delete(directory);
            }
        }
    }
}
