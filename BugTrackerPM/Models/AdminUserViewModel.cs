using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPM.Models
{
    public class AdminUserViewModel
    {
        //name id *roles *absentroles aSelectedCurrentRoles aSelectedAbsentRoles

        public string id { get; set; }
        public string name { get; set; }

        public MultiSelectList roles { get; set; }
        public MultiSelectList absentRoles { get; set; }

        //public string[] SelectedCurrentRoles { get; set; }
        public string[] SelectedAbsentRoles { get; set; }

        //public ApplicationUser User { get; set; }
        //public ApplicationUser Roles { get; set; }
    }

    public class AdminDashboardViewModel
    {
        //public IEnumerable<string> ids { get; set; }
        //public IEnumerable<string> names { get; set; }

        public IList<string> ids { get; set; }
        public IList<string> names { get; set; }

    }
}