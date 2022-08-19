namespace GitHubRegistryNetworking.Scripts.Networking.GitHubAPI
{
    using Requests;

    public static class GitHubRequests
    {
        private static Requests requests = new Requests();

        public static void GetAllReleasesForPackage(string token, string authorName, string projectName)
        {
            requests.SendRequest(GitHubAPIHandler.BuildLinkForProjectReleases(authorName, projectName), GitHubAPIHandler.BuildGetHeaders(token));
        }
    }
}
