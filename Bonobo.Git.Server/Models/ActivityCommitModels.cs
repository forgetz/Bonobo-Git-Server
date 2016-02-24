using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Models
{
    public class ActivityCommitModels
    {
        public Guid id { get; set; }
        public string idSha { get; set; }
        public string ProjectName { get; set; }
        public string CommitterName { get; set; }
        public string Email { get; set; }
        public DateTime When { get; set; }
        public string Message { get; set; }
    }


}