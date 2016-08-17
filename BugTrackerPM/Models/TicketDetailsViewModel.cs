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

    }

    public class TicketCommentViewModel
    {
        public TicketComment comment { get; set; }
        public Ticket ticket { get; set; }
    }
}