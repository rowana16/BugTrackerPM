using BugTrackerPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPM.Controllers
{
    

    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            List<Project> viewProjects = new List<Project>();
            List<Ticket> viewTickets = new List<Ticket>();
            List<ApplicationUser> viewUsers = new List<ApplicationUser>();

            viewProjects = db.Projects.ToList();
            viewTickets = db.Ticket.ToList();
            viewUsers = db.Users.ToList();

            int created = viewTickets.Count(p => p.Status.StatusDescription == "Created");
            int assigned = viewTickets.Count(p => p.Status.StatusDescription == "Assigned");
            int inProcess = viewTickets.Count(p => p.Status.StatusDescription == "In Process");
            int review = viewTickets.Count(p => p.Status.StatusDescription == "Review");
            int resolved = viewTickets.Count(p => p.Status.StatusDescription == "Resolved");
            
            ViewBag.created = created;
            ViewBag.assigned = assigned;
            ViewBag.inProcess = inProcess;
            ViewBag.review = review;
            ViewBag.resolved = resolved;
            ViewBag.Projects = viewProjects;
            ViewBag.Tickets = viewTickets;
            ViewBag.Users = viewUsers;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}