using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp.Controllers
{
	[Authorize(Roles = "admin")]
	[Route("/Admin/[controller]/{action=Index}/{id?}")]
	public class AnimesController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;
		private readonly int pageSize = 5;

		public AnimesController(ApplicationDbContext context,IWebHostEnvironment environment) 
		{
			this.context = context;
			this.environment = environment;
		}
		public IActionResult Index(int pageIndex,string?search, string? column, string? orderBy)
		{
			IQueryable<Anime> query = context.Animes!;
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


			var animes= query?.ToList();

			ViewData["PageIndex"] = pageIndex;
			ViewData["TotalPages"] = totalPages;

			ViewData["Search"] = search ?? "";

			ViewData["Column"] = column;
			ViewData["OrderBy"] = orderBy;

			return View(animes);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(AnimeDto animeDto)
		{
			if (animeDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The Image File is required");
			}

			if (!ModelState.IsValid)
			{
				return View(animeDto);
			}

			//Save The Image File
			string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			NewFileName += Path.GetExtension(animeDto.ImageFile!.FileName);

			string imageFullPath = environment.WebRootPath + "/animeImages/" + NewFileName;

			using (var stream = System.IO.File.Create(imageFullPath))
			{
				animeDto.ImageFile.CopyTo(stream);
			}

			//Save The new product in database
			Anime anime = new Anime()
			{
				Title = animeDto.Title,
				Genre = animeDto.Genre,
				Description = animeDto.Description,
				Rate = animeDto.Rate,
				Year = animeDto.Year,
				ImageFileName = NewFileName,
			};

			context.Animes?.Add(anime);
			context.SaveChanges();

			return RedirectToAction("Index", "Animes");
		}

		public IActionResult Edit(int id)
		{
			var anime = context.Animes?.Find(id);

			if (anime == null)
			{
				return RedirectToAction("Index", "Animes");
			}

			var animeDto = new AnimeDto()
			{
				Title=anime.Title,
				Genre=anime.Genre,
				Rate=anime.Rate,
				Year=anime.Year,
				Description = anime.Description,
			};

			ViewData["AnimeId"] = anime.Id;
			ViewData["ImageFileName"] = anime.ImageFileName;
			return View(animeDto);
		}

		[HttpPost]
		public IActionResult Edit(int id, AnimeDto animeDto)
		{
			var anime = context.Animes?.Find(id);

			if (animeDto == null)
			{
				return RedirectToAction("Index", "Animes");
			}

			if (!ModelState.IsValid)
			{
				ViewData["AnimeId"] = anime?.Id;
				ViewData["ImageFileName"] = anime?.ImageFileName;

				return View(animeDto);
			}


			string NewFileName = anime.ImageFileName;
			if (animeDto.ImageFile != null)
			{
				//Update The Image File
				NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				NewFileName += Path.GetExtension(animeDto.ImageFile!.FileName);

				string imageFullPath = environment.WebRootPath + "/animeImages/" + NewFileName;

				using (var stream = System.IO.File.Create(imageFullPath))
				{
					animeDto.ImageFile.CopyTo(stream);
				}

				//Delete old image file
				string OldImageFullPath = environment.WebRootPath + "/animeImages/" + anime.ImageFileName;
				System.IO.File.Delete(OldImageFullPath);
			}

			//update product in database
			anime.Title = animeDto.Title;
			anime.Description = animeDto.Description;
			anime.Genre = animeDto.Genre;
            anime.Rate = animeDto.Rate;
            anime.Year = animeDto.Year;
			anime.ImageFileName = NewFileName;

			context.SaveChanges();
			return RedirectToAction("Index", "Animes");
		}

		public IActionResult Delete(int id)
		{
			var anime = context.Animes?.Find(id);

			if (anime == null)
			{
				return RedirectToAction("Index", "Animes");
			}

			string ImageFullPath = environment.WebRootPath + "/animeImages/" + anime.ImageFileName;
			System.IO.File.Delete(ImageFullPath);

			context.Animes?.Remove(anime);
			context.SaveChanges();

			return RedirectToAction("Index", "Animes");
		}

		[AllowAnonymous]
		public IActionResult Details(int id)
		{
			var anime = context.Animes?.Find(id);
			if (anime == null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View(anime);
		}
	}
}
