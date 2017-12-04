using Bonobo.Git.Server.Data;
using Bonobo.Git.Server.Models;
using Bonobo.Git.Server.Security;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bonobo.Git.Server.Controllers
{
    
    public class MatrixController : Controller
    {
        //Dependency
        [Dependency]
        public IMembershipService MembershipService { get; set; }

        [Dependency]
        public IRoleProvider RoleProvider { get; set; }

        [Dependency]
        public ITeamRepository TeamRepository { get; set; }

        [Dependency]
        public IRepositoryRepository Repository { get; set; }




        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult Index()
        {
            var users = MembershipService.GetAllUsers();
            var roles = RoleProvider.GetAllRoles().ToList();

            return View();
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult UserRole()
        {
            var matrixModel = new MatrixModel();
            var users = MembershipService.GetAllUsers();
            var roles = RoleProvider.GetAllRoles().ToList();
            var roleProvider = new EFRoleProvider { CreateContext = () => new BonoboGitServerContext() };

            var data = new List<MatrixUserRole>();
            foreach (var user in users)
            {
                var mur = new MatrixUserRole();
                mur.Username = user.Username;
                var userInRoles = new List<MatrixFlagItem>();
                foreach (var role in roles)
                {
                    var isUserinRole = roleProvider.FindUsersInRole(role, user.Username);
                    var isInRole = isUserinRole.Count() > 0;
                    var userinRole = new MatrixFlagItem();

                    userinRole.Name = role;
                    userinRole.Flag = isInRole;
                    userInRoles.Add(userinRole);
                }

                mur.FlagItem = userInRoles;
                data.Add(mur);
            }

            matrixModel.MatrixUserRole = data;
            matrixModel.Title = "User Role";
            matrixModel.TableHeaderName1 = "User";
            matrixModel.TableHeaderName2 = "Role";
            matrixModel.ExcelFileName = "UserRole";

            //using (var db = new BonoboGitServerContext())
            //{

            //    var roles = db.Roles.ToList();


            //}

            return View(matrixModel);
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult TeamMember()
        {
            var matrixModel = new MatrixModel();
            var users = MembershipService.GetAllUsers();
            var teams = TeamRepository.GetAllTeams();

            var data = new List<MatrixUserRole>();
            foreach (var user in users)
            {
                var mur = new MatrixUserRole();
                mur.Username = user.Username;
                var userInRoles = new List<MatrixFlagItem>();
                foreach (var team in teams)
                {
                    var isUserInTeam = team.Members.Where(u => u.Username == user.Username).FirstOrDefault();
                    var isInRole = isUserInTeam != null;
                    var userinRole = new MatrixFlagItem();

                    userinRole.Name = team.Name;
                    userinRole.Flag = isInRole;
                    userInRoles.Add(userinRole);
                }

                mur.FlagItem = userInRoles;
                data.Add(mur);
            }

            matrixModel.MatrixUserRole = data;
            matrixModel.Title = "Team Member";
            matrixModel.TableHeaderName1 = "User";
            matrixModel.TableHeaderName2 = "Team";
            matrixModel.ExcelFileName = "TeamMember";

            return View(matrixModel);
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult RepositoryContributor()
        {
            var matrixModel = new MatrixModel();
            var users = MembershipService.GetAllUsers();
            var repos = Repository.GetAllRepositories().OrderBy(s=>s.Name).ToList();

            var data = new List<MatrixUserRole>();
            foreach (var user in users)
            {
                var mur = new MatrixUserRole();
                mur.Username = user.Username;
                var userInRoles = new List<MatrixFlagItem>();
                foreach (var repo in repos)
                {
                    var isUserInTeam = repo.Users.Where(u => u.Username == user.Username).FirstOrDefault();
                    var isInRole = isUserInTeam != null;
                    var userinRole = new MatrixFlagItem();

                    userinRole.Name = repo.Name;
                    userinRole.Flag = isInRole;
                    userInRoles.Add(userinRole);
                }

                mur.FlagItem = userInRoles;
                data.Add(mur);
            }

            matrixModel.MatrixUserRole = data;
            matrixModel.Title = "Repository Contributor";
            matrixModel.TableHeaderName1 = "User";
            matrixModel.TableHeaderName2 = "Repository";
            matrixModel.ExcelFileName = "RepositoryContributor";

            return View(matrixModel);
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult RepositoryAdmin()
        {
            var matrixModel = new MatrixModel();
            var users = MembershipService.GetAllUsers();
            var repos = Repository.GetAllRepositories().OrderBy(s => s.Name).ToList();

            var data = new List<MatrixUserRole>();
            foreach (var user in users)
            {
                var mur = new MatrixUserRole();
                mur.Username = user.Username;
                var userInRoles = new List<MatrixFlagItem>();
                foreach (var repo in repos)
                {
                    var isUserInTeam = repo.Administrators.Where(u => u.Username == user.Username).FirstOrDefault();
                    var isInRole = isUserInTeam != null;
                    var userinRole = new MatrixFlagItem();

                    userinRole.Name = repo.Name;
                    userinRole.Flag = isInRole;
                    userInRoles.Add(userinRole);
                }

                mur.FlagItem = userInRoles;
                data.Add(mur);
            }

            matrixModel.MatrixUserRole = data;
            matrixModel.Title = "Repository Admin";
            matrixModel.TableHeaderName1 = "User";
            matrixModel.TableHeaderName2 = "Repository";
            matrixModel.ExcelFileName = "RepositoryAdmin";

            return View(matrixModel);
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult RepositoryTeam()
        {
            var matrixModel = new MatrixModel();
            var teams = TeamRepository.GetAllTeams();
            var repos = Repository.GetAllRepositories().OrderBy(s => s.Name).ToList();

            var data = new List<MatrixUserRole>();
            foreach (var team in teams)
            {
                var mur = new MatrixUserRole();
                mur.Username = team.Name;
                var userInRoles = new List<MatrixFlagItem>();
                foreach (var repo in repos)
                {
                    var isUserInTeam = repo.Teams.Where(u => u.Name == team.Name).FirstOrDefault();
                    var isInRole = isUserInTeam != null;
                    var userinRole = new MatrixFlagItem();

                    userinRole.Name = repo.Name;
                    userinRole.Flag = isInRole;
                    userInRoles.Add(userinRole);
                }

                mur.FlagItem = userInRoles;
                data.Add(mur);
            }

            matrixModel.MatrixUserRole = data;
            matrixModel.Title = "Repository Team";
            matrixModel.TableHeaderName1 = "Team";
            matrixModel.TableHeaderName2 = "Repository";
            matrixModel.ExcelFileName = "RepositoryTeam";

            return View(matrixModel);
        }




    }

}

