using System;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common.Enums;
using BESBlocks.Revit.Common.Extensions;

namespace BESBlocks.Revit.Common
{
    public class MEPCurvePair
    {
        public MEPCurve First { get; }
        public MEPCurve Second { get; }
        public Connector FirstNearest { get; }
        public Connector FirstDistant { get; }
        public Connector SecondNearest { get; }
        public Connector SecondDistant { get; }
        public Plane FirstPlane { get; }
        public Plane SecondPlane { get; }
        public XYZ FirstIntersectionPoint { get; }
        public XYZ SecondIntersectionPoint { get; }
        public XYZ NearestDirection { get; }
        public double AngleInRad { get; }
        public double FirstDistance { get; }
        public double SecondDistance { get; }
        public double Offset { get; }
        public bool IsParallel { get; }

        public MEPCurvePair(MEPCurve first, MEPCurve second)
        {
            First = first ?? throw new ArgumentNullException(nameof(first));
            Second = second ?? throw new ArgumentNullException(nameof(second));

            FirstNearest = First.ConnectorManager.Connectors.NearestTo(Second.ConnectorManager.Connectors);
            SecondNearest = Second.ConnectorManager.Connectors.NearestTo(FirstNearest.Origin);

            NearestDirection = FirstNearest.Origin.DirectionTo(SecondNearest.Origin);

            FirstDistant = First.ConnectorManager.Connectors.DistantTo(Second.ConnectorManager.Connectors);
            SecondDistant = Second.ConnectorManager.Connectors.DistantTo(FirstDistant.Origin);

            AngleInRad = FirstNearest.CoordinateSystem.BasisZ.AngleTo(SecondNearest.CoordinateSystem.BasisZ);

            double angle = Math.Round(AngleInRad.RadianToDegree());

            IsParallel = angle == 0.0 || angle == 180.0;

            if (IsParallel)
            {
                FirstPlane = Plane.CreateByOriginAndBasis(FirstNearest.CoordinateSystem.Origin,
                    FirstNearest.CoordinateSystem.BasisX, FirstNearest.CoordinateSystem.BasisY);
                SecondPlane = Plane.CreateByOriginAndBasis(SecondNearest.CoordinateSystem.Origin,
                    SecondNearest.CoordinateSystem.BasisX, SecondNearest.CoordinateSystem.BasisY);
            }
            else
            {
                XYZ crossProduct = FirstNearest.CoordinateSystem.BasisZ
                    .CrossProduct(SecondNearest.CoordinateSystem.BasisZ).Normalize();

                FirstPlane = Plane.CreateByOriginAndBasis(FirstNearest.CoordinateSystem.Origin,
                    FirstNearest.CoordinateSystem.BasisZ, crossProduct);
                SecondPlane = Plane.CreateByOriginAndBasis(SecondNearest.CoordinateSystem.Origin,
                    SecondNearest.CoordinateSystem.BasisZ, crossProduct);
            }

            double maxDistance = FirstDistant.Origin.DistanceTo(SecondDistant.Origin);

            Line firstLine = Line.CreateBound(FirstDistant.Origin,
                FirstNearest.Origin.MoveFrom(FirstDistant.Origin, maxDistance));
            Line secondLine = Line.CreateBound(SecondDistant.Origin,
                SecondNearest.Origin.MoveFrom(SecondDistant.Origin, maxDistance));

            PlaneIntersectionResult firstIntersectionResult = SecondPlane.Intersect(firstLine, 0.0,
                out XYZ intersectionFirstPoint, out double firstParameter);
            PlaneIntersectionResult secondIntersectionResult = FirstPlane.Intersect(secondLine, 0.0,
                out XYZ intersectionSecondPoint, out double secondParameter);

            FirstIntersectionPoint = intersectionFirstPoint;
            SecondIntersectionPoint = intersectionSecondPoint;

            FirstDistance = FirstDistant.Origin.DistanceTo(FirstIntersectionPoint);
            SecondDistance = SecondDistant.Origin.DistanceTo(SecondIntersectionPoint);

            if (IsParallel)
            {
                Offset = FirstNearest.Origin.DistanceTo(SecondIntersectionPoint);
            }
            else
            {
                Offset = FirstIntersectionPoint.DistanceTo(SecondIntersectionPoint);
            }
        }
    }
}