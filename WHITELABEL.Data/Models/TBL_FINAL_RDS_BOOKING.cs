namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("FINAL_RDS_BOOKING")]
    public class TBL_FINAL_RDS_BOOKING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string BOOKING_GATEWAY { get; set; }
        public string TRAN_ID { get; set; }
        public string PNR { get; set; }
        public string OPR_ID { get; set; }
        public decimal BOOKING_AMT { get; set; }
        public decimal PG_CHARGE { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public string BOOKING_TRAN_STATUS { get; set; }
        public string TRAN_STATUS { get; set; }
        public DateTime BOOKING_TIME { get; set; }
        public string CURRENCY_TYPE { get; set; }
        public string APP_CODE { get; set; }
        public string PAYMODE { get; set; }
        public string SECURITY_ID { get; set; }
        public string RU { get; set; }
        public string PAY_REQ { get; set; }
        public string RET_RES { get; set; }
        public long WLP_ID { get; set; }
        public long DIST_ID { get; set; }
        public long MER_ID { get; set; }
        public string MER_RAIL_ID { get; set; }
        public long BOOKING_MER_ID { get; set; }
        public string BOOKING_MER_RAIL_ID { get; set; }
        public bool PG_CHARGE_APPLY { get; set; }
        public decimal PG_CHARGE_MAX_VAL { get; set; }
        public decimal PG_CHARGE_LESS_THAN_2000 { get; set; }
        public decimal PG_CHARGE_GREATER_THAN_2000 { get; set; }
        public bool PG_CHARGE_GST_APPLY { get; set; }
        public decimal PG_CHARGE_GST_VAL { get; set; }
        public bool ADDN_CHARGE_APPLY { get; set; }
        public decimal ADDN_CHARGE_MAX_VAL { get; set; }
        public decimal ADDN_CHARGE_AC { get; set; }
        public decimal ADDN_CHARGE_NON_AC { get; set; }
        public bool ADDN_CHARGE_GST_APPLY { get; set; }
        public decimal ADDN_CHARGE_GST_VAL { get; set; }
        public decimal TOTAL_NET_PAYBLE_WITHOUT_GST { get; set; }
        public decimal TOTAL_NET_PAYBLE_GST { get; set; }
        public decimal TOTAL_NET_PAYBLE { get; set; }
        public string CORRELATION_ID { get; set; }
        public decimal GST_RATE { get; set; }
        public string REMARKS { get; set; }
        public string NOTES { get; set; }
        public string IP_ADDRESS { get; set; }
        [NotMapped]
        public string MERCHANT_NAME { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
        [NotMapped]
        public long SerialNo { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public long SUPER_ID { get; set; }
        [NotMapped]
        public string Company_Name { get; set; }
        [NotMapped]
        public string Company_GST { get; set; }
    }
}
