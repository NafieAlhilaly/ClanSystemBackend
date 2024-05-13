using AppLogger;
using MongoDB.Driver;

namespace Database
{
    static class Database
    {
        private static readonly ILogger logger = Logger.GetLogger();
        public static string DatabaseName = "ClanSystemDB";
        public static string ClanCollectionName = "clan";
        public static string UserCollectionName = "user";
        public static MongoClient Client { get; set; }
        public static IMongoDatabase? MongoDatabase()
        {
            logger.LogInformation("Attempting to connect to Database");
            try
            {
                string? DB_URL = Environment.GetEnvironmentVariable("DB_URL");
                if (string.IsNullOrWhiteSpace(DB_URL))
                {
                    throw new ArgumentException("Invalid DB URL");
                }
                logger.LogInformation("Connecting to " + DB_URL);
                Client ??= new(DB_URL);
                return Client.GetDatabase("ClanSystemDB");
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to database, reason: " + ex.Message);
            }
        }
    }
}