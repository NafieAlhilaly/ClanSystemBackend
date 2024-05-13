using System.Net;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Schema.Clan;
using Schema.User;

namespace API.ClanPoints
{
    public static class ClanPointsEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static IMongoCollection<Clan> ClanCollection = database.GetCollection<Clan>("clan");
        private static readonly IMongoCollection<UserSchema> UserCollection = database.GetCollection<UserSchema>("user");
        public static void RegisterAddPointsToClanByIdEndpoint(this WebApplication app)
        {
            app.MapPost("/clan/add-points", async (HttpRequest request, HttpResponse response) =>
            {
                UpdatePointsBody updatePointsBody = await request.ReadFromJsonAsync<UpdatePointsBody>();
                var userData = UserCollection.Find(Builders<UserSchema>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                Dictionary<string, object?> res = [];
                if (userData.clanId == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "You are not in any clan, join a clan to be able to add points.");
                    res.Add("data", null);
                    return res;
                }
                long count = await ClanCollection.CountDocumentsAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(userData.clanId)));
                if (count == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "Clan with id " + userData.clanId + " not found.");
                    res.Add("data", null);
                    return res;
                }
                await ClanCollection.UpdateOneAsync(Builders<Clan>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(userData.clanId) }, { "contributions.name", userData.name } }.ToBsonDocument()), Builders<Clan>.Update.Inc("contributions.$.points", updatePointsBody.points));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "Points updated.");
                return res;
            });
        }
        public static void RegisterSubtractPointsToClanByIdEndpoint(this WebApplication app)
        {
            app.MapPost("/clan/subtract-points", async (HttpRequest request, HttpResponse response) =>
            {
                UpdatePointsBody updatePointsBody = await request.ReadFromJsonAsync<UpdatePointsBody>();
                var userData = UserCollection.Find(Builders<UserSchema>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                Dictionary<string, object?> res = [];
                if (userData.clanId == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "You are not in any clan, join a clan to be able to add points.");
                    res.Add("data", null);
                    return res;
                }
                long count = await ClanCollection.CountDocumentsAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(userData.clanId)));
                if (count == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "Clan with id " + userData.clanId + " not found.");
                    res.Add("data", null);
                    return res;
                }
                await ClanCollection.UpdateOneAsync(Builders<Clan>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(userData.clanId) }, { "contributions.name", userData.name } }.ToBsonDocument()), Builders<Clan>.Update.Inc("contributions.$.points", updatePointsBody.points * -1));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "Points updated.");
                return res;
            });
        }
        public static void RegisterSetPointsToClanByIdEndpoint(this WebApplication app)
        {
            app.MapPost("/clan/set-points", async (HttpRequest request, HttpResponse response) =>
            {
                UpdatePointsBody updatePointsBody = await request.ReadFromJsonAsync<UpdatePointsBody>();
                var userData = UserCollection.Find(Builders<UserSchema>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                Dictionary<string, object?> res = [];
                if (userData.clanId == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "You are not in any clan, join a clan to be able to add points.");
                    res.Add("data", null);
                    return res;
                }
                long count = await ClanCollection.CountDocumentsAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(userData.clanId)));
                if (count == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "Clan with id " + userData.clanId + " not found.");
                    res.Add("data", null);
                    return res;
                }
                await ClanCollection.UpdateOneAsync(Builders<Clan>.Filter.Eq("_id", new ObjectId(userData.clanId)), Builders<Clan>.Update.Set("contributions.$[].points", 0));
                await ClanCollection.UpdateOneAsync(Builders<Clan>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(userData.clanId) }, { "contributions.name", userData.name } }.ToBsonDocument()), Builders<Clan>.Update.Set("contributions.$.points", updatePointsBody.points));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "Points updated.");
                return res;
            });
        }

    }
}