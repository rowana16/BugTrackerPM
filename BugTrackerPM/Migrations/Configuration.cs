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

            context.Status.AddOrUpdate( x => x.Id,
                new Status() { Id = 1, StatusDescription = "Created" },
                new Status() { Id = 2, StatusDescription = "Assigned" },
                new Status() { Id = 3, StatusDescription = "In Process" },
                new Status() { Id = 4, StatusDescription = "Review" },
                new Status() { Id = 5, StatusDescription = "Resolved" }
            );

            context.TicketType.AddOrUpdate(x => x.Id,
                new TicketType() { Id = 1, TypeDescription = "Feature Requests" },
                new TicketType() { Id = 2, TypeDescription = "Stories" },
                new TicketType() { Id = 3, TypeDescription = "Access Requests" },
                new TicketType() { Id = 4, TypeDescription = "Undetermined" },
                new TicketType() { Id = 5, TypeDescription = "Computer" },
                new TicketType() { Id = 6, TypeDescription = "Printer" },
                new TicketType() { Id = 7, TypeDescription = "Display" },
                new TicketType() { Id = 8, TypeDescription = "Phone" }
            );

            context.Prioritiy.AddOrUpdate(x => x.Id,
                new Priority() { Id = 1, PriorityLevel = "Low" },
                new Priority() { Id = 2, PriorityLevel = "Medium" },
                new Priority() { Id = 3, PriorityLevel = "High" },
                new Priority() { Id = 4, PriorityLevel = "Urgent" }
            );
            DateTime Date1 = new DateTime(2016, 8, 1);
            context.Projects.AddOrUpdate(x => x.Id,
                new Project() { Id = 1, ProjectTitle = "Pyramids of Giza", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 2, ProjectTitle = "Stonehenge", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 3, ProjectTitle = "Colossus", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 4, ProjectTitle = "Great Lighthouse of Alexandria", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 5, ProjectTitle = "Great Wall of China", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 6, ProjectTitle = "Hanging Gardens", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 7, ProjectTitle = "Parthenon", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 8, ProjectTitle = "Chichen Itza", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 9, ProjectTitle = "Hagia Sophia", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 10, ProjectTitle = "Notre Dame", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 11, ProjectTitle = "Sistine Chapel", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 12, ProjectTitle = "Taj Mahal", CreateDate = Date1, UpdateDate = Date1 },
                new Project() { Id = 13, ProjectTitle = "Eiffel Tower", CreateDate = Date1, UpdateDate = Date1 }
            );
            
            context.Ticket.AddOrUpdate(x => x.Id,
            new Ticket() { Id = 1, SubmitterId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", AssignedId = "23d93cac-62b3-4325-bb44-fea987295075", ProjectId = 3, PriorityId = 4, TicketTypeId = 4, StatusId = 5, Description = "Ticket 1", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 2, SubmitterId = "23c2dccf-846f-4a52-8f6c-36a6ac828c69", AssignedId = "23d93cac-62b3-4325-bb44-fea987295075", ProjectId = 5, PriorityId = 3, TicketTypeId = 5, StatusId = 5, Description = "Ticket 2", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 3, SubmitterId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", AssignedId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", ProjectId = 6, PriorityId = 3, TicketTypeId = 7, StatusId = 2, Description = "Ticket 3", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 4, SubmitterId = "23c2dccf-846f-4a52-8f6c-36a6ac828c69", AssignedId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", ProjectId = 10, PriorityId = 4, TicketTypeId = 8, StatusId = 3, Description = "Ticket 4", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 5, SubmitterId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", AssignedId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", ProjectId = 3, PriorityId = 3, TicketTypeId = 1, StatusId = 3, Description = "Ticket 5", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 6, SubmitterId = "23c2dccf-846f-4a52-8f6c-36a6ac828c69", AssignedId = "23c2dccf-846f-4a52-8f6c-36a6ac828c69", ProjectId = 10, PriorityId = 1, TicketTypeId = 3, StatusId = 4, Description = "Ticket 6", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 7, SubmitterId = "b7cfbdce-1c19-4b8d-a2b0-0f387d77a799", AssignedId = "b7cfbdce-1c19-4b8d-a2b0-0f387d77a799", ProjectId = 8, PriorityId = 4, TicketTypeId = 5, StatusId = 4, Description = "Ticket 7", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 8, SubmitterId = "23d93cac-62b3-4325-bb44-fea987295075", AssignedId = "23d93cac-62b3-4325-bb44-fea987295075", ProjectId = 5, PriorityId = 2, TicketTypeId = 3, StatusId = 1, Description = "Ticket 8", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 9, SubmitterId = "c61f0447-c501-40a9-ad4d-95566302be03", AssignedId = "91f7e362-4b06-4494-806a-31e1028ae44d", ProjectId = 2, PriorityId = 1, TicketTypeId = 4, StatusId = 3, Description = "Ticket 9", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 10, SubmitterId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", AssignedId = "5626de9b-b867-4d6c-ae48-873dd7341d17", ProjectId = 8, PriorityId = 4, TicketTypeId = 7, StatusId = 5, Description = "Ticket 10", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 11, SubmitterId = "5626de9b-b867-4d6c-ae48-873dd7341d17", AssignedId = "23d93cac-62b3-4325-bb44-fea987295075", ProjectId = 6, PriorityId = 1, TicketTypeId = 2, StatusId = 5, Description = "Ticket 11", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 12, SubmitterId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", AssignedId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", ProjectId = 2, PriorityId = 4, TicketTypeId = 1, StatusId = 3, Description = "Ticket 12", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 13, SubmitterId = "b7cfbdce-1c19-4b8d-a2b0-0f387d77a799", AssignedId = "c61f0447-c501-40a9-ad4d-95566302be03", ProjectId = 1, PriorityId = 2, TicketTypeId = 2, StatusId = 5, Description = "Ticket 13", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 14, SubmitterId = "23d93cac-62b3-4325-bb44-fea987295075", AssignedId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", ProjectId = 11, PriorityId = 3, TicketTypeId = 7, StatusId = 5, Description = "Ticket 14", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 15, SubmitterId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", AssignedId = "23c2dccf-846f-4a52-8f6c-36a6ac828c69", ProjectId = 4, PriorityId = 3, TicketTypeId = 8, StatusId = 4, Description = "Ticket 15", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 16, SubmitterId = "c61f0447-c501-40a9-ad4d-95566302be03", AssignedId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", ProjectId = 13, PriorityId = 1, TicketTypeId = 7, StatusId = 3, Description = "Ticket 16", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 17, SubmitterId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", AssignedId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", ProjectId = 8, PriorityId = 3, TicketTypeId = 4, StatusId = 1, Description = "Ticket 17", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 18, SubmitterId = "c61f0447-c501-40a9-ad4d-95566302be03", AssignedId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", ProjectId = 10, PriorityId = 2, TicketTypeId = 8, StatusId = 1, Description = "Ticket 18", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 19, SubmitterId = "b91bfb05-08ea-494c-92cb-7da8ad4085b3", AssignedId = "c61f0447-c501-40a9-ad4d-95566302be03", ProjectId = 3, PriorityId = 4, TicketTypeId = 8, StatusId = 5, Description = "Ticket 19", CreateDate = Date1, UpdatedDate = Date1 },
            new Ticket() { Id = 20, SubmitterId = "bf3f26f2-4f90-4ff2-9e47-c77dec1e31b7", AssignedId = "cc1236eb-23d2-42b5-ac7a-e21886a91d3d", ProjectId = 10, PriorityId = 2, TicketTypeId = 4, StatusId = 4, Description = "Ticket 20", CreateDate = Date1, UpdatedDate = Date1 }

             );



        }
    }
}
