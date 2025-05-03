namespace RocketTaskPlanner.Infrastructure.Env;

/// <summary>
/// Читатель переменных окружения через файл
/// </summary>
public sealed class FileEnvReader : BasicEnvReader
{
    private readonly string _filePath;

    public FileEnvReader(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath));
        if (!filePath.EndsWith(".env"))
            throw new InvalidDataException("File is not .env file");
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);
        _filePath = filePath;
        Load();
    }

    /// <summary>
    /// Загрузка переменных окружения
    /// </summary>
    private void Load()
    {
        foreach (var line in File.ReadAllLines(_filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            if (!string.IsNullOrWhiteSpace(key))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
