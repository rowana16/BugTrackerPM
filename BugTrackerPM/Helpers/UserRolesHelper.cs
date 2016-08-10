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
            /*
             * var roles = roleManager.Roles.Where(r=> r.Name != null).select(r=> r.Name).ToList();
             * var AbsentuserRoles = new List<string>();
             * foreach(var role in roles)
             * {
             *  if(!IsUserInRole(userId, role))
             *  {
             *      Absent UserRoles.Add(role);
             *  }
             * }
             * return AbsentUserRoles;
             */


            string[] roles = new string[100];
            string[] currRoles = new string[100];
           // string[] absentRoles = new string[100];
            int iCount = 0;
            int iRoles = 0;


            //  Fill Current Roles Array for this User
            List<string> currentRoles = userManager.GetRoles(userId).ToList();
            foreach(string c in currentRoles)
            {
                currRoles[iCount] = c;
                iCount++;
            }

            iCount = 0;

            // Fill Array with All Roles Possible
            IList<IdentityRole> allRoleItems = roleManager.Roles.ToList();
            foreach (IdentityRole R in allRoleItems)
            {
                roles[iCount]= R.Name;
                iCount++;
            }
            iCount = 0;  
                
            // Go through list of currRoles and remove them from the roles list to develop list of Absent Roles
            foreach (string c in currRoles)
            {
                foreach (string r in roles)
                {
                    if (currRoles[iCount] == roles[iRoles])
                    {
                        roles[iRoles] = "";
                    }
                    iRoles++;
                }
                iRoles = 0;
                iCount++;
            }

            IList<string> absentRoles = roles;

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