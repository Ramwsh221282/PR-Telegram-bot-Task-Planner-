namespace RocketTaskPlanner.Infrastructure.Env;

public interface IEnvReader
{
    string GetEnvironmentVariable(string key);
}
