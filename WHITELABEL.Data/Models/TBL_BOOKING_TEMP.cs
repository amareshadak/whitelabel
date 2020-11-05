namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("BOOKING_TEMP")]
    public class TBL_BOOKING_TEMP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string TRN_ID { get; set; }
        public string PNR_NO { get; set; }
        public string CLIENT_TXN_ID { get; set; }
        public decimal BOOKING_AMT { get; set; }
        public string PG_NAME { get; set; }
        public string PNR_CLASS { get; set; }
        public DateTime? TRN_DATE { get; set; }
        public string USER_ID { get; set; }
        public bool? STATUS { get; set; }
        public DateTime? SYSTEM_UPDATE_TIME { get; set; }
    }
}
