namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("PAYMENT_GATEWAY_RESPONSE")]
    public class TBL_PAYMENT_GATEWAY_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public string RES_MSG { get; set; }
        public DateTime? RES_DATE { get; set; }
        public string RES_STATUS { get; set; }
        public string PAY_REF_NO { get; set; }
        public string CORELATION_ID { get; set; }
        public string EMAIL_ID { get; set; }
        public string MOBILE_No { get; set; }
        public decimal? TRANSACTION_AMOUNT { get; set; }
        public string RES_CODE { get; set; }
        public string TRANSACTION_DETAILS { get; set; }
        [NotMapped]
        public string TrnDate { get; set; }
        [NotMapped]
        public decimal Amount { get; set; }
        [NotMapped]
        public string Member_Name { get; set; }
        [NotMapped]
        public string Member_Company_Name { get; set; }
        [NotMapped]
        public long Serial_No { get; set; }
    }
}
