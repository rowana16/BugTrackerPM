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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize (Roles ="Admin")]
        public ActionResult Create()
        {
            //var projects = db.Projects.ToList();
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string ProjectTitle, Project project)
        {
            project.CreateDate = DateTime.Now;
            project.UpdateDate = DateTime.Now;
            project.ProjectTitle = ProjectTitle; 

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

/* ================================Edit Action ==============================================*/

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectEditViewModel EditViewModel = new ProjectEditViewModel();

            //////////// Get Project and All Users //////////////////////
            var userFilter = db.Users.ToList();
            EditViewModel.project = db.Projects.Find(id);

            //////////// Create List of Absent Users //////////////////////

            var absentUsersList = new List<ApplicationUser>();
            bool found = false;

            foreach (var filter in userFilter)
            {
                foreach (var projectUser in EditViewModel.project.Users)
                {
                
                    if(filter == projectUser)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    absentUsersList.Add(filter);
                }
                found = false;
            }
            EditViewModel.absentUserList = absentUsersList;

            //////////// Create Multi Select Lists //////////////////////
            IList<string> transfer = new List<string>();

            foreach (var i in absentUsersList)
            {
                transfer.Add(i.FirstName + " " + i.LastName);
            }

            EditViewModel.absentUsers = new MultiSelectList(transfer);

            IList<string> transfer2 = new List<string>();
            foreach (var i in EditViewModel.project.Users)
            {
                transfer2.Add(i.FirstName + " " + i.LastName);
            }
            EditViewModel.assignedUsers = new MultiSelectList(transfer2);



            if (EditViewModel.project == null)
            {
                return HttpNotFound();
            }
            return View(EditViewModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Project project)
        {
            //project.ProjectTitle = ProjectTitle;
            project.UpdateDate = DateTime.Now;

           


            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
