using System.Net;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Schema.User;

namespace API.ClanLeave
{
    public static class ClanLeaveEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static IMongoCollection<Clan> ClanCollection = database.GetCollection<Clan>("clan");
        private static readonly IMongoCollection<UserSchema> UserCollection = database.GetCollection<UserSchema>("user");

        public static void RegisterLeaveClanByIdEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/leave", async (HttpRequest request, HttpResponse response) =>
            {
                var userData = UserCollection.Find(Builders<UserSchema>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                Dictionary<string, object?> res = [];
                long count = await ClanCollection.CountDocumentsAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(userData.clanId)));
                if (count == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "You are not part of any clan.");
                    res.Add("data", null);
                    return res;
                }

                await ClanCollection.UpdateOneAsync(
                    Builders<Clan>.Filter.Or(
                        new Dictionary<string, object>() { { "_id", new ObjectId(userData.clanId) }, { "contributions.name", userData.name } }.ToBsonDocument()), Builders<Clan>.Update.Set("contributions.$.joined", 0));
                await UserCollection.UpdateOneAsync(Builders<UserSchema>.Filter.Eq("_id", new ObjectId(userData.id)), Builders<UserSchema>.Update.Set(u => u.clanId, null));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "You left clan.");
                return res;
            });
        }
        
    }
}