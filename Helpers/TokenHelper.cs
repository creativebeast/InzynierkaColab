using MimeKit;
using System.Net;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
//using System.Net.Mail;

namespace Inzynierka.Helpers
{
    public static class TokenHelper
    {
        public static string CreateToken()
        {
            // Creating object of random class
            Random rand = new Random();

            string str = "";
            char letter;
            for (int i = 0; i < 5; i++)
            {

                // Generating random character by converting
                // the random number into character.
                letter = Convert.ToChar(rand.Next(0, 26) + 65);

                // Appending the letter to string.
                str += letter;
            }
            return str;
        }

        public static string CreateNumericToken(int size)
        {
            // Creating object of random class
            Random rand = new Random();

            string str = "";
            for (int i = 0; i < size; i++)
            {
                str += rand.Next(0, 9).ToString();
            }
            return str;
        }

        public static void SendTokenViaMail(string targetEmail, string token)
        {
            string senderEmail = "inzynierkaomiel@gmail.com";
            string senderPswr = "oyltyhlhbcgkvxld";
            var email = new MimeMessage();
            try
            {
                email.From.Add(new MailboxAddress("Sender Name", senderEmail));
                email.To.Add(new MailboxAddress("Receiver Name", targetEmail));

                email.Subject = "Authentication Code - Inzynierka";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"<b>Here is your authentication code: <h3>{token}</h3></b>"
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(senderEmail, senderPswr);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong: " + e);
            }
            
        }
    }
}
