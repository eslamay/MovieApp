using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
	[Authorize(Roles = "admin")]
	[Route("/Admin/[controller]/{action=Index}/{id?}")]
	public class MoviesController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;
		private readonly int pageSize = 5;

		public MoviesController(ApplicationDbContext context,IWebHostEnvironment environment) 
		{
			this.context = context;
			this.environment = environment;
		}
		public IActionResult Index(int pageIndex,string?search, string? column, string? orderBy)
		{
			IQueryable<Movie> query = context.Movies!;
			query = query.OrderByDescending(x => x.Id);

			//sort
			string[] validColumns = { "Id", "Title", "Genre", "Year", "Rate"};
			string[] validOrderBy = { "desc", "asc" };

			if (!validColumns.Contains(column))
			{
				column = "Id";
			}

			if (!validOrderBy.Contains(orderBy))
			{
				orderBy = "desc";
			}

			if (column == "Title")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Title);
				}
				else
				{
					query = query.OrderByDescending(p => p.Title);
				}
			}
			else if (column == "Genre")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Genre);
				}
				else
				{
					query = query.OrderByDescending(p => p.Genre);
				}
			}
			else if (column == "Year")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Year);
				}
				else
				{
					query = query.OrderByDescending(p => p.Year);
				}
			}
			else if (column == "Rate")
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Rate);
				}
				else
				{
					query = query.OrderByDescending(p => p.Rate);
				}
			}
			else
			{
				if (orderBy == "asc")
				{
					query = query.OrderBy(p => p.Id);
				}
				else
				{
					query = query.OrderByDescending(p => p.Id);
				}
			}

			//Search Func
			if (search != null)
			{
				query = query.Where(p => p.Title.Contains(search) || p.Genre.Contains(search));
			}

			//Pagination Func
			if (pageIndex<1)
			{
				pageIndex = 1;
			}

			decimal count = query.Count();
			int totalPages = (int)Math.Ceiling(count / pageSize);
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


			var movies= query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(movies);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(MovieDto movieDto)
		{
			if (movieDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The Image File is required");
			}

			if (!ModelState.IsValid)
			{
				return View(movieDto);
			}

			//Save The Image File
			string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			NewFileName += Path.GetExtension(movieDto.ImageFile!.FileName);

			string imageFullPath = environment.WebRootPath + "/movieImages/" + NewFileName;

			using (var stream = System.IO.File.Create(imageFullPath))
			{
				movieDto.ImageFile.CopyTo(stream);
			}

			//Save The new product in database
			Movie movie = new Movie()
			{
				Title = movieDto.Title,
				Genre = movieDto.Genre,
				Description = movieDto.Description,
				Rate = movieDto.Rate,
				Year = movieDto.Year,
				ImageFileName = NewFileName,
			};

			context.Movies?.Add(movie);
			context.SaveChanges();

			return RedirectToAction("Index", "Movies");
		}

		public IActionResult Edit(int id)
		{
			var movie = context.Movies?.Find(id);

			if (movie == null)
			{
				return RedirectToAction("Index", "Movies");
			}

			var movieDto = new MovieDto()
			{
				Title=movie.Title,
				Genre=movie.Genre,
				Rate=movie.Rate,
				Year=movie.Year,
				Description = movie.Description,
			};

			ViewData["MovieId"] = movie.Id;
			ViewData["ImageFileName"] = movie.ImageFileName;
			return View(movieDto);
		}

		[HttpPost]
		public IActionResult Edit(int id, MovieDto movieDto)
		{
			var movie = context.Movies?.Find(id);

			if (movieDto == null)
			{
				return RedirectToAction("Index", "Movies");
			}

			if (!ModelState.IsValid)
			{
				ViewData["MovieId"] = movie?.Id;
				ViewData["ImageFileName"] = movie?.ImageFileName;

				return View(movieDto);
			}


			string NewFileName = movie.ImageFileName;
			if (movieDto.ImageFile != null)
			{
				//Update The Image File
				NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				NewFileName += Path.GetExtension(movieDto.ImageFile!.FileName);

				string imageFullPath = environment.WebRootPath + "/movieImages/" + NewFileName;

				using (var stream = System.IO.File.Create(imageFullPath))
				{
					movieDto.ImageFile.CopyTo(stream);
				}

				//Delete old image file
				string OldImageFullPath = environment.WebRootPath + "/movieImages/" + movie.ImageFileName;
				System.IO.File.Delete(OldImageFullPath);
			}

			//update product in database
			movie.Title = movieDto.Title;
			movie.Description = movieDto.Description;
			movie.Genre = movieDto.Genre;
            movie.Rate = movieDto.Rate;
            movie.Year = movieDto.Year;
			movie.ImageFileName = NewFileName;

			context.SaveChanges();
			return RedirectToAction("Index", "Movies");
		}

		public IActionResult Delete(int id)
		{
			var movie = context.Movies?.Find(id);

			if (movie == null)
			{
				return RedirectToAction("Index", "Movies");
			}

			string ImageFullPath = environment.WebRootPath + "/movieImages/" + movie.ImageFileName;
			System.IO.File.Delete(ImageFullPath);

			context.Movies?.Remove(movie);
			context.SaveChanges();

			return RedirectToAction("Index", "Movies");
		}

		[AllowAnonymous]
		public IActionResult Details(int id)
		{
			var movie = context.Movies?.Find(id);
			if (movie == null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View(movie);
		}
	}
}
