using MongoDB.Bson;

namespace BsonTraverseTransform;

public  class BsonDocumentAnchestor : BsonAnchestor
{
    public BsonDocumentAnchestor(BsonDocument bsonDocument, int index) : base(index)
    {
        BsonDocument = bsonDocument;
    }

    public BsonDocument BsonDocument { get;  }

    public void Create(BsonElement bsonElement) => BsonDocument.SetElement(bsonElement);

    public void Update(BsonElement bsonElement) => BsonDocument.SetElement(Index, bsonElement);

    public void Delete() => BsonDocument.RemoveAt(Index);
}