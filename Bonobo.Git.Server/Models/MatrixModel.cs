using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Models
{
    public class MatrixModel
    {
        public string Title { get; set; }
        public string TableHeaderName1 { get; set; }
        public string TableHeaderName2 { get; set; }
        public string ExcelFileName { get; set; }
        public List<MatrixUserRole> MatrixUserRole { get; set; }

    }


    public class MatrixUserRole
    {
        public string Username { get; set; }
        public List<MatrixFlagItem> FlagItem { get; set; }
    }

    public class MatrixFlagItem
    {
        public string Name { get; set; }
        public bool Flag { get; set; }
    }
}