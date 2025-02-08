using Microsoft.AspNetCore.Identity;

public class NullEmailSender<TUser> : IEmailSender<TUser>
    where TUser : class
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // No operation
        return Task.CompletedTask;
    }

    public Task SendConfirmationLinkAsync(TUser user, string confirmationLink, string email)
    {
        // No operation
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(TUser user, string resetCode, string email)
    {
        // No operation
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(TUser user, string resetLink, string email)
    {
        // No operation
        return Task.CompletedTask;
    }
}