using System.Collections.Immutable;

namespace BsonTraverseTransform;

public static class BsonAnchestorsExtensions
{
    public static BsonDocumentAnchestor PeekAsDocument(this ImmutableStack<BsonAnchestor> bsonAnchestors) => (BsonDocumentAnchestor)bsonAnchestors.Peek();
    public static BsonArrayAnchestor PeekAsArray(this ImmutableStack<BsonAnchestor> bsonAnchestors) => (BsonArrayAnchestor)bsonAnchestors.Peek();
}