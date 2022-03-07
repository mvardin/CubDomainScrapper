using System;

namespace CubDomain.Crawler.BO
{
    public interface IBaseBO
    {
        Guid ID { get; set; }
        Guid InsertUserAccountId { get; set; }
        DateTime InsertDateTime { get; set; }
        Guid UpdateUserAccountId { get; set; }
        DateTime UpdateDateTime { get; set; }
        byte Version { get; set; }
    }
}
