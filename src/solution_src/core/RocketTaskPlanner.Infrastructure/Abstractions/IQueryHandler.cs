namespace RocketTaskPlanner.Infrastructure.Abstractions;

/// <summary>
/// Обработчик запроса
/// </summary>
/// <typeparam name="TQuery">Тип запроса</typeparam>
/// <typeparam name="TQueryResponse">Тип результата запроса</typeparam>
public interface IQueryHandler<in TQuery, TQueryResponse>
    where TQuery : IQuery
{
    Task<TQueryResponse> Handle(TQuery query, CancellationToken ct = default);
}
