namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RAIL_ID_ALLOCATION")]
    public class TBL_RAIL_ID_ALLOCATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long SELLER_ID { get; set; }
        [Required(ErrorMessage = "Buyer name is required")]
        public long BUYER_ID { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Required(ErrorMessage = "Rail Id quantity is required")]
        public int RAIL_ID_QUANTITY { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(Decimal), "0", "999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal RAIL_ID_SELLING_RATE { get; set; }
        public decimal GROSS_AMOUNT { get; set; }
        public decimal GST_RATE { get; set; }
        public decimal GST_AMOUNT { get; set; }
        public DateTime SALE_DATE { get; set; }
        public bool STATUS { get; set; }
        public string CORELATIONID { get; set; }
        public DateTime SYSTEM_DATE { get; set; }
        [NotMapped]
        public string GSTApplied { get; set; }
        [NotMapped]
        public string WLPUserName { get; set; }
    }
}
