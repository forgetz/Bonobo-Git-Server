using Bonobo.Git.Server.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bonobo.Git.Server.Models;
using LibGit2Sharp;

namespace Bonobo.Git.Server.Controllers
{
    public class ActivityController : Controller
    {

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult LastedCommit(int page = 1)
        {
            int pageSize = 10;
            var repositoryDirectory = UserConfiguration.Current.Repositories;
            var di = new DirectoryInfo(repositoryDirectory);
            var allRepo = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            var list = new List<ActivityCommitModels>();

            foreach (var repoPath in allRepo)
            {
                using (var repo = new Repository(repoPath.FullName))
                {
                    var filter = new CommitFilter
                    {
                        SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Reverse,
                        Since = repo.Refs
                    };

                    var commits = repo.Commits.QueryBy(filter);

                    foreach (var com in commits)
                    {
                        var id = com.Id.Sha;
                        var committer = com.Committer.Name;
                        var committMail = com.Committer.Email;
                        var committWhen = com.Committer.When;
                        var message = com.Message;

                        var ac = new ActivityCommitModels();
                        ac.ProjectName = repoPath.Name;
                        ac.CommitterName = committer;
                        ac.Email = committMail;
                        ac.When = committWhen.UtcDateTime;
                        ac.Message = message;
                        ac.id = id;
                        list.Add(ac);
                    }

                }
            }

            ViewBag.TotalCount = list.Count;
            list = list.OrderByDescending(c => c.When).ToList();

            //if (page >= 1 && pageSize >= 1)
            //    ct = ct.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(list);
        }


    }
}