using MongoDB.Bson;

namespace BsonTraverseTransform;

public  class BsonArrayAnchestor : BsonAnchestor
{
    public BsonArrayAnchestor(BsonArray bsonArray, int index) : base(index)
    {
        BsonArray = bsonArray;
    }

    public BsonArray BsonArray { get; }

    public void Create(BsonValue bsonValue) => BsonArray.Add(bsonValue);

    public void Update(BsonValue bsonValue) => BsonArray[Index]= bsonValue;

    public void Delete() => BsonArray.RemoveAt(Index);
}