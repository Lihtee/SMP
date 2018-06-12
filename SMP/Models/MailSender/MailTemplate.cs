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
        protected string topic;

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
            res.Subject = topic;
            res.IsBodyHtml = true;
            return res;
        }
    }

    public class NewWorkTemplate : MailTemplate
    {
        public NewWorkTemplate(string _receiverEmail, Project _p) : base(_receiverEmail, _p)
        {
            this.topic = "Новая работа";
        }

        protected override string Body =>
                        $"<h2>У вас появилось новая работа: {p.projectName}</h2>" +
                        $"<p>Дедлайн: {p.endDateTime.ToShortDateString()}, {p.endDateTime.ToShortTimeString()} </p>" +
                        $"<p>Описание работы: {p.description}</p>";

        public override MailMessage GetHTMLtext()
        {
            MailMessage res = base.GetHTMLtext();
            return res;
        }
    }
}