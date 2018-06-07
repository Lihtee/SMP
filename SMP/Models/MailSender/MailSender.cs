using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SMP.Models.MailSender
{
    public class MailSender
    {
        private string systemEmail;

        public MailSender()
        {
            this.systemEmail = "smp.notifier@gmail.com";
        }
        public bool Send(string from, string to, string content)
        {
            throw new NotImplementedException();
        }
        public bool Send(string receiverMail, string content)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(systemEmail, "Уведомитель SMP");
            // кому отправляем
            MailAddress to = new MailAddress(receiverMail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Уведомление";
            // текст письма
            m.Body = $"<h2>{content}</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                // логин и пароль
                smtp.Credentials = new NetworkCredential("smp.notifier@gmail.com", "smp.notifier2018");
                smtp.EnableSsl = true;
                try
                {
                    smtp.Send(m);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }

            }
            return true;
        }
    }
}