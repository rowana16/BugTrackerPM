using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class Project
    {
        public Project()
        {
            this.Users = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public string ProjectTitle { get; set; }    
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        

        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}