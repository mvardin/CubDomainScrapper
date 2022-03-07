using CubDomain.Crawler.BO;
using System;

namespace CubDomain.Crawler.Data
{
    public class CDomainData : GenericRepository<CDomainEO>
    {
        public int SaveDomains(string query)
        {
            this.context = new CContext();
            return this.context.Database.ExecuteSqlCommand(query);
        }
    }
}
