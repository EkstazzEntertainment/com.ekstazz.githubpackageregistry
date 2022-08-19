namespace GitHubRegistryNetworking.Scripts.Networking.GitHubAPI
{
    using System.Collections.Generic;
    using Requests;

    public class GitHubAPIHandler
    {
        public static List<Header> BuildGetHeaders(string token)
        {
            List<Header> headers = new List<Header>()
            {
                new Header {Name = "Accept", Value = "application/vnd.github+json"},
                new Header {Name = "Authorization", Value = "token " + token}
            };
            return headers;
        }

        public void BuildLinkForAllRepositories()
        {
            
        }

        public static string BuildLinkForProjectReleases(string authorName, string repName)
        {
            var basePortion = "https://api.github.com/repos/";
            var finalPortion = "releases";

            var fullLink = basePortion + authorName + "/" + repName + "/" + finalPortion;
            
            return fullLink;
        }
        
    } 
}
