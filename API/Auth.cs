using Models;
using MongoDB.Driver;
using Schema;

namespace API.Auth
{
    public static class AuthEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static readonly IMongoCollection<User> UserDocument = database.GetCollection<User>("user");
        public static void RegisterLoginEndpoint(this WebApplication app)
        {
            app.MapPost("/login", async (HttpRequest request) =>
            {
                var person = await request.ReadFromJsonAsync<LoginBody>();
                long count = await UserDocument.CountDocumentsAsync(Builders<User>.Filter.Eq("name", person.username.ToString()));
                if (count == 0)
                {
                    await UserDocument.InsertOneAsync(new User
                    {
                        name = person.username.ToString(),
                        clanId = null
                    });
                }
                var userData = UserDocument.Find(Builders<User>.Filter.Eq("name", person.username.ToString())).First();
                return userData;
            });
        }
    }
}