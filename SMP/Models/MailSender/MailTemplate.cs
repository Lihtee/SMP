using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SMP.Models.MailSender
{
    /// <summary>
    /// Класс - шаблон для письма. Определяет получателя, тему сооб
    /// </summary>
    public abstract class MailTemplate
    {
        protected string receiverEmail;
        protected Project p;

        protected abstract string Topic { get; }

        protected abstract string Body { get; }

        public MailTemplate(string _receiverEmail, Project _p)
        {
            this.receiverEmail = _receiverEmail;
            this.p = _p;
        }


        public virtual MailMessage GetHTMLtext()
        {
            MailMessage res = new MailMessage();
            res.To.Add(receiverEmail);
            res.Body = Body;
            res.Subject = Topic;
            res.IsBodyHtml = true;
            return res;
        }
    }

    public class NewWorkTemplate : MailTemplate
    {
        public NewWorkTemplate(string _receiverEmail, Project _p) : base(_receiverEmail, _p){}

        protected override string Topic => "Новая работа";

        protected override string Body =>
                        $"<h2>У вас появилось новая работа: {p.projectName}</h2>" +
                        $"<p>Дедлайн: {p.endDateTime.ToShortDateString()}, {p.endDateTime.ToShortTimeString()} </p>" +
                        $"<p>Описание работы: {p.description}</p>";

    }

    public class WorkChangedMail : MailTemplate
    {
        public WorkChangedMail(string _receiverEmail, Project _p) : base(_receiverEmail, _p)
        {
        }

        protected override string Topic => "Изменения в работе";
        protected override string Body =>
                        $"<h2>У вас изменилась работа: {p.projectName}</h2>" +
                        $"<p>Новый дедлайн: {p.endDateTime.ToShortDateString()}, {p.endDateTime.ToShortTimeString()} </p>" +
                        $"<p>Новое описание работы: {p.description}</p>";

    }

    public class WordCanceledMail : MailTemplate
    {
        public WordCanceledMail(string _receiverEmail, Project _p) : base(_receiverEmail, _p)
        {
        }

        protected override string Topic => "Отмена работы";
        protected override string Body =>
                        $"<h2>Ваша работа {p.projectName} была отменена.</h2>";
                        

    }
}