using ManageCourse.Core.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Core.Helpers
{
    public class EmailService : IEmailService
    {
        private readonly string SmtpHost = "smtp.gmail.com";
        private readonly int SmtpPort = 587;
        private readonly string SmtpUser = "TRƯỜNG_SA_HOÀNG_SA_LÀ_CỦA_VIỆT_NAM_NHÉ@gmail.com";
        private readonly string SmtpPass = "CŨNG_LÀ__TRƯỜNG_SA_HOÀNG_SA_LÀ_CỦA_VIỆT_NAM_NHÉ_NHƯNG_MÀ_LÀ_PASSWORD";

        public EmailService()
        {

        }

        public void Send(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(SmtpUser));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(SmtpUser, SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
