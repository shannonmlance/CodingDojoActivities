using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CodingDojoActivities.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [DataType(DataType.Text)]
        [AlphaOnly]
        [MinLength(2)]
        [MaxLength(45)]
        [Display(Name="First Name")]
        public string FirstName {get;set;}

        [Required]
        [DataType(DataType.Text)]
        [AlphaOnly]
        [MinLength(2)]
        [MaxLength(45)]
        [Display(Name="Last Name")]
        public string LastName {get;set;}

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name="Email")]
        public string Email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [PasswordReqs]
        [MinLength(8)]
        [Display(Name="Password")]
        public string Password {get;set;}

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name="Confirm Password")]
        public string CPW {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<cdActivity> CreatedActivities {get;set;}
        public List<Participation> Participations {get;set;}
    }

    public class AlphaOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is string)
            {
                string name = (string)value;
                Regex r = new Regex("^[a-zA-Z]*$");
                if(r.IsMatch(name))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Use only letters.");
                }
            }
            else
            {
                return new ValidationResult("Use only letters.");
            }
        }
    }
    public class PasswordReqsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is string)
            {
                string password = (string)value;
                Regex rr = new Regex("^(?=.*?[A-Za-z])(?=.*?[0-9])(?=.*?[@$!%*#?&]).{8,}$");
                if(rr.IsMatch(password))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Must contain at least 1 number, 1 letter, and a special character.");
                }
            }
            else
            {
                return new ValidationResult("Must contain at least 1 number, 1 letter, and a special character.");
            }
        }
    }
}