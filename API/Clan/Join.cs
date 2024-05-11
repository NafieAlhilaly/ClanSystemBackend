using System.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Schema.Clan;
using Schema.User;

namespace API.Clan.Join
{
    public static class ClanJoinEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static IMongoCollection<ClanSchema> ClanCollection = database.GetCollection<ClanSchema>("clan");
        private static readonly IMongoCollection<UserSchema> UserCollection = database.GetCollection<UserSchema>("user");

        public static void RegisterJoinClanByIdEndpoint(this WebApplication app)
        {
            app.MapGet("/clan/join/{clanId}", async (string clanId, HttpRequest request, HttpResponse response) =>
            {
                Dictionary<string, object?> res = [];
                UserSchema userData = UserCollection.Find(Builders<UserSchema>.Filter.Eq("name", request.Headers.Authorization.ToString())).First();
                if (userData.clanId != null)
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    res.Add("error", "You are already in a clan!");
                    res.Add("data", null);
                    return res;
                }
                long count = await ClanCollection.CountDocumentsAsync(Builders<ClanSchema>.Filter.Eq("_id", new ObjectId(clanId)));
                if (count == 0)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Add("error", "Clan with id " + clanId + " not found.");
                    res.Add("data", null);
                    return res;
                }
                bool noNullSpot = await ClanCollection.CountDocumentsAsync(Builders<ClanSchema>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(clanId) }, { "contributions.name", BsonNull.Value } }.ToBsonDocument())) == 0;
                bool noLeftSpot = await ClanCollection.CountDocumentsAsync(Builders<ClanSchema>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(clanId) }, { "contributions.joined", 0 } }.ToBsonDocument())) == 0;
                if (noNullSpot && noLeftSpot)
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    res.Add("error", "Clan is full.");
                    res.Add("data", null);
                    return res;
                }
                if (userData.clanId == clanId)
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    res.Add("error", "You are already in the clan.");
                    res.Add("data", null);
                    return res;
                }
                BsonDocument checkIfMemberWasInClanRawQuery = BsonSerializer.Deserialize<BsonDocument>("{ _id: ObjectId('" + clanId + "'), 'contributions.name': ObjectId('" + userData.id + "') , 'contributions.joined': 0}");
                if (await ClanCollection.CountDocumentsAsync(checkIfMemberWasInClanRawQuery) != 0)
                {
                    await ClanCollection.UpdateOneAsync(
                        Builders<ClanSchema>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(clanId) }, { "contributions.name", userData.id } }.ToBsonDocument()),
                        Builders<ClanSchema>.Update.Set("contributions.$.joined", 1));
                    await UserCollection.UpdateOneAsync(Builders<UserSchema>.Filter.Eq("_id", new ObjectId(userData.id)), Builders<UserSchema>.Update.Set("clanId", new ObjectId(clanId)));
                    response.StatusCode = (int)HttpStatusCode.OK;
                    res.Add("error", null);
                    res.Add("data", "Clan rejoined.");
                    return res;
                }
                if (noLeftSpot)
                {
                    await ClanCollection.UpdateOneAsync(
                        Builders<ClanSchema>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(clanId) }, { "contributions.name", BsonNull.Value } }.ToBsonDocument()),
                        new Dictionary<string, Dictionary<string, object>>(){
                        {"$set", new Dictionary<string, object>(){
                        { "contributions.$.name", userData.name },
                        {"contributions.$.points", 0},
                        {"contributions.$.joined", 1}}}}.ToBsonDocument());
                }
                else
                {
                    await ClanCollection.UpdateOneAsync(
                        Builders<ClanSchema>.Filter.Or(new Dictionary<string, object>() { { "_id", new ObjectId(clanId) }, { "contributions.joined", 0 } }.ToBsonDocument()), new Dictionary<string, Dictionary<string, object>>(){
                    {"$set", new Dictionary<string, object>(){
                    { "contributions.$.name", userData.name },
                    {"contributions.$.points", 0},
                    {"contributions.$.joined", 1}}}
                }.ToBsonDocument());
                }
                await UserCollection.UpdateOneAsync(Builders<UserSchema>.Filter.Eq("_id", new ObjectId(userData.id)), Builders<UserSchema>.Update.Set("clanId", new ObjectId(clanId)));
                response.StatusCode = (int)HttpStatusCode.OK;
                res.Add("error", null);
                res.Add("data", "Clan joined.");
                return res;
            });
        }
        
    }
}