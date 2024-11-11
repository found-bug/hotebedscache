using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp; 
using SendGrid;
using SendGrid.Helpers.Mail;
using Hotebedscache.Domain.Models;

namespace Hotebedscache.Service.Helper
{
    public class EmailHelper
    {
        private string to { get; set; }
        private string from { get; set; }
        private string subject { get; set; }
        private string plainTextMesage { get; set; }
        private string htmlMessage { get; set; }
        private string apiKey { get; set; }
        private string displayName { get; set; }


        public EmailHelper(string to, string from, string subject, string plainTextMesage, string htmlMessage, string apiKey, string displayName = null)
        {
            this.to = to;
            this.from = from;
            this.subject = subject;
            this.plainTextMesage = plainTextMesage;
            this.displayName = displayName;
            this.apiKey = apiKey;
            this.htmlMessage = htmlMessage;
        }
        public EmailHelper() { }

        /// <summary>
        /// Use this function for Godaday.com 
        /// Specially Use System.Net.Mail so that can send email instead of MailKit.Net.Smtp
        /// Not in used now
        /// </summary>
        private async Task<bool> EmailSenderAsync(EmailModel mailDetails)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress(mailDetails.User, "Message from Hotebedscache");
                message.To.Add(new System.Net.Mail.MailAddress(to));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlMessage;

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(mailDetails.Server, Convert.ToInt32(mailDetails.Port));

                await client.SendMailAsync(message).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Not in used now
        /// </summary>
        /// <param name="mailDetails"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(EmailModel mailDetails)
        {
            try
            {
                // This code is obsolete, because we have to use System.Net.Mail library instead of mailkit.
                // Mailkit technique

                EmailSender objEmailSender = new EmailSender();
                bool result = await objEmailSender.SendEmailAsync(mailDetails, to, from, subject, plainTextMesage, htmlMessage, "");
                return result;

                //System.Net.Mail technique
                //bool result = await EmailSenderAsync(mailDetails);
                //return result;
            }
            catch (Exception exp)
            {

                throw exp;
            }
        }

        /// <summary>
        /// Using sand grid
        /// </summary> 
        /// <returns>bool</returns>
        public async Task<bool> SendGridEmail()
        {
            try
            {
                var client = new SendGridClient(apiKey);
                var emailFrom = new EmailAddress(from, displayName != null ? "Message from " + displayName :  "Message from Hotebedscache"); ;
                var subjectLine = subject;
                var emailTo = new EmailAddress(to);
                var plainTextContent = "";
                var htmlContent = htmlMessage;
                var msg = MailHelper.CreateSingleEmail(emailFrom, emailTo, subjectLine, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// Not in used now
    /// </summary>
    public class EmailSender
    {
        public EmailSender()
        {
        }
        public async Task<bool> SendEmailAsync(EmailModel mailDetails, string to, string from,
                                               string subject, string plainTextMessage, string htmlMessage,
                                               string replyTo = null)
        {
            bool hasPlainText = !string.IsNullOrWhiteSpace(plainTextMessage);
            bool hasHtml = !string.IsNullOrWhiteSpace(htmlMessage);
            var message = new MimeMessage();

            #region Argument Exceptions
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("no To address provided");
            }
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("no from address provided");
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("no subject provided");
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("no message provided");
            }
            #endregion

            message.From.Add(new MailboxAddress("Hotebedscache.org", from));
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                message.ReplyTo.Add(new MailboxAddress("", replyTo));
            }
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = plainTextMessage;
            }
            if (hasHtml)
            {
                bodyBuilder.HtmlBody = htmlMessage;
            }

            message.Body = bodyBuilder.ToMessageBody();
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(
                        mailDetails.Server,
                        mailDetails.Port,
                        SecureSocketOptions.StartTlsWhenAvailable).ConfigureAwait(true);

                    // XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    if (mailDetails.RequiresAuthentication)
                    {
                        await client.AuthenticateAsync(mailDetails.User, mailDetails.Password)
                                    .ConfigureAwait(false);
                    }
                    await client.SendAsync(message).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
