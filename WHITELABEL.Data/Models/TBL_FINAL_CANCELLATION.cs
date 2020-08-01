namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("FINAL_CANCELLATION")]
    public class TBL_FINAL_CANCELLATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string TRN_ID { get; set; }
        public string PNR_NO { get; set; }
        public string OPR_ID { get; set; }
        public string PNR_CLASS { get; set; }
        public decimal REFUND_AMT { get; set; }
        public string WT_AUTO_CAN { get; set; }
        public DateTime? TRN_DATE { get; set; }
        public DateTime? SYSTEM_DATE { get; set; }
        public string TDR_CAN { get; set; }
        public string CANCELLATION_TYPE { get; set; }
        public string CANCELLATION_ID { get; set; }
        public string MER_RAIL_ID { get; set; }
        public string CANCELLATION_AGST_MER_RAIL_ID { get; set; }
        public long MER_ID { get; set; }
        public long DIST_ID { get; set; }
        public long SUP_ID { get; set; }
        public long WLP_ID { get; set; }
        public bool PG_CHARGE_APPLY { get; set; }
        public decimal? PG_CHARGE_MAX_VAL { get; set; }
        public decimal? PG_CHARGE_LESS_THAN_2000 { get; set; }
        public decimal? PG_CHARGE_GREATER_THAN_2000 { get; set; }
        public bool PG_CHARGE_GST_APPLY { get; set; }
        public decimal? PG_CHARGE_GST_VAL { get; set; }
        public bool ADDN_CHARGE_APPLY { get; set; }
        public decimal? ADDN_CHARGE_MAX_VAL { get; set; }
        public decimal? ADDN_CHARGE_AC { get; set; }
        public decimal? ADDN_CHARGE_NON_AC { get; set; }
        public bool ADDN_CHARGE_GST_APPLY { get; set; }
        public decimal? ADDN_CHARGE_GST_VAL { get; set; }
        public decimal? TOTAL_NET_PAYBLE_WITHOUT_GST { get; set; }
        public decimal? TOTAL_NET_PAYBLE_GST { get; set; }
        public decimal? TOTAL_NET_PAYBLE { get; set; }
        public string CORRELATION_ID { get; set; }
        public decimal? GST_RATE { get; set; }
        public string REMARKS { get; set; }
        public string NOTES { get; set; }
        public string IP_ADDRESS { get; set; }
        [NotMapped]
        public string MERCHANT_NAME { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
    }
}
