using Octokit;
using System;
using System.Reflection;
using Vajehyar.Properties;


namespace Vajehyar.Utility
{
    public class GithuHelper
    {
        private static readonly string GithubUserName = Settings.Default.GithubId;
        private static readonly string GithubRepoName = Settings.Default.GithubRepo;

        public static bool HasNewRelease
        {
            get
            {
                try
                {
                    GitHubClient client = new GitHubClient(new ProductHeaderValue("App"));
                    Release latestRelease = client.Repository.Release.GetLatest(GithubUserName, GithubRepoName).Result;
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Version latestVersion = new Version(latestRelease.TagName);

                    if (currentVersion > latestVersion)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }
    }
}

