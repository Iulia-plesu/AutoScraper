using System.Net.Mail;
using System.Net;
using DataLink.Models;

namespace DataLink.Services
{
    public interface IEmailService
    {
        Task SendNewsDigestAsync(ScrapedData newsData);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendNewsDigestAsync(ScrapedData newsData)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                var fromEmail = smtpSettings["FromEmail"] ??
                    throw new InvalidOperationException("FromEmail is not configured in appsettings.json");
                var toEmail = smtpSettings["ToEmail"] ??
                    throw new InvalidOperationException("ToEmail is not configured in appsettings.json");
                var smtpServer = smtpSettings["SmtpServer"] ??
                    throw new InvalidOperationException("SmtpServer is not configured in appsettings.json");
                var portStr = smtpSettings["Port"] ??
                    throw new InvalidOperationException("Port is not configured in appsettings.json");
                var username = smtpSettings["Username"] ??
                    throw new InvalidOperationException("Username is not configured in appsettings.json");
                var password = smtpSettings["Password"] ??
                    throw new InvalidOperationException("Password is not configured in appsettings.json");

                if (!int.TryParse(portStr, out var port))
                {
                    throw new InvalidOperationException($"Invalid port number: {portStr}");
                }

                using var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = $"News Digest - {DateTime.Now:MMMM dd, yyyy}",
                    IsBodyHtml = true,
                    Body = BuildEmailBody(newsData)
                };

                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("News digest email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send news digest email");
                throw;
            }
        }

        private string BuildEmailBody(ScrapedData newsData)
        {
            var body = new System.Text.StringBuilder();
            body.AppendLine("<html><body style='font-family: Arial, sans-serif;'>");
            body.AppendLine("<h1 style='color: #333;'>Daily News Digest</h1>");
            body.AppendLine($"<p>Generated on {DateTime.Now:MMMM dd, yyyy HH:mm}</p>");

            // Add categories
            foreach (var category in newsData.Categories)
            {
                body.AppendLine($"<h2 style='color: #2c5282; margin-top: 20px;'>{category.Key}</h2>");
                body.AppendLine("<ul style='list-style-type: none; padding: 0;'>");

                foreach (var article in category.Value)
                {
                    body.AppendLine("<li style='margin-bottom: 15px;'>");
                    body.AppendLine($"<a href='{article.Url}' style='color: #2b6cb0; text-decoration: none; font-weight: bold;'>{article.Title}</a>");

                    if (!string.IsNullOrEmpty(article.Timestamp))
                    {
                        body.AppendLine($"<span style='color: #666; font-size: 0.9em;'> - {article.Timestamp}</span>");
                    }

                    if (article.Tags.Any())
                    {
                        body.AppendLine("<div style='margin-top: 5px;'>");
                        foreach (var tag in article.Tags)
                        {
                            body.AppendLine($"<span style='background-color: #e2e8f0; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; margin-right: 5px;'>{tag}</span>");
                        }
                        body.AppendLine("</div>");
                    }

                    body.AppendLine("</li>");
                }

                body.AppendLine("</ul>");
            }

            // Add other articles if any
            if (newsData.Other.Any())
            {
                body.AppendLine("<h2 style='color: #2c5282; margin-top: 20px;'>Other News</h2>");
                body.AppendLine("<ul style='list-style-type: none; padding: 0;'>");

                foreach (var article in newsData.Other)
                {
                    body.AppendLine("<li style='margin-bottom: 15px;'>");
                    body.AppendLine($"<a href='{article.Url}' style='color: #2b6cb0; text-decoration: none; font-weight: bold;'>{article.Title}</a>");

                    if (!string.IsNullOrEmpty(article.Timestamp))
                    {
                        body.AppendLine($"<span style='color: #666; font-size: 0.9em;'> - {article.Timestamp}</span>");
                    }

                    if (article.Tags.Any())
                    {
                        body.AppendLine("<div style='margin-top: 5px;'>");
                        foreach (var tag in article.Tags)
                        {
                            body.AppendLine($"<span style='background-color: #e2e8f0; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; margin-right: 5px;'>{tag}</span>");
                        }
                        body.AppendLine("</div>");
                    }

                    body.AppendLine("</li>");
                }

                body.AppendLine("</ul>");
            }

            body.AppendLine("</body></html>");
            return body.ToString();
        }
    }
}
