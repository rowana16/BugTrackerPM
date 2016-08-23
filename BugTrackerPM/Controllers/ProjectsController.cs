using BugTrackerPM.Helpers;
using Microsoft.AspNet.Identity;
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
        

        /* ==============================================  Get Dashboard List ===================================*/
        // GET: Projects
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        public ActionResult Index()
        {
            ProjectIndexViewModel passModel = new ProjectIndexViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);

            //Get list of User Roles
            string userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var roles = helper.ListUserRoles(userId);


            //if UserRole includes "Admin"
            foreach (string i in roles)
            {
                if (i == "Admin")
                {
                    //Return ALL Projects
                    passModel = HelperBuildViewModel(db.Projects.ToList());
                    passModel.loggedInUser = user;
                    return View(passModel);
                }
            }

            //Otherwise Return just assigned Projects
            //Build View Model            
            passModel = HelperBuildViewModel(user.Projects.ToList());
            passModel.loggedInUser = user;

            return View(passModel);
        }

        /* ==============================================  Get Dashboard Helpers ===================================*/
        public ProjectIndexViewModel HelperBuildViewModel (List<Project> ProjectList)
        {
            ProjectIndexViewModel CreatedModel = new ProjectIndexViewModel();
            if (ProjectList.Count > 0)
            {
                
                foreach (Project currentProject in ProjectList)
                {
                    ProjectListElement individualProject = new ProjectListElement();
                    individualProject.project = currentProject;
                    individualProject.userList = HelperUserList(currentProject);
                    CreatedModel.currentProjects.Add(individualProject);
                    
                }
            }

            else
            {
                ProjectListElement individualProject = new ProjectListElement();
                Project BlankProject = new Project();

                individualProject.project = BlankProject;
                individualProject.userList = "";
                CreatedModel.currentProjects.Add(individualProject);
            }

            return CreatedModel;
        }

        public string HelperUserList (Project project)
        {
            string passString = ""; 
            foreach(ApplicationUser user in project.Users)
            {
                passString += user.DisplayName + ", ";
            }
            if (passString != "") { passString = passString.Substring(0, passString.Length - 2); }
            return passString;
        }

/* ==============================================  Details Get ===================================*/


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

        /* ==============================================  Create Get ===================================*/
        // GET: Projects/Create
        [Authorize (Roles ="Admin")]
        public ActionResult Create()
        {
            //var projects = db.Projects.ToList();
            return View();
        }

        /* ==============================================  Create Post ===================================*/
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
        [Authorize]
        public ActionResult Edit(int? id)
        {
            string loggedInUserId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectEditViewModel EditViewModel = new ProjectEditViewModel();

            //////////// Get Project and All Users //////////////////////
            var allUsers = db.Users.ToList();
            EditViewModel.project = db.Projects.Find(id);

            foreach(ApplicationUser checkUser in db.Projects.Find(id).Users)
            {
                if(checkUser.Id == loggedInUserId)
                {
                    var assignedUsers = EditViewModel.project.Users;

                    //////////// Create List of Absent Users //////////////////////

                    var absentUsersList = new List<ApplicationUser>();
                    bool found = false;

                    foreach (var filter in allUsers)
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
                    EditViewModel.absentUsers = new MultiSelectList(absentUsersList, "Id", "DisplayName");
                    EditViewModel.assignedUsers = new MultiSelectList(assignedUsers, "Id", "DisplayName");

                    if (EditViewModel.project == null)
                    {
                        return HttpNotFound();
                    }
                    return View(EditViewModel);
                }
            } // end user find

            return RedirectToAction("index");
        }

      /*========================================== Edit Post Functions ======================================================== */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult AddAssignment( int? id, List<string> SelectedAbsentAssignments)
        {
            //project.ProjectTitle = ProjectTitle;
            Project project = db.Projects.Find(id);
            project.UpdateDate = DateTime.Now;

            if (SelectedAbsentAssignments == null)
            {
                return RedirectToAction("Edit", "Projects", new { id = id });
            }


            //Get Users from userIds 
            ICollection<ApplicationUser> transfer = new List<ApplicationUser>();

           foreach(string i in SelectedAbsentAssignments)
            {
                transfer.Add(db.Users.Find(i));    
            }

            //Add userIds to project
            project.Users = transfer;

            
            
            //Save project
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "Projects", new { id = id });
            }
            return RedirectToAction("Edit", "Projects" ,new { id = id }) ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult RemoveAssignment ( int? id, List<string> SelectedCurrentAssignments)
        {
            Project project = db.Projects.Find(id);
            project.UpdateDate = DateTime.Now;
            
            if(SelectedCurrentAssignments == null)
            {
                return RedirectToAction("Edit", "Projects", new { id = id });
            }

            //Get Users from userIds
            ICollection<ApplicationUser> remove = new List<ApplicationUser>();

            foreach (string i in SelectedCurrentAssignments)
            {
                remove.Add(db.Users.Find(i));
            }

            // remove users from project
            ICollection<ApplicationUser> remain = new List<ApplicationUser>();
            bool found = new bool();

            foreach (ApplicationUser p in project.Users)
            {
                foreach(ApplicationUser r in remove)
                {
                    if (r==p)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    remain.Add(p);
                }
                found = false;
            }

            project.Users = remain;
            //Save project
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "Projects", new { id = id });
            }

            return RedirectToAction("Edit", "Projects", new { id = id });
        }

        /*========================================== Delete Functions ======================================================== */
        // GET: Projects/Delete/5
        
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
