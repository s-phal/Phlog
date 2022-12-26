using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Phlog.Data;
using Phlog.Models;
using Phlog.Services;

namespace Phlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));


            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Custom services - SP
            builder.Services.AddTransient<ImageService>();
            builder.Services.AddTransient<TagService>();
            builder.Services.AddRazorPages(); 

            builder.Services.AddIdentity<SiteOwner, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI() // Add 
                .AddDefaultTokenProviders() // Add
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Posts}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}