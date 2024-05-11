using Schema.Auth;
using MongoDB.Driver;
using Schema.User;

namespace API.Auth
{
    public static class AuthEndpoints
    {
        private static readonly IMongoDatabase database = Database.Database.MongoDatabase();
        private static readonly IMongoCollection<UserSchema> UserDocument = database.GetCollection<UserSchema>("user");
        public static void RegisterLoginEndpoint(this WebApplication app)
        {
            app.MapPost("/login", async (HttpRequest request) =>
            {
                var person = await request.ReadFromJsonAsync<LoginBody>();
                long count = await UserDocument.CountDocumentsAsync(Builders<UserSchema>.Filter.Eq("name", person.username.ToString()));
                if (count == 0)
                {
                    await UserDocument.InsertOneAsync(new UserSchema
                    {
                        name = person.username.ToString(),
                        clanId = null
                    });
                }
                var userData = UserDocument.Find(Builders<UserSchema>.Filter.Eq("name", person.username.ToString())).First();
                return userData;
            });
        }
    }
}