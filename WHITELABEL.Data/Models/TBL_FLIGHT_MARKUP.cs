namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("FLIGHT_MARKUP")]
    public class TBL_FLIGHT_MARKUP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public long ASSIGN_BY { get; set; }
        [Required]
        [Display(Name = "Internation MarkUp")]
        [Range(typeof(Decimal), "0", "9999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal INTERNATIONAL_MARKUP { get; set; }
        [Required]
        [Display(Name = "Domestic MarkUp")]
        [Range(typeof(Decimal), "0", "9999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal DOMESTIC_MARKUP { get; set; }
        public DateTime? ASSIGN_DATE { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public int STATUS { get; set; }
        public string ASSIGN_TYPE { get; set; }
        public long DIST_ID { get; set; }
        [NotMapped]
        public string MEMBER_UNIQUE_ID { get; set; }
        [NotMapped]
        public string MEMBER_NAME { get; set; }
        [NotMapped]
        public string MEMBER_MOBILE { get; set; }
        [NotMapped]
        public string MEMBER_EMAIL { get; set; }
        [NotMapped]
        public string MEMBER_COMPANY { get; set; }
        [NotMapped]
        public string MEMBER_ROLE { get; set; }
        [NotMapped]
        public string MEMBER_ADDRESS { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public long Serial_No { get; set; }
        [NotMapped]
        public string DIST_NAME { get; set; }
        [NotMapped]
        public string DIST_MEM_ID { get; set; }
    }
}
