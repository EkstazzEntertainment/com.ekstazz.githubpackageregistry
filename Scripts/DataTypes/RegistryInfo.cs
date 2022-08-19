namespace GitHubRegistryNetworking.Scripts.Registries
{
    using System.Collections.Generic;
    using DataTypes;

    public class RegistryInfo
    {
        public string RepositoryLink;
        public string AuthorName;
        public string Token;

        public List<PackageInfo> Packages;
    }
}