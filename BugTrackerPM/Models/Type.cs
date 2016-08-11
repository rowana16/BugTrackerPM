using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Models
{
    public class Type
    {
        public Type()
        {
            this.Tickets = new HashSet<Ticket>();
            this.TicketHistories = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }
        public string TypeDescription { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
    }
}