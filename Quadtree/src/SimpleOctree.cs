using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace SpacePartitioning
{
    [IsVisibleInDynamoLibrary(false)]
    public class SimpleOctree
    {
        List<object> _objectReferences;
        SimpleOctreeNode _rootNode;

        public OctreeOptions Options;

        public SimpleOctree(BoundingBox worldBox, OctreeOptions options)
        {
            Options = options;
            _objectReferences = new List<object>();
            _rootNode = new SimpleOctreeNode(this, worldBox);
        }

        public void Add(object obj, BoundingBox boundingBox)
        {
            _objectReferences.Add(obj);
            int objectIndex = _objectReferences.Count - 1;
            _rootNode.Add(objectIndex, boundingBox);
        }

        public List<object> QueryRange(BoundingBox searchBox)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.QueryRange(searchBox))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> QueryRay(Point rayPoint, Vector rayVector)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.QueryRay(rayPoint, rayVector))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> QueryRadius(Point origin, double radius)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.QueryRadius(origin, radius))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> QueryHit(Point testPoint)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.QueryHit(testPoint))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public object QueryNearestNeighbor(Point testPoint)
        {
            int index = _rootNode.QueryNearestNeighbor(testPoint);
            return _objectReferences[index];
        }

        private class SimpleOctreeNode
        {
            SimpleOctree _parent;
            List<SimpleOctreeNode> _childNodes = null;
            List<Tuple<int, BoundingBox>> _childObjects;
            BoundingBox _boundingBox;
            Point _origin;
            Vector _halfDimension;

            public SimpleOctreeNode(SimpleOctree parent, BoundingBox nodeBox)
            {
                _parent = parent;
                _boundingBox = nodeBox;
                _origin = Utils.GetOriginFromBoundingBox(_boundingBox);
                _halfDimension = Utils.GetHalfDimensionsFromBoundingBox(_boundingBox);
                _childObjects = new List<Tuple<int, BoundingBox>>();
            }

            public bool Add(int objectIndex, BoundingBox boundingBox)
            {
                if (!Utils.NodeContainsBoundingBox(_origin, _halfDimension, boundingBox))
                    return false;

                if (!IsLeaf())
                {
                    int octant = GetOctantContainingPoint(Utils.GetOriginFromBoundingBox(boundingBox));
                    bool wasAddedToChild = _childNodes[octant].Add(objectIndex, boundingBox);
                    if (wasAddedToChild)
                    {
                        return true;
                    }
                }

                _childObjects.Add(new Tuple<int, BoundingBox>(objectIndex, boundingBox));
                if (IsLeaf() && _childObjects.Count > _parent.Options.LeafCapacity)
                {
                    Split();
                }

                return true;
            }

            public bool IsLeaf()
            {
                return _childNodes == null;
            }

            private int GetOctantContainingPoint(Point pt)
            {
                int octant = 0;
                if (pt.X >= _origin.X) octant |= 4;
                if (pt.Y >= _origin.Y) octant |= 2;
                if (pt.Z >= _origin.Z) octant |= 1;
                return octant;
            }

            private void Split()
            {
                if (!IsLeaf())
                {
                    return;
                }

                _childNodes = new List<SimpleOctreeNode>();
                for (int i = 0; i < 8; ++i)
                {
                    double x = _origin.X + _halfDimension.X * ((i & 4) != 0 ? .5 : -.5);
                    double y = _origin.Y + _halfDimension.Y * ((i & 2) != 0 ? .5 : -.5);
                    double z = _origin.Z + _halfDimension.Z * ((i & 1) != 0 ? .5 : -.5);
                    Point newOrigin = Point.ByCoordinates(x, y, z);
                    Vector newHalfDimension = _halfDimension.Scale(0.5);
                    _childNodes.Add(new SimpleOctreeNode(_parent, BoundingBox.ByCorners(newOrigin.Subtract(newHalfDimension), newOrigin.Add(newHalfDimension))));
                }

                var oldChildren = new List<Tuple<int, BoundingBox>>();
                oldChildren.AddRange(_childObjects);
                _childObjects = new List<Tuple<int, BoundingBox>>();

                foreach (var tuple in oldChildren)
                {
                    bool wasAddedToChild = Add(tuple.Item1, tuple.Item2);
                    if (!wasAddedToChild)
                    {
                        _childObjects.Add(tuple);
                    }
                }
            }

            private List<int> GetAllChildObjects()
            {
                var ret = new List<int>();

                foreach (var childObject in _childObjects)
                {
                    ret.Add(childObject.Item1);
                }

                if (!IsLeaf())
                {
                    foreach (var childNode in _childNodes)
                    {
                        ret.AddRange(childNode.GetAllChildObjects());
                    }
                }

                return ret;
            }

            public List<int> QueryRange(BoundingBox searchBox)
            {
                var ret = new List<int>();

                if (!IsLeaf())
                {
                    foreach (var childNode in _childNodes)
                    {
                        if (searchBox.Intersects(childNode._boundingBox))
                        {
                            ret.AddRange(childNode.QueryRange(searchBox));
                        }
                    }
                }

                foreach (var obj in _childObjects)
                {
                    if (searchBox.Intersects(obj.Item2))
                    {
                        ret.Add(obj.Item1);
                    }
                }

                return ret;
            }

            public List<int> QueryRay(Point rayPoint, Vector rayVector)
            {
                throw new NotImplementedException();
                //var ret = new List<int>();
                //return ret;
            }

            public List<int> QueryRadius(Point origin, double radius)
            {
                var ret = new List<int>();

                foreach (var obj in _childObjects)
                {
                    if (Utils.BoundingBoxIntersectsSphere(obj.Item2, origin, radius))
                    {
                        ret.Add(obj.Item1);
                    }
                }

                if (!IsLeaf())
                {
                    foreach (var node in _childNodes)
                    {
                        if (Utils.BoundingBoxIntersectsSphere(node._boundingBox, origin, radius))
                        {
                            ret.AddRange(node.QueryRadius(origin, radius));
                        }
                    }
                }

                return ret;
            }

            public List<int> QueryHit(Point testPoint)
            {
                var ret = new List<int>();

                if (!IsLeaf())
                {
                    foreach (var childNode in _childNodes)
                    {
                        if (childNode._boundingBox.Contains(testPoint))
                        {
                            ret.AddRange(childNode.QueryHit(testPoint));
                        }
                    }
                }

                foreach (var obj in _childObjects)
                {
                    if (obj.Item2.Contains(testPoint))
                    {
                        ret.Add(obj.Item1);
                    }
                }

                return ret;
            }

            public int QueryNearestNeighbor(Point testPoint)
            {
                throw new NotImplementedException();
                //var ret = new List<int>();
                //return ret;
            }
        }
    }
}
