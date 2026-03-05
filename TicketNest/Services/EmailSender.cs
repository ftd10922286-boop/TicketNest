using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Options;

namespace TicketNest.Services
{
    public class EmailSender : IEmailSender
    {
        public SendGridOptions _sendGridOptions { get; }
        public IDotnetdesk _dotnetdesk { get; }
        public SmtpOptions _smtpOptions { get; }

        public EmailSender(IOptions<SendGridOptions> sendGridOptions, IDotnetdesk dotnetdesk, IOptions<SmtpOptions> smtpOptions)
        {
            _sendGridOptions = sendGridOptions.Value;
            _dotnetdesk = dotnetdesk;
            _smtpOptions = smtpOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            _dotnetdesk.SendEmailBySendGridAsync(_sendGridOptions.Host, _sendGridOptions.SendGridKey, 
                                                 _sendGridOptions.FromEmail, 
                                                 _sendGridOptions.FromFullName, subject, message, email).Wait();

            //send email using smtp via dotnetdesk. uncomment to use it
            /*
            _dotnetdesk.SendEmailByGmailAsync(_smtpOptions.fromEmail,
                _smtpOptions.fromFullName,
                subject,
                message,
                email,
                email,
                _smtpOptions.smtpUserName,
                _smtpOptions.smtpPassword,
                _smtpOptions.smtpHost,
                _smtpOptions.smtpPort,
                _smtpOptions.smtpSSL).Wait();
                */

            return Task.CompletedTask;
        }

    }
}
