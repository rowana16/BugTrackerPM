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

namespace BugTrackerPM.Models
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /* ==================================================  Dashboard  ===================================================== */
        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            TicketViewModel ViewModel = new TicketViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);

            if (helper.IsUserInRole(currentUserId, "Admin")) {
                var tickets = db.Ticket.Include(t => t.Assigned).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Submitter).Include(t => t.TicketType);
                ViewModel.Ticket = tickets.ToList();
                ViewModel.user = currentUser;
                return View(ViewModel);
            }

            if (helper.IsUserInRole(currentUserId, "ProjectManager"))
            {
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

                var projectTickets = new List<Ticket>();
                var assignedTickets = new List<Ticket>();

                var projects = currentUser.Projects;
                foreach (Project p in projects)
                {
                    foreach (Ticket t in p.Tickets)
                    {
                        projectTickets.Add(t);
                    }
                }

                assignedTickets = db.Ticket.Where(i => i.AssignedId == currentUserId).ToList();
                IEnumerable<Ticket> tickets = projectTickets.Union(assignedTickets);

                //var assignedProjects = db.Projects.Where(p => p.Users.Contains(currentUser));
                //var projectTickets = from t in db.Ticket
                //                     join p in assignedProjects on t.ProjectId equals p.Id                                     
                //                        select t;

                ViewModel.Ticket = tickets;
                return View(ViewModel);
            }

            if (helper.IsUserInRole(currentUserId, "Submitter"))
            {
                var tickets = db.Ticket.Where(i => i.SubmitterId == currentUserId);
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

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticket = db.Ticket.Find(id);
            ViewModel.ticket = ticket;
            ViewModel.ticketComments = ticket.TicketComments;
            ViewModel.ticketAttachments = ticket.TicketAttachments;

            if (ticket == null)
            {
                return HttpNotFound();
            }



            return View(ViewModel);

        }

        /* ==================================================  Create Ticket Get ===================================================== */

        // GET: Tickets/Create
        public ActionResult Create()
        {

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
        public ActionResult Create(int PriorityId, int TicketTypeId, string Description, Ticket ticket)
        {

            ticket.SubmitterId = User.Identity.GetUserId();
            ticket.AssignedId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3"; //Admin User Id
            ticket.ProjectId = 1;
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
                return RedirectToAction("Index");
            }

            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }
        /* ================================================== Edit Tickets Get ===================================================== */

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        public ActionResult Edit(int? id)
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
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.Find(currentUserId);
            UserRolesHelper helper = new UserRolesHelper(db);

            if (!(ticket.AssignedId == currentUserId || ticket.Project.Users.Contains(currentUser) || ticket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
            {
                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'> alert('You do Not Have Access To This Ticket')</Script>");
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


        /* ==================================================  Edit Tickets Post ===================================================== */

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SubmitterId,AssignedId,ProjectId,PriorityId,TicketTypeId,StatusId,Description,CreateDate,UpdatedDate")] Ticket ticket)
        {

            //ticket.CreateDate = DateTime.Now;
            ticket.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
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


            if (!(currentTicket.AssignedId == currentUserId || currentTicket.SubmitterId == currentUserId || helper.IsUserInRole(User.Identity.GetUserId(), "Admin")))
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
                return RedirectToAction("Details", new { id = currentAttachment.TicketId });
            }

            TicketAttachmentViewModel viewModel = new TicketAttachmentViewModel();
            viewModel.attachment = currentAttachment;
            Ticket currentTicket = db.Ticket.Find(currentAttachment.TicketId);
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
                CreateHistory(4, 2, currentTicket.Id);
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
            string history = "Ticket Created";
            if (history != "")
            {
                history = history.Substring(0, history.Length - 2);
                SaveHistory(history, ticketId);
            }

        }

        public void CreateHistory(bool assigned, bool type, bool status, bool description, int ticketId)
        {
            string history = "";
            if (assigned) { HistoryStringBuilder("Assigned Changed, ", history); }
            if (type) { HistoryStringBuilder("Type Changed, ", history); }
            if(status) { HistoryStringBuilder("Status Changed, ", history); }
            if(description) { HistoryStringBuilder("Description Changed, ", history); }

            if (history != "")
            {
                history = history.Substring(0, history.Length - 2);
                SaveHistory(history, ticketId);
            }



        }
        public void CreateHistory(int historyType, int historySubType, int ticketId)
        {
            string history = "";
            if (historyType == 3)
            {
                if (historySubType == 1) { HistoryStringBuilder("Comment Created", history); }
                if (historySubType == 2) { HistoryStringBuilder("Comment Edited", history); }
               
            }

            if (historyType == 4)
            {
                if (historySubType == 1) { HistoryStringBuilder("Attachment Created", history); }
                if (historySubType == 2) { HistoryStringBuilder("Attachment Edited", history); }
                           
            }

            if (history != "")
            {
                history = history.Substring(0, history.Length - 2);
                SaveHistory(history, ticketId);
            }
        }

        private string HistoryStringBuilder(string newChange, string currentString)
        {
            return currentString + newChange;

        }

        private void SaveHistory(string ChangeDescription, int TicketId)
        {
            TicketHistory CurrentHistoryItem = new TicketHistory();
            CurrentHistoryItem.Description = ChangeDescription;
            CurrentHistoryItem.TicketId = TicketId;
            CurrentHistoryItem.HistoryCreateDate = DateTime.Now;
            CurrentHistoryItem.HistoryUpdatedDate = DateTime.Now;

            db.TicketHistory.Add(CurrentHistoryItem);
            db.SaveChangesAsync();          

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
