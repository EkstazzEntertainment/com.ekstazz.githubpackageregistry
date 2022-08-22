namespace GitHubRegistryNetworking.Scripts.DataTypes
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ManifestJson
    {
        public Dictionary<string, string> dependencies;
        public bool enableLockFile;
        public string registry;
        public string resolutionStrategy = ResolutionStrategy.lowest.ToString();
        public string[] testables;
        public bool useSatSolver;
        public ScopedRegistryInfo[] scopedRegistries = new ScopedRegistryInfo[0];
    }
 
    [Serializable]
    public class ScopedRegistryInfo
    {
        public string name;
        public string url;
        public string[] scopes;
    }

    [Serializable]
    public enum ResolutionStrategy
    {
        lowest,
        highestPatch,
        highestMinor,
        highest
    }
}
