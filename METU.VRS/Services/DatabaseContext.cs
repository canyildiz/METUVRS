using METU.VRS.Models;
using METU.VRS.Models.Interface;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;

namespace METU.VRS.Services
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCategory> UserCategories { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<BranchAffiliate> BranchsAffiliates { get; set; }
        public virtual DbSet<Quota> Quotas { get; set; }
        public virtual DbSet<StickerApplication> StickerApplications { get; set; }
        public virtual DbSet<StickerTerm> StickerTerms { get; set; }
        public virtual DbSet<StickerType> StickerTypes { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Sticker> Stickers { get; set; }

        public DatabaseContext() : this(new SqlConnection()) { }

        public DatabaseContext(SqlConnection conn) : base(conn, true)
        {

#if DEBUG
            conn.ConnectionString = WebConfigurationManager.ConnectionStrings["DbConnectionTest"].ConnectionString;
#else
            conn.ConnectionString = WebConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
#endif
            if (conn.DataSource != "(localdb)\\MSSQLLocalDB")
            {
                conn.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
            }

            Database.SetInitializer<DatabaseContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public override int SaveChanges()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.CurrentValues.PropertyNames.Contains("ID")
                    && item.State == EntityState.Added
                    && item.CurrentValues.GetValue<int>("ID") > 0)
                {
                    item.State = EntityState.Unchanged;
                }

                if (item.CurrentValues.PropertyNames.Contains("LastModified")
                    && item.State == EntityState.Modified)
                {
                    ((ILastModified)item.Entity).LastModified = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
