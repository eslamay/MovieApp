using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;
		private readonly int pageSize = 16;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,IWebHostEnvironment environment)
        {
            _logger = logger;
			this.context = context;
			this.environment = environment;
		}

        public IActionResult Index()
        {
			var movie = context.Movies?.OrderByDescending(p => p.Rate).Take(4).ToList();
			var series = context.Series?.OrderByDescending(p => p.Rate).Take(4).ToList();
			var animes = context.Animes?.OrderByDescending(p => p.Rate).Take(4).ToList();

			var tuple = (Movies: movie, Series: series , Animes: animes);
			return View(tuple); ;
        }

		public IActionResult AllMovies(int pageIndex, string? search, string? column, string? orderBy)
		{
			IQueryable<Movie> query = context.Movies!;
			query = query.OrderByDescending(x => x.Id);

			//sort
			string[] validColumns = { "Id", "Title", "Genre", "Year", "Rate" };
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
			if (pageIndex < 1)
			{
				pageIndex = 1;
			}

			decimal count = query.Count();
			int totalPages = (int)Math.Ceiling(count / pageSize);
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


			var movies = query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(movies);
		}

		public IActionResult AllSeries(int pageIndex, string? search, string? column, string? orderBy)
		{
			IQueryable<Series> query = context.Series!;
			query = query.OrderByDescending(x => x.Id);

			//sort
			string[] validColumns = { "Id", "Title", "Genre", "Year", "Rate" };
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
			if (pageIndex < 1)
			{
				pageIndex = 1;
			}

			decimal count = query.Count();
			int totalPages = (int)Math.Ceiling(count / pageSize);
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


			var series = query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(series);
		}

		public IActionResult AllAnimes(int pageIndex, string? search, string? column, string? orderBy)
		{
			IQueryable<Anime> query = context.Animes!;
			query = query.OrderByDescending(x => x.Id);

			//sort
			string[] validColumns = { "Id", "Title", "Genre", "Year", "Rate" };
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
			if (pageIndex < 1)
			{
				pageIndex = 1;
			}

			decimal count = query.Count();
			int totalPages = (int)Math.Ceiling(count / pageSize);
			query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


			var animes = query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(animes);
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
    }
}
