using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using CubDomain.Crawler.BO;

namespace CubDomain.Crawler.Data
{
    public class CContext : DbContext
    {
        public CContext()
            : base("name=CContext")
        {
            this.Database.Initialize(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer<CContext>(null);
        }

		
			public virtual DbSet<CDomainEO> CDomains { get; set; }
		
    }
}
