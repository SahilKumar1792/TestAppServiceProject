using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaxAppealPlus.Models;

namespace TaxAppealPlus.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogDbContext _db;
        private readonly string _apiKey;

        public BlogController(BlogDbContext db, IConfiguration configuration)
        {
            _db = db;
            _apiKey = configuration["BlogApi:ApiKey"] ?? string.Empty;
        }

        private bool IsAuthorized()
        {
            if (!Request.Headers.TryGetValue("x-api-key", out var provided))
            {
                return false;
            }
            return string.Equals(provided.ToString(), _apiKey, StringComparison.Ordinal);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] JsonElement payload)
        {
            try
            {
                if (!IsAuthorized())
                    return Unauthorized(new { success = false, error = "Unauthorized" });

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // ?? Important fix
                };

                if (payload.ValueKind == JsonValueKind.Array)
                {
                    var posts = JsonSerializer.Deserialize<List<BlogPost>>(payload.GetRawText(), options);
                    if (posts == null || posts.Count == 0)
                        return BadRequest(new { success = false, error = "No blog posts provided." });

                    foreach (var post in posts)
                    {
                        if (string.IsNullOrWhiteSpace(post.Slug))
                            return BadRequest(new { success = false, error = "Slug is required for all posts." });

                        var existing = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == post.Slug);
                        if (existing == null)
                        {
                            post.LastUpdatedAt = post.LastUpdatedAt ?? post.PublishedAt;
                            _db.BlogPosts.Add(post);
                        }
                        else
                        {
                            existing.Title = post.Title;
                            existing.Excerpt = post.Excerpt;
                            existing.Content = post.Content;
                            existing.Author = post.Author;
                            existing.FeaturedImage = post.FeaturedImage;
                            existing.PublishedAt = post.PublishedAt;
                            existing.LastUpdatedAt = DateTime.UtcNow;
                        }
                    }

                    await _db.SaveChangesAsync();
                    return Ok(new { success = true, count = posts.Count, message = "Blogs added/updated successfully." });
                }
                else if (payload.ValueKind == JsonValueKind.Object)
                {
                    var post = JsonSerializer.Deserialize<BlogPost>(payload.GetRawText(), options);
                    if (post == null)
                        return BadRequest(new { success = false, error = "Invalid blog post data." });

                    if (string.IsNullOrWhiteSpace(post.Slug))
                        return BadRequest(new { success = false, error = "Slug is required." });

                    var existing = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == post.Slug);
                    if (existing == null)
                    {
                        post.LastUpdatedAt = post.LastUpdatedAt ?? post.PublishedAt;
                        _db.BlogPosts.Add(post);
                    }
                    else
                    {
                        existing.Title = post.Title;
                        existing.Excerpt = post.Excerpt;
                        existing.Content = post.Content;
                        existing.Author = post.Author;
                        existing.FeaturedImage = post.FeaturedImage;
                        existing.PublishedAt = post.PublishedAt;
                        existing.LastUpdatedAt = DateTime.UtcNow;
                    }

                    await _db.SaveChangesAsync();
                    return Ok(new { success = true, message = "Blog added or updated successfully." });
                }

                return BadRequest(new { success = false, error = "Invalid JSON structure." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }


        public class UpdateRequest
        {
            public int? Id { get; set; }
            public string? Slug { get; set; }
            public string? Title { get; set; }
            public string? Excerpt { get; set; }
            public string? Content { get; set; }
            public string? Author { get; set; }
            public string? FeaturedImage { get; set; }
            public DateTime? PublishedAt { get; set; }
            public DateTime? LastUpdatedAt { get; set; }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UpdateRequest payload)
        {
            if (!IsAuthorized()) return Unauthorized(new { success = false, error = "Unauthorized" });

            if (payload.Id is null && string.IsNullOrWhiteSpace(payload.Slug))
            {
                return BadRequest(new { success = false, error = "Id or Slug is required" });
            }

            var post = payload.Id.HasValue
                ? await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == payload.Id.Value)
                : await _db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == payload.Slug);

            if (post is null)
            {
                return NotFound(new { success = false, error = "Post not found" });
            }

            if (!string.IsNullOrWhiteSpace(payload.Title)) post.Title = payload.Title;
            if (!string.IsNullOrWhiteSpace(payload.Excerpt)) post.Excerpt = payload.Excerpt;
            if (!string.IsNullOrWhiteSpace(payload.Content)) post.Content = payload.Content;
            if (!string.IsNullOrWhiteSpace(payload.Author)) post.Author = payload.Author;
            if (!string.IsNullOrWhiteSpace(payload.FeaturedImage)) post.FeaturedImage = payload.FeaturedImage;
            if (payload.PublishedAt.HasValue) post.PublishedAt = payload.PublishedAt.Value;
            post.LastUpdatedAt = payload.LastUpdatedAt ?? DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new { success = true, id = post.Id });
        }

        public class DeleteRequest
        {
            public int? Id { get; set; }
            public string? Slug { get; set; }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest payload)
        {
            if (!IsAuthorized()) return Unauthorized(new { success = false, error = "Unauthorized" });

            if (payload.Id is null && string.IsNullOrWhiteSpace(payload.Slug))
            {
                return BadRequest(new { success = false, error = "Id or Slug is required" });
            }

            var post = payload.Id.HasValue
                ? await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == payload.Id.Value)
                : await _db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == payload.Slug);

            if (post is null)
            {
                return NotFound(new { success = false, error = "Post not found" });
            }

            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}


