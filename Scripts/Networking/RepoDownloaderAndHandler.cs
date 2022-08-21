namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using DataTypes;
    using GitHubAPI;
    using UnityEditor;
    using UnityEditor.VersionControl;
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
                                HandleDownloadedPackage(bytes, packageInfo, ".zip");
                            });
        }

        private void HandleDownloadedPackage(byte[] bytes, PackageInfo packageInfo, string format)
        {
            SaveToDisk(bytes, packageInfo, format);
            DeCompressDownloadedZipPackage(packageInfo.name, format);
            ImportExtractedPackage(packageInfo.name);
        }

        private void SaveToDisk(byte[] bytes, PackageInfo packageInfo, string format)
        {
            DeleteOldUndeletedDirectories(packageInfo.name);
            
            CreatePackagesFolder();
            File.WriteAllBytes(BuildPackageSavePath(packageInfo.name) + format, bytes);
        }

        private void DeleteOldUndeletedDirectories(string packageName)
        {
            DeleteDirectory(Application.persistentDataPath + "/" + CustomPackagesFolder);
            DeleteDirectory("Assets/" + packageName);
            DeleteFile("Assets/" + packageName + ".meta");
            // DeleteFile("Assets/" + packageName + ".unitypackage");
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
 
        private async void ImportExtractedPackage(string packageName)
        { 
            var link = BuildPackageSavePath(packageName) + "/" + packageName;
            MovePackageTemporarilyToAssets(packageName);
            ExportPackageOptions exportFlags = ExportPackageOptions.Default | ExportPackageOptions.Interactive | ExportPackageOptions.Recurse;
            await Task.Delay(10000);
            AssetDatabase.ExportPackage("Assets/com.ekstazz.ads", "Assets/com.ekstazz.ads.unitypackage", ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
            DeleteOldUndeletedDirectories(packageName);
        }
  
        private void MovePackageTemporarilyToAssets(string packageName)
        {
            var link = BuildPackageSavePath(packageName) + "/" + packageName;
            Directory.Move(link, "Assets/" + packageName);
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
