using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class TicketAttachment
    {
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public int Id { get; set; }
        public string AttachmentURL { get; set; }
        public string AttachementDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}