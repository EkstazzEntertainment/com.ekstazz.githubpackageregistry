namespace GitHubRegistryNetworking.Scripts.Networking.DataTypes
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PackageInfo
    {
        public long id;
        public string name;
        public string description;
    }

    [Serializable]
    public class PackageInfos
    {
        public List<PackageInfo> packages;
    }
}
