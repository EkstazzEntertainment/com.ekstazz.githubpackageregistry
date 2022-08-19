namespace GitHubRegistryNetworking.Scripts.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataTypes;
    using GitHubAPI;
    using Registries;

    public class RegistriesLoader
    {
        public void LoadAllRegistriesData(List<RegistryInfo> registryInfos, Action callback = null)
        {
            foreach (var registryInfo in registryInfos)
            {
                LoadRegistry(registryInfo);
            }
            
            callback?.Invoke();
        }
         
        private async void LoadRegistry(RegistryInfo registryInfo)
        {
            var loaded = false;
            // GitHubRequests.GetAllReleasesForPackage(registryInfo.Token, registryInfo.AuthorName, "com.ekstazz.ads");
            GitHubRequests.GetAllPackagesForAuthor<List<PackageInfo>>(
                registryInfo.Token, 
                registryInfo.AuthorName,
                (result) =>
                {
                    registryInfo.Packages = result;
                    loaded = true;
                });
 
            while (!loaded)
            {
                await Task.Delay(50);
            }
        } 
    }
}
