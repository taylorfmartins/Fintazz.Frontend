namespace Fintazz.Web.Modules.HouseHolds;

public record HouseHoldResponse(Guid Id, string? Name, Guid AdminUserId, List<Guid>? MemberIds);

public record HouseHoldMemberResponse(
    Guid UserId,
    string? FullName,
    string? NickName,
    string? Email,
    bool IsAdmin);
