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
using Bonobo.Git.Server.Security;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using Bonobo.Git.Server.Helpers;

namespace Bonobo.Git.Server.Controllers
{
    public class ActivityController : Controller
    {


        [Dependency]
        public ITeamRepository TeamRepository { get; set; }

        [Dependency]
        public IRepositoryRepository RepositoryRepository { get; set; }

        [Dependency]
        public IRepositoryPermissionService RepositoryPermissionService { get; set; }

        [WebAuthorize]
        public ActionResult LastedCommit(int page = 1)
        {
            page = page >= 1 ? page : 1;
            int pageSize = 10;

            var di = new DirectoryInfo(UserConfiguration.Current.Repositories);
            var allRepo = di.GetDirectories("*.*", SearchOption.TopDirectoryOnly);

            var list = new List<ActivityCommitModels>();
            var repoList = this.GetIndexModel();

            foreach (var repoPath in allRepo)
            {
                using (var browser = new RepositoryBrowser(Path.Combine(UserConfiguration.Current.Repositories, repoPath.Name)))
                {
                    var name = PathEncoder.Decode("");
                    string referenceName;
                    int totalCount;
                    var commits = browser.GetCommits(name, 1, 10, out referenceName, out totalCount).ToList();

                    if (commits.Count < 1)
                        continue;

                    var com = commits.OrderByDescending(c => c.Date).FirstOrDefault();

                    var id = com.ID;
                    var committer = com.Author;
                    var committMail = com.AuthorEmail;
                    var committWhen = com.Date;
                    var message = com.Message;

                    if (repoList.Where(r => r.Name == repoPath.Name).FirstOrDefault() == null)
                        continue;

                    var repoGuid = repoList.Where(r => r.Name == repoPath.Name).FirstOrDefault().Id;

                    var ac = new ActivityCommitModels();
                    ac.ProjectName = repoPath.Name;
                    ac.CommitterName = committer;
                    ac.Email = committMail;
                    ac.When = committWhen;
                    ac.Message = message;
                    ac.idSha = id;
                    ac.id = repoGuid;
                    list.Add(ac);
                    
                }
            }

            ViewBag.TotalCount = list.Count;
            var sortList = list.OrderByDescending(c => c.When).ToList();

            if (page >= 1 && pageSize >= 1)
            {
                sortList = sortList.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            return View(sortList);
        }

        private IEnumerable<RepositoryDetailModel> GetIndexModel()
        {
            return RepositoryPermissionService.GetAllPermittedRepositories(User.Id(), RepositoryAccessLevel.Pull).Select(ConvertRepositoryModel).ToList();
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