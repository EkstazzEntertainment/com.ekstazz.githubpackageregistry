namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Threading;
    using DataTypes;
    using GitHubAPI;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;
    using PackageInfo = DataTypes.PackageInfo;
    using RegistryInfo = Registries.RegistryInfo;


    public class RepoDownloaderAndHandler
    {
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
            SaveToDisk(bytes, packageInfo, ".zip");
        }

        private void SaveToDisk(byte[] bytes, PackageInfo packageInfo, string format)
        {
            File.WriteAllBytes(Application.persistentDataPath + "/" + packageInfo.name + format, bytes);
        }

        private void DeCompressDownloadedZipPackage()
        {
            
        }
    }
}
