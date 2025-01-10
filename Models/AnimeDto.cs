using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
	public class AnimeDto
	{
		[Required]
		[MaxLength(200)]
		public string Title { get; set; } = "";

		[Range(1880, 2100)]
		public int Year { get; set; }

		[Required]
		[MaxLength(100)]
		public string Genre { get; set; } = "";

		[Range(0.0, 10.0)]
		public double Rate { get; set; }

		[MaxLength(2000)]
		public string Description { get; set; } = "";

		public IFormFile? ImageFile { get; set; }
	}
}
