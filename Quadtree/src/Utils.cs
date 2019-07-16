using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using System;

namespace SpacePartitioning
{
    [IsVisibleInDynamoLibrary(false)]
    class Utils
    {
        public static Point GetOriginFromBoundingBox(BoundingBox boundingBox)
        {
            return boundingBox.MinPoint.Add(GetHalfDimensionsFromBoundingBox(boundingBox));
        }

        public static Vector GetHalfDimensionsFromBoundingBox(BoundingBox boundingBox)
        {
            return Vector.ByCoordinates(
                (boundingBox.MaxPoint.X - boundingBox.MinPoint.X) * 0.5,
                (boundingBox.MaxPoint.Y - boundingBox.MinPoint.Y) * 0.5,
                (boundingBox.MaxPoint.Z - boundingBox.MinPoint.Z) * 0.5
            );
        }
        private static double DOUBLE_EPS = 10e-6;

        public static bool leqEps(double a, double b)
        {
            return a <= b + DOUBLE_EPS;
        }

        public static bool geqEps(double a, double b)
        {
            return a + DOUBLE_EPS >= b;
        }

        public static bool NodeContainsBoundingBox(Point origin, Vector halfDimension, BoundingBox boundingBox)
        {
            if (leqEps(boundingBox.MaxPoint.X, origin.X + halfDimension.X) &&
                leqEps(boundingBox.MaxPoint.Y, origin.Y + halfDimension.Y) &&
                leqEps(boundingBox.MaxPoint.Z, origin.Z + halfDimension.Z) &&
                geqEps(boundingBox.MinPoint.X, origin.X - halfDimension.X) &&
                geqEps(boundingBox.MinPoint.Y, origin.Y - halfDimension.Y) &&
                geqEps(boundingBox.MinPoint.Z, origin.Z - halfDimension.Z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static BoundingBox UnionBoundingBoxes(BoundingBox a, BoundingBox b)
        {
            return BoundingBox.ByCorners(
                Point.ByCoordinates(Math.Min(a.MinPoint.X, b.MinPoint.X), Math.Min(a.MinPoint.Y, b.MinPoint.Y), Math.Min(a.MinPoint.Z, b.MinPoint.Z)),
                Point.ByCoordinates(Math.Max(a.MaxPoint.X, b.MaxPoint.X), Math.Max(a.MaxPoint.Y, b.MaxPoint.Y), Math.Max(a.MaxPoint.Z, b.MaxPoint.Z))
            );
        }

        public static double DistanceSquared(Point a, Point b)
        {
            return Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2) + Math.Pow(b.Z - a.Z, 2);
        }

        public static bool BoundingBoxIntersectsSphere(BoundingBox bbox, Point p, double radius)
        {
            double d = radius * radius;

            if (p.X < bbox.MinPoint.X)
            {
                d -= Math.Pow(p.X - bbox.MinPoint.X, 2);
            }
            else if (p.X > bbox.MaxPoint.X)
            {
                d -= Math.Pow(p.X - bbox.MaxPoint.X, 2);
            }

            if (p.Y < bbox.MinPoint.Y)
            {
                d -= Math.Pow(p.Y - bbox.MinPoint.Y, 2);
            }
            else if (p.Y > bbox.MaxPoint.Y)
            {
                d -= Math.Pow(p.Y - bbox.MaxPoint.Y, 2);
            }

            if (p.Z < bbox.MinPoint.Z)
            {
                d -= Math.Pow(p.Z - bbox.MinPoint.Z, 2);
            }
            else if (p.Z > bbox.MaxPoint.Z)
            {
                d -= Math.Pow(p.Z - bbox.MaxPoint.Z, 2);
            }

            return d > 0;
        }
    }
}
