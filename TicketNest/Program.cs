using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TicketNest.Data;
using TicketNest.Models;
using TicketNest.Services;

namespace TicketNest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                                            .AddEntityFrameworkStores<ApplicationDbContext>()
                                            .AddDefaultTokenProviders();

            // Add application services.
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Add DI for Dotnetdesk
            builder.Services.AddTransient<IDotnetdesk, Dotnetdesk>();
            // Get SendGrid configuration options

            builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGridOptions"));

            // Get SMTP configuration options

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpOptions"));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}