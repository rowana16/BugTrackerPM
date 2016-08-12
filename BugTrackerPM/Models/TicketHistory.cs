using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class TicketHistory
    {
        
        public int Id { get; set; }      
        public string Description { get; set; }
        public DateTime HistoryCreateDate { get; set; }
        public string UpdateReasonNote { get; set; }
        public DateTime HistoryUpdatedDate { get; set; }
        public int TicketId { get; set; }        
        
        public virtual ApplicationUser Assigned { get; set; }
        public virtual Project Project { get; set; }
        public virtual string Priority { get; set; }
        public virtual string TicketType { get; set; }
        public virtual string Status { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}