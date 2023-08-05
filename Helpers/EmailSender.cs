using System.Diagnostics;
using System.Net;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Inzynierka.Helpers
{
    //public interface IEmailSender
    //{
    //    Task SendEmailAsync(string email, string subject, string message);
    //}

    public class EmailSender
    {
        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    //https://10-minutemail.com/temporary-email/30-minutes-mail
        //    var client = new SmtpClient("smtp.gmail.com", 587)
        //    {
        //        EnableSsl = true,
        //        UseDefaultCredentials = false,
                
        //        Credentials = new NetworkCredential("inzynierkaOmiel@gmail.com", "OrzechowyKasztan123")
        //    };

        //    return client.SendMailAsync(
        //        new MailMessage(from: "inzynierkaOmiel@gmail.com",
        //                        to: email,
        //                        subject,
        //                        message
        //                        ));
        //}
    }

}
