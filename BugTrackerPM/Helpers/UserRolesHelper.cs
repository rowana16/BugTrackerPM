using BugTrackerPM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerPM.Helpers
{
    public class UserRolesHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            this.db = context;

        }

        public bool IsUserInRole (string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public IList<string> ListAbsentUserRoles(string userId)
        {
            List<string> currentUserRoles = ListUserRoles(userId).ToList();
            IList<IdentityRole> allRoleItems = roleManager.Roles.ToList();
            List<string> absentRoles = new List<string>();
            bool found = false;

            foreach (IdentityRole role in allRoleItems)
            {
                foreach(string currentRole in currentUserRoles)
                {
                    if (role.Name == currentRole) { found = true; }
                }

                if (!found) { absentRoles.Add(role.Name); }
                found = false;
            }
           return (absentRoles);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var userIds = roleManager.FindByName(roleName).Users.Select(r => r.UserId);
            return userManager.Users.Where(u => userIds.Contains(u.Id)).ToList();
        }
    }
}