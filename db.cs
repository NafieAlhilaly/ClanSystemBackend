using AppLogger;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Database
{
    static class Database
    {
        private static readonly ILogger logger = Logger.GetLogger();
        public static string DatabaseName = "ClanSystemDB";
        public static string ClanCollectionName = "clan";
        public static string UserCollectionName = "user";
        public static IMongoDatabase? MongoDatabase()
        {
            logger.LogInformation("Attempting to connect to Database");
            string? DB_URL = Environment.GetEnvironmentVariable("DB_URL");
            if (string.IsNullOrWhiteSpace(DB_URL))
            {
                throw new ArgumentException("Invalid DB URL");
            }
            MongoClient client = new(DB_URL);
            return client.GetDatabase(DatabaseName);
        }
    }
}