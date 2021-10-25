using Microsoft.Extensions.Configuration;
using System.IO;

namespace _301104393Lu_Lab3.Classes
{
    public static class ConfigurationHelper
    {
        public static string GetConfValueByKey(string key, string section = "AWSCredentials")
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

            return builder.GetSection(section).GetSection(key).Value;
        }
    }
}
