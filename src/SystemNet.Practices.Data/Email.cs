using System;
using System.Net;
using System.Net.Mail;
using SystemNet.Shared;

namespace SystemNet.Practices.Data
{
    public static class Email
    {
        public static SmtpClient smtpClient()
        {
            SmtpClient ClientSmtp = new SmtpClient();
            ClientSmtp.EnableSsl = true;
            ClientSmtp.UseDefaultCredentials = false;
            ClientSmtp.Credentials =  new NetworkCredential(Runtime.User.ToLower(),Runtime.Password);
            ClientSmtp.Host = Runtime.Smtp.ToLower();
            ClientSmtp.Port = Convert.ToInt32(Runtime.Port);
            ClientSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            return ClientSmtp;
        }

    }
}
