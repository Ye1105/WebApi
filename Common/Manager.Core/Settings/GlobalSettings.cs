namespace Manager.Core.Settings
{
    public  class GlobalSettings
    {
        private static GlobalSettings instance;

        /*
         * Appsettings 配置
         */
        public AppSettings AppSettings { get; set; }

        private GlobalSettings()
        {
        }

        public static GlobalSettings Instance()
        {
            instance ??= new GlobalSettings();  
            return instance;
        }
    }
}
