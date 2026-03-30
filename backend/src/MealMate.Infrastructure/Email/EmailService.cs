using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MealMate.Application.Interfaces;

namespace MealMate.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendInviteCodeAsync(string toEmail, string householdName, string code, string invitedByName)
    {
        var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUser = _configuration["Email:SmtpUser"];
        var smtpPass = _configuration["Email:SmtpPass"];
        var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
        var fromName = _configuration["Email:FromName"] ?? "MealMate";

        if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
        {
            _logger.LogWarning("Email configuration is missing. Skipping email send.");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = $"You're invited to join {householdName} on MealMate!";

        var builder = new BodyBuilder
        {
            HtmlBody = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <h2 style="color: #22c55e;">🍽️ MealMate Household Invitation</h2>
                    <p>Hi there!</p>
                    <p><strong>{invitedByName}</strong> has invited you to join the <strong>{householdName}</strong> household on MealMate.</p>
                    <p>Use the following code to join:</p>
                    <div style="background: #f3f4f6; padding: 20px; text-align: center; border-radius: 8px; margin: 20px 0;">
                        <span style="font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #22c55e;">{code}</span>
                    </div>
                    <p>This code expires in 7 days.</p>
                    <p>To join, register or log in to MealMate and enter this code in the "Join Household" section.</p>
                    <p><em>Note: This code can only be used with this email address for security reasons.</em></p>
                    <hr style="border: none; border-top: 1px solid #e5e7eb; margin: 20px 0;" />
                    <p style="color: #6b7280; font-size: 12px;">If you didn't expect this invitation, you can safely ignore this email.</p>
                </div>
                """
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUser, smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
