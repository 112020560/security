using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Akros.Authorizer.Domain.Common;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }

    DateTime CreatedAt { get; }
}
