using BugTrackerPM.Helpers;
using BugTrackerPM.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BugTrackerPM.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
//  ========================================  Edit Get ============================================
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);

            //instantiate AdminUserVieweModel
            AdminUserViewModel AdminModel = new AdminUserViewModel();

            //instantiate UserRolesHelper
            UserRolesHelper helper = new UserRolesHelper(db);

            //Set each property of the AdminUserViewModel with help of UserRolesHelper
            
            AdminModel.id = user.Id;
            AdminModel.name = user.FirstName + " " + user.LastName;
            AdminModel.userEdit = user;

            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            
            AdminModel.absentRoles = new MultiSelectList(absentRoles);
            AdminModel.roles = new MultiSelectList(currentRoles);
            return View(AdminModel);
        }

//  ========================================  Edit Post ============================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, string email, string name)
        {
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            ApplicationUser userEdit = db.Users.Find(id);
            if (name != null || name != "")
            {
                userEdit.DisplayName = name;
            }
            
            if(email != null || email != "")
            {
                userEdit.Email = email;
            }

            if (ModelState.IsValid)
            {
                db.Entry(userEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit");
        }
//  ========================================  Edit Post Special 1 ============================================
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(string Id, List<string> SelectedAbsentRoles)
        {
            if (SelectedAbsentRoles == null)
            {
                return RedirectToAction("Edit", "Admin", new { id = Id });
            }

            var helper = new UserRolesHelper(db);

            if (ModelState.IsValid)
            {
                foreach (string role in SelectedAbsentRoles)
                {
                    helper.AddUserToRole(Id, role);
                }
               
            }
            return RedirectToAction("Edit","Admin", new { id = Id });
        }

//  ========================================  Edit Post Special 2 ============================================
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveRole(string Id, List<string> SelectedCurrentRoles)
        {
            if (SelectedCurrentRoles == null)
            {
                return RedirectToAction("Edit", "Admin", new { id = Id });
            }


            var helper = new UserRolesHelper(db);

            if (ModelState.IsValid)
            {
                foreach (string role in SelectedCurrentRoles)
                {
                    helper.RemoveUserFromRole(Id, role);
                }

            }
            return RedirectToAction("Edit", "Admin", new { id = Id });
        }

        //  ========================================  User Dashboard ============================================
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            AdminCompositeViewModel compositeViewModel = new AdminCompositeViewModel();
            List<AdminIndexViewModel> viewModel = new List<AdminIndexViewModel>();
            //AdminIndexViewModel transferUser = new AdminIndexViewModel();

            List<ApplicationUser> users = db.Users.ToList();  
            string currentUserProjects = "";
            string currentUserRoles = "";

            UserRolesHelper helper = new UserRolesHelper(db);

            //Iterate through list of all users

            foreach (ApplicationUser currentUser in users)
            {
                AdminIndexViewModel transferUser = new AdminIndexViewModel();
                if (currentUser.Projects.Count > 0)
                {
                    foreach (Project currentProject in currentUser.Projects)                                 //Create a string of all projects and Roles
                    {
                        currentUserProjects += currentProject.ProjectTitle + ", ";
                    }
                    if (currentUserProjects != "") { currentUserProjects = currentUserProjects.Substring(0, currentUserProjects.Length - 2); }
                }

                transferUser.projectNameList = currentUserProjects;

                foreach(string currentRole in helper.ListUserRoles(currentUser.Id))
                {
                    currentUserRoles += currentRole + ", ";
                }


                // Build an instance of AdminIndexViewModel
                if (currentUserRoles != "") { currentUserRoles = currentUserRoles.Substring(0, currentUserRoles.Length - 2); }
                transferUser.roleNameList = currentUserRoles;
                transferUser.user = currentUser;

                // Add that instance the list of individual models
                viewModel.Add(transferUser);

                //Blank Transfer Info
                currentUserRoles = "";
                currentUserProjects = "";


            }

            compositeViewModel.details = viewModel;

            return View(compositeViewModel);
        }

        //  ========================================  Details Get ============================================
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            AdminIndexViewModel viewModel = new AdminIndexViewModel();
            ApplicationUser user = db.Users.Find(id);

            viewModel.user = user;

            return View(viewModel);
        }

        //  ========================================  Delete Get ============================================
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            AdminIndexViewModel viewModel = new AdminIndexViewModel();
            ApplicationUser user = db.Users.Find(id);

            viewModel.user = user;

            return View(viewModel);
        }

//  ========================================  Delete Post ============================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string Id, ApplicationUser user)
        {
            
            ApplicationUser userDelete = db.Users.Find(Id);
            if (userDelete != null)
            {

                db.Users.Remove(userDelete);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
           
        }

    }
}