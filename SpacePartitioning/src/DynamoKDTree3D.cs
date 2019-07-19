using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using KDTreeImpl = Supercluster.KDTree.KDTree<double, object>;

namespace SpacePartitioning
{
    public class KDTree3D
    {
        private KDTreeImpl _Impl = null;
        private bool hasObjects = false;

        private KDTree3D()
        {
        }

        private static Func<double[], double[], double> DistanceSquared = (x, y) =>
        {
            double dist = 0;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return dist;
        };

        public static KDTree3D ByPoints(Point[] points)
        {
            var t = new KDTree3D();

            var data = new List<double[]>();
            foreach (var pt in points)
            {
                data.Add(new double[] { pt.X, pt.Y, pt.Z });
            }

            var objects = new List<object>();
            objects.AddRange(System.Linq.Enumerable.Repeat<object>(null, data.Count));

            t._Impl = new KDTreeImpl(3, data.ToArray(), objects.ToArray(), DistanceSquared);

            return t;
        }

        public static KDTree3D ByPointsAndObjects(Point[] points, object[] objects)
        {
            var t = new KDTree3D();
            t.hasObjects = true;

            var data = new List<double[]>();
            foreach (var pt in points)
            {
                data.Add(new double[] { pt.X, pt.Y, pt.Z });
            }

            t._Impl = new KDTreeImpl(3, data.ToArray(), objects, DistanceSquared);

            return t;
        }

        [MultiReturn(new[] { "points", "objects" })]
        public Dictionary<string, object> FindObjectsWithinRadius(Point testPoint, double testRadius)
        {
            if (_Impl == null)
            {
                return null;
            }

            var searchResult = _Impl.RadialSearch(center: new double[] { testPoint.X, testPoint.Y, testPoint.Z }, radius: testRadius * testRadius);

            var pointCollection = new List<Point>();
            var objectCollection = new List<object>();

            foreach (var result in searchResult)
            {
                pointCollection.Add(Point.ByCoordinates(result.Item1[0], result.Item1[1], result.Item1[2]));
                objectCollection.Add(result.Item2);
            }

            return new Dictionary<string, object>
            {
                { "points", pointCollection },
                { "objects", hasObjects ? objectCollection : null }
            };
        }

        [MultiReturn(new[] { "point", "object" })]
        public Dictionary<string, object> FindNearestNeighbor(Point testPoint)
        {
            return FindNearestNeighbors(testPoint, 1);
        }

        [MultiReturn(new[] { "points", "objects" })]
        public Dictionary<string, object> FindNearestNeighbors(Point testPoint, int numNeighbors)
        {
            if (_Impl == null)
            {
                return null;
            }

            var searchResult = _Impl.NearestNeighbors(point: new double[] { testPoint.X, testPoint.Y, testPoint.Z }, neighbors: numNeighbors);

            var pointCollection = new List<Point>();
            var objectCollection = new List<object>();

            foreach (var result in searchResult)
            {
                pointCollection.Add(Point.ByCoordinates(result.Item1[0], result.Item1[1], result.Item1[2]));
                objectCollection.Add(result.Item2);
            }

            return new Dictionary<string, object>
            {
                { "points", pointCollection },
                { "objects", hasObjects ? objectCollection : null }
            };
        }
    }
}
