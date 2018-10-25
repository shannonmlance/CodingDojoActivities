using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingDojoActivities.Models
{
    public class LoginUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name="Email")]
        public string LoginEmail {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string LoginPassword {get;set;}
    }
}