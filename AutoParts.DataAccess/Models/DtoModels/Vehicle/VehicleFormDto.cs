namespace AutoParts.DataAccess.Models.DtoModels.Vehicle;

public class VehicleFormDto
{
    public int ModelId { get; set; }
    public int YearFrom { get; set; }
    public int YearTo { get; set; }
    public string? Generation { get; set; }
    public string? Engine { get; set; }
    public string? BodyType { get; set; }
    public string? Note { get; set; }
}
