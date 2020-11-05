namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public class RailAgentViewModel
    {
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public string RAIL_USER_ID { get; set; }
        public string IRCTC_LOGIN_ID { get; set; }
        [Required(ErrorMessage = "Travel agent name is required")]
        [MaxLength(350, ErrorMessage = "Travel agent name is not greater then 350 digit")]
        [MinLength(5, ErrorMessage = "Travel agent name is not less then 5 digit")]
        public string TRAVEL_AGENT_NAME { get; set; }
        [Required(ErrorMessage = "Agency name is required")]
        [MaxLength(500, ErrorMessage = "Agency name is not greater then 500 digit")]
        [MinLength(5, ErrorMessage = "Agency name is not less then 5 digit")]
        public string AGENCY_NAME { get; set; }
        [Required(ErrorMessage = "Office address is required")]
        public string OFFICE_ADDRESS { get; set; }
        [Required(ErrorMessage = "Residence address is required")]
        public string RESIDENCE_ADDRESS { get; set; }
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Email is not greater then 250 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        public string EMAIL_ID { get; set; }
        [Required]
        [Display(Name = "Mobile No")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MOBILE_NO { get; set; }
        public string OFFICE_PHONE { get; set; }
        [Display(Name = "Member Mobile")]
        [StringLength(10, ErrorMessage = "Pan no must be 10 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Pan no")]
        public string PAN_NO { get; set; }
        public string DIGITAL_CERTIFICATE_DETAILS { get; set; }
        [Required(ErrorMessage = "Certificate begin date is required")]
        public DateTime? CERTIFICATE_BEGIN_DATE { get; set; }
        [Required(ErrorMessage = "Certificate end date is required")]
        public DateTime? CERTIFICATE_END_DATE { get; set; }
        public string USER_STATE { get; set; }
        public string AGENT_VERIFIED_STATUS { get; set; }
        public string DEACTIVATION_REASON { get; set; }
        public string AADHAAR_VERIFICATION_STATUS { get; set; }
        public DateTime? ENTRY_DATE { get; set; }
        public string FLAG1 { get; set; }
        public string FLAG2 { get; set; }
        public bool STATUS { get; set; }        
        [Required]
        [Display(Name = "Please Select State")]
        public long AGENT_STATE_ID { get; set; }
    }
}