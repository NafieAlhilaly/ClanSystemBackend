using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Schema.Clan
{
    class UpdatePointsBody
    {
        public int points { get; set; }
    }
    class ClanContributionSchema
    {
        [BsonElement("name")]
        public string? name { get; set; }
        [BsonElement("points")]
        public int points { get; set; }
        [BsonElement("joined")]
        public int joined { get; set; }
    }
    class ClanSchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string id { get; set; }
        public string name { get; set; }
        [BsonElement("contributions")]
        public List<ClanContributionSchema> contributions { get; set; }
    }
}