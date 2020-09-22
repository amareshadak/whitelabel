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
        public long ID { get; set; }
        public long MEM_ID { get; set; }
        public string EASEBUZZ_RESPONSE { get; set; }
        public DateTime? EXECUTE_DATE { get; set; }
        public string STATUS { get; set; }
    }
}
