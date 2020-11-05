namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("CANCELLATION_TEMP")]
    public class TBL_CANCELLATION_TEMP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string TRN_ID { get; set; }
        public string PNR_NO { get; set; }
        public string OPER_ID { get; set; }
        public string PNR_CLASS { get; set; }
        public Decimal REFUND_AMOUNT { get; set; }
        public string WAITING_AUTO_CANCELLED { get; set; }
        public DateTime? TRN_DATE { get; set; }
        public DateTime? ACTUAL_REFUND_DATE { get; set; }
        public string TDR_CAN { get; set; }
        public string USER_ID { get; set; }
        public string CANCELLATION_ID { get; set; }
        public DateTime? SYSTEM_UPDATED_DATE { get; set; }
    }
}
