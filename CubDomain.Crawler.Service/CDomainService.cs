using CubDomain.Crawler.Data;
using CubDomain.Crawler.Data;
using CubDomain.Crawler.BO;

namespace CubDomain.Crawler.Service
{
    public class CDomainService : GenericService<CDomainEO>
    {
        public CDomainService()
            : base(new CContext())
        {

        }
        public int SaveDomains(string query)
        {
            return new CDomainData().SaveDomains(query);
        }
    }
}