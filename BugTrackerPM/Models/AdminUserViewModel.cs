﻿using System;
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
        public ApplicationUser userEdit { get; set; }

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

    public class AdminIndexViewModel
    {
        public ApplicationUser user { get; set; }
        public string projectNameList { get; set; }
        public string roleNameList { get; set; }
    }

    public class AdminCompositeViewModel
    {
        public IEnumerable<AdminIndexViewModel>details { get; set; }
    }

    public class HomeViewModel
    {
        public ApplicationUser currentUser { get; set; }
        public List<Ticket> createdTickets { get; set; }
        public List<Project> viewProjects { get; set; }
        public List<Ticket> viewTickets { get; set; }
        public List<ApplicationUser> viewUsers { get; set; }
        public int created { get; set; }
        public int assigned { get; set; }
        public int inProcess { get; set; }
        public int review { get; set; }
        public int resolved { get; set; }
        
        

    }

}