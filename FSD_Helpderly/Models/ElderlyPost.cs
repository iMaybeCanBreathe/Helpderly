using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace FSD_Helpderly.Models
{
    public class ElderlyPost
    {
        [Display(Name = "Additional Info")]
        [StringLength(50, ErrorMessage = " Its To long You may Add it in the Description ")]
        public string AdditionalInfo { get; set; }
        
        [Display(Name = "Description")]
        [StringLength(150, ErrorMessage = "Your Discription Is to long")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "End Of Time")]
        public DateTime? EOT { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Your name is too long")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "Your name is too long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "Location")]
        [StringLength(50, ErrorMessage = "Your Location is to long")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "Mobile Number")]
        [StringLength(10, ErrorMessage = "Your Number is to long")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Pls Dont leave A Blank")]
        [Display(Name = "Start Time")]
        public DateTime? StartTime { get; set; }

    }
}
