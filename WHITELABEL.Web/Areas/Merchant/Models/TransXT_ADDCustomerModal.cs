namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class TransXT_ADDCustomerModal
    {
        [Required]
        [Display(Name = "Member Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MobileNumber { get; set;   }
        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(255, ErrorMessage = "Customer name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid customer name")]
        //[MaxLength(30, ErrorMessage = "Customer name is not greater then 30 digit")]
        //[MinLength(5, ErrorMessage = "Customer name is not less then 5 digit")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        //[Display(Name = "REQUEST DATE")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [Required]
        [Display(Name = "OTP")]
        public string OTP { get; set; }
        [NotMapped]
        public string TRANSACTIONLIMIT { get; set; }
    }
}