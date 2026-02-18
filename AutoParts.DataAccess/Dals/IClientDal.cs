using AutoParts.Core.DataAccess;
using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DatabaseModels;

namespace AutoParts.DataAccess.Dals;

public interface IClientDal : IEntityRepository<Client> {}

public class ClientDal : EfEntityRepository<Client, AutoPartsStoreDb>, IClientDal
{
    public ClientDal(AutoPartsStoreDb context) : base(context)
    {
    }
}
