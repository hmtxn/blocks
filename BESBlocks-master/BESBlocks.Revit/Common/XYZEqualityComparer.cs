using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common
{
    public class XYZEqualityComparer : IEqualityComparer<XYZ>
    {
        private readonly int _digits;
        public XYZEqualityComparer(int digits)
        {
            _digits = digits;
        }

        public bool Equals(XYZ firstPoint, XYZ secondPoint)
        {
            double firstX = Math.Round(firstPoint.X, _digits);
            double firstY = Math.Round(firstPoint.Y, _digits);
            double firstZ = Math.Round(firstPoint.Z, _digits);

            double secondX = Math.Round(secondPoint.X, _digits);
            double secondY = Math.Round(secondPoint.Y, _digits);
            double secondZ = Math.Round(secondPoint.Z, _digits);

            bool resultX = firstX == secondX;
            bool resultY = firstY == secondY;
            bool resultZ = firstZ == secondZ;

            return resultX && resultY && resultZ;
        }

        public int GetHashCode(XYZ point)
        {
            double x = Math.Round(point.X, _digits);
            double y = Math.Round(point.Y, _digits);
            double z = Math.Round(point.Z, _digits);

            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

    }
}