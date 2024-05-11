using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Schema.User
{
    class UserSchema
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