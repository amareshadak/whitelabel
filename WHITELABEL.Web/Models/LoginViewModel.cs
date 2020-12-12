using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WHITELABEL.Web.Models
{
    public class LoginViewModel
    {
        //[Required]
        //[EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Required(ErrorMessage = "Boom Travels User ID is required")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        [NotMapped]
        public string GetIPAddress { get; set; }
    }
    public class ForgettenPassword
    {
        //[Required]
        //[EmailAddress(ErrorMessage = "Not a valid email address")]
        //public string Email { get; set; }
        //[Required]
        //[EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }
        //[Required]
        //[Display(Name = "Member Mobile")]
        //[MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        //[MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MobileNo { get; set; }
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
        [Required]
        [Display(Name = "Enter Otp ")]
        [MaxLength(6, ErrorMessage = "Otp not greater then 6 digit")]
        [MinLength(6, ErrorMessage = "Otp not less then 6 digit")]
        public string OTPVerification { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "New password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
    //public class TBL_PASSWORD_RESET
    //{
    //    public string ID { get; set; }
    //    public string EmailID { get; set; }
    //    public DateTime Time { get; set; }
    //}
}