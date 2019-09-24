using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using ZPF;
using ZPF.SQL;
using System.Net;
using System.Net.Mail;

namespace HorsDA
{
   public class MailController : Controller
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      [AllowCrossSiteJson]
      [Route("~/HorsDA/Mail/Test/{Addr}")]
      [HttpGet]
      public string SendMail([FromRoute] string Addr)
      {
         string Body = $"To ensure you receive our future emails such as maintenance notices and renewal notices, please add us to your contact list.\n\nHere is your code: {Addr}\n\n";

         return Send(Addr, "HorsDA Test Mail", Body );
      }

      private string Send(string email, string subject, string body)
      {
         string Result = "ok";

         try
         {
            SmtpClient MySMTPClient;
            MailMessage myEmail;

            MySMTPClient = new SmtpClient("mail.zeprogfactory.com", 25);
            MySMTPClient.Credentials = new NetworkCredential("support@zeprogfactory.com", "MossIsTheBoss!");

            myEmail = new MailMessage(new MailAddress("support@zeprogfactory.com"), new MailAddress(email));

            myEmail.Subject = subject;
            myEmail.Body = body;

            try
            {
               MySMTPClient.Send(myEmail);
            }
            catch (Exception ex)
            {
               Result = ex.Message.Replace("/r", "</br>") + "</br>";
               Result = Result + ex.StackTrace.Replace("/r", "</br>") + "</br>";
            };
         }
         catch (Exception ex)
         {
            Result = ex.Message;
         }

         return Result;
      }

      // string URL = $"http://wswiki.zpf.fr/mail/01?name{name}=&company={company}&email={email}&message={message}";

      [AllowCrossSiteJson]
      [Route("~/HorsDA/Mail/Send/{id}")]
      [HttpGet]
      public string SendMail([FromRoute] string id, [FromQuery] string name, [FromQuery] string company, [FromQuery] string email, [FromQuery] string message)
      {
         return Send(name, company, email, message);
      }

      private string Send(string name, string company, string email, string message)
      {
         string Result = "ok";

         try
         {
            SmtpClient MySMTPClient;
            MailMessage myEmail;

            MySMTPClient = new SmtpClient("mail.zeprogfactory.com", 25);
            MySMTPClient.Credentials = new NetworkCredential("support@zeprogfactory.com", "MossIsTheBoss!");

            myEmail = new MailMessage(new MailAddress("support@zeprogfactory.com"), new MailAddress("Support@ZPF.fr"));

            myEmail.Subject = "Website Contact Form:  " + name;
            myEmail.Body = string.Format(
               "You have received a new message from your website contact form.\n\nHere are the details:\nName: {0}\nEmail: {1}\nCompany: {2}\n\nMessage:\n{3}",
               name, email, company, message);

            try
            {
               MySMTPClient.Send(myEmail);
            }
            catch (Exception ex)
            {
               Result = ex.Message.Replace("/r", "</br>") + "</br>";
               Result = Result + ex.StackTrace.Replace("/r", "</br>") + "</br>";
            };
         }
         catch (Exception ex)
         {
            Result = ex.Message;
         }

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
