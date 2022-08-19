namespace GitHubRegistryNetworking.Scripts.Networking.GitHubAPI
{
    using System.Collections.Generic;
    using Requests;

    public class GitHubAPIHandler
    {
        private static string BaseAPILink = "https://api.github.com/";
        
        
        public static List<Header> BuildGetHeaders(string token)
        {
            List<Header> headers = new List<Header>
            {
                new Header {Name = "Accept", Value = "application/vnd.github+json"},
                new Header {Name = "Authorization", Value = "token " + token}
            };
            return headers;
        }

        public static string BuildLinkForAllRepositories()
        {
            var majorFolder = "user/";
            var finalPortion = "repos";
            var parameters = "?visibility=private";

            var fullLink = BaseAPILink + majorFolder + finalPortion + parameters;
            
            return fullLink;
        } 

        public static string BuildLinkForProjectReleases(string authorName, string repName)
        {
            var majorFolder = "repos/";
            var finalPortion = "releases";

            var fullLink = BaseAPILink + majorFolder + authorName + "/" + repName + "/" + finalPortion;
            
            return fullLink;
        }
    } 
}
