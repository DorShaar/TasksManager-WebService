using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Tasker.App.Services;
using Tasker.Infra.Options;

namespace Tasker.Infra.Services
{
    internal class NotifierService : INotifierService
    {
        private readonly ILogger<NotifierService> mLogger;
        private readonly IOptionsMonitor<TaskerConfiguration> mOptions;

        public NotifierService(IOptionsMonitor<TaskerConfiguration> options, ILogger<NotifierService> logger)
        {
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            mOptions = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Notify()
        {
            if (ShouldNotify())
            {
                await SendEmail(mOptions.CurrentValue.RecipientsToNotify, "Send Mail Test", mOptions.CurrentValue.Password)
                    .ConfigureAwait(false);
            }
        }

        private bool ShouldNotify()
        {
            return true;
        }

        public async Task SendEmail(List<string> recipients, string mailBody, string password)
        {
            const string fromEmail = "dordatas@gmail.com";
            const string userName = "dordatas";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Sending email from {fromEmail} to ");

            recipients.ForEach(recipient => stringBuilder.Append(recipient).Append(", "));
            mLogger.LogDebug(stringBuilder.ToString());

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(fromEmail);
                mail.Subject = "Task Notifier";
                mail.Body = mailBody;
                recipients.ToList().ForEach(recipient => mail.To.Add(recipient));

                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(userName, password);
                smtpClient.EnableSsl = true;

                // https://myaccount.google.com/lesssecureapps
                await smtpClient.SendMailAsync(mail).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Failed to send mail");
            }
        }
    }
}