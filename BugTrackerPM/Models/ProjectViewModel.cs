using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPM.Models
{
    public class ProjectEditViewModel
    {
        public Project project { get; set; }
        public List<ApplicationUser> absentUserList { get; set; }
        
        public MultiSelectList assignedUsers { get; set; }
        public MultiSelectList absentUsers { get; set; }

        public string[] assignments { get; set; }
        public string[] removals { get; set; }
    }
}