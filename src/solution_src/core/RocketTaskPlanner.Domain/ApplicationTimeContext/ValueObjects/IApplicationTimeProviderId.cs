namespace RocketTaskPlanner.Domain.ApplicationTimeContext.ValueObjects;

/// <summary>
/// ID провайдера временных зон
/// </summary>
public interface IApplicationTimeProviderId
{
    /// <summary>
    /// ID
    /// </summary>
    string Id { get; }
}
