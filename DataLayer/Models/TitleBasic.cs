using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class TitleBasic
    {
        public string TConst { get; set; }
        public string? TitleType { get; set; }
        public string? PrimaryTitle { get; set; }
        public string? OriginalTitle { get; set; }
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }
        public int? RunTimeMinutes { get; set; }
        public string? Awards { get; set; }
        public string? Plot { get; set; }
        public string? Rated { get; set; }
        public string? ReleaseDate { get; set; }
        public string? ProductionCompany { get; set; }
        public string? Poster { get; set; }
        public string? BoxOffice { get; set; }
    }
}