using AutoParts.DataAccess.Models.DtoModels.Sale;

namespace AutoParts.DataAccess.Models.DtoModels.Client;

public class ClientDto : DtoBaseEntity
{
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public List<SaleDto> Sales { get; set; } = [];
}

public class ClientFormDto
{
    public required string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
}