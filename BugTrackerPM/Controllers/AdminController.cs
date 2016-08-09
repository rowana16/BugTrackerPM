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
        public ActionResult AssignUserRoles(string id)
        {
            var user = db.Users.Find(id);

            //instantiate AdminUserVieweModel
            AdminUserViewModel AdminModel = new AdminUserViewModel();

            //instantiate UserRolesHelper
            UserRolesHelper helper = new UserRolesHelper(db);
            //Set each property of the AdminUserViewModel with help of UserRolesHelper

            var currentRoles = helper.ListUserRoles(id);
            var absentRoles = helper.ListAbsentUserRoles(id);
            AdminModel.SelectedAbsentRoles = new MultiSelectList(absentRoles);
            AdminModel.SelectedCurrentRoles = new MultiSelectList(currentRoles);
            AdminModel.id = user.Id;
            AdminModel.name = user.FirstName + " " + user.LastName;

            return View(AdminModel);



        }
    }
}