﻿namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class WaterSupplyPaymentModel
    {
        [Required]
        [Display(Name = "Account ID")]
        [MaxLength(12, ErrorMessage = "Customer ID is not greater then 12 digit")]
        [MinLength(8, ErrorMessage = "Customer ID is not less then 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string CustomerId { get; set; }
        //[Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmount { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        public string Service_Name { get; set; }
        public string Service_Key { get; set; }
        public string MobileNo { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        //[RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public decimal RechargeAmt { get; set; }
        public string PIN { get; set; }
        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [NotMapped]
        public string WaterRefNo { get; set; }
    }
}