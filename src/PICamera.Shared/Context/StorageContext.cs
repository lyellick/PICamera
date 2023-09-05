using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PICamera.Shared.Models;

namespace PICamera.Shared.Context
{
    public class StorageContext : DbContext
    {
        public virtual DbSet<Configuration> Configurations { get; set; }

        protected StorageContext()
        {
        }

        public StorageContext(DbContextOptions<StorageContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
                builder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
