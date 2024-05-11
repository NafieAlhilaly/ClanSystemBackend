using AppLogger;
using MongoDB.Driver;

namespace Database
{
    static class Database
    {
        private static readonly ILogger logger = Logger.GetLogger();
        public static IMongoDatabase? MongoDatabase()
        {
            logger.LogInformation("Attempting to connect to Database");
            try
            {
                MongoClient client = new(Environment.GetEnvironmentVariable("DB_URL"));
                logger.LogInformation("Connected to Database");
                return client.GetDatabase("ClanSystemDB");
            }
            catch
            {
                logger.LogCritical("Could not connect to database.");
                return null;
            }

        }
    }
}