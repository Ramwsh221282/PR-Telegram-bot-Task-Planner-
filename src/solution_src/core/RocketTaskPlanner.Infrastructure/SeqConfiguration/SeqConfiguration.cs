using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Infrastructure.Env;

namespace RocketTaskPlanner.Infrastructure.SeqConfiguration;

public sealed class SeqConfiguration
{
    public string HostName { get; set; } = string.Empty;

    public static SeqConfiguration FromEnvFile(IServiceCollection services, string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string hostName = reader.GetEnvironmentVariable("SEQ_HOST");
        SeqConfiguration configuration = new SeqConfiguration() { HostName = hostName };
        services.AddSingleton(configuration);
        return configuration;
    }

    public static SeqConfiguration FromEnvironmentVariables(IServiceCollection services)
    {
        IEnvReader reader = new SystemEnvReader();
        string envVariable = reader.GetEnvironmentVariable("SEQ_HOST");
        SeqConfiguration configuration = new SeqConfiguration() { HostName = envVariable };
        services.AddSingleton(configuration);
        return configuration;
    }
}
