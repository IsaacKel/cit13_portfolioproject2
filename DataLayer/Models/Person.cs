using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models
{
    public class Person
    {

        public string ActualName { get; set; }
        [Key]
        public string NConst { get; set; }
        public string? BirthYear { get; set; }
        public string? DeathYear { get; set; }
    }
}
