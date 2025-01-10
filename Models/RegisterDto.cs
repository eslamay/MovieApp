using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
	public class RegisterDto
	{
		[Required(ErrorMessage = "The First Name field is required "), MaxLength(100)]
		public string FirstName { get; set; } = "";

		[Required(ErrorMessage = "The Last Name field is required "), MaxLength(100)]
		public string LastName { get; set; } = "";

		[Required, EmailAddress, MaxLength(200)]
		public string Email { get; set; } = "";

		[Required, MaxLength(200)]
		public string Address { get; set; } = "";

		[Required(ErrorMessage = "The format of Phone Number is not valid"), MaxLength(20)]
		public string? PhoneNumber { get; set; }

		[Required, MaxLength(100)]
		public string Password { get; set; } = "";

		[Required(ErrorMessage = "The Confirm Password field is required")]
		[Compare("Password", ErrorMessage = "The Confirm Password and Password do not match")]
		public string ConfirmPassword { get; set; } = "";
	}
}
