using System.Net;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.ClanLeave
{
    public static class ClanLeaveEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static IMongoCollection<Clan> ClanCollection = database.GetCollection<Clan>(Database.Database.ClanCollectionName);
        private static readonly IMongoCollection<User> UserCollection = database.GetCollection<User>(Database.Database.UserCollectionName);

        public static void RegisterLeaveClanByIdEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/leave", async (HttpRequest request, HttpResponse response) =>
            {
                User userData = UserCollection.Find(Builders<User>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                Dictionary<string, object?> res = [];
                if (userData.clanId == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "You are not part of any clan.");
                    res.Add("data", null);
                    return res;
                }

                await ClanCollection.UpdateOneAsync(
                    Builders<Clan>.Filter.Or(
                        new Dictionary<string, object>() { { "_id", new ObjectId(userData.clanId) }, { "contributions.name", userData.name } }.ToBsonDocument()), Builders<Clan>.Update.Set("contributions.$.joined", 0));
                await UserCollection.UpdateOneAsync(Builders<User>.Filter.Eq("_id", new ObjectId(userData.id)), Builders<User>.Update.Set(u => u.clanId, null));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "You left clan.");
                return res;
            });
        }
        
    }
}