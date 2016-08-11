using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class TicketHistory
    {
        
        public int Id { get; set; }
        //public string SubmitterId { get; set; }
        public string AssignedId { get; set; }
        public int ProjectId { get; set; }
        public int PriorityId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        //public DateTime CreateDate { get; set; }
        public string UpdateReasonNote { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int TicketId { get; set; }
        
        //public virtual ApplicationUser Submitter { get; set; }
        public virtual ApplicationUser Assigned { get; set; }
        public virtual Project Project { get; set; }
        public virtual string Priority { get; set; }
        public virtual string Type { get; set; }
        public virtual string Status { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}