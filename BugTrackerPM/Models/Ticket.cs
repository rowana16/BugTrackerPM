using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class Ticket
    {
        public Ticket()
        {
            //TicketCommentID, TicketAttachmentId, TicketHistoryId
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketHistories = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }
        public string SubmitterId { get; set; }
        public string AssignedId { get; set; }
        public int ProjectId { get; set; }
        public int PriorityId { get; set; }
        public int TicketTypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ApplicationUser Submitter { get; set; }
        public virtual ApplicationUser Assigned { get; set; }
        public virtual Project Project { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Status Status { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }

       


    }
}