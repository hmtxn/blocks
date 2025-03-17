using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class LineExtensions
    {
        public static Line ToZeroZ(this Line line)
        {
            XYZ start = line.GetEndPoint(0);
            XYZ end = line.GetEndPoint(1);

            XYZ startZeroZ = new XYZ(start.X, start.Y, 0.0);
            XYZ endZeroZ = new XYZ(end.X, end.Y, 0.0);

            Line lineZeroZ = Line.CreateBound(startZeroZ, endZeroZ);

            return lineZeroZ;
        }

        public static double AngleTo(this Line baseLine, Line line)
        {
            List<XYZ> basePoints = baseLine.GetPoints();
            List<XYZ> points = line.GetPoints();

            XYZ pointA = basePoints.DistantTo(points);
            XYZ pointB = basePoints.NearestTo(points);
            XYZ pointC = points.NearestTo(basePoints);
            XYZ pointD = points.DistantTo(basePoints);

            XYZ directionBA = pointB.DirectionTo(pointA);
            XYZ directionDC = pointC.DirectionTo(pointD);

            double angle = directionBA.AngleTo(directionDC);

            return angle;
        }

        public static bool IsPerpendicularTo(this Line baseLine, Line line, double angleToleranceInRad)
        {
            double angle = baseLine.AngleTo(line);

            double value = Math.Abs(angle - (Math.PI / 2.0));

            return value <= angleToleranceInRad;
        }

        public static bool IsParallelTo(this Line baseLine, Line line, double angleToleranceInRad)
        {
            double angle = baseLine.AngleTo(line);

            double value = Math.Abs(angle - Math.PI);

            return value <= angleToleranceInRad;
        }
    }
}