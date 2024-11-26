﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class TitlePrincipal
    {
        public string TConst { get; set; }
        public string NConst { get; set; }
        public int Ordering { get; set; }
        //public string? Category { get; set; }
        //public string? Job { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public string? Roles { get; set; }
    }

}