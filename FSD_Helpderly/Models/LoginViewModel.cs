using System.ComponentModel.DataAnnotations;

namespace WEB_Assignment.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}