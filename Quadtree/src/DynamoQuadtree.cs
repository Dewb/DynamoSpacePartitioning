using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoQuadtree
{
    public class Quadtree
    {
        private SimpleQuadtree _Impl = null;

        private Quadtree()
        {
        }

        public List<object> FindObjectsByBoundingBox(BoundingBox boundingBox)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.Query(boundingBox);
        }

        public List<object> FindObjectsByPoint(Point testPoint)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.Query(testPoint);
        }

        public List<object> FindObjectsByFrustrum(Point origin, double viewAngle, double distance)
        {
            var l = new List<object>();
            return l;
        }

        public List<object> FindObjectsByDistance(Point origin, double distance)
        {
            var l = new List<object>();
            return l;
        }

        [MultiReturn(new[] { "visibleObjects", "visibleFrom" })]
        public Dictionary<string, object> BidirectionalVisibility(double viewAngle, double distance)
        {
            var visibleObjects = new Dictionary<object, List<object>>();
            var visibleFrom = new Dictionary<object, List<object>>();

            return new Dictionary<string, object>
            {
                { "visibleObjects", visibleObjects },
                { "visibleFrom", visibleFrom }
            };
        }

        public Dictionary<object, List<object>> FindAllAdjacencies(double tolerance = 0.001)
        {
            var adjacencies = new Dictionary<object, List<object>>();

            return adjacencies;
        }


        public Dictionary<object, List<object>> FindAllNeighbors(double distance)
        {
            var neighbors = new Dictionary<object, List<object>>();

            return neighbors;
        }

        public List<List<object>> FindAllOverlapping()
        {
            var overlaps = new List<List<object>>();

            return overlaps;
        }
        
        public static Quadtree ByObjectsAndBoundingBoxes(object[] objects, BoundingBox[] boundingBoxes)
        {
            var q = new Quadtree();
            BoundingBox worldBox = BoundingBox.ByCorners(boundingBoxes[0].MinPoint, boundingBoxes[0].MaxPoint);
            for (int i = 1; i < boundingBoxes.Length; i++)
            {
                worldBox = Utils.UnionBoundingBoxes(worldBox, boundingBoxes[i]);
            }

            q._Impl = new SimpleQuadtree(worldBox, new QuadtreeOptions());

            for (int i = 0; i < Math.Min(objects.Length, boundingBoxes.Length); i++)
            {
                q._Impl.Add(objects[i], boundingBoxes[i]);
            }
            
            return q;
        }

    }
}
