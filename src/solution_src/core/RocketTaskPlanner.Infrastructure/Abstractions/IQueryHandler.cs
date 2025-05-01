namespace RocketTaskPlanner.Infrastructure.Abstractions;

public interface IQueryHandler<TQuery, TQueryResponse>
    where TQuery : IQuery
{
    Task<TQueryResponse> Handle(TQuery query, CancellationToken ct = default);
}
