using System.Collections.Immutable;
using MongoDB.Bson;

namespace BsonTraverseTransform;

public class BsonTree
{
    private readonly Action<ImmutableStack<BsonAnchestor>, BsonElement> _preOrderElement;
    private readonly Action<ImmutableStack<BsonAnchestor>, BsonValue> _preOrderValue;
    private readonly Action<ImmutableStack<BsonAnchestor>, BsonElement> _postOrderElement;
    private readonly Action<ImmutableStack<BsonAnchestor>, BsonValue> _postOrderValue;

    private BsonTree(
        Action<ImmutableStack<BsonAnchestor>, BsonElement> preOrderElement,
        Action<ImmutableStack<BsonAnchestor>, BsonValue> preOrderValue,
        Action<ImmutableStack<BsonAnchestor>, BsonElement> postOrderElement,
        Action<ImmutableStack<BsonAnchestor>, BsonValue> postOrderValue)
    {
        _preOrderElement = preOrderElement;
        _preOrderValue = preOrderValue;
        _postOrderElement = postOrderElement;
        _postOrderValue = postOrderValue;
    }

    private void Traverse(ImmutableStack<BsonAnchestor> ancestors, object bsonObject)
    {
        switch (bsonObject)
        {
            case BsonElement element:
                _preOrderElement(ancestors, element);
                break;
            case BsonValue value:
                _preOrderValue(ancestors, value);
                break;
        }

        switch (bsonObject)
        {
            case BsonDocument bsonDocument:
                var bsonElements = bsonDocument.ToArray();
                for (var index = 0; index < bsonElements.Length; index++)
                {
                    Traverse(ancestors.Push(new BsonDocumentAnchestor(bsonDocument, index)), bsonElements[index]);
                }
                break;
            case BsonArray bsonArray:
                var bsonValues = bsonArray.ToArray();
                for (var index = 0; index < bsonValues.Length; index++)
                {
                    Traverse(ancestors.Push(new BsonArrayAnchestor(bsonArray, index)), bsonValues[index]);
                }
                break;
            case BsonElement { Value: BsonDocument or BsonArray } bsonElement:
                Traverse(ancestors, bsonElement.Value);
                break;
        }

        switch (bsonObject)
        {
            case BsonElement element:
                _postOrderElement(ancestors, element);
                break;
            case BsonValue value:
                _postOrderValue(ancestors, value);
                break;
        }
    }

    public static void Traverse(
        BsonDocument bsonDocument,
        Action<ImmutableStack<BsonAnchestor>, BsonElement>? preOrderElement = null,
        Action<ImmutableStack<BsonAnchestor>, BsonValue>? preOrderValue = null,
        Action<ImmutableStack<BsonAnchestor>, BsonElement>? postOrderElement = null,
        Action<ImmutableStack<BsonAnchestor>, BsonValue>? postOrderValue = null) =>
        new BsonTree(
                preOrderElement ?? ((_, _) => { }),
                preOrderValue ?? ((_, _) => { }),
                postOrderElement ?? ((_, _) => { }),
                postOrderValue ?? ((_, _) => { }))
            .Traverse(ImmutableStack<BsonAnchestor>.Empty, bsonDocument);
}