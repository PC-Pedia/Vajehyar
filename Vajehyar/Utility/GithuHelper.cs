using Octokit;
using System;
using System.Reflection;
using System.Security.Policy;
using Vajehyar.Properties;


namespace Vajehyar.Utility
{
    public class GithuHelper
    {
        private static string githubUserName = Settings.Default.GithubId;
        private static string githubRepoName = Settings.Default.GithubRepo;

        public static bool HasNewRelease
        {
            get
            {
                try
                {
                    GitHubClient client = new GitHubClient(new ProductHeaderValue("App"));
                    Release latestRelease = client.Repository.Release.GetLatest(githubUserName, githubRepoName).Result;
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Version latestVersion = new Version(latestRelease.TagName);

                    if (currentVersion > latestVersion)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }
        }
    }
}

