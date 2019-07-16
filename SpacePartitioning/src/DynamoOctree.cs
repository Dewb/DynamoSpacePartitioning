using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SpacePartitioning
{
    public class Octree
    {
        private SimpleOctree _Impl = null;

        private Octree()
        {
        }

        public static Octree ByObjectsAndBoundingBoxes(object[] objects, BoundingBox[] boundingBoxes)
        {
            var q = new Octree();
            BoundingBox worldBox = BoundingBox.ByCorners(boundingBoxes[0].MinPoint, boundingBoxes[0].MaxPoint);
            for (int i = 1; i < boundingBoxes.Length; i++)
            {
                worldBox = Utils.UnionBoundingBoxes(worldBox, boundingBoxes[i]);
            }

            q._Impl = new SimpleOctree(worldBox, new OctreeOptions());

            for (int i = 0; i < Math.Min(objects.Length, boundingBoxes.Length); i++)
            {
                q._Impl.Add(objects[i], boundingBoxes[i]);
            }

            return q;
        }

        public List<object> FindObjectsIntersectingBoundingBox(BoundingBox boundingBox)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.QueryRange(boundingBox);
        }

        public List<object> FindObjectsIntersectingPoint(Point testPoint)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.QueryHit(testPoint);
        }

        public List<object> FindObjectsWithinRadius(Point testPoint, double testRadius)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.QueryRadius(testPoint, testRadius);
        }

        public List<object> FindObjectsIntersectingRay(Point rayPoint, Vector rayVector)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.QueryRay(rayPoint, rayVector);
        }

        public object FindNearestNeighbor(Point testPoint)
        {
            if (_Impl == null)
            {
                return null;
            }
            return _Impl.QueryNearestNeighbor(testPoint);
        }

        /*
        public List<object> FindObjectsByFrustrum(Point origin, double viewAngle, double distance)
        {
            throw new NotImplementedException();
            //var l = new List<object>();
            //return l;
        }

        public List<object> FindObjectsByDistance(Point origin, double distance)
        {
            throw new NotImplementedException();
            //var l = new List<object>();
            //return l;
        }

        [MultiReturn(new[] { "visibleObjects", "visibleFrom" })]
        public Dictionary<string, List<List<int>>> Compute2DVisibility(double viewHeading, double fieldOfView, double distance)
        {
            throw new NotImplementedException();

            //var visibleObjects = new List<List<int>>();
            //var visibleFrom = new List<List<int>>();

            //return new Dictionary<string, List<List<int>>>
            //{
            //    { "visibleObjects", visibleObjects },
            //    { "visibleFrom", visibleFrom }
            //};
        }

        [MultiReturn(new[] { "visibleObjects", "visibleFrom" })]
        public Dictionary<string, List<List<int>>> Compute3DVisibility(Vector viewHeading, double horizontalFieldOfView, double verticalFieldOfView, double distance)
        {
            throw new NotImplementedException();

            //var visibleObjects = new List<List<int>>();
            //var visibleFrom = new List<List<int>>();

            //return new Dictionary<string, List<List<int>>>
            //{
            //    { "visibleObjects", visibleObjects },
            //    { "visibleFrom", visibleFrom }
            //};
        }


        public List<List<int>> ComputeAdjacencies(double tolerance = 0.001)
        {
            throw new NotImplementedException();
            //var adjacencies = new List<List<int>>();
            //return adjacencies;
        }


        public List<List<int>> ComputeNeighbors(double distance)
        {
            throw new NotImplementedException();
            //var neighbors = new List<List<int>>();
            //return neighbors;
        }

        public List<List<int>> ComputeOverlaps()
        {
            throw new NotImplementedException();
            //var overlaps = new List<List<int>>();
            //return overlaps;
        }

        public List<Dictionary<int, double>> ComputeDistances()
        {
            throw new NotImplementedException(); 
            //var distances = new List<Dictionary<int, double>>();
            //return distances;
        }
        */
       
    }
}
