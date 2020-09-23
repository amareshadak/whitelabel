﻿namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BALANCE_TRANSFER_LOGS")]
    public class TBL_BALANCE_TRANSFER_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long TO_MEMBER { get; set; }
        public long FROM_MEMBER { get; set; }
        
        [Required]
        ////[Display(Name = "REQUEST DATE")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        ////[Range(typeof(DateTime), "1/1/1966", "1/1/2020")]
        public DateTime REQUEST_DATE { get; set; }
        public DateTime REQUEST_TIME { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(Decimal), "0", "9999999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        //[RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]           
        public decimal AMOUNT { get; set; }
        
        [Display(Name = "Bank Account")]
        [Required]
        public string BANK_ACCOUNT { get; set; }
        [Required]
        [Display(Name = "Payment Method")]        
        public string PAYMENT_METHOD { get; set; }
        public string TRANSACTION_DETAILS { get; set; }
        public string STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVAL_DATE { get; set; }
        public DateTime? APPROVAL_TIME { get; set; }
        [Required]
        public string TRANSFER_METHOD { get; set; }
        public string PG_TYPE { get; set; }
        public string CARDNUM { get; set; }
        public string BANK_REF_NO { get; set; }
        public string BANKCODE { get; set; }
        public string PAYGATEWAT_REFID { get; set; }
        public string REF_NO { get; set; }
        public string REMARKS { get; set; }
        public string TransactionID { get; set; }
        [Range(typeof(Decimal), "0", "99999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal BANK_CHARGES { get; set; }
        [Required]
        [Display(Name = "Reference")]       
        public string REFERENCE_NO { get; set; }  
        public long INSERTED_BY { get; set; }
        public string PAYMENT_TXN_DETAILS { get; set; }
        [NotMapped]
        public string INSERTED_USERNAME {get;set;  }
        [NotMapped]
        public string ToUser { get; set; }
        [NotMapped]
        [Required]
        [Display(Name = "user name")]
        public string FromUser { get; set; }
        [NotMapped]
        public string BankInfo { get; set; }
        [NotMapped]
        public long Serial_No { get; set; }
        [NotMapped]
        public string trnsdate { get; set; }
        [NotMapped]
        public string RequisitionSendTO  { get; set; }
        [NotMapped]
        public string WhiteLableID { get; set; }
        [NotMapped]
        public string MemberRole { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }
        [NotMapped]
        public bool checkboxBilldesk { get; set; }
    }
}
