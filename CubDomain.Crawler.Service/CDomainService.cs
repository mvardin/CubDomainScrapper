using CubDomain.Crawler.Data;
using CubDomain.Crawler.Data;
using CubDomain.Crawler.BO;
using System.Collections.Generic;
using System;

namespace CubDomain.Crawler.Service
{
    public class CDomainService : GenericService<CDomainEO>
    {
        public CDomainService()
            : base(new CContext())
        {

        }
        public int SaveDomains(string[] domains, DateTime registerDate)
        {
            return new CDomainData().SaveDomains(domains, registerDate);
        }
    }
}