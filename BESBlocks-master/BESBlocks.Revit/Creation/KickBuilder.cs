using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common;
using BESBlocks.Revit.Common.Extensions;

namespace BESBlocks.Revit.Creation
{
    public class KickBuilder
    {
        private readonly Document _doc;
        
        public KickBuilder(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }

        public void Create(IEnumerable<MEPCurvePair> pairs, double angle)
        {
            foreach (MEPCurvePair pair in pairs)
            {
                Create(pair, angle);
            }
        }
        
        public void Create(MEPCurvePair pair, double angle)
        {
            if (pair.IsParallel)
                throw new OperationCanceledException("Elements is parallel.");

            if (pair.Offset <= MEPCurveSorter.OFFSET_TOLERANCE)
                throw new OperationCanceledException("Create a kick connection with a zero offset of elements is not possible.");

            double distanceBase = pair.SecondDistant.Origin.DistanceTo(pair.SecondIntersectionPoint);

            double distance = pair.Offset / Math.Tan(angle.DegreeToRadian());

            double secondLength = distanceBase - distance;

            if (secondLength <= 0.0)
                throw new OperationCanceledException($"This angle of {angle} degrees cannot be used to create a kick connection.");

            pair.FirstNearest.Origin = pair.FirstIntersectionPoint;
            pair.SecondNearest.Origin = pair.SecondDistant.Origin.MoveTo(pair.SecondNearest.Origin, secondLength);

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