namespace AutoParts.Business.Services.Identity;

public class IdentityUserApiClientStub : IIdentityUserApiClient
{
    public Task<Dictionary<string, string>> GetDisplayNamesAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken = default)
        => Task.FromResult(new Dictionary<string, string>());
}
