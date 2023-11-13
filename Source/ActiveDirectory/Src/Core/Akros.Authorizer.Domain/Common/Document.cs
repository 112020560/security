using MongoDB.Bson;

namespace Akros.Authorizer.Domain.Common;

/// <summary>
/// Clase base para las entidades de mongodb
/// </summary>
public class Document : IDocument
{
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}
