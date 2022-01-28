using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ManageCourse.Core.Helpers
{
    public class EmailHelper


    {
        private readonly string SmtpHost = "smtp.gmail.com";
        private readonly int SmtpPort = 587;
        private readonly string SmtpUser = "ledungpython@gmail.com";
        // TODO: CHANGE PASS
        private readonly string SmtpPass = "Mystrongpassword00@1999";

        public bool SendConfirmMail(string userEmail, string confirmationLink, string subject = "Confirm your email")
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(SmtpUser));
                email.To.Add(MailboxAddress.Parse(userEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) {Text = confirmationLink};

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(SmtpUser, SmtpPass);
                smtp.Send(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}