namespace GitHubRegistryNetworking.Scripts.Networking.GitHubAPI
{
    using System;
    using Requests;

    public static class GitHubRequests
    {
        private static Requests requests = new Requests();

        public static void GetAllPackagesForAuthor<T>(string token, string authorName, Action<T> callback = null)
        {
            requests.SendRequest(
                GitHubAPIHandler.BuildLinkForAllRepositories(), 
                GitHubAPIHandler.BuildGetHeaders(token),
                callback);
        }
        
        public static void GetAllReleasesForPackage<T>(string token, string authorName, string projectName, Action<T> callback = null)
        {
            requests.SendRequest<T>(
                GitHubAPIHandler.BuildLinkForProjectReleases(authorName, projectName),
                GitHubAPIHandler.BuildGetHeaders(token),
                callback);
        }
    }
}
