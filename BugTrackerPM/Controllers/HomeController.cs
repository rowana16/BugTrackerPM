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
            HomeViewModel viewModel = new HomeViewModel();
            

            viewModel.viewProjects = db.Projects.ToList();
            viewModel.viewTickets = db.Ticket.ToList();
            viewModel.viewUsers = db.Users.ToList();

            viewModel.createdTickets = viewModel.viewTickets.Where(p => p.Status.StatusDescription == "Created").ToList();
            viewModel.created = viewModel.createdTickets.Count();
            viewModel.assigned = viewModel.viewTickets.Count(p => p.Status.StatusDescription == "Assigned");
            viewModel.inProcess = viewModel.viewTickets.Count(p => p.Status.StatusDescription == "In Process");
            viewModel.review = viewModel.viewTickets.Count(p => p.Status.StatusDescription == "Review");
            viewModel.resolved = viewModel.viewTickets.Count(p => p.Status.StatusDescription == "Resolved");
            
            return View(viewModel);
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