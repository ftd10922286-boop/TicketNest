using Microsoft.AspNetCore.Identity;
using TicketNest.Data;
using TicketNest.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace TicketNest.Services
{
    public interface IDotnetdesk
    {
        Task SendEmailBySendGridAsync(string host,string apiKey, 
                                      string fromEmail, string fromFullName, string subject, string message, string email);

        Task<bool> IsAccountActivatedAsync(string email, UserManager<ApplicationUser> userManager);

        Task SendEmailByGmailAsync(string fromEmail,
                                   string fromFullName,
                                   string subject,
                                   string messageBody,
                                   string toEmail,
                                   string toFullName,
                                   string smtpUser,
                                   string smtpPassword,
                                   string smtpHost,
                                   int smtpPort,
                                   bool smtpSSL);

        Task CreateDefaultOrganization(string applicationUserId, ApplicationDbContext context);

        Task<string> UploadFile(List<IFormFile> files, IHostingEnvironment env);

    }
}
