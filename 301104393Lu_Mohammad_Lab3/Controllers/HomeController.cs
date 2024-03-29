﻿using _301104393Lu_Mohammad_Lab3.Data;
using _301104393Lu_Mohammad_Lab3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Mohammad_Lab3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDynamoDbContext<Movie> _context;

        public HomeController(ILogger<HomeController> logger, IDynamoDbContext<Movie> context)
        {
            this._context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<JsonResult> GetMovieByGenre(string genre)
        {
            var movies = string.IsNullOrEmpty(genre) ? await _context.GetAll() : await _context.GetByGenreAsync(genre);
            var groupedMovie = movies.GroupBy(t => new { MovieTitle = t.MovieTitle})
                .Select(g => new
                {
                    Rate = g.Average(p => p.Rate),
                    MovieTitle = g.Key.MovieTitle
                }).ToList();

            return Json(groupedMovie);
        }
        
        public async Task<JsonResult> GetAllComments(string title)
        {
            var movies = await _context.GetByTitleAsync(title);
            var comments = movies.Select(i => new { i.UserId, i.Comment , i.CommentDate })
                        .OrderByDescending(i => i.CommentDate).ToList();

            return Json(comments);
        }
    }
}