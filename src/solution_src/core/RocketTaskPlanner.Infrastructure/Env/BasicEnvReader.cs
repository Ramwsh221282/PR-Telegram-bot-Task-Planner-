namespace RocketTaskPlanner.Infrastructure.Env;

/// <summary>
/// Читатель переменных окружения
/// </summary>
public abstract class BasicEnvReader : IEnvReader
{
    private const string VariableNotExistsError = "Variable with key: {0} does not exist.";

    public string GetEnvironmentVariable(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(value))
            throw new KeyNotFoundException(string.Format(VariableNotExistsError, key));
        return value;
    }
}
