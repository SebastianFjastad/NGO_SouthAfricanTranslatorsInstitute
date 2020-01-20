using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using SATI.Entities;
using SATI.Models;

namespace SATI.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("Name=DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>("DefaultConnection"));
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<SocialMediaLink> SocialMediaLinks { get; set; }
        public DbSet<UserSocialMediaLink> UserSocialMediaLinks { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Specialisation> Specialisations { get; set; }
        public DbSet<Accreditation> Accreditations { get; set; }
        public DbSet<Capability> Capabilities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentCategory> PaymentCategories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Title> Titles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public void SetDefaultPropertyAttributes(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<string>()
                        .Configure(s => s.HasMaxLength(200)
                        .HasColumnType("varchar"));
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}