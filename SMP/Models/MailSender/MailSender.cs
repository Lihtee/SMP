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
        private string systemPass;
        private string SMTPadress;
        private int SMTPport;

        public MailSender()
        {
            this.systemEmail = "smp.notifier@gmail.com";
            this.systemPass = "smp.notifier2018";
            this.SMTPadress = "smtp.gmail.com";
            this.SMTPport = 587;
        }
        
        public bool SendNewWork(string receiverMail, Project work)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(systemEmail, "Уведомитель SMP");
            // кому отправляем
            MailAddress to = new MailAddress(receiverMail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Новая работа";
            // текст письма
            m.Body = $"<h2>У вас появилось новая работа: {work.projectName}</h2></br>" +
                        $"Дедлайн: {work.endDateTime.ToShortDateString()}, {work.endDateTime.ToShortTimeString()} </br>" +
                        $"<b>Описание работы:</b> {work.description}";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            using (SmtpClient smtp = new SmtpClient(SMTPadress, SMTPport))
            {
                // логин и пароль
                smtp.Credentials = new NetworkCredential(systemEmail, systemPass);
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

        public bool SendChangeWork(string receiverMail, Project work)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(systemEmail, "Уведомитель SMP");
            // кому отправляем
            MailAddress to = new MailAddress(receiverMail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Изменение в  условиях работы";
            // текст письма
            m.Body = $"<h2>У вас изменилась работа: {work.projectName}</h2></br>" +
                        $"Новый дедлайн: {work.endDateTime.ToShortDateString()}, {work.endDateTime.ToShortTimeString()} </br>" +
                        $"<b>Новое описание работы:</b> {work.description}";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            using (SmtpClient smtp = new SmtpClient(SMTPadress, SMTPport))
            {
                // логин и пароль
                smtp.Credentials = new NetworkCredential(systemEmail, systemPass);
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

        public bool SendCanceledWork(string receiverMail, Project work)
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress(systemEmail, "Уведомитель SMP");
            // кому отправляем
            MailAddress to = new MailAddress(receiverMail);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Отмена работы";
            // текст письма
            m.Body = $"<h2>Работа была отменена:{work.projectName}</h2></br>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            using (SmtpClient smtp = new SmtpClient(SMTPadress, SMTPport))
            {
                // логин и пароль
                smtp.Credentials = new NetworkCredential(systemEmail, systemPass);
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