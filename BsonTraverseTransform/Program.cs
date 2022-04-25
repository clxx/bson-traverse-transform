using BsonTraverseTransform;
using MongoDB.Bson;

var bsonDocument = BsonDocument.Parse(@"{
    ""a"": {
        ""b"": null,
        ""c"": ""text1"",
        ""d"": 0,
        ""e"": {
            ""f"": [
                null,
                ""text2"",
                1,
                {
                    ""g"": {
                        ""h"": 2
                    }
                }
            ]
        },
        ""i"": [
            100,
            200,
            300
        ]
    }
}");

BsonTree.Traverse(
    bsonDocument,
    null,
    (ancestors, bsonElement, bsonValue) =>
    {
        if (bsonElement.Name is "d")
        {
            var (document, _, index) = ancestors.Peek();
            document?.SetElement(index, new BsonElement("zzz", bsonElement.Value));
        }

        else if (bsonElement.Name is "c")
        {
            var (document, _, index) = ancestors.Peek();
            document?.RemoveAt(index);
        }

        else if (bsonValue == 200)
        {
            var (_, array, index) = ancestors.Peek();
            array?.RemoveAt(index);
        }
    });

Console.WriteLine(bsonDocument);
