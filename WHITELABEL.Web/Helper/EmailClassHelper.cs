namespace WHITELABEL.Web.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Web;
    public class EmailClassHelper
    {
        public void SendUserEmail(string ToEmail, string Subject, string Message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            try
            {
                SmtpClient client = new SmtpClient();

                client.Host = "103.240.91.147";
                client.Port = 25;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("connect@devconedutechservices.com", "basir12345@");
                //MailMessage message = CreateMailMessage(ToEmail, Subject, Message);
                MailMessage usermessage = CreateUserMailMessage(ToEmail, Subject, Message);
                //client.Send(message);
                client.Send(usermessage);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i <= ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if ((status == SmtpStatusCode.MailboxBusy) | (status == SmtpStatusCode.MailboxUnavailable))
                    {
                        System.Threading.Thread.Sleep(5000);
                        smtp.Send(mail);

                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

        }
        private MailMessage CreateMailMessage(string email, string sub, string msgbody)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            ////mail.IsBodyHtml = false;
            mailMessage.Priority = MailPriority.Normal;
            //mailMessage.From = new MailAddress("connect@devconedutechservices.com");
            //mailMessage.To.Add("atanucomp@gmail.com");
            //mailMessage.Body = "Test";
            //mailMessage.Subject = "TEst VAl";
            mailMessage.From = new MailAddress("connect@devconedutechservices.com");
            mailMessage.To.Add("connect@devconedutechservices.com");
            mailMessage.Body = msgbody.ToString();
            mailMessage.Subject = sub.ToString();
            return mailMessage;
        }
        private MailMessage CreateUserMailMessage(string email, string usersub, string usermsgbody)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            ////mail.IsBodyHtml = false;
            mailMessage.Priority = MailPriority.Normal;
            //mailMessage.From = new MailAddress("connect@devconedutechservices.com");
            //mailMessage.To.Add("atanucomp@gmail.com");
            //mailMessage.Body = "Test";
            //mailMessage.Subject = "TEst VAl";
            mailMessage.From = new MailAddress("connect@devconedutechservices.com");
            mailMessage.To.Add(email.ToString().Trim());
            mailMessage.Body = usermsgbody.ToString();
            mailMessage.Subject = usersub.ToString();
            return mailMessage;
        }
    }

}