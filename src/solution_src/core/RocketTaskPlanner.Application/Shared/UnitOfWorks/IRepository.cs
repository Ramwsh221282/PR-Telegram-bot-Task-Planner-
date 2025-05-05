using System.Data;

namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

public interface IRepository
{
    public IDbConnection CreateConnection();
}
