using System.Collections.Generic;

namespace TaxAppealPlus.Models
{
    public class BlogIndexViewModel
    {
        public List<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}


