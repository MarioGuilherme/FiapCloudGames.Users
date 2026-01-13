namespace FiapCloudGames.Users.Domain.Events;

public record class SendPendingEmailEvent(int UserId, string Subject, string HtmlContent);
