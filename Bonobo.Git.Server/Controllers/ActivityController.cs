using Bonobo.Git.Server.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bonobo.Git.Server.Models;
using LibGit2Sharp;
using Microsoft.Practices.Unity;
using Bonobo.Git.Server.Data;

namespace Bonobo.Git.Server.Controllers
{
    public class ActivityController : Controller
    {
        [Dependency]
        public ITeamRepository TeamRepository { get; set; }

        [Dependency]
        public IRepositoryRepository RepositoryRepository { get; set; }

        [WebAuthorize]
        public ActionResult LastedCommit(int page = 1)
        {
            int pageSize = 10;
            var repositoryDirectory = UserConfiguration.Current.Repositories;
            var di = new DirectoryInfo(repositoryDirectory);
            var allRepo = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            var list = new List<ActivityCommitModels>();

            var repoList = this.GetIndexModel();

            foreach (var repoPath in allRepo)
            {
                using (var repo = new LibGit2Sharp.Repository(repoPath.FullName))
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

                        if (repoList.Where(r => r.Name == repoPath.Name).FirstOrDefault() == null)
                            continue;

                        var repoGuid = repoList.Where(r => r.Name == repoPath.Name).FirstOrDefault().Id; 

                        var ac = new ActivityCommitModels();
                        ac.ProjectName = repoPath.Name;
                        ac.CommitterName = committer;
                        ac.Email = committMail;
                        ac.When = committWhen.UtcDateTime;
                        ac.Message = message;
                        ac.idSha = id;
                        ac.id = repoGuid;
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


        private IEnumerable<RepositoryDetailModel> GetIndexModel()
        {
            IEnumerable<RepositoryModel> repositoryModels;
            if (User.IsInRole(Definitions.Roles.Administrator))
            {
                repositoryModels = RepositoryRepository.GetAllRepositories();
            }
            else
            {
                var userTeams = TeamRepository.GetTeams(User.Id()).Select(i => i.Id).ToArray();
                repositoryModels = RepositoryRepository.GetPermittedRepositories(User.Id(), userTeams);
            }
            return repositoryModels.Select(ConvertRepositoryModel).ToList();
        }

        private RepositoryDetailModel ConvertRepositoryModel(RepositoryModel model)
        {
            return model == null ? null : new RepositoryDetailModel
            {
                Id = model.Id,
                Name = model.Name,
                Group = model.Group,
                Description = model.Description,
                Users = model.Users,
                Administrators = model.Administrators,
                Teams = model.Teams,
                IsCurrentUserAdministrator = model.Administrators.Select(x => x.Username).Contains(User.Username(), StringComparer.OrdinalIgnoreCase),
                AllowAnonymous = model.AnonymousAccess,
                Status = GetRepositoryStatus(model),
                AuditPushUser = model.AuditPushUser,
                Logo = new RepositoryLogoDetailModel(model.Logo),
            };
        }

        private RepositoryDetailStatus GetRepositoryStatus(RepositoryModel model)
        {
            string path = Path.Combine(UserConfiguration.Current.Repositories, model.Name);
            if (!Directory.Exists(path))
            {
                return RepositoryDetailStatus.Missing;
            }
            else
            {
                return RepositoryDetailStatus.Valid;
            }
        }

    }
}