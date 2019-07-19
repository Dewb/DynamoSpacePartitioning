using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using KDTreeImpl = Supercluster.KDTree.KDTree<double, object>;

namespace SpacePartitioning
{
    public class KDTree2D
    {
        private KDTreeImpl _Impl = null;
        private bool hasObjects = false;

        private KDTree2D()
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

        public static KDTree2D ByUVs(UV[] uvs)
        {
            var t = new KDTree2D();

            var points = new List<double[]>();
            foreach (var uv in uvs)
            {
                points.Add(new double[] { uv.U, uv.V });
            }

            var objects = new List<object>();
            objects.AddRange(System.Linq.Enumerable.Repeat<object>(null, points.Count));

            t._Impl = new KDTreeImpl(2, points.ToArray(), objects.ToArray(), DistanceSquared);

            return t;
        }

        public static KDTree2D ByUVsAndObjects(UV[] uvs, object[] objects)
        {
            var t = new KDTree2D();
            t.hasObjects = true;

            var points = new List<double[]>();
            foreach (var uv in uvs)
            {
                points.Add(new double[] { uv.U, uv.V });
            }

            t._Impl = new KDTreeImpl(2, points.ToArray(), objects, DistanceSquared);

            return t;
        }

        [MultiReturn(new[] { "uvs", "objects" })]
        public Dictionary<string, object> FindObjectsWithinRadius(UV testPoint, double testRadius)
        {
            if (_Impl == null)
            {
                return null;
            }

            var searchResult = _Impl.RadialSearch(center: new double[] { testPoint.U, testPoint.V }, radius: testRadius * testRadius);

            var uvCollection = new List<UV>();
            var objectCollection = new List<object>();
            
            foreach (var result in searchResult)
            {
                uvCollection.Add(UV.ByCoordinates(result.Item1[0], result.Item1[1]));
                objectCollection.Add(result.Item2);
            }

            return new Dictionary<string, object>
            {
                { "uvs", uvCollection },
                { "objects", hasObjects ? objectCollection : null }
            };
        }

        [MultiReturn(new[] { "uv", "object" })]
        public Dictionary<string, object> FindNearestNeighbor(UV testPoint)
        {
            if (_Impl == null)
            {
                return null;
            }

            var searchResult = _Impl.NearestNeighbors(point: new double[] { testPoint.U, testPoint.V }, neighbors: 1);

            return new Dictionary<string, object>
            {
                { "uv", UV.ByCoordinates(searchResult[0].Item1[0], searchResult[0].Item1[1]) },
                { "object", searchResult[0].Item2 }
            };
        }

        [MultiReturn(new[] { "uvs", "objects" })]
        public Dictionary<string, object> FindNearestNeighbors(UV testPoint, int numNeighbors)
        {
            if (_Impl == null)
            {
                return null;
            }

            var searchResult = _Impl.NearestNeighbors(point: new double[] { testPoint.U, testPoint.V }, neighbors: numNeighbors);

            var uvCollection = new List<UV>();
            var objectCollection = new List<object>();

            foreach (var result in searchResult)
            {
                uvCollection.Add(UV.ByCoordinates(result.Item1[0], result.Item1[1]));
                objectCollection.Add(result.Item2);
            }

            return new Dictionary<string, object>
            {
                { "uvs", uvCollection },
                { "objects", hasObjects ? objectCollection : null }
            };
        }
    }
}
