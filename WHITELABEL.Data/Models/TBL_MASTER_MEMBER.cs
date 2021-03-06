﻿namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("MASTER_MEMBER")]
    public class TBL_MASTER_MEMBER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MEM_ID { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        [MaxLength(45, ErrorMessage = "User name is not greater then 45 digit")]
        [MinLength(5, ErrorMessage = "User name is not less then 5 digit")]
        [Column("User_Name")]
        public string UName { get; set; }

        public long? UNDER_WHITE_LEVEL { get; set; }

        [Required]
        [Display(Name = "Member Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MEMBER_MOBILE { get; set; }

        [Required]
        [Display(Name = "Member Name")]
        [StringLength(255, ErrorMessage = "Member name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid member name")]
        [MaxLength(250, ErrorMessage = "Member name is not greater then 250 digit")]
        [MinLength(5, ErrorMessage = "Member name is not less then 5 digit")]
        public string MEMBER_NAME { get; set; }
        [MaxLength(250, ErrorMessage = "Company name is not greater then 250 digit")]
        [MinLength(5, ErrorMessage = "Company name is not less then 5 digit")]
        public string COMPANY { get; set; }

        [Required]
        [Display(Name = "Please Select Member Role")]
        public long? MEMBER_ROLE { get; set; }
        public long? INTRODUCER { get; set; }
        [MaxLength(500, ErrorMessage = "Address is not greater then 500 digit")]
        [MinLength(5, ErrorMessage = "Address is not less then 5 digit")]
        public string ADDRESS { get; set; }

        [StringLength(50, ErrorMessage = "Character must be 50 digit")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabet required")]
        [RegularExpression(@"[a-zA-Z ]*$", ErrorMessage = "Only Alphabet required")]
        public string CITY { get; set; }

        [MaxLength(10,ErrorMessage ="Pin no is not greater then 10 digit")]
        [MinLength(6, ErrorMessage = "Pin no is not less then 6 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pin Code must be number")]
        public string PIN { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Email is not greater then 150 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        [StringLength(255, ErrorMessage = "Email must be 255 digit")]        
        public string EMAIL_ID { get; set; }
        
        [Required]
        [Display(Name = "Password")]
        //[DataType(DataType.Password)]
        [MaxLength(12, ErrorMessage = "Password not greater then 12 digit")]
        [MinLength(10, ErrorMessage = "Password not less then 10 digit")]        
        [Column("PASSWORD_MD5")]
        public string User_pwd { get; set; }

        //[Required]        
        //[Display(Name = "Security Pin")]
        [StringLength(4, ErrorMessage = "Security must be 4 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Enter only Numeric Value")]
        public string SECURITY_PIN_MD5 { get; set; }

        public decimal? BALANCE { get; set; }
        
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]

        public decimal? BLOCKED_BALANCE { get; set; }

        public bool? ACTIVE_MEMBER { get; set; }

        public DateTime? JOINING_DATE { get; set; }
        [StringLength(12, ErrorMessage = "Aadhaar card must be 12 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Aadhaar Card must be number")]
        public string AADHAAR_NO { get; set; }

        //[StringLength(10, ErrorMessage = "Pan card no must be 10 digit")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Pan no")]
        [Display(Name = "PAN Card:")]
        [Required(ErrorMessage = "PAN Number is required")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN Number")]
        public string PAN_NO { get; set; }        

        public string AADHAAR_FILE_NAME { get; set; }
        public string PAN_FILE_NAME { get; set; }
        public bool? KYC_VERIFIED { get; set; }
        public long? KYC_VERIFIED_USER { get; set; }
        public DateTime? VERIFICATION_DATE { get; set; }
        public bool? IS_DELETED { get; set; }
        public long? CREATED_BY { get; set; }
        public long? MODIFIED_BY { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public long? DELETED_BY { get; set; }
        public string RAIL_ID { get; set; }
        public string RAIL_PWD { get; set; }
        public int? GST_MODE { get; set;}
        public int? TDS_MODE { get; set; }
        //[Required(ErrorMessage = "GST No is required")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid GST No")]
        //[MaxLength(15, ErrorMessage = "GST No is not greater then 15 digit")]
        //[MinLength(10, ErrorMessage = "GST No is not less then 10 digit")]
        public string COMPANY_GST_NO { get; set; }
        public long? GST_FLAG { get; set; }
        public long? TDS_FLAG { get; set; }
        public string LOGO { get; set; }
        public string LOGO_STYLE { get; set; }
        public decimal? CREDIT_BALANCE { get; set; }
        public decimal? DUE_CREDIT_BALANCE { get; set; }
        public bool? IS_TRAN_START { get; set; }
        [Required]
        [Display(Name = "Please Select State")]
        public long? STATE_ID { get; set; }
        public string FACEBOOK_ID { get; set; }
        public string WEBSITE_NAME { get; set; }
        public string NOTES { get; set; }
        [MaxLength(15, ErrorMessage = "Optional Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Optional Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Optional Mobile must be number")]
        public string OPTIONAL_MOBILE_NO { get; set; }
        [MaxLength(15, ErrorMessage = "Sec Optional Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Sec Optional Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Sec Optional Mobile must be number")]
        public string SEC_OPTIONAL_MOBILE_NO { get; set; }
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Optional Email is not greater then 150 digit")]
        [MinLength(10, ErrorMessage = "Optional Email is not less then 10 digit")]
        [StringLength(255, ErrorMessage = "Optional Email must be 255 digit")]
        public string OPTIONAL_EMAIL_ID { get; set; }
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Sec Optional Email is not greater then 150 digit")]
        [MinLength(10, ErrorMessage = "Sec Optional Optional Email is not less then 10 digit")]
        [StringLength(255, ErrorMessage = "Sec Optional Email must be 255 digit")]
        public string SEC_OPTIONAL_EMAIL_ID { get; set; }
        public string OLD_MEMBER_ID { get; set; }
        public decimal? CREDIT_LIMIT { get; set; }
        public decimal? RESERVED_CREDIT_LIMIT { get; set;    }
        public string MEM_UNIQUE_ID { get; set; }
        public int? RAIL_ID_QUANTITY { get; set; }
        public DateTime? RENEWAL_DATE { get; set; }
        public string PAYMENT_MODE { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public long SUPER_ID { get; set; }
        [NotMapped]
        public long DISTRIBUTOR_ID { get; set; }
        [NotMapped]
        public string SuperName { get; set; }
        [NotMapped]
        public string DistributorName { get; set; }
        [NotMapped]
        public string MerchantName { get; set; }
        [NotMapped]
        public string RAIL_USER_ID { get; set; }
        [NotMapped]
        public string IRCTC_LOGIN_ID { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Travel agent name is required")]
        [MaxLength(350, ErrorMessage = "Travel agent name is not greater then 350 digit")]
        [MinLength(5, ErrorMessage = "Travel agent name is not less then 5 digit")]
        public string TRAVEL_AGENT_NAME { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Agency name is required")]
        [MaxLength(500, ErrorMessage = "Agency name is not greater then 500 digit")]
        [MinLength(5, ErrorMessage = "Agency name is not less then 5 digit")]
        public string AGENCY_NAME { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Office address is required")]
        public string OFFICE_ADDRESS { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Residence address is required")]
        public string RESIDENCE_ADDRESS { get; set; }
        [NotMapped]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Email is not greater then 250 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        public string AGENTEMAIL_ID { get; set; }
        [NotMapped]
        //[Required]
        [Display(Name = "Mobile No")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MOBILE_NO { get; set; }
        [NotMapped]
        public string OFFICE_PHONE { get; set; }
        [NotMapped]
        [Display(Name = "Member Mobile")]
        [StringLength(10, ErrorMessage = "Pan no must be 10 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Pan no")]
        public string AGENTPAN_NO { get; set; }
        [NotMapped]
        public string DIGITAL_CERTIFICATE_DETAILS { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Certificate begin date is required")]
        public DateTime? CERTIFICATE_BEGIN_DATE { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Certificate end date is required")]
        public DateTime? CERTIFICATE_END_DATE { get; set; }
        [NotMapped]
        public string USER_STATE { get; set; }
        [NotMapped]
        public string AGENT_VERIFIED_STATUS { get; set; }
        [NotMapped]
        public string DEACTIVATION_REASON { get; set; }
        [NotMapped]
        public string AADHAAR_VERIFICATION_STATUS { get; set; }
        [NotMapped]
        public DateTime? ENTRY_DATE { get; set; }
        [NotMapped]
        public string FLAG1 { get; set; }
        [NotMapped]
        public string FLAG2 { get; set; }
        [NotMapped]
        public bool STATUS { get; set; }
        [NotMapped]
        public bool RailIdTagged { get; set; }
        [NotMapped]
        public long RailSLN { get; set; }
        [NotMapped]
        [Required]
        [Display(Name = "Please Select State")]
        public long AGENT_STATE_ID { get; set; }
    }


}