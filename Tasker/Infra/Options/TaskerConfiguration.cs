using System;
using System.Collections.Generic;
using System.IO;

namespace Tasker.Infra.Options
{
    public class TaskerConfiguration
    {
        public TimeSpan NotifierInterval { get; set; }
        public TimeSpan SummaryEmailInterval { get; set; }
        public int SummaryEmailHour { get; set; }
        public List<string> RecipientsToNotify { get; set; } = new List<string>();
        public string PasswordPath { get; set; }
        public string Password => GetPassword();

        private string GetPassword()
        {
            try
            {
                return File.ReadAllText(PasswordPath);
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return null;
            }
        }

        public TaskerConfiguration()
        {
            RecipientsToNotify.Add("dordatas@gmail.com");
        }
    }
}