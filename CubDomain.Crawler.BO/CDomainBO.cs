using System;
namespace CubDomain.Crawler.BO
{
    [System.ComponentModel.DataAnnotations.Schema.Table("CDomain")]
    public partial class CDomainEO : IBaseBO
    {
        [System.ComponentModel.DataAnnotations.Key]

        public Guid CDomainId { get; set; }
		
        public string Domain { get; set; }
		
        public DateTime RegisterDate { get; set; }
		
        public string Extention { get; set; }
		

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Guid ID
        {
            get
            {
                return this.CDomainId;
            }
            set
            {
                this.CDomainId = value;
            }
        }

        public Guid InsertUserAccountId
        {
            get;
            set;
        }

        public DateTime InsertDateTime
        {
            get;
            set;
        }

        public Guid UpdateUserAccountId
        {
            get;
            set;
        }

        public DateTime UpdateDateTime
        {
            get;
            set;
        }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public byte Version
        {
            get;
            set;
        }
    }
}
