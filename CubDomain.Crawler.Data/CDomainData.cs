using CubDomain.Crawler.BO;
using System;
using System.Collections.Generic;
using System.IO;

namespace CubDomain.Crawler.Data
{
    public class CDomainData : GenericRepository<CDomainEO>
    {
        public int SaveDomains(string[] domains, DateTime registerDate)
        {
            this.context = new CContext();
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            foreach (var domain in domains)
            {
                string domainString = domain.Trim().ToLower();
                CDomainEO domainEO = new CDomainEO();
                domainEO.Domain = domainString;
                domainEO.Extention = Path.GetExtension(domainString);
                domainEO.RegisterDate = registerDate;
                domainEO.CDomainId = Guid.NewGuid();
                domainEO.InsertDateTime = DateTime.Now;
                domainEO.InsertUserAccountId = Guid.Empty;
                domainEO.UpdateDateTime = DateTime.Now;
                domainEO.UpdateUserAccountId = Guid.Empty;
                context.CDomains.Add(domainEO);
            }
            context.SaveChanges();
            return domains.Length;
        }
    }
}
