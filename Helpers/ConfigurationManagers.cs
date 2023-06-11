namespace api_aspnetcore6.Helpers
{
    static class ConfigurationManagers
    {
        public static IConfiguration AppSetting
        {
            get;
        }
        static ConfigurationManagers()
        {
            AppSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }
    }
}