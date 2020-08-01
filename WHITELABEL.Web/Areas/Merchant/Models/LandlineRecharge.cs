﻿namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class LandlineRecharge
    {
        [Required]
        [Display(Name = "ContactNo")]
        [MaxLength(15, ErrorMessage = "LandLine no not greater then 15 digit")]
        [MinLength(8, ErrorMessage = "LandLine no not less then 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "LandLine must be number")]
        public string ContactNo { get; set; }

        [Required]
        [Display(Name = "CustomerNo")]
        [MaxLength(15, ErrorMessage = "Customer no not greater then 15 digit")]
        [MinLength(8, ErrorMessage = "Customer no not less then 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Customer no must be number")]
        public string CustomerNo { get; set; }

        [Required]
        [Display(Name = "Operator Name")]
        public string OperatorName { get; set; }
        public long PRODUCTID { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmt { get; set; }
        [Required]
        [Display(Name = "AccountNo")]
        [MaxLength(15, ErrorMessage = "Account no not greater then 15 digit")]
        public string AccountNo { get; set; }
        [Required]
        [Display(Name = "CircleName")]
        [MaxLength(50, ErrorMessage = "Circle not greater then 50 digit")]
        public string CircleName { get; set; }
        [Required]
        [Display(Name = "STDCode")]
        [MaxLength(8, ErrorMessage = "STD Code not greater then 8 digit")]
        public string STDCode { get; set; }
        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [NotMapped]
        public string LandLineRefId { get; set; }
    }
}