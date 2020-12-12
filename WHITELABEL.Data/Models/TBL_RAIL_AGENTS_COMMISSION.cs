namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RAIL_AGENTS_COMMISSION")]
    public class TBL_RAIL_AGENTS_COMMISSION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN {  get; set; }
        public long WLP_ID { get; set; }
        public long DIST_ID { get; set; }
        public long MEM_ID { get; set; }
        public string RAIL_AGENT_ID { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]

        public decimal PG_MAX_VALUE { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal PG_EQUAL_LESS_2000 { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal PG_EQUAL_GREATER_2000 { get; set; }
        [Required]
        [Display(Name = "PG GST STATUS")]
        public string PG_GST_STATUS { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal ADDITIONAL_CHARGE_MAX_VAL { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal ADDITIONAL_CHARGE_AC { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal ADDITIONAL_CHARGE_NON_AC { get; set; }
        [Required]
        [Display(Name = "Additional GST STATUS")]
        public string ADDITIONAL_GST_STATUS { get; set; }
        public DateTime? COMM_ENTRY_DATE { get; set; }
        public DateTime? COMM_UPDATE_DATE { get; set; }
        public bool STATUS { get; set; }
        public bool PG_CHARGES_APPLY { get; set; }
        public bool ADDITIONAL_CHARGES_APPLY { get; set; }
        [NotMapped]
        public string WLP_NAME { get; set; }
        [NotMapped]
        public string DIST_NAME { get; set; }
        [NotMapped]
        public long Rail_table_Id { get; set; }
        [NotMapped]
        public string PG_Charges_Apply_Val { get; set; }
        [NotMapped]
        public string Additional_Charges_Apply_Val { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public long SUPER_ID { get; set; }
    }
}
