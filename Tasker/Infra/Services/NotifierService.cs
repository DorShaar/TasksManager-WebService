using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Services;
using Tasker.Infra.Options;
using Triangle;

namespace Tasker.Infra.Services
{
    internal class NotifierService : INotifierService
    {
        private readonly ILogger<NotifierService> mLogger;
        private readonly IOptionsMonitor<TaskerConfiguration> mOptions;
        private readonly ITasksGroupService mTasksGroupService;

        public NotifierService(ITasksGroupService tasksGroupService,
            IOptionsMonitor<TaskerConfiguration> options,
            ILogger<NotifierService> logger)
        {
            mTasksGroupService = tasksGroupService ?? 
                throw new ArgumentNullException(nameof(tasksGroupService));
            mOptions = options ?? throw new ArgumentNullException(nameof(options));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Notify()
        {
            if (await ShouldNotify().ConfigureAwait(false))
            {
                await SendEmail(
                    mOptions.CurrentValue.RecipientsToNotify, 
                    "Send Mail Test", 
                    mOptions.CurrentValue.Password).ConfigureAwait(false);
            }
        }

        private async Task<bool> ShouldNotify()
        {
            // TODO Test + 

            // TODO build report from all tasks.
            CollectTaskMeasurementsToNotify();

            return false;
        }

        private async Task<List<TaskTriangle>> CollectTaskMeasurementsToNotify()
        {
            // TODO return Task Trignale + task name and task id.

            List<TaskTriangle> taskTriangles = new List<TaskTriangle>();

            IEnumerable<ITasksGroup> groups = await mTasksGroupService.ListAsync().ConfigureAwait(false);

            foreach (ITasksGroup group in groups)
            {
                IEnumerable<IWorkTask> tasks = group.GetAllTasks();

                foreach (IWorkTask task in tasks)
                {
                    if (task.TaskMeasurement == null)
                        continue;

                    if (task.TaskMeasurement.ShouldNotify())
                        taskTriangles.Add(task.TaskMeasurement);
                }
            }
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