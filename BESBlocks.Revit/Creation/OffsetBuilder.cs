using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common;
using BESBlocks.Revit.Common.Enums;
using BESBlocks.Revit.Common.Extensions;

namespace BESBlocks.Revit.Creation
{
    public class OffsetBuilder
    {
        private readonly Document _doc;

        public OffsetBuilder(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }
       
        public void Create(IEnumerable<MEPCurvePair> pairs, Plane centerPlane, double angle)
        {
            foreach (MEPCurvePair pair in pairs)
            {
                Create(pair, centerPlane, angle);
            }
        }

        public void Create(MEPCurvePair pair, Plane centerPlane, double angle)
        {
            if (!pair.IsParallel)
                throw new OperationCanceledException("Elements is not parallel.");
            
            double maxDistance = pair.FirstNearest.Origin.DistanceTo(pair.SecondNearest.Origin);

            Line firstLine = Line.CreateBound(pair.FirstDistant.Origin,
                pair.FirstNearest.Origin.MoveFrom(pair.FirstDistant.Origin, maxDistance));
            Line secondLine = Line.CreateBound(pair.SecondDistant.Origin,
                pair.SecondNearest.Origin.MoveFrom(pair.SecondDistant.Origin, maxDistance));

            PlaneIntersectionResult firstIntersectionResult =
                centerPlane.Intersect(firstLine, 0.0, out XYZ pointB, out double firstParameter);
            PlaneIntersectionResult secondIntersectionResult =
                centerPlane.Intersect(secondLine, 0.0, out XYZ pointC, out double secondParameter);

            XYZ pointX = pointB.MiddleTo(pointC);

            double distanceBX = pointB.DistanceTo(pointX);
            double distanceCX = pointC.DistanceTo(pointX);

            double angleInRad = angle.DegreeToRadian();

            double offset = distanceBX / Math.Tan(angleInRad);

            pair.FirstNearest.Origin = pointB.MoveTo(pair.FirstNearest.Origin, offset);
            pair.SecondNearest.Origin = pointC.MoveTo(pair.SecondNearest.Origin, offset);

            MEPCurve mepCurveBC = pair.First.Clone(pair.FirstNearest.Origin, pair.SecondNearest.Origin);

            Connector conB = mepCurveBC.ConnectorManager.Connectors.NearestTo(pair.FirstNearest.Origin);
            Connector conC = mepCurveBC.ConnectorManager.Connectors.NearestTo(pair.SecondNearest.Origin);

            FamilyInstance elbowABC = _doc.Create.NewElbowFitting(pair.FirstNearest, conB);
            FamilyInstance elbowBCD = _doc.Create.NewElbowFitting(pair.SecondNearest, conC);

            pair.First.TransferTextParametersTo(elbowABC);
            pair.First.TransferTextParametersTo(elbowBCD);

            pair.First.TransferWorksetToElement(elbowABC);
            pair.First.TransferWorksetToElement(elbowBCD);
        }
    }
}