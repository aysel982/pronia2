using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proniam.DAL;
using Proniam.Models;

namespace Proniam
{
   
    public class Program
    {
       
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            }
            ).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });
            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllerRoute(
               "Admin",
               "{area:exists}/{controller=home}/{action=index}/{id?}");



            app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}/{id?}");

            app.Run();
        }
    }
}
