using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TaxAppealPlus.Models;

namespace TaxAppealPlus.Controllers
{
    public class BlogPageController : Controller
    {
        private readonly BlogDbContext _db;
        private readonly IMemoryCache _cache;

        public BlogPageController(BlogDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        [HttpGet("blog")]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 6;
            var cacheKey = $"blog_index_posts_{page}";
            if (!_cache.TryGetValue(cacheKey, out BlogIndexViewModel? vm))
            {
                var totalCount = await _db.BlogPosts.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                page = Math.Clamp(page, 1, Math.Max(1, totalPages));

                var posts = await _db.BlogPosts
                    .OrderByDescending(p => p.PublishedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                vm = new BlogIndexViewModel
                {
                    Posts = posts,
                    Page = page,
                    TotalPages = totalPages
                };

                _cache.Set(cacheKey, vm, TimeSpan.FromMinutes(5));
            }
            return View(vm);
        }

        [HttpGet("blog/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var post = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == slug);
            if (post is null) return NotFound();

            // Fetch 3 related posts (for simplicity, just recent ones excluding current)
            var relatedPosts = await _db.BlogPosts
                .Where(p => p.Id != post.Id)
                .OrderByDescending(p => p.PublishedAt)
                .Take(3)
                .ToListAsync();

            // Pass both post and related posts to the view
            ViewBag.RelatedPosts = relatedPosts;
            return View(post);
        }

    }
}


