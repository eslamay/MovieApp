using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Services;

namespace MovieApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(
                options=>options.UseSqlServer(
                    builder.Configuration.GetConnectionString("constr")
                    ));

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
				options =>
				{
					options.Password.RequiredLength = 6;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
				}
				).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

			using (var scope = app.Services.CreateScope())
			{
				var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
					as UserManager<ApplicationUser>;

				var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
					as RoleManager<IdentityRole>;

				await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
			}

			app.Run();
        }
    }
}
