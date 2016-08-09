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
        public MultiSelectList users { get; set; }
        public string[] SelectedCurrentRoles { get; set; }
        public string[] SelectedAbsentRoles { get; set; }
    }
}