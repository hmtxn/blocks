using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class XYZExtensions
    {
        public static bool IsEqual(this XYZ basePoint, XYZ point, int digits)
        {
            double baseX = Math.Round(basePoint.X, digits);
            double baseY = Math.Round(basePoint.Y, digits);
            double baseZ = Math.Round(basePoint.Z, digits);

            double x = Math.Round(point.X, digits);
            double y = Math.Round(point.Y, digits);
            double z = Math.Round(point.Z, digits);

            bool resultX = baseX == x;
            bool resultY = baseY == y;
            bool resultZ = baseZ == z;

            return resultX && resultY && resultZ;
        }

        public static XYZ MoveTo(this XYZ movablePoint, XYZ point, double distance)
        {
            XYZ direction = movablePoint.DirectionTo(point);

            return Move(movablePoint, direction, distance);
        }

        public static XYZ MoveFrom(this XYZ movablePoint, XYZ point, double distance)
        {
            XYZ direction = point.DirectionTo(movablePoint);

            return Move(movablePoint, direction, distance);
        }

        public static XYZ Move(this XYZ movablePoint, XYZ direction, double distance)
        {
            return movablePoint + (direction * distance);
        }
        
        public static XYZ MiddleTo(this XYZ start, XYZ end)
        {
            return (start + end) * 0.5;
        }

        public static XYZ DirectionTo(this XYZ startPoint, XYZ point)
        {
            return (point - startPoint).Normalize();
        }

        public static XYZ NearestTo(this IEnumerable<XYZ> basePoints, XYZ point)
        {
            XYZ targetPoint = null;
            double targetDistance = double.MaxValue;

            foreach (XYZ basePoint in basePoints)
            {
                double distance = basePoint.DistanceTo(point);
                if (targetDistance > distance)
                {
                    targetPoint = basePoint;
                    targetDistance = distance;
                }
            }

            return targetPoint;
        }

        public static XYZ NearestTo(this IEnumerable<XYZ> basePoints, IEnumerable<XYZ> points)
        {
            XYZ targetPoint = null;
            double targetDistance = double.MaxValue;

            foreach (XYZ basePoint in basePoints)
            {
                foreach (XYZ point in points)
                {
                    double distance = basePoint.DistanceTo(point);
                    if (targetDistance > distance)
                    {
                        targetPoint = basePoint;
                        targetDistance = distance;
                    }
                }
            }

            return targetPoint;
        }

        public static XYZ DistantTo(this IEnumerable<XYZ> basePoints, XYZ point)
        {
            XYZ targetPoint = null;
            double targetDistance = double.MinValue;

            foreach (XYZ basePoint in basePoints)
            {
                double distance = basePoint.DistanceTo(point);
                if (targetDistance < distance)
                {
                    targetPoint = basePoint;
                    targetDistance = distance;
                }
            }

            return targetPoint;
        }

        public static XYZ DistantTo(this IEnumerable<XYZ> basePoints, IEnumerable<XYZ> points)
        {
            XYZ targetPoint = null;
            double targetDistance = double.MinValue;

            foreach (XYZ basePoint in basePoints)
            {
                foreach (XYZ point in points)
                {
                    double distance = basePoint.DistanceTo(point);
                    if (targetDistance < distance)
                    {
                        targetPoint = basePoint;
                        targetDistance = distance;
                    }
                }
            }

            return targetPoint;
        }
    }
}