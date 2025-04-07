using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Net.Mail;
using System.Net;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IRazorViewToStringRenderer _razorRenderer;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IRazorViewToStringRenderer razorRenderer)
        {
            _configuration = configuration;
            _logger = logger;
            _razorRenderer = razorRenderer;
        }

        public async Task SendEmailVerificationAsync(string email, string fullName, string verificationUrl)
        {
            var model = new EmailVerificationModel
            {
                FullName = fullName,
                VerificationUrl = verificationUrl
            };

            string emailBody = await _razorRenderer.RenderViewToStringAsync("EmailVerification", model);
            await SendEmailAsync(email, "Verify Your Email for Dhaka Metro Rail Account", emailBody);
        }

        public async Task SendPasswordResetEmailAsync(string email, string fullName, string resetUrl)
        {
            var model = new PasswordResetModel
            {
                FullName = fullName,
                ResetUrl = resetUrl
            };

            string emailBody = await _razorRenderer.RenderViewToStringAsync("PasswordReset", model);
            await SendEmailAsync(email, "Password Reset Request for Dhaka Metro Rail Account", emailBody);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}: {ex.Message}");
                throw;
            }
        }
    }
}
