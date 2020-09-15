using Microsoft.EntityFrameworkCore;
using ServiceChannel.Model;

namespace ServiceChannel
{
    public class Covid19Context : DbContext
    {
        public Covid19Context(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region ConfigureCounty
            modelBuilder.Entity<County>(
                b =>
                {
                    b.Property("ID");
                    b.HasKey("ID");
                    b.Property(e => e.Name);                    
                    b.Property(e => e.Iso2);
                    b.Property(e => e.Iso3);
                    b.Property(e => e.FIPS);
                    b.Property(e => e.Lat);
                    b.Property(e => e.Long);
                    b.Property(e => e.State);
                    b.Property(e => e.UID);
                    b.Property(e => e.Country);
                    b.Property(e => e.Combined_Key);
                    b.Property(e => e.Code3);
                    b.HasMany(e => e.Infections).WithOne().IsRequired();
                });
            #endregion

            #region ConfigureInfections
            modelBuilder.Entity<Infections>(
                b =>
                {
                    b.Property("ID");
                    b.HasKey("ID");                    
                    b.Property(e => e.Date);
                    b.Property(e => e.Count);
                });
            #endregion

            modelBuilder.Seed();
        }
    }
}