namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;


    public class FlightRescheduleViewModel
    {
        public long MEM_ID  { get; set; }
        public long SLN { get; set; }
        public string CORELATION_ID { get; set; }
        [Required(ErrorMessage = "Please enter your reschedule process reply")]
        public string RescheduleResplyMsg { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
    }
}