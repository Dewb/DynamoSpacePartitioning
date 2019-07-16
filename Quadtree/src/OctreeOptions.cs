using Autodesk.DesignScript.Runtime;

namespace SpacePartitioning
{
    [IsVisibleInDynamoLibrary(false)]
    public class OctreeOptions
    {
        public int LeafCapacity = 3;
    }
}
