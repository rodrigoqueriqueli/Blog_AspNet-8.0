namespace Blog.Services
{
    public interface IEmailService
    {
        bool Send(
            string toName,
            string toEmail,
            string subject,
            string body,
            string fromName="Rodrigo Q",
            string fromEmail = "rqueriqueli@bt.com");
    }
}
