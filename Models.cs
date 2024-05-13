using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    class ClanContribution
    {
        [BsonElement("name")]
        public string? name { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("joined")]
        public int joined { get; set; }
    }
    class Clan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [BsonElement("contributions")]
        public List<ClanContribution> contributions { get; set; }
    }

    class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string id { get; set; }
        public string name { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? clanId { get; set; }
    }
}