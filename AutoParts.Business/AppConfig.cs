using Microsoft.Extensions.Configuration;

namespace AutoParts.Business;

public static class AppConfig
{
    public static IConfiguration? Configuration { get; set; }
}