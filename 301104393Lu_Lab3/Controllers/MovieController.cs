using _301104393Lu_Lab3.Classes;
using _301104393Lu_Lab3.Data;
using _301104393Lu_Lab3.Models;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3.Controllers
{
    public class MovieController : Controller
    {
        private readonly IDynamoDbContext<Movie> _context;
        private readonly IDynamoDbContext<Comment> commentContext;
        private readonly IWebHostEnvironment env;
        IConfiguration configuration;

        public MovieController(IDynamoDbContext<Movie> context, IDynamoDbContext<Comment> commentContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            this._context = context;
            this.commentContext = commentContext;
            this.env = env;
            this.configuration = configuration;
        }

        // GET: MovieController
        public async Task<ActionResult> Index()
        {
            var movie = await _context.GetAll();
            var comment = commentContext.GetAll().Result.ToList();
            foreach (var item in movie)
            {
                var spComment = comment.Where(_ => _.MovieId == item.MovieId).ToList();
                item.Rate = spComment.Count > 0 ? spComment.Average(_ => _.Rate) : 0;
            }

            return View(movie);
        }

        // GET: MoviePosts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.GetByIdAsync(id);
            if (movie == null || movie.Count() == 0)
            {
                return NotFound();
            }

            return View(movie.First());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Genre,Rate,Year,Cast,Director")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                string filename = string.Empty;
                if (Request.Form.Files != null && Request.Form.Files.Count > 0 && !string.IsNullOrEmpty(Request.Form.Files["trailer"].FileName) && (Request.Form.Files["trailer"].ContentType == "video/mp4"))
                {
                    filename = Request.Form.Files["trailer"].FileName;
                    var uploadPath = Path.Combine(env.WebRootPath);
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, filename);
                    using (var strem = System.IO.File.Create(filePath))
                    {
                        Request.Form.Files["trailer"].CopyTo(strem);
                    }
                    UploadFile(filePath);
                    System.IO.File.Delete(filePath);
                }
                movie.FileName = filename;
                movie.MovieId = Guid.NewGuid().ToString();
                await _context.SaveAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.GetByIdAsync(id);
            if (movie == null || movie.Count() == 0)
            {
                return NotFound();
            }

            DownloadFile2(movie.First().FileName).GetAwaiter().GetResult();
            MovieViewModel vm = new MovieViewModel();
            vm.Movie = movie.First();
            vm.Casts = String.Join(",", movie.First().Cast);
            vm.Directors = String.Join(",", movie.First().Director);

            return View(vm);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MovieViewModel viewModel)
        {
            if (id != viewModel.Movie.MovieId)
            {
                return NotFound();
            }

            var oldmovie = await _context.GetByIdAsync(id);
            _context.DeleteAsync(oldmovie.First());
            var uploadPath = Path.Combine(env.WebRootPath);
            if (ModelState.IsValid)
            {
                string filename = string.Empty;
                if (Request.Form.Files != null && Request.Form.Files.Count > 0 && !string.IsNullOrEmpty(Request.Form.Files["trailer"].FileName) && (Request.Form.Files["trailer"].ContentType == "video/mp4"))
                {
                    filename = Request.Form.Files["trailer"].FileName;

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, filename);
                    using (var strem = System.IO.File.Create(filePath))
                    {
                        Request.Form.Files["trailer"].CopyTo(strem);
                    }
                    UploadFile(filePath);
                    System.IO.File.Delete(filePath);

                    viewModel.Movie.FileName = filename;
                }

                viewModel.Movie.Cast = (viewModel.Casts.Split(',')).Select(t => t).ToList(); 
                viewModel.Movie.Director = (viewModel.Directors.Split(',')).Select(t => t).ToList(); 
                await _context.SaveAsync(viewModel.Movie);

                try
                {
                    var filePath2 = Path.Combine(uploadPath, "trailer.mp4");
                    System.IO.File.Delete(filePath2);
                }
                catch
                { }

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel.Movie);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.GetByIdAsync(id);
            if (movie == null || movie.Count() == 0)
            {
                return NotFound();
            }

            return View(movie.First());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movie = await _context.GetByIdAsync(id);
            await _context.DeleteAsync(movie.First());
            await DeleteObjectNonVersionedBucketAsync(movie.First().FileName);
            return RedirectToAction(nameof(Index));
        }

        private bool UploadFile(string filePath)
        {
            try
            {
                var accessKeyID = configuration["AccesskeyID"];
                var secretKey = configuration["Secretaccesskey"];
                var credentials = new BasicAWSCredentials(accessKeyID, secretKey);

                RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
                IAmazonS3 s3Client = new AmazonS3Client(accessKeyID, secretKey, RegionEndpoint.USEast1);

                var fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.Upload(filePath, configuration["BucketName"]);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<bool> DownloadFile2(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            try
            {
                var accessKeyID = configuration["AccesskeyID"];
                var secretKey = configuration["Secretaccesskey"];
                var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };
                using var client = new AmazonS3Client(credentials, config);
                var fileTransferUtility = new TransferUtility(client);

                var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = configuration["BucketName"],
                    Key = filename
                });

                var uploadPath = Path.Combine(env.WebRootPath);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, "trailer.mp4");
                using (var strem = System.IO.File.Create(filePath))
                {
                    objectResponse.ResponseStream.CopyTo(strem);
                }

                return true;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }

        }

        public async Task<IActionResult> DownloadFile(string id)
        {
            var oldmovie = await _context.GetByIdAsync(id);
            string filename = oldmovie.First().FileName;
            try
            {
                var accessKeyID = configuration["AccesskeyID"];
                var secretKey = configuration["Secretaccesskey"];
                var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };
                using var client = new AmazonS3Client(credentials, config);
                var fileTransferUtility = new TransferUtility(client);

                var objectResponse = await fileTransferUtility.S3Client.GetObjectAsync(new GetObjectRequest()
                {
                    BucketName = configuration["BucketName"],
                    Key = filename
                });

                return File(objectResponse.ResponseStream, objectResponse.Headers.ContentType, filename);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null
                    && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }

        }

        [HttpPost]
        public async Task<ActionResult> Index(string rating)
        {
            if (!string.IsNullOrEmpty(rating))
            {
                return View(await _context.GetByRating(Convert.ToInt32(rating)));
            }
            else
                return RedirectToAction(nameof(Index));
        }

        private async Task DeleteObjectNonVersionedBucketAsync(string keyName)
        {
            var accessKeyID = configuration["AccesskeyID"];
            var secretKey = configuration["Secretaccesskey"];
            var credentials = new BasicAWSCredentials(accessKeyID, secretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };
            using var client = new AmazonS3Client(credentials, config);

            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = configuration["BucketName"],
                    Key = keyName
                };

                await client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch 
            {
            }
        }
    }
}
