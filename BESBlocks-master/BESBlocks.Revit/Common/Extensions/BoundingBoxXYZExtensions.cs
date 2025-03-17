using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class BoundingBoxXYZExtensions
    {
        public static void ExpandToContain(this BoundingBoxXYZ box, XYZ point)
        {
            double xMin = box.Min.X;
            double yMin = box.Min.Y;
            double zMin = box.Min.Z;

            double xMax = box.Max.X;
            double yMax = box.Max.Y;
            double zMax = box.Max.Z;

            if (point.X < xMin) { xMin = point.X; }
            if (point.Y < yMin) { yMin = point.Y; }
            if (point.Z < zMin) { zMin = point.Z; }
            if (point.X > xMax) { xMax = point.X; }
            if (point.Y > yMax) { yMax = point.Y; }
            if (point.Z > zMax) { zMax = point.Z; }

            XYZ min = new XYZ(xMin, yMin, zMin);
            XYZ max = new XYZ(xMax, yMax, zMax);

            box.Min = min;
            box.Max = max;
        }
        
        public static void Expand(this BoundingBoxXYZ box, double distance)
        {
            box.Max = box.Max + (XYZ.BasisX * distance);
            box.Max = box.Max + (XYZ.BasisY * distance);
            box.Max = box.Max + (XYZ.BasisZ * distance);

            box.Min = box.Min + (XYZ.BasisX.Negate() * distance);
            box.Min = box.Min + (XYZ.BasisY.Negate() * distance);
            box.Min = box.Min + (XYZ.BasisZ.Negate() * distance);
        }
    }
}