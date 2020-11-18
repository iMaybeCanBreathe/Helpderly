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
        [StringLength(50, ErrorMessage = "Info too long, you may add it in the description instead.")]
        public string AdditionalInfo { get; set; }
        
        [Display(Name = "Description")]
        [StringLength(150, ErrorMessage = "Description is too long")]
        public string Description { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Your name is too long")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "Location")]
        [StringLength(50, ErrorMessage = "Your location is too long")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "Mobile Number")]
        [StringLength(10, ErrorMessage = "Your number is too long")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "Quantity")]
        public int quantityVolunteer { get; set; }

        [Required(ErrorMessage = "Please do not leave this blank")]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }


    }
}
