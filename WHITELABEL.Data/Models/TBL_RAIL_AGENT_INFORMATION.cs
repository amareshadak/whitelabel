namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RAIL_AGENT_INFORMATION")]
    public class TBL_RAIL_AGENT_INFORMATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public string RAIL_USER_ID { get; set; }
        public string IRCTC_LOGIN_ID { get; set; }       
        public string TRAVEL_AGENT_NAME { get; set; }        
        public string AGENCY_NAME { get; set; }        
        public string OFFICE_ADDRESS { get; set; }        
        public string RESIDENCE_ADDRESS { get; set; }        
        public string EMAIL_ID { get; set; }        
        public string MOBILE_NO { get; set; }
        public string OFFICE_PHONE { get; set; }        
        public string PAN_NO { get; set; }
        public string DIGITAL_CERTIFICATE_DETAILS { get; set; }        
        public DateTime? CERTIFICATE_BEGIN_DATE { get; set; }        
        public DateTime? CERTIFICATE_END_DATE { get; set; }
        public string USER_STATE { get; set; }
        public string AGENT_VERIFIED_STATUS { get; set; }
        public string DEACTIVATION_REASON { get; set; }
        public string AADHAAR_VERIFICATION_STATUS { get; set; }
        public DateTime? ENTRY_DATE{ get; set; }
        public string FLAG1 { get; set; }
        public string FLAG2  { get; set; }
        public bool STATUS { get; set; }
        public long? STATE_ID { get; set; }
        public bool? RAIL_COMM_TAG { get; set; }
    }
}
