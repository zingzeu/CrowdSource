using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CrowdSource.Models;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Use JSONB data type in PostgreSQL
            builder.Entity<Group>()
            .Property(b => b.GroupMetadata)
            .ForNpgsqlHasColumnType("jsonb");

            builder.Entity<Suggestion>()
            .Property(b => b.Content)
            .ForNpgsqlHasColumnType("jsonb");
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<GroupVersion> GroupVersions { get; set; }
        public DbSet<GroupVersionRefersSuggestion> GVSuggestions { get; set; }
        public DbSet<ApplicationUserEndorsesGroupVersion> Reviews { get; set; }
    }
}
