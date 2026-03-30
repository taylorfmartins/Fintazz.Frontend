namespace Fintazz.Web.Modules.Admin;

public record JobStatsResponse(
    long Enqueued,
    long Processing,
    long Succeeded,
    long Failed,
    long Scheduled);

public record UserSummaryResponse(Guid Id, string FullName, string NickName, string Email, bool IsAdmin);
