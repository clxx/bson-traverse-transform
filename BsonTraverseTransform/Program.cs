using BsonTraverseTransform;
using MongoDB.Bson;
using MongoDB.Bson.IO;

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
    (ancestors, bsonElement) =>
    {
        if (bsonElement.Name is "d")
        {
            var document = ancestors.PeekAsDocument();
            document.Update(new BsonElement("zzz", bsonElement.Value));
        }
        else if (bsonElement.Name is "c")
        {
            var document = ancestors.PeekAsDocument();
            document.Delete();
        }
    },
    (ancestors, bsonValue) =>
    {
        if (bsonValue == 200)
        {
            var array = ancestors.PeekAsArray();
            array.Delete();
        }
    });

Console.WriteLine(bsonDocument.ToJson(new JsonWriterSettings { Indent = true }));
