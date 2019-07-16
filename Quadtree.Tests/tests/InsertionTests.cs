using Autodesk.DesignScript.Geometry;
using TestServices;
using NUnit.Framework;

namespace SpacePartitioning.Tests
{
    public class InsertionTests :  GeometricTestBase
    {
        [Test]
        public void InsertOne()
        {
            var boundingBox = BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(1, 1, 1));
            var tree = new SimpleOctree(boundingBox, new OctreeOptions());
            tree.Add("test object", boundingBox);
            var list = tree.QueryRange(boundingBox);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object", list[0]);
        }

        [Test]
        public void InsertMany()
        {
            var boundingBox = BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(1, 1, 1));
            var tree = new SimpleOctree(boundingBox, new OctreeOptions());
            tree.Add("test object 1", boundingBox);
            tree.Add("test object 2", boundingBox);
            tree.Add("test object 3", boundingBox);
            tree.Add("test object 4", boundingBox);
            var list = tree.QueryRange(boundingBox);

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("test object 1", list[0]);
            Assert.AreEqual("test object 2", list[1]);
            Assert.AreEqual("test object 3", list[2]);
            Assert.AreEqual("test object 4", list[3]);
        }
    }
}
