namespace WHITELABEL.Web.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Web;
    using Data;
    using Models;
    using System.Net;
    public class EmailHelper
    {
        private static string SMTPHost = System.Configuration.ConfigurationManager.AppSettings["SMTPHOST"];
        private static string SMTPPort = System.Configuration.ConfigurationManager.AppSettings["SMTPPORT"];
        private static string EMAILID = System.Configuration.ConfigurationManager.AppSettings["EMAILID"];
        private static string Password = System.Configuration.ConfigurationManager.AppSettings["EMAILIDPASSWORD"];
        public void SendUserEmail(string ToEmail, string Subject, string Message)
        {
           // MailMessage mail = new MailMessage();
            //SmtpClient smtp = new SmtpClient();
            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = "103.240.91.157";
                client.Port = 25;

                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("test@boomtravels.com", "India12345@");


                MailMessage message = CreateMailMessage();
                client.Send(message);
                //int SMTP_port = 0;
                //int.TryParse(SMTPPort,out SMTP_port);
                ////mail.From = new MailAddress("boomtravels786@gmail.com", "Boom Travels");
                //mail.From = new MailAddress(EMAILID, "Boom Travels");
                //mail.To.Add(ToEmail);
                //mail.Subject = Subject;
                //mail.Body = Message;
                ////mail.IsBodyHtml = true;
                //mail.IsBodyHtml = true;
                //mail.Priority = MailPriority.Normal;
                ////smtp.Host = "smtp.gmail.com";
                ////smtp.Port = 587;
                //smtp.Host = SMTPHost;
                //smtp.Port = SMTP_port;
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                ////smtp.Credentials = new System.Net.NetworkCredential("boomtravels786@gmail.com", "BoomTravels786");
                //smtp.Credentials = new System.Net.NetworkCredential(EMAILID, Password);
                //MailMessage message = CreateMailMessage(EMAILID, ToEmail, Subject, Message);
                ////smtp.EnableSsl = true;
                //smtp.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                //for (int i = 0; i <= ex.InnerExceptions.Length; i++)
                //{
                //    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                //    if ((status == SmtpStatusCode.MailboxBusy) | (status == SmtpStatusCode.MailboxUnavailable))
                //    {
                //        System.Threading.Thread.Sleep(5000);
                //        smtp.Send(mail);

                //    }
                //}

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

        }

        private MailMessage CreateMailMessage()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("test@boomtravels.com");
            mailMessage.To.Add("rahuls1017@gmail.com");
            mailMessage.Body = "Test";
            mailMessage.Subject = "TEst VAl";
            return mailMessage;
        }
        public string GetEmailTemplate(string Username, string Description, string TemplateName)
        {
            string body = string.Empty;
            string directory = HttpContext.Current.Server.MapPath("~/EmailTemplate");
            using (StreamReader reader = new StreamReader(directory + "\\" + TemplateName))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", Username);
            body = body.Replace("{Description}", Description);
            return body;
        }

        public string GetEmailTemplate(Dictionary<string, string> parameters, string TemplateName)
        {
            string body = string.Empty;
            string directory = HttpContext.Current.Server.MapPath("~/EmailTemplate");
            using (StreamReader reader = new StreamReader(directory + "\\" + TemplateName))
            {
                body = reader.ReadToEnd();
            }
            foreach (var pair in parameters)
            {
                body = body.Replace(pair.Key, pair.Value);
            }
            return body;
        }
    }
}