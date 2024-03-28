using Microsoft.Extensions.Configuration;

namespace MyAdhan.Scheduler
{
    public static class ConfigurationManager
    {
        public static string GetConfigValue(string keys)
        {
            if (string.IsNullOrEmpty(keys)) return "";

            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json")
            .Build();

            string[] keysArray = keys.Split(',');

            var section = configuration.GetSection(keysArray[0]);

            if (keysArray.Length > 1)
            {
                for (int i = 1; i < keysArray.Length; i++)
                {
                    section = section.GetRequiredSection(keysArray[i].Trim());
                }
            }

            return section.Value;
        }
    }
}
