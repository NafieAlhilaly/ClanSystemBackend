using System.Net;
using AppLogger;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Schema.Clan;
using Schema.User;

namespace API.Clan.Info
{
    public static class ClanInfoEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static readonly ILogger logger = Logger.GetLogger();
        private static IMongoCollection<ClanSchema> ClanCollection = database.GetCollection<ClanSchema>("clan");
        private static readonly IMongoCollection<UserSchema> UserCollection = database.GetCollection<UserSchema>("user");
        public static void RegisterGetClansEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/", async (HttpRequest request, HttpResponse response) =>
            {
                Dictionary<string, object?> res = [];
                List<ClanSchema> clans = await ClanCollection.Find(Builders<ClanSchema>.Filter.Empty).ToListAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", clans);
                return res;
            });
        }
        public static void RegisterGetClanByIdEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/{clanId}", async (string clanId, HttpRequest request, HttpResponse response) =>
        {
            Dictionary<string, object?> res = [];
            long count = await ClanCollection.CountDocumentsAsync(Builders<ClanSchema>.Filter.Eq("_id", new ObjectId(clanId)));
            if (count == 0)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                res.Add("error", "Clan with id " + clanId + " not found.");
                res.Add("data", null);
                return res;
            }
            ClanSchema clanData = ClanCollection.Find(Builders<ClanSchema>.Filter.Eq("_id", new ObjectId(clanId))).First();
            response.StatusCode = (int)HttpStatusCode.OK;
            res.Add("error", null);
            res.Add("data", clanData);
            return res;
        });
        }
    }
}