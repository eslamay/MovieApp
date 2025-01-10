using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models;

namespace MovieApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public AccountController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}
		public IActionResult Register()
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}

			if (!ModelState.IsValid)
			{
				return View(registerDto);
			}

			//Create new account and authenticate the user
			var user = new ApplicationUser()
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				Address = registerDto.Address,
				UserName = registerDto.Email,
				CreatedAt = DateTime.Now,
			};

			var result = await userManager.CreateAsync(user, registerDto.Password);

			if (result.Succeeded)
			{
				//successful user registeration
				await userManager.AddToRoleAsync(user, "client");

				//Sign in the new user
				await signInManager.SignInAsync(user, false);

				return RedirectToAction("Index", "Home");
			}


			//registeration faild=> show registration errors
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}



			return View(registerDto);
		}

		public async Task<IActionResult> Logout()
		{
			if (signInManager.IsSignedIn(User))
			{
				await signInManager.SignOutAsync();
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Login()
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			if (signInManager.IsSignedIn(User))
			{
				return RedirectToAction("Index", "Home");
			}

			if (!ModelState.IsValid)
			{
				return View(loginDto);
			}

			var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,
				loginDto.RememberMe, false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.ErrorMessage = "Invalid login attempt";
			}

			return View(loginDto);
		}

		[Authorize]
		public async Task<IActionResult> Profile()
		{
			var appUser = await userManager.GetUserAsync(User);
			if (appUser == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var profileDto = new ProfileDto()
			{
				FirstName = appUser.FirstName,
				LastName = appUser.LastName,
				Email = appUser.Email ?? "",
				PhoneNumber = appUser.PhoneNumber,
				Address = appUser.Address,
			};
			return View(profileDto);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Profile(ProfileDto profileDto)
		{

			if (!ModelState.IsValid)
			{
				ViewBag.ErrorMessage = "Please fill all required fields with valid values";
				return View(profileDto);
			}

			ViewBag.SuccessMessage = "Profile updated successfully";

			//Get the current user
			var appUser = await userManager.GetUserAsync(User);
			if (appUser == null)
			{
				return RedirectToAction("Index", "Home");
			}
			//Update the user profile
			appUser.FirstName = profileDto.FirstName;
			appUser.LastName = profileDto.LastName;
			appUser.UserName = profileDto.Email;
			appUser.Email = profileDto.Email;
			var phoneNumber = profileDto.PhoneNumber;
			var address = profileDto.Address;

			var result = await userManager.UpdateAsync(appUser);

			if (result.Succeeded)
			{
				ViewBag.SuccessMessage = "Profile updated successfully";
			}
			else
			{
				ViewBag.ErrorMessage = "Unable to update the profile : " + result.Errors.First().Description;
			}

			return View(profileDto);
		}

		[Authorize]
		public IActionResult Password()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Password(PasswordDto passwordDto)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			//Get the current user
			var appUser = await userManager.GetUserAsync(User);
			if (appUser == null)
			{
				return RedirectToAction("Index", "Home");
			}

			//Update the password
			var result = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);

			if (result.Succeeded)
			{
				ViewBag.SuccessMessage = "Password updated successfully";
			}
			else
			{
				ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
			}

			return View();
		}

		public IActionResult AccessDenied()
		{
			return RedirectToAction("Index", "Home");
		}

		[Authorize(Roles = "admin")]
		public IActionResult Users()
		{
			var users = userManager.Users.ToList();
			return View(users);
		}
	}
}
