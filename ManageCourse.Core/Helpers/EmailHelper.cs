using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Helpers
{
    public class EmailHelper
    {
        public bool SendConfirmMail (string userEmail, string confirmationLink, string subject = "Confirm your email")
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("tanhank2k@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationLink;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new System.Net.NetworkCredential("tanhank2k@gmail.com", "CQjLOwTzKtXMsExf");
            smtpClient.Host = "smtp-relay.sendinblue.com";
            smtpClient.Port = 587;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
