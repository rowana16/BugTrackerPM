using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class TicketDetailsViewModel
    {
        public Ticket ticket { get; set; }
        public ICollection<TicketComment> ticketComments { get; set; }
        public ICollection<TicketAttachment> ticketAttachments { get; set; }
        public ICollection<TicketHistory>ticketHistories { get; set; }

    }

    public class TicketCommentViewModel
    {
        public TicketComment comment { get; set; }
        public Ticket ticket { get; set; }
    }

    public class TicketAttachmentViewModel
    {
        public TicketAttachment attachment { get; set; }
        public Ticket ticket { get; set; }
    }



}
