using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chamberlain.AppServer.Api.Endpoint.Models
{
    #region

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    #endregion

    /// <summary>
    /// The class application datebase context
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool IsTest = false;

        /// <summary>
        /// 
        /// </summary>
        public ApplicationDbContext() : base(GenerateOptions())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(GenerateOptions(options))
        {
        }

        /// <summary>
        /// The on model creating
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        
        private static DbContextOptions<ApplicationDbContext> GenerateOptions()
        {
            return IsTest
                ? new DbContextOptionsBuilder<ApplicationDbContext>().UseLazyLoadingProxies().UseInMemoryDatabase("Domotica.IdentityTest").Options
                : new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        }
        
        private static DbContextOptions<ApplicationDbContext> GenerateOptions(DbContextOptions<ApplicationDbContext> options)
        {
            return IsTest
                ? new DbContextOptionsBuilder<ApplicationDbContext>().UseLazyLoadingProxies().UseInMemoryDatabase("Domotica.IdentityTest").Options
                : options;
        }

        /// <summary>
        /// The on configuring options builder
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=Domotica.Identity;Trusted_Connection=True;MultipleActiveResultSets=true");
                optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(CoreEventId.IncludeIgnoredWarning));
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

    }
}