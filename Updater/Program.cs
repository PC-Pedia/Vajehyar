using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (HasNewRelease())
            {
                Update();
            }
        }

        private static void Update()
        {
        }

        private static bool HasNewRelease()
        {
            //var client = new GitHubClient(new ProductHeaderValue("Vajehyar"));
            return false;
        }
    }
}
