using AutoParts.DataAccess.Models.DtoModels.Sale;

namespace AutoParts.DataAccess.Models.DtoModels.Client;

public class ClientDto : DtoBaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    
    public List<SaleDto> Sales { get; set; } = [];
}