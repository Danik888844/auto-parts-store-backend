namespace AutoParts.Business.Services.Identity;

public interface IIdentityUserApiClient
{
    Task<Dictionary<string, string>> GetDisplayNamesAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken = default);
}
