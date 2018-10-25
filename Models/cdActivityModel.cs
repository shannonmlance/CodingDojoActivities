using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingDojoActivities.Models
{
    public class cdActivity
    {
        [Key]
        public int cdActivityId {get;set;}

        [Required]
        [DataType(DataType.Text)]
        [MinLength(2)]
        [Display(Name="Title")]
        public string Title {get;set;}

        [Required]
        [DataType(DataType.Date)]
        [FutureDate]
        [Display(Name="Date")]
        public DateTime Date {get;set;}

        [Required]
        [DataType(DataType.Time)]
        [Display(Name="Time")]
        public DateTime Time {get;set;}

        [Required]
        [Range(1,9999)]
        [Display(Name="Duration")]
        public int Duration {get;set;}

        [Required]
        public string DurationType {get;set;}
        public TimeSpan DurationTimespan {get;set;}

        [Required]
        [DataType(DataType.Text)]
        [MinLength(10)]
        [Display(Name="Description")]
        public string Description {get;set;}

        public int UserId {get;set;}
        public User Creator {get;set;}

        public List<Participation> Participations {get;set;}
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is DateTime)
            {
                DateTime after = (DateTime)value;
                if(after <= DateTime.Today)
                {
                    return new ValidationResult("Date must be after today.");
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Must be a date.");
        }
    }
}