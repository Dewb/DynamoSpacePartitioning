using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

namespace DynamoQuadtree
{
    [IsVisibleInDynamoLibrary(false)]
    public class SimpleQuadtree
    {
        List<object> _objectReferences;
        SimpleQuadtreeNode _rootNode;

        public QuadtreeOptions Options;

        public SimpleQuadtree(BoundingBox worldBox, QuadtreeOptions options)
        {
            Options = options;
            _objectReferences = new List<object>();
            _rootNode = new SimpleQuadtreeNode(this, worldBox);
        }

        public void Add(object obj, BoundingBox boundingBox)
        {
            _objectReferences.Add(obj);
            int objectIndex = _objectReferences.Count - 1;
            _rootNode.Add(objectIndex, boundingBox);
        }

        public List<object> Query(BoundingBox searchBox)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.Query(searchBox))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> Query(Vector ray)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.Query(ray))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> Query(Point origin, double radius)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.Query(origin, radius))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        public List<object> Query(Point testPoint)
        {
            var ret = new List<object>();
            foreach (int index in _rootNode.Query(testPoint))
            {
                ret.Add(_objectReferences[index]);
            }
            return ret;
        }

        private class SimpleQuadtreeNode
        {
            SimpleQuadtree _parent;
            List<SimpleQuadtreeNode> _childNodes = null;
            List<Tuple<int, BoundingBox>> _childObjects;
            BoundingBox _boundingBox;
            Point _origin;
            Vector _halfDimension;

            public SimpleQuadtreeNode(SimpleQuadtree parent, BoundingBox nodeBox)
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

                _childNodes = new List<SimpleQuadtreeNode>();
                for (int i = 0; i < 8; ++i)
                {
                    double x = _origin.X + _halfDimension.X * ((i & 4) != 0 ? .5 : -.5);
                    double y = _origin.Y + _halfDimension.Y * ((i & 2) != 0 ? .5 : -.5);
                    double z = _origin.Z + _halfDimension.Z * ((i & 1) != 0 ? .5 : -.5);
                    Point newOrigin = Point.ByCoordinates(x, y, z);
                    Vector newHalfDimension = _halfDimension.Scale(0.5);
                    _childNodes.Add(new SimpleQuadtreeNode(_parent, BoundingBox.ByCorners(newOrigin.Subtract(newHalfDimension), newOrigin.Add(newHalfDimension))));
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

            public List<int> Query(BoundingBox searchBox)
            {
                var ret = new List<int>();

                if (!IsLeaf())
                {
                    foreach (var childNode in _childNodes)
                    {
                        if (searchBox.Intersects(childNode._boundingBox))
                        {
                            ret.AddRange(childNode.Query(searchBox));
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

            public List<int> Query(Vector ray)
            {
                var ret = new List<int>();
                return ret;
            }

            public List<int> Query(Point origin, double radius)
            {
                var ret = new List<int>();
                return ret;
            }

            public List<int> Query(Point testPoint)
            {
                var ret = new List<int>();

                if (!IsLeaf())
                {
                    foreach (var childNode in _childNodes)
                    {
                        if (childNode._boundingBox.Contains(testPoint))
                        {
                            ret.AddRange(childNode.Query(testPoint));
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

        }
    }
}
