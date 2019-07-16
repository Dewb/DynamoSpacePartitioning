using Autodesk.DesignScript.Geometry;
using TestServices;
using NUnit.Framework;
using System.Collections.Generic;

namespace SpacePartitioning.Tests
{
    public class QueryTests : GeometricTestBase
    {
        private SimpleOctree tree = null;
        [SetUp]
        public void SetUpOctree()
        {
            var boundingBox = BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(10, 10, 10));
            tree = new SimpleOctree(boundingBox, new OctreeOptions());

            // Create octree with eight objects, one in each first-level cell
            tree.Add("test object 1", BoundingBox.ByCorners(Point.ByCoordinates(1, 1, 1), Point.ByCoordinates(4, 4, 4)));
            tree.Add("test object 2", BoundingBox.ByCorners(Point.ByCoordinates(1, 6, 1), Point.ByCoordinates(4, 9, 4)));
            tree.Add("test object 3", BoundingBox.ByCorners(Point.ByCoordinates(6, 1, 1), Point.ByCoordinates(9, 4, 4)));
            tree.Add("test object 4", BoundingBox.ByCorners(Point.ByCoordinates(6, 6, 1), Point.ByCoordinates(9, 9, 4)));
            tree.Add("test object 5", BoundingBox.ByCorners(Point.ByCoordinates(1, 1, 6), Point.ByCoordinates(4, 4, 9)));
            tree.Add("test object 6", BoundingBox.ByCorners(Point.ByCoordinates(1, 6, 6), Point.ByCoordinates(4, 9, 9)));
            tree.Add("test object 7", BoundingBox.ByCorners(Point.ByCoordinates(6, 1, 6), Point.ByCoordinates(9, 4, 9)));
            tree.Add("test object 8", BoundingBox.ByCorners(Point.ByCoordinates(6, 6, 6), Point.ByCoordinates(9, 9, 9)));
        }

        [Test]
        public void QueryRange()
        {
            List<object> list = null;

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(10, 10, 10)));
            Assert.AreEqual(8, list.Count);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(10, 10, 5)));
            Assert.AreEqual(4, list.Count);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 5), Point.ByCoordinates(10, 10, 10)));
            Assert.AreEqual(4, list.Count);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(5, 5, 5)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 1", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 5, 0), Point.ByCoordinates(5, 10, 5)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 2", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(5, 0, 0), Point.ByCoordinates(10, 5, 5)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 3", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(5, 5, 0), Point.ByCoordinates(10, 10, 5)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 4", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 0, 5), Point.ByCoordinates(5, 5, 10)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 5", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(0, 5, 5), Point.ByCoordinates(5, 10, 10)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 6", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(5, 0, 5), Point.ByCoordinates(10, 5, 10)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 7", list[0]);

            list = tree.QueryRange(BoundingBox.ByCorners(Point.ByCoordinates(5, 5, 5), Point.ByCoordinates(10, 10, 10)));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 8", list[0]);
        }

        [Test]
        public void QueryHit()
        {
            List<object> list = null;

            list = tree.QueryHit(Point.ByCoordinates(2, 2, 2));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 1", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(2, 7, 2));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 2", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(7, 2, 2));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 3", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(7, 7, 2));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 4", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(2, 2, 7));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 5", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(2, 7, 7));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 6", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(7, 2, 7));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 7", list[0]);

            list = tree.QueryHit(Point.ByCoordinates(7, 7, 7));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test object 8", list[0]);
        }

        [Test]
        public void QueryRadius()
        {
            // todo
        }
    }
}
