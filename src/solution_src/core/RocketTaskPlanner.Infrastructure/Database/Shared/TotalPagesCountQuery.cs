using System.Data;
using Dapper;

namespace RocketTaskPlanner.Infrastructure.Database.Shared;

/// <summary>
/// Класс для расчета количества страниц
/// </summary>
public sealed class TotalPagesCountQuery
{
    /// <summary>
    /// SQL команда, которая вернет общее число элементов
    /// </summary>
    private readonly CommandDefinition _command;

    /// <summary>
    /// Размер страницы
    /// </summary>
    private readonly int _pageLimit;

    public TotalPagesCountQuery(CommandDefinition command, int pageLimit)
    {
        _command = command;
        _pageLimit = pageLimit;
    }

    /// <summary>
    /// Расчёт количества страниц
    /// <param name="connection">
    ///     <inheritdoc cref="IDbConnection"/>
    /// </param>
    /// <returns>Количество страниц</returns>
    /// </summary>
    public async Task<int> CalculateTotalPagesCount(IDbConnection connection)
    {
        int totalCount = await connection.ExecuteScalarAsync<int>(_command);
        int pagesCount = (int)Math.Round(totalCount / (double)_pageLimit);
        return pagesCount;
    }
}
