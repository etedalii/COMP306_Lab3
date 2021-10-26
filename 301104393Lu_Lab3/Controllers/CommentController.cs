using _301104393Lu_Lab3.Data;
using _301104393Lu_Lab3.Models;
using _301104393Lu_Lab3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IDynamoDbContext<Comment> commentContext;
        private readonly IDynamoDbContext<Movie> movieContext;
        private readonly IWebHostEnvironment env;
        IConfiguration configuration;

        public CommentController(IDynamoDbContext<Comment> context, IDynamoDbContext<Movie> movieContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            this.movieContext = movieContext;
            this.commentContext = context;
            this.env = env;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            CommentViewModel vm = new CommentViewModel();
            vm.Movies = movieContext.GetAll().Result.ToList();
            vm.Comments = commentContext.GetAll().Result.ToList();
            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel vm)
        {
            var movie = movieContext.GetByTitleAsync(vm.Movie.Title).Result.FirstOrDefault();
            if (movie == null)
            {
                ViewBag.MovieError = $"The Movie with Name {vm.Movie.Title} not exist";
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                vm.Comment.StatementDate = DateTime.Now.ToString();
                vm.Comment.Username = User.Identity.Name;
                vm.Comment.MovieId = movie.MovieId;
                vm.Comment.CommentId = Guid.NewGuid().ToString();
                await commentContext.SaveAsync(vm.Comment);
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult FillAutoComplete()
        {
            List<string> lookup = new List<string>();
            foreach (var item in movieContext.GetAll().Result.ToList())
            {
                lookup.Add(item.Title);
            }

            return Json(lookup);
        }
    }
}
