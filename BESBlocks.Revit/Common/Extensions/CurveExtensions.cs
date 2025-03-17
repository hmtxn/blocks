using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class CurveExtensions
    {
        public static bool IsHorizontal(this Curve curve, double slopeTolerance = 0.0)
        {
            // slopeTolerance in percentage

            XYZ start = curve.GetEndPoint(0);
            XYZ end = curve.GetEndPoint(1);

            XYZ tempStart = new XYZ(start.X, start.Y, 0.0);
            XYZ tempEnd = new XYZ(end.X, end.Y, 0.0);

            double rise = Math.Abs(start.Z - end.Z);
            double run = tempStart.DistanceTo(tempEnd);
            double slope = (rise / run) * 100.0;

            double roundedSlope = Math.Round(slope, 3);

            return roundedSlope <= slopeTolerance;
        }

        public static bool IsVertical(this Curve curve)
        {
            XYZ start = curve.GetEndPoint(0);
            XYZ end = curve.GetEndPoint(1);
            XYZ direction = end - start;
            XYZ normalize = direction.Normalize();

            double z = Math.Round(Math.Abs(normalize.Z), 3);

            return z == 1.0;
        }

        public static List<XYZ> GetPoints(this Curve curve)
        {
            return new List<XYZ>()
            {
                curve.GetEndPoint(0),
                curve.GetEndPoint(1),
            };
        }
    }
}