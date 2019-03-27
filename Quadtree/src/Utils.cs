using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using System;

namespace DynamoQuadtree
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
    }
}
