namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RAIL_DSC_INFORMATION")]
    public class TBL_RAIL_DSC_INFORMATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string RAIL_DSC_ID { get; set; }
        public long MEM_ID { get; set; }
        public string RAIL_USER_ID { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public bool STATUS { get; set; }
        public string DSC_DOC_Path { get; set; }
        [NotMapped]
        public string MerchantName { get; set; }
        [NotMapped]
        public string DistributorName { get; set; }
    }
}
