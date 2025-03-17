using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class MEPCurveExtensions
    {
               public static MEPCurve Clone(this MEPCurve baseMEPCurve, XYZ start, XYZ end)
        {
            MEPCurve clone = ElementTransformUtils.CopyElement(baseMEPCurve.Document, baseMEPCurve.Id, start)
                .Select(i => baseMEPCurve.Document.GetElement(i))
                .Cast<MEPCurve>()
                .First();

            List<Connector> connectors = clone.ConnectorManager.Connectors.ToList();

            connectors[0].Origin = start;
            connectors[1].Origin = end;

            return clone;
        }
        public static Line AsLine(this MEPCurve mepCurve)
        {
            LocationCurve locCurve = mepCurve.Location as LocationCurve;

            return locCurve.Curve as Line;
        }
        public static bool IsPerpendicularOnZeroZTo(this MEPCurve baseMEPCurve, MEPCurve mepCurve, double angleToleranceInRad)
        {
            Line baseLine = baseMEPCurve.AsLine();
            Line line = mepCurve.AsLine();

            Line baseLineZeroZ = baseLine.ToZeroZ();
            Line lineZeroZ = line.ToZeroZ();

            return baseLineZeroZ.IsPerpendicularTo(lineZeroZ, angleToleranceInRad);
        }
        public static bool IsParallelOnZeroZTo(this MEPCurve baseMEPCurve, MEPCurve mepCurve, double angleToleranceInRad)
        {
            Line baseLine = baseMEPCurve.AsLine();
            Line line = mepCurve.AsLine();

            Line baseLineZeroZ = baseLine.ToZeroZ();
            Line lineZeroZ = line.ToZeroZ();

            return baseLineZeroZ.IsParallelTo(lineZeroZ, angleToleranceInRad);
        }
        public static bool IsPerpendicularTo(this MEPCurve baseMEPCurve, MEPCurve mepCurve, double angleToleranceInRad)
        {
            Line baseLine = baseMEPCurve.AsLine();
            Line line = mepCurve.AsLine();

            return baseLine.IsPerpendicularTo(line, angleToleranceInRad);
        }
        public static bool IsParallelTo(this MEPCurve baseMEPCurve, MEPCurve mepCurve, double angleToleranceInRad)
        {
            Line baseLine = baseMEPCurve.AsLine();
            Line line = mepCurve.AsLine();

            return baseLine.IsParallelTo(line, angleToleranceInRad);
        }
 
    }
}