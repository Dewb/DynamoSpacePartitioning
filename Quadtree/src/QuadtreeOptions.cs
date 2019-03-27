using Autodesk.DesignScript.Runtime;

namespace DynamoQuadtree
{
    [IsVisibleInDynamoLibrary(false)]
    public class QuadtreeOptions
    {
        public int LeafCapacity = 3;
    }
}
