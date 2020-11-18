using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FSD_Helpderly.Models
{
    public class ChangePassword
    {

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        [Compare("DatabasePassword", ErrorMessage = "The Current Password do not match.")]
        public string Password { get; set; }

        public string DatabasePassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
