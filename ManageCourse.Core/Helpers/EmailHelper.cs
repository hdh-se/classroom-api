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
        public bool SendConfirmMail (string userEmail, string confirmationLink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("tanhanh2kocean@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Confirm your email";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationLink;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Credentials = new System.Net.NetworkCredential("tanhanh2kocean@gmail.com", "ypkgs8tqb19APaxn");
            smtpClient.Host = "smtp-relay.sendinblue.com";
            smtpClient.Port = 587;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
