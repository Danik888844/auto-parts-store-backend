using System.ComponentModel.DataAnnotations;
using AutoParts.Core.DataAccess;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class BaseEntity : IEntity
{
    [Key] public int Id { get; set; }
    public long CreatedDate { get; set; }
    public long ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
}
