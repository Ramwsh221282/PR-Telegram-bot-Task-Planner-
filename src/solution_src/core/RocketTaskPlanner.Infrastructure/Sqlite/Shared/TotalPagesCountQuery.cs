using System.Data;
using Dapper;

namespace RocketTaskPlanner.Infrastructure.Sqlite.Shared;

public sealed class TotalPagesCountQuery
{
    private readonly CommandDefinition _command;
    private readonly int _pageLimit;

    public TotalPagesCountQuery(CommandDefinition command, int pageLimit)
    {
        _command = command;
        _pageLimit = pageLimit;
    }

    public async Task<int> CalculateTotalPagesCount(IDbConnection connection)
    {
        int totalCount = await connection.ExecuteScalarAsync<int>(_command);
        int pagesCount = (int)Math.Round(totalCount / (double)_pageLimit);
        return pagesCount;
    }
}
