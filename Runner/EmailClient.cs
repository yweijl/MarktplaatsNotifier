using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Runner
{
    public static class EmailClient
    {
        public static void Send(List<Advertisement> Items)
        {
            if (Items == null)
            {
                throw new ArgumentNullException(nameof(Items));
            }

            if (Items.Count == 0)
            {
                return;
            }

            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("opnieuwhipnotifier@gmail.com", "VUNG9chok3dut!plak"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("opnieuwhipnotifier@gmail.com"),
                Subject = "Nieuwe advertenties",
                Body = Items.CreateBody(),
                IsBodyHtml = true,
            };
            mailMessage.To.Add("opnieuwhipnotifier@gmail.com");
            client.Send(mailMessage);
        }
    }
}
