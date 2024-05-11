using AppLogger;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Database
{
    static class Database
    {
        private static readonly ILogger logger = Logger.GetLogger();
        public static IMongoDatabase? MongoDatabase()
        {
            logger.LogInformation("Attempting to connect to Database using ");
            try
            {
                string? DB_URL = Environment.GetEnvironmentVariable("DB_URL");
                if (string.IsNullOrWhiteSpace(DB_URL))
                {
                    throw new ArgumentException("Invalid DB URL");
                }
                MongoClient client = new(DB_URL);
                logger.LogInformation("Connected to Database");
                logger.LogInformation("Getting database ClanSystemDB");
                IMongoDatabase database = client.GetDatabase("ClanSystemDB");
                logger.LogInformation(client.ListDatabaseNames().ToJson());
                return client.GetDatabase("ClanSystemDB");
            }
            catch
            {
                logger.LogCritical("Could not connect to database, reason: ");
                return null;
            }

        }
    }
}