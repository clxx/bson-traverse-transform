using System.Collections.Immutable;
using MongoDB.Bson;

namespace BsonTraverseTransform;

public class BsonTree
{
    private static void Traverse(
        ImmutableStack<(BsonDocument?, BsonArray?, int)> ancestors,
        object bsonObject,
        Action<ImmutableStack<(BsonDocument?, BsonArray?, int)>, BsonElement, BsonValue?>? preOrder,
        Action<ImmutableStack<(BsonDocument?, BsonArray?, int)>, BsonElement, BsonValue?>? postOrder)
    {
        preOrder?.Invoke(ancestors, bsonObject as BsonElement? ?? default, bsonObject as BsonValue);
        switch (bsonObject)
        {
            case BsonDocument bsonDocument:
                var bsonElements = bsonDocument.ToArray();
                for (var index = 0; index < bsonElements.Length; index++)
                {
                    Traverse(ancestors.Push((bsonDocument, null, index)), bsonElements[index], preOrder, postOrder);
                }
                break;
            case BsonArray bsonArray:
                var bsonValues = bsonArray.ToArray();
                for (var index = 0; index < bsonValues.Length; index++)
                {
                    Traverse(ancestors.Push((null, bsonArray, index)), bsonValues[index], preOrder, postOrder);
                }
                break;
            case BsonElement { Value: BsonDocument or BsonArray } bsonElement:
                Traverse(ancestors, bsonElement.Value, preOrder, postOrder);
                break;
        }
        postOrder?.Invoke(ancestors, bsonObject as BsonElement? ?? default, bsonObject as BsonValue);
    }

    public static void Traverse(
        BsonDocument bsonDocument,
        Action<ImmutableStack<(BsonDocument?, BsonArray?, int)>, BsonElement, BsonValue?>? preOrder,
        Action<ImmutableStack<(BsonDocument?, BsonArray?, int)>, BsonElement, BsonValue?>? postOrder) =>
        Traverse(
            ImmutableStack<(BsonDocument?, BsonArray?, int)>.Empty,
            bsonDocument,
            preOrder,
            postOrder);
}