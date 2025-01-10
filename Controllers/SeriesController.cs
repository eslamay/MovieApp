using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
	[Authorize(Roles = "admin")]
	[Route("/Admin/[controller]/{action=Index}/{id?}")]
	public class SeriesController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;
		private readonly int pageSize = 5;

		public	SeriesController(ApplicationDbContext context,IWebHostEnvironment environment)
		{
			this.context = context;
			this.environment = environment;
		}
		public IActionResult Index(int pageIndex,string?search, string? column, string? orderBy)
		{
			IQueryable<Series> query = context.Series!;
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


			var series= query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(series);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(SeriesDto seriesDto)
		{
			if (seriesDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The Image File is required");
			}

			if (!ModelState.IsValid)
			{
				return View(seriesDto);
			}

			//Save The Image File
			string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			NewFileName += Path.GetExtension(seriesDto.ImageFile!.FileName);

			string imageFullPath = environment.WebRootPath + "/seriesImages/" + NewFileName;

			using (var stream = System.IO.File.Create(imageFullPath))
			{
				seriesDto.ImageFile.CopyTo(stream);
			}

			//Save The new product in database
			Series series = new Series()
			{
				Title = seriesDto.Title,
				Genre = seriesDto.Genre,
				Description = seriesDto.Description,
				Rate = seriesDto.Rate,
				Year = seriesDto.Year,
				ImageFileName = NewFileName,
			};

			context.Series?.Add(series);
			context.SaveChanges();

			return RedirectToAction("Index", "Series");
		}

		public IActionResult Edit(int id)
		{
			var series = context.Series?.Find(id);

			if (series == null)
			{
				return RedirectToAction("Index", "Series");
			}

			var seriesDto = new SeriesDto()
			{
				Title=series.Title,
				Genre=series.Genre,
				Rate=series.Rate,
				Year=series.Year,
				Description = series.Description,
			};

			ViewData["SeriesId"] = series.Id;
			ViewData["ImageFileName"] = series.ImageFileName;
			return View(seriesDto);
		}

		[HttpPost]
		public IActionResult Edit(int id, SeriesDto seriesDto)
		{
			var series = context.Series?.Find(id);

			if (seriesDto == null)
			{
				return RedirectToAction("Index", "Series");
			}

			if (!ModelState.IsValid)
			{
				ViewData["SeriesId"] = series?.Id;
				ViewData["ImageFileName"] = series?.ImageFileName;

				return View(seriesDto);
			}


			string NewFileName = series.ImageFileName;
			if (seriesDto.ImageFile != null)
			{
				//Update The Image File
				NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				NewFileName += Path.GetExtension(seriesDto.ImageFile!.FileName);

				string imageFullPath = environment.WebRootPath + "/seriesImages/" + NewFileName;

				using (var stream = System.IO.File.Create(imageFullPath))
				{
					seriesDto.ImageFile.CopyTo(stream);
				}

				//Delete old image file
				string OldImageFullPath = environment.WebRootPath + "/seriesImages/" + series.ImageFileName;
				System.IO.File.Delete(OldImageFullPath);
			}

			//update product in database
			series.Title = seriesDto.Title;
			series.Description = seriesDto.Description;
			series.Genre = seriesDto.Genre;
            series.Rate = seriesDto.Rate;
            series.Year = seriesDto.Year;
			series.ImageFileName = NewFileName;

			context.SaveChanges();
			return RedirectToAction("Index", "Series");
		}

		public IActionResult Delete(int id)
		{
			var series = context.Series?.Find(id);

			if (series == null)
			{
				return RedirectToAction("Index", "Series");
			}

			string ImageFullPath = environment.WebRootPath + "/seriesImages/" + series.ImageFileName;
			System.IO.File.Delete(ImageFullPath);

			context.Series?.Remove(series);
			context.SaveChanges();

			return RedirectToAction("Index", "Series");
		}

		[AllowAnonymous]
		public IActionResult Details(int id)
		{
			var series = context.Series?.Find(id);
			if (series == null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View(series);
		}
	}
}
