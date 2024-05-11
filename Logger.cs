namespace AppLogger
{
    class Logger
    {
        private static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug));
        public static ILogger GetLogger()
        {
            return loggerFactory.CreateLogger<Program>();
        }
    }
}