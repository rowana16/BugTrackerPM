using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class TicketViewModel
    {
        public IEnumerable<Ticket> Ticket { get; set; }
        
    }
}