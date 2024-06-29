
namespace CRET.Domain.Models.DataModel
{
    public class EmailRequest
    {
        public EmailRequest()
        {
        }
        public EmailRequest(string toEmail, string subject, string body, bool isBodyHtml)
        {
            this.ToEmail = toEmail;
            this.Subject = subject;
            this.Body = body;
            this.IsBodyHtml = isBodyHtml;
        }

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
