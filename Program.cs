using MongoDB.Driver;
using System.Net;
using AppLogger;
using API.Auth;
using Models;
using API.ClanInfo;
using API.ClanJoin;
using API.ClanLeave;
using API.ClanPoints;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string CorsMiddlewareName = "OriginsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsMiddlewareName, policy => { policy.WithOrigins(["http://127.0.0.1:3000", "http://localhost:3000"]).AllowAnyHeader(); });
});
builder.Logging.AddConsole();
WebApplication app = builder.Build();
IMongoDatabase database = Database.Database.MongoDatabase();
ILogger logger = Logger.GetLogger();
IMongoCollection<User> UserDocument = database.GetCollection<User>("user");
IMongoCollection<Clan> ClanDocument = database.GetCollection<Clan>("clan");

// using (StreamReader r = new StreamReader("clans.json"))
// {
//     string json = r.ReadToEnd();
//     List<Clan> rawQuery = BsonSerializer.Deserialize<List<Clan>>(json);
//     ClanDocument.InsertMany(rawQuery);
// }

app.UseCors(CorsMiddlewareName);
app.UseWhen(context => context.Request.Path.StartsWithSegments("/clan"), appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        if (!context.Request.Headers.ContainsKey("authorization"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        string username = context.Request.Headers.Authorization.ToString();
        long count = await UserDocument.CountDocumentsAsync(Builders<User>.Filter.Eq("name", username));
        if (count == 0)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        await next.Invoke();
    });
});
app.RegisterLoginEndpoint();
app.RegisterGetClansEndpoint();
app.RegisterGetClanByIdEndpoint();
app.RegisterJoinClanByIdEndpoint();
app.RegisterLeaveClanByIdEndpoint();
app.RegisterAddPointsToClanByIdEndpoint();
app.RegisterSubtractPointsToClanByIdEndpoint();
app.RegisterSetPointsToClanByIdEndpoint();

app.Run();