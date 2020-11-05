namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public class MemberPasswordChange
    {
        public long MEM_ID { get; set; }

        public string MemberName { get; set; }
        public string MemberRole { get; set; }
        public string MemberEmailId { get; set; }
        public string MemberMobileNo { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(12, ErrorMessage = "Password not greater then 12 digit")]
        [MinLength(10, ErrorMessage = "Password not less then 10 digit")]
        public string User_pwd { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "Comfirm password must be at least 10 characters long", MinimumLength = 10)]
        [System.ComponentModel.DataAnnotations.Compare("User_pwd", ErrorMessage = "The new password and confirm passwords are not matching")]
        public string CONFIRMPASSWORD { get; set; }
        
    }
}