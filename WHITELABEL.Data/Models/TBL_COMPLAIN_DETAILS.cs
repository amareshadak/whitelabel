namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("COMPLAIN_DETAILS")]
    public  class TBL_COMPLAIN_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public long DIST_ID { get; set; }
        public long WLP_ID { get; set; }
        public string COMPLAIN_TYPE { get; set; }
        [Required]
        [Display(Name = "Complain Description")]
        public string COMPLAIN_DETAILS { get; set; }
        public DateTime? COMPLAIN_DATE { get; set; }
        public bool? COMPLAIN_STATUS { get; set; }
        public string REPLY_DETAILS { get; set; }
        public long? REPLY_ID { get; set; }
        public DateTime? REPLY_DATE { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
        [NotMapped]
        public string Member_Name { get; set; }
        [NotMapped]
        public string Member_Company { get; set; }
        [NotMapped]
        public string Member_Company_GST { get; set; }
        [NotMapped]
        public long Serial_No { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        
    }
}
