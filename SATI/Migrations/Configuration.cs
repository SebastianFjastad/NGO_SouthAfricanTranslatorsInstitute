using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SATI.Entities;
using SATI.Models;

namespace SATI.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(Context.ApplicationDbContext context)
        {
            //#region Skills
            context.Skills.AddOrUpdate(s => s.Name,
                new Skill { Name = "Copywriting", IsTransitive = false },
                new Skill { Name = "Editing", IsTransitive = false },
                new Skill { Name = "Indexing", IsTransitive = true },
                new Skill { Name = "Interpreting - Consecutive", IsTransitive = true },
                new Skill { Name = "Interpreting - Court", IsTransitive = false },
                new Skill { Name = "Interpreting - Liaison", IsTransitive = true },
                new Skill { Name = "Interpreting - Simultaneous", IsTransitive = true },
                new Skill { Name = "Language Teaching", IsTransitive = false },
                new Skill { Name = "Layout", IsTransitive = false },
                new Skill { Name = "Mentorship", IsTransitive = false },
                new Skill { Name = "Proofreading", IsTransitive = false },
                new Skill { Name = "Sworn Translation", IsTransitive = true },
                new Skill { Name = "Sworn Translation - Germany", IsTransitive = true },
                new Skill { Name = "Sworn Translation - Mozambique", IsTransitive = true },
                new Skill { Name = "Sworn Translation - Spain", IsTransitive = true },
                new Skill { Name = "Terminology", IsTransitive = false },
                new Skill { Name = "Transcription", IsTransitive = false },
                new Skill { Name = "Translation", IsTransitive = true },
                new Skill { Name = "Transcreation", IsTransitive = true }
                );

            //#endregion

            //#region Accreditations
            //context.Accreditations.AddOrUpdate(a => a.Name,
            //    new Accreditation { Name = "Interpreting - AIIC" },
            //    new Accreditation { Name = "Interpreting - Consecutive" },
            //    new Accreditation { Name = "Interpreting - Liaison" },
            //    new Accreditation { Name = "Interpreting - Simultaneous" },
            //    new Accreditation { Name = "Language Editing" },
            //    new Accreditation { Name = "Terminology" },
            //    new Accreditation { Name = "Translation - ATA" },
            //    new Accreditation { Name = "Translation - General" },
            //    new Accreditation { Name = "Translation - Sworn" });
            //#endregion

            //#region Payment Methods
            //context.PaymentMethods.AddOrUpdate(p => p.Name,
            //new PaymentMethod { Name = "EFT" },
            //new PaymentMethod { Name = "Credit Card" },
            //new PaymentMethod { Name = "Cash" },
            //new PaymentMethod { Name = "Cheque" },
            //new PaymentMethod { Name = "Paypal" },
            //new PaymentMethod { Name = "Crypto Currency" }
            //);
            //#endregion

            //#region Countries

            //context.Countries.AddOrUpdate(c => c.CountryId,
            //    new Country { Name = "South Africa", CountryId = 1 },
            //    new Country { Name = "Spain", CountryId = 2 },
            //    new Country { Name = "England", CountryId = 3 },
            //    new Country { Name = "Germany", CountryId = 4 },
            //    new Country { Name = "Sweden", CountryId = 5 }
            //    );

            //#endregion

            //#region Titles
            //context.Titles.AddOrUpdate(t => t.Name,
            //    new Title { Name = "Mr", TitleId = 1 },
            //    new Title { Name = "Ms", TitleId = 2 },
            //    new Title { Name = "Mrs", TitleId = 3 },
            //    new Title { Name = "Miss", TitleId = 4 },
            //    new Title { Name = "Mnr", TitleId = 5 },
            //    new Title { Name = "Me", TitleId = 6 },
            //    new Title { Name = "Mej", TitleId = 7 },
            //    new Title { Name = "Mev", TitleId = 8 },
            //    new Title { Name = "Dr", TitleId = 9 },
            //    new Title { Name = "Prof", TitleId = 10 },
            //    new Title { Name = "Lt Kol", TitleId = 11 }
            //    );
            //#endregion

            #region Roles
            var store = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(store);

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }
            #endregion

            #region Admin User
            var manager = new UserManager<User>(new UserStore<User>(context));
            var adminExists = manager.FindByEmail("admin@admin.com");
            if (adminExists == null)
            {
                var user = new User
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Title = "Mrs",
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                    PhoneNumber = "11223344",
                    EmailConfirmed = true,
                    MemberType = MemberType.Company,
                    CountryId = 1
                };
                manager.Create(user, "admin123");
                manager.AddToRole(user.Id, "Admin");
            }
            #endregion 
        }
    }
}
