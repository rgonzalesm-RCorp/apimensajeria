using Microsoft.Extensions.Configuration;
using System.IO;

public static class AppSettings
{
    public static IConfiguration Configuration { get; }

    static AppSettings()
    {
        // Construimos la configuración
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string GetSetting(string key)
    {
        return Configuration[key];
    }
}
