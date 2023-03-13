using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebApp.Strategy.Models;
using WebApp.Strategy.Repositories;

namespace BaseProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IProductRepository>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var claim = httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault();
                var context = sp.GetRequiredService<AppIdentityDbContext>();

                if (claim == null) return new ProductRepositoryFromSqlServer(context);

                var databaseType = (EDatabaseType)int.Parse(claim.Value);

                return databaseType switch
                {
                    EDatabaseType.SqlServer => new ProductRepositoryFromSqlServer(context),
                    EDatabaseType.MongoDb => new ProductRepositoryFromMongoDb(builder.Configuration),
                    _ => throw new NotImplementedException()
                };

            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                // IdentityDbContext'ten bir nesne örneði alýnarak örnek user'lar oluþturulacak

                var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                identityDbContext.Database.Migrate(); // Uygulama ayaða kalktýðýnda otomatik migrate eder.

                if (!userManager.Users.Any())
                {
                    userManager.CreateAsync(new User() { UserName = "user1", Email = "user1@gmail.com" }, "Password12*").Wait();
                    userManager.CreateAsync(new User() { UserName = "user2", Email = "user2@gmail.com" }, "Password12*").Wait();
                    userManager.CreateAsync(new User() { UserName = "user3", Email = "user3@gmail.com" }, "Password12*").Wait();
                    userManager.CreateAsync(new User() { UserName = "user4", Email = "user4@gmail.com" }, "Password12*").Wait();
                    userManager.CreateAsync(new User() { UserName = "user5", Email = "user5@gmail.com" }, "Password12*").Wait();
                }

            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}