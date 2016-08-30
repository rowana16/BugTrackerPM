using BugTrackerPM.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;

namespace BugTrackerPM.Models
{
    public class TicketsController : Controller
    {
       
        private ApplicationUserManager _userManager;

        public TicketsController()
        {
        }

        public TicketsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;           
        }      

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
        private ApplicationDbContext db = new ApplicationDbContext();
        /* ==================================================  Dashboard  ===================================================== */
        // GET: Tickets
        [Authorize (Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Index()
        {
            TicketViewModel ViewModel = new TicketViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);

            if (helper.IsUserInRole(currentUserId, "Admin"))
            {
                // Admin sees all tickets
                var tickets = db.Ticket.Include(t => t.Assigned).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Submitter).Include(t => t.TicketType);
                ViewModel.Ticket = tickets.ToList();
                ViewModel.user = currentUser;
                return View(ViewModel);
            }

            if (helper.IsUserInRole(currentUserId, "ProjectManager"))
            {
                //Project Manager can see all tickets to which they belong
                var projects = currentUser.Projects;
                var tickets = new List<Ticket>();

                foreach (Project p in projects)
                {
                    foreach (Ticket t in p.Tickets)
                    {
                        tickets.Add(t);
                    }
                }

                ViewModel.Ticket = tickets;
                ViewModel.user = currentUser;
                return View(ViewModel);
            }

            if (helper.IsUserInRole(currentUserId, "Developer"))
            {
                //Developer sees all tickets they are assigned or submitted
                var submittedTickets = new List<Ticket>();
                var assignedTickets = new List<Ticket>();
                var assignedProjects = new List<Project>();

                //var projects = currentUser.Projects;


                //assignedProjects = currentUser.Projects.ToList();
                //foreach (Project p in assignedProjects)
                //{
                //    foreach (Ticket t in p.Tickets)
                //    {                       
                //            projectTickets.Add(t);
                //    }
                //}

                submittedTickets = db.Ticket.Where(i => i.SubmitterId == currentUserId).ToList();
                assignedTickets = db.Ticket.Where(i => i.AssignedId == currentUserId).ToList();
                IEnumerable<Ticket> tickets = submittedTickets.Union(assignedTickets);

                //var assignedProjects = db.Projects.Where(p => p.Users.Contains(currentUser));
               
                ViewModel.user = currentUser;
                ViewModel.Ticket = tickets;
                return View(ViewModel);
            }

            if (helper.IsUserInRole(currentUserId, "Submitter"))
            {
                var tickets = db.Ticket.Where(i => i.SubmitterId == currentUserId);
                ViewModel.user = currentUser;
                ViewModel.Ticket = tickets.ToList();
                return View(ViewModel);
            }

            return View();
        }


        /* ==================================================  Details  ===================================================== */
        // GET: Tickets/Details/5
        public ActionResult Details(int id)
        {
            TicketDetailsViewModel ViewModel = new TicketDetailsViewModel();

                       
            var ticket = db.Ticket.Find(id);
           
            ViewModel.ticket = ticket;
            ViewModel.ticketComments = ticket.TicketComments;
            ViewModel.ticketAttachments = ticket.TicketAttachments;
            ViewModel.ticketHistories = ticket.TicketHistories;

            if (ticket.Project == null)
            {
                return RedirectToAction("index");
            }
            if (ticket == null)
            {
                return HttpNotFound();
            }



            return View(ViewModel);

        }

        /* ==================================================  Create Ticket Get ===================================================== */

        // GET: Tickets/Create
        [Authorize (Roles ="Submitter")]
        public ActionResult Create()
        {
            UserRolesHelper helper = new UserRolesHelper(db);

            if (!helper.IsUserInRole(User.Identity.GetUserId(),"Submitter"))
            {
                return RedirectToAction("index");
            }
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription");
            return View();
        }


