using System;
using System.Net;
using System.Net.Mail;
using SystemNet.Shared;

namespace SystemNet.Practices.Data
{
    public static class Email
    {
        public static SmtpClient SmtpClient()
        {
            SmtpClient ClientSmtp = new SmtpClient
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Runtime.User.ToLower(), Runtime.Password),
                Host = Runtime.Smtp.ToLower(),
                Port = Convert.ToInt32(Runtime.Port),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            return ClientSmtp;
        }

    }
}
