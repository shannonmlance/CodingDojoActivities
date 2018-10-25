using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingDojoActivities.Models
{
    public class Participation
    {
        [Key]
        public int ParticipationId {get;set;}

        public int UserId {get;set;}
        public User User {get;set;}

        public int cdActivityId {get;set;}
        public cdActivity cdActivity {get;set;}
    }
}