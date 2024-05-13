using System.Net;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.ClanInfo
{
    public static class ClanInfoEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static IMongoCollection<Clan> ClanCollection = database.GetCollection<Clan>("clan");
        public static void RegisterGetClansEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/", async (HttpRequest request, HttpResponse response) =>
            {
                Dictionary<string, object?> res = [];
                List<Clan> clans = await ClanCollection.Find(Builders<Clan>.Filter.Empty).ToListAsync();
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
            long count = await ClanCollection.CountDocumentsAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(clanId)));
            if (count == 0)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                res.Add("error", "Clan with id " + clanId + " not found.");
                res.Add("data", null);
                return res;
            }
            Clan clanData = ClanCollection.Find(Builders<Clan>.Filter.Eq("_id", new ObjectId(clanId))).First();
            response.StatusCode = (int)HttpStatusCode.OK;
            res.Add("error", null);
            res.Add("data", clanData);
            return res;
        });
        }
    }
}