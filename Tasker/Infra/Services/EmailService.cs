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
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> mLogger;
        private readonly IOptionsMonitor<TaskerConfiguration> mOptions;

        public EmailService(IOptionsMonitor<TaskerConfiguration> options,
                ILogger<EmailService> logger)
        {
            mOptions = options ?? throw new ArgumentNullException(nameof(options));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmail(string mailBody)
        {
            const string fromEmail = "dordatas@gmail.com";
            const string userName = "dordatas";

            List<string> recipients = mOptions.CurrentValue.RecipientsToNotify;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Sending email from ").Append(fromEmail).Append(" to ");

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
                smtpClient.Credentials = new System.Net.NetworkCredential(userName, mOptions.CurrentValue.Password);
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