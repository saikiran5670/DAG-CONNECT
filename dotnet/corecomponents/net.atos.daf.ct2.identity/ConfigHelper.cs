using Microsoft.Extensions.Configuration;

namespace net.atos.daf.ct2.identity
{
public static class ConfigHelper
{
    public static IConfiguration GetConfig()
    {
        var builder = new ConfigurationBuilder().SetBasePath(System.AppContext.BaseDirectory)
                    .AddJsonFile("./appsettings.json", optional: true, reloadOnChange: true);
                    return builder.Build();
    }

}}