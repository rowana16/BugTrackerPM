namespace BugTrackerPM.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTrackerPM.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTrackerPM.Models.ApplicationDbContext context)
        {
    //==============================  Roles  ===========================================================
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));


            //==============================  Admin  ===========================================================
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }


            //==============================  ProjectManager  ===========================================================
            new RoleStore<IdentityRole>(context);
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }

            //==============================  Developer  ===========================================================
            new RoleStore<IdentityRole>(context);
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            //==============================  Submitter  ===========================================================
            new RoleStore<IdentityRole>(context);
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

    //==============================  Seed Users  ===========================================================
            

            var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "aaron.rowan@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "aaron.rowan@gmail.com",
                    Email = "aaron.rowan@gmail.com",
                    FirstName = "Aaron",
                    LastName = "Rowan",
                    DisplayName = "Aaron Rowan"
                }, "CFdb100!");
            }

            if (!context.Users.Any(u => u.Email == "jtwichell@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "jtwichell@coderfoundry.com",
                    Email = "jtwichell@coderfoundry.com",
                    FirstName = "Jason",
                    LastName = "Twichell",
                    DisplayName = "J-Twich"
                }, "Abc&123!");
            }

            var userId = userManager.FindByEmail("aaron.rowan@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("jtwichell@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Projectmanager");
        }
    }
}
