namespace MealMate.Application.Interfaces;

public interface IEmailService
{
    Task SendInviteCodeAsync(string toEmail, string householdName, string code, string invitedByName);
}

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
}
