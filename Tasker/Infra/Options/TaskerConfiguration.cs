using System;
using System.Collections.Generic;
using System.IO;

namespace Tasker.Infra.Options
{
    public class TaskerConfiguration
    {
        public TimeSpan NotifierInterval { get; set; } = TimeSpan.FromSeconds(60);
        public List<string> RecipientsToNotify { get; set; } = new List<string>();
        public string Password { get; set; }
        public string PasswordPath { get; set; } = @"C:\Users\dor.shaar.CORP\Desktop\temp.txt";

        public TaskerConfiguration()
        {
            try
            {
                Password = File.ReadAllText(PasswordPath);
            }
            catch (FileNotFoundException)
            {
                Password = null;
            }

            RecipientsToNotify.Add("dordatas@gmail.com");
        }
    }
}