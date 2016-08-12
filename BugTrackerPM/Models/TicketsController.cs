using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPM.Models
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var ticket = db.Ticket.Include(t => t.Assigned).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Submitter).Include(t => t.TicketType);
            return View(ticket.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
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

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.AssignedId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription");
            ViewBag.SubmitterId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SubmitterId,AssignedId,ProjectId,PriorityId,TicketTypeId,StatusId,Description,CreateDate,UpdatedDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.AssignedId);
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
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
            ViewBag.AssignedId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.AssignedId);
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SubmitterId,AssignedId,ProjectId,PriorityId,TicketTypeId,StatusId,Description,CreateDate,UpdatedDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.AssignedId);
            ViewBag.PriorityId = new SelectList(db.Prioritiy, "Id", "PriorityLevel", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectTitle", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusDescription", ticket.StatusId);
            ViewBag.SubmitterId = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "TypeDescription", ticket.TicketTypeId);
            return View(ticket);
        }

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
