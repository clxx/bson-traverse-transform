namespace BsonTraverseTransform;

public abstract class BsonAnchestor
{
    protected BsonAnchestor(int index)
    {
        Index = index;
    }

    public int Index { get;  }
}