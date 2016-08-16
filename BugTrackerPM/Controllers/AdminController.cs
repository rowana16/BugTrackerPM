using BugTrackerPM.Helpers;
using BugTrackerPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTrackerPM.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

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

            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            
            AdminModel.absentRoles = new MultiSelectList(absentRoles);
            AdminModel.roles = new MultiSelectList(currentRoles);
            return View(AdminModel);
        }

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

        public ActionResult Index()
        {
            //AdminDashboardViewModel AdminModel = new AdminDashboardViewModel();
            var users = db.Users.ToList();
            //List<string> tempIds = new List<string>();
            //List<string> tempNames = new List<string>();
            ////string[] names = new string[];

            //foreach (var user in users)
            //{
            //   tempIds.Add(user.Id);
            //   tempNames.Add(user.FirstName + " " + user.LastName);
            //}

            //AdminModel.ids = tempIds;
            //AdminModel.names = tempNames;

            return View(users);
        }

       

    }
}