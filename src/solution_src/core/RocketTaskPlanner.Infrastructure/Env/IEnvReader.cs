namespace RocketTaskPlanner.Infrastructure.Env;

/// <summary>
/// Контракт читателя переменных окружения
/// </summary>
public interface IEnvReader
{
    string GetEnvironmentVariable(string key);
}
