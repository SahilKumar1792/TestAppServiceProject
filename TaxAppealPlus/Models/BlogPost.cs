using System;

namespace TaxAppealPlus.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string FeaturedImage { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}