        /* ==================================================  Create Ticket Post ===================================================== */

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int PriorityId, int TicketTypeId, string Description, Ticket ticket)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            
            if (!helper.IsUserInRole(User.Identity.GetUserId(), "Submitter"))
            {
                return RedirectToAction("index");
            }
            ticket.SubmitterId = User.Identity.GetUserId();
            ticket.AssignedId = "eb01b7bf-922a-49db-a36b-cf3b835c0400"; //Admin User Id
            //ticket.ProjectId = 1;
            ticket.PriorityId = PriorityId;
            ticket.TicketTypeId = TicketTypeId;
            ticket.StatusId = 1;
            ticket.Description = Description;
            ticket.CreateDate = DateTime.Now;
            ticket.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Ticket.Add(ticket);
                db.SaveChanges();
                CreateHistory(ticket.Id);
                return RedirectToAction("Index");
            }

            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }
        /* ================================================== Edit Tickets Get ===================================================== */

        // GET: Tickets/Edit/5
        [Authorize (Roles ="Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Ticket.Find(id);
            Project nullProject = new Project();

            if (ticket == null)
            {
                return HttpNotFound();
            }
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);

            
            if (!(/*ticket.AssignedId == currentUserId ||*/ helper.IsUserInRole(User.Identity.GetUserId(),"Developer")/* || ticket.SubmitterId == currentUserId*/ || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
                return RedirectToAction("Index");
            }
           
            else
            {
                
                List<Project> projectList = new List<Project>();
                projectList = db.Projects.ToList();

                nullProject.ProjectTitle = "Unassigned";
                projectList.Add(nullProject);
                ViewBag.ProjectTitle = new SelectList(projectList, "Id", "ProjectTitle", ticket.ProjectId);
            }

            List<ApplicationUser> developers = new List<ApplicationUser>();
            foreach (ApplicationUser user in db.Users.ToList())
            {
                if (helper.IsUserInRole(user.Id, "Developer"))
                {
                    developers.Add(user);
                }
            }

            ViewBag.AssignedId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedId);
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }


        /* ==================================================  Edit Tickets Post ===================================================== */

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Developer, Submitter")]
        public ActionResult Edit(int ProjectId, int PriorityId , int StatusId, int TicketTypeId, string AssignedId, string SubmitterId, string Description, DateTime CreateDate, Ticket ticket)
        {
            bool bProject = false;
            bool bAssigned = false;
            bool bType = false;
            bool bStatus = false;
            bool bDescription = false;
            Ticket origTicket = new Ticket();
            origTicket =   db.Ticket.Find(ticket.Id);

            if(ProjectId != origTicket.ProjectId) { bProject = true; }
            if (AssignedId != origTicket.AssignedId) {
                bAssigned = true;
                sendEmail(ticket.AssignedId, "Ticket Assignment", "You have been assigned to ticket " + ticket.Id + ": " + ticket.Description);
                    }
            if (TicketTypeId != origTicket.TicketTypeId)   { bType = true;  }
            if (StatusId != origTicket.StatusId) { bStatus = true; }
            if (Description != origTicket.Description) { bDescription = true; }

            ticket.ProjectId = ProjectId;
            ticket.PriorityId = PriorityId;
            ticket.StatusId = StatusId;
            ticket.TicketTypeId = TicketTypeId;
            ticket.AssignedId = AssignedId;
            ticket.SubmitterId = SubmitterId;
            ticket.Description = Description;
            ticket.CreateDate = CreateDate;
            ticket.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(origTicket).CurrentValues.SetValues(ticket);
                db.SaveChanges();

                if (bProject || bAssigned || bType || bStatus || bDescription)
                {
                    CreateHistory(bProject, bAssigned, bType, bStatus, bDescription, ticket.Id);
                    if(!(bAssigned || ticket.AssignedId == User.Identity.GetUserId()))
                    {
                        sendEmail(ticket.AssignedId, "Ticket: " + ticket.Id + " Updated", "Your Ticket has been updated by another user.");
                    }

                }
                return RedirectToAction("Index");
            }           

            ViewBag.AssignedId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedId);
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }

        /* ==================================================  Delete Ticket Get ===================================================== */
        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        /* ==================================================  Delete Ticket Post ===================================================== */
        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /* ==================================================  Ticket Comment ===================================================== */
        /* ==================================================  Ticket Comment ===================================================== */
        /* ==================================================  Ticket Comment ===================================================== */


        /* ==================================================  Create TicketComment Get ===================================================== */
       
        public ActionResult CreateTicketComment(int id)
        {

            Ticket currentTicket = db.Ticket.Find(id);
            TicketCommentViewModel viewModel = new TicketCommentViewModel();
            viewModel.ticket = currentTicket;
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);


            if (!(currentTicket.AssignedId == currentUserId || currentTicket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")) )
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
                return RedirectToAction("Details", new { id = id });

            }


            return View(viewModel);

        }

        /* ==================================================  Create TicketComment Post ===================================================== */

        [HttpPost]
        public ActionResult CreateTicketComment(string body, int id)
        {
            TicketCommentViewModel viewModel = new TicketCommentViewModel();
            Ticket currentTicket = db.Ticket.Find(id);
            TicketComment submittedComment = new TicketComment();
            submittedComment.Body = body;
            submittedComment.CreateDate = DateTime.Now;
            submittedComment.UpdateDate = DateTime.Now;
            submittedComment.TicketId = id;
            submittedComment.AuthorId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.TicketComment.Add(submittedComment);
                db.SaveChanges();
                CreateHistory(3, 1, currentTicket.Id);
                if (submittedComment.AuthorId != currentTicket.AssignedId)
                {
                    sendEmail(submittedComment.AuthorId, "Ticket Commment Created", "A ticket comment was created by another user for ticket " + id);
                }
            }

            viewModel.ticket = currentTicket;
            return RedirectToAction("Details", new { id = currentTicket.Id });
        }

        /* ==================================================  Edit TicketComment Get ===================================================== */

        public ActionResult EditTicketComment(int id)
        {
            TicketCommentViewModel viewModel = new TicketCommentViewModel();
            TicketComment currentComment = db.TicketComment.Find(id);
            Ticket parentTicket = db.Ticket.Find(currentComment.TicketId);

            viewModel.ticket = parentTicket;
            viewModel.comment = currentComment;

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);
            Ticket currentTicket = parentTicket;

            if (!(currentTicket.AssignedId == currentUserId || currentTicket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
                return RedirectToAction("Details", new { id = id });

            }

            return View(viewModel);
        }

        /* ==================================================  Edit TicketComment Post ===================================================== */

        [HttpPost]
        public ActionResult EditTicketComment(int id, string body, int ticketId)
        {
            TicketCommentViewModel viewModel = new TicketCommentViewModel();
            TicketComment currentComment = db.TicketComment.Find(id);
            currentComment.Body = body;

            if (ModelState.IsValid)
            {
                db.Entry(currentComment).State = EntityState.Modified;
                db.SaveChanges();
                CreateHistory(3, 2, ticketId);
                return RedirectToAction("Details", new { id = ticketId });
            }

            return View(viewModel);
        }

        /* ==================================================  Ticket Attachment ===================================================== */
        /* ==================================================  Ticket Attachment ===================================================== */
        /* ==================================================  Ticket Attachment ===================================================== */


        /* ==================================================  Create TicketAttachment Get ===================================================== */

        public ActionResult CreateTicketAttachment(int id)
        {
            Ticket currentTicket = db.Ticket.Find(id);
            TicketAttachment currentAttachment = new TicketAttachment();
            TicketAttachmentViewModel viewModel = new TicketAttachmentViewModel();
            viewModel.attachment = currentAttachment;
            viewModel.ticket = currentTicket;

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);


            if (!(currentTicket.AssignedId == currentUserId || currentTicket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
                return RedirectToAction("Details", new { id = id });

            }

            return View(viewModel);
        }

        /* ==================================================  Create TicketAttachment Post ===================================================== */

        [HttpPost]
        public ActionResult CreateTicketAttachment(int id, HttpPostedFileBase URL, string body )
        {
            TicketAttachment currentAttachment = new TicketAttachment();
            Ticket currentTicket = new Ticket();
            currentTicket = db.Ticket.Find(id);

            if (URL != null && URL.ContentLength >0 )
            {
                var extension = Path.GetExtension(URL.FileName).ToLower();
                if (extension != ".png" && extension != ".jpg" && extension != ".gif" && extension != ".jpeg" && extension != ".bmp" && extension != ".doc" && extension != ".docx" && extension != ".pdf" && extension != ".xls" && extension != ".xlsx" && extension != ".xslm" && extension != ".ppt" && extension != ".pptx" && extension != ".txt")
                { ModelState.AddModelError("image", "Invalid Format."); }
            }

            if (ModelState.IsValid)
            {
                currentAttachment.AuthorId = User.Identity.GetUserId();
                currentAttachment.TicketId = id;
                currentAttachment.CreateDate = DateTime.Now;
                currentAttachment.UpdateDate = DateTime.Now;
                currentAttachment.AttachementDescription = body;
                if(URL != null)
                {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    currentAttachment.AttachmentURL = filePath + URL.FileName;
                    URL.SaveAs(Path.Combine(absPath, URL.FileName));
                }

                CreateHistory(4, 1, id);
                db.TicketAttachment.Add(currentAttachment);
                db.SaveChanges();
                if (currentAttachment.AuthorId != currentTicket.AssignedId)
                {
                    sendEmail(currentAttachment.AuthorId, "Ticket Attachment Created", "A ticket attachment was created by another user for ticket " + id);
                }
                return RedirectToAction("Details", new { id = currentAttachment.TicketId });

            }

            TicketAttachmentViewModel viewModel = new TicketAttachmentViewModel();
            viewModel.attachment = currentAttachment;            
            viewModel.ticket = currentTicket;

            return View(viewModel);
        }

        /* ==================================================  Edit TicketAttachment Get ===================================================== */

        public ActionResult EditTicketAttachment(int id)
        {
            
            TicketAttachmentViewModel viewModel = new TicketAttachmentViewModel();
            viewModel.attachment = db.TicketAttachment.Find(id);
            viewModel.ticket = viewModel.attachment.Ticket;
            Ticket currentTicket = viewModel.ticket;

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);


            if (!(currentTicket.AssignedId == currentUserId || currentTicket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
               
                return RedirectToAction("Details", new { id = id });

            }

            return View(viewModel);
        }

        

        /* Ticket Changes Overloads :
         *               Ticket Creation ( Ticket ID Only)
         *               Ticket Direct Changes (bool assigned, bool type, bool status, bool description, int ticketId)
         *               Ticket Comment Changes (int historyType, int historySubType, int ticketId)
         *               */

        public void CreateHistory(int ticketId)
        {
            string history = "Ticket Created, ";
            SaveHistory(HistoryStringBuilder(history), ticketId);

        }

        public void CreateHistory(bool project, bool assigned, bool type, bool status, bool description, int ticketId)
        {
            string history = "";
            if (project) { history = HistoryStringBuilder("Project Changed, ", history); }
            if (assigned) {history = HistoryStringBuilder("Assigned Changed, ", history);}
            if (type) { history = HistoryStringBuilder("Type Changed, ", history); }
            if(status) { history = HistoryStringBuilder("Status Changed, ", history); }
            if(description) { history = HistoryStringBuilder("Description Changed, ", history); }

            SaveHistory(HistoryStringBuilder(history), ticketId);



        }
        public void CreateHistory(int historyType, int historySubType, int ticketId)
        {
            string history = "";
            if (historyType == 3)
            {
                if (historySubType == 1) { history = HistoryStringBuilder("Comment Created, ", history); }
                if (historySubType == 2) { history = HistoryStringBuilder("Comment Edited, ", history); }
               
            }

            if (historyType == 4)
            {
                if (historySubType == 1) { history = HistoryStringBuilder("Attachment Created, ", history); }
                if (historySubType == 2) { history = HistoryStringBuilder("Attachment Edited, ", history); }
                           
            }
            
            SaveHistory(HistoryStringBuilder(history), ticketId);
           
        }

        private string HistoryStringBuilder(string currentString)
        {
            if (currentString != "") return currentString.Substring(0, currentString.Length - 2);
            else return currentString;
        }

        private string HistoryStringBuilder(string newChange, string currentString)
        {
            return currentString + newChange;
        }



        private void SaveHistory(string ChangeDescription, int TicketId)
        {
            TicketHistory CurrentHistoryItem = new TicketHistory();
            CurrentHistoryItem.Description = ChangeDescription;
            Ticket historyTicket = db.Ticket.Find(TicketId);

            CurrentHistoryItem.TicketId = TicketId;
            CurrentHistoryItem.HistoryCreateDate = DateTime.Now;
            CurrentHistoryItem.HistoryUpdatedDate = DateTime.Now;            
            //CurrentHistoryItem.Project = historyTicket.Project;            
            //CurrentHistoryItem.Ticket = historyTicket;
            //CurrentHistoryItem.Priority = historyTicket.Priority.PriorityLevel;
            //CurrentHistoryItem.TicketType = historyTicket.TicketType.TypeDescription;
            //CurrentHistoryItem.Status = historyTicket.Status.StatusDescription;

            db.TicketHistory.Add(CurrentHistoryItem);
            db.SaveChanges();          

        }

        private void sendEmail(string userId, string subject, string body)
        {            
           UserManager.SendEmail(userId, subject, body);
        }
    
        /* ==================================================  Garbage Collection ===================================================== */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
