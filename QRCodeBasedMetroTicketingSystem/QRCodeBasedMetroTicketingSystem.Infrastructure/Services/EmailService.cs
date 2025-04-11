using Microsoft.Extensions.Configuration;
using QRCodeBasedMetroTicketingSystem.Application.Interfaces.Services;
using System.Net.Mail;
using System.Net;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;

namespace QRCodeBasedMetroTicketingSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IRazorViewToStringRenderer _razorRenderer;

        public EmailService(IConfiguration configuration, IRazorViewToStringRenderer razorRenderer)
        {
            _configuration = configuration;
            _razorRenderer = razorRenderer;
        }

        public async Task<bool> SendEmailVerificationAsync(string email, string fullName, string verificationUrl)
        {
            var model = new EmailVerificationModel
            {
                FullName = fullName,
                VerificationUrl = verificationUrl
            };

            string emailBody = await _razorRenderer.RenderViewToStringAsync("EmailVerification", model);
            string emailSubject = "Verify Your Email for Dhaka Metro Rail Account";
            return await SendEmailAsync(email, emailSubject, emailBody);    
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string fullName, string resetUrl)
        {
            var model = new PasswordResetEmailModel
            {
                FullName = fullName,
                ResetUrl = resetUrl
            };

            string emailBody = await _razorRenderer.RenderViewToStringAsync("PasswordReset", model);
            string emailSubject = "Password Reset Request for Dhaka Metro Rail Account";
            return await SendEmailAsync(email, emailSubject, emailBody);
        }

        public async Task<bool> SendPasswordResetConfirmationMail(PasswordResetConfirmationEmailModel model)
        {
            string emailBody = await _razorRenderer.RenderViewToStringAsync("PasswordResetConfirmation", model);
            string emailSubject = "Password Reset Confirmation for Dhaka Metro Rail Account";
            return await SendEmailAsync(model.Email, emailSubject, emailBody);
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
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
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
