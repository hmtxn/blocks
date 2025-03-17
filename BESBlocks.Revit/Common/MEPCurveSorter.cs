using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common.Enums;
using BESBlocks.Revit.Common.Extensions;

namespace BESBlocks.Revit.Common
{
    public class MEPCurveSorter
    {
        public const double ANGLE_TOLERANCE = 1.0;
        public const double OFFSET_TOLERANCE = 0.001;

        public List<List<MEPCurve>> FindParallelGroups(List<MEPCurve> mepCurves)
        {
            List<ElementId> processedIds = new List<ElementId>();

            List<List<MEPCurve>> groups = new List<List<MEPCurve>>();

            foreach (MEPCurve first in mepCurves)
            {
                if (processedIds.Contains(first.Id))
                    continue;

                List<MEPCurve> parallelGroup = new List<MEPCurve>() { first };

                processedIds.Add(first.Id);

                foreach (MEPCurve second in mepCurves)
                {
                    if (processedIds.Contains(second.Id))
                        continue;

                    Connector firstCon =
                        first.ConnectorManager.Connectors.NearestTo(second.ConnectorManager.Connectors);
                    Connector secondCon = second.ConnectorManager.Connectors.NearestTo(firstCon.Origin);

                    double angleInRad = firstCon.CoordinateSystem.BasisZ.AngleTo(secondCon.CoordinateSystem.BasisZ);
                    double angle = angleInRad.RadianToDegree();
#if DEBUG
                    Debug.WriteLine(
                        $"[FindParallelGroups] Angle: {angle}; Pair: {firstCon.Owner.Id},{secondCon.Owner.Id}");
#endif
                    if (angle < ANGLE_TOLERANCE)
                    {
                        parallelGroup.Add(second);
                        processedIds.Add(second.Id);
                    }
                }

                groups.Add(parallelGroup);
            }

            return groups;
        }

        public List<MEPCurvePair> FindPairsUsingDistanceAlgorithm(List<MEPCurve> firstGroup, List<MEPCurve> secondGroup)
        {
            List<MEPCurvePair> pairs = new List<MEPCurvePair>();

            List<ElementId> processedIds = new List<ElementId>();

            List<MEPCurve> firstTempGroup = new List<MEPCurve>(firstGroup);
            List<MEPCurve> secondTempGroup = new List<MEPCurve>(secondGroup);

            while (firstTempGroup.Count > 0 && secondTempGroup.Count > 0)
            {
                List<MEPCurvePair> firstPairs = firstTempGroup.Where(i => !processedIds.Contains(i.Id))
                    .Select(i => new MEPCurvePair(secondTempGroup[0], i))
                    .OrderBy(i => Math.Round(i.FirstDistance, 2))
                    .ThenBy(i => Math.Round(i.Offset, 2))
                    .ToList();

                List<MEPCurvePair> secondPairs = secondTempGroup.Where(i => !processedIds.Contains(i.Id))
                    .Select(i => new MEPCurvePair(firstPairs[0].Second, i))
                    .OrderBy(i => Math.Round(i.FirstDistance, 2))
                    .ThenBy(i => Math.Round(i.Offset, 2))
                    .ToList();

                MEPCurvePair targetPair = null;

                foreach (MEPCurvePair secondPair in secondPairs)
                {
                    foreach (MEPCurvePair firstPair in firstPairs)
                    {
                        if (secondPair.SecondNearest.Radius == firstPair.SecondNearest.Radius)
                        {
                            MEPCurvePair pair = new MEPCurvePair(firstPair.Second, secondPair.Second);
#if DEBUG
                            Debug.WriteLine(
                                $"[FindMEPCurvePairs] Offset: {pair.Offset}; Pair: {pair.First.Id},{pair.Second.Id}");
#endif
                            XYZ firstDirection = pair.FirstDistant.Origin.DirectionTo(pair.FirstIntersectionPoint);
                            XYZ secondDirection = pair.SecondDistant.Origin.DirectionTo(pair.SecondIntersectionPoint);

                            bool isEquealFirstDirection =
                                pair.FirstNearest.CoordinateSystem.BasisZ.IsEqual(firstDirection, 4);
                            bool isEquealSecondDirection =
                                pair.SecondNearest.CoordinateSystem.BasisZ.IsEqual(secondDirection, 4);

                            if (isEquealFirstDirection && isEquealSecondDirection)
                            {
                                targetPair = pair;
                                break;
                            }
                        }
                    }

                    if (targetPair != null)
                        break;
                }

                if (targetPair == null)
                    break;

                pairs.Add(targetPair);

                processedIds.Add(targetPair.First.Id);
                processedIds.Add(targetPair.Second.Id);

                firstTempGroup.Remove(targetPair.First);
                secondTempGroup.Remove(targetPair.Second);
            }

            return pairs;
        }

        public List<MEPCurvePair> FindPairsUsingDirectionAlgorithm(List<MEPCurve> firstGroup,
            List<MEPCurve> secondGroup)
        {
            List<MEPCurvePair> pairs = new List<MEPCurvePair>();

            foreach (MEPCurve first in firstGroup)
            {
                foreach (MEPCurve second in secondGroup)
                {
                    MEPCurvePair pair = new MEPCurvePair(first, second);

                    if (pair.FirstNearest.Radius == pair.SecondNearest.Radius)
                        pairs.Add(pair);
                }
            }

            MEPCurvePair maxPair =
                pairs.OrderByDescending(i => i.FirstNearest.Origin.DistanceTo(i.SecondNearest.Origin)).First();

            Plane firstPlane = Plane.CreateByOriginAndBasis(maxPair.FirstNearest.Origin,
                maxPair.FirstNearest.CoordinateSystem.BasisX, maxPair.FirstNearest.CoordinateSystem.BasisY);
            Plane secondPlane = Plane.CreateByOriginAndBasis(maxPair.SecondNearest.Origin,
                maxPair.SecondNearest.CoordinateSystem.BasisX, maxPair.SecondNearest.CoordinateSystem.BasisY);

            XYZEqualityComparer comparer = new XYZEqualityComparer(4);

            IEnumerable<IGrouping<XYZ, MEPCurvePair>> groups = pairs
                .GroupBy(i => AlignByPlanesAndGetNearestDirection(i, firstPlane, secondPlane), comparer)
                .OrderByDescending(i => i.Count());

            pairs = groups.First().ToList();

            return pairs;
        }

        public bool CheckNearestDirectionsIsEqual(List<MEPCurvePair> pairs)
        {
            XYZEqualityComparer comparer = new XYZEqualityComparer(4);

            IEnumerable<IGrouping<XYZ, MEPCurvePair>> groups = pairs.GroupBy(i => i.NearestDirection, comparer);

            return groups.Count() == 1;
        }

        private XYZ AlignByPlanesAndGetNearestDirection(MEPCurvePair pair, Plane firstPlane, Plane secondPlane)
        {
            double maxDistance = pair.FirstNearest.Origin.DistanceTo(pair.SecondNearest.Origin);

            Line firstLine = Line.CreateBound(pair.FirstDistant.Origin,
                pair.FirstNearest.Origin.MoveFrom(pair.FirstDistant.Origin, maxDistance));
            Line secondLine = Line.CreateBound(pair.SecondDistant.Origin,
                pair.SecondNearest.Origin.MoveFrom(pair.SecondDistant.Origin, maxDistance));

            PlaneIntersectionResult firstIntersectionResult =
                firstPlane.Intersect(firstLine, 0.0, out XYZ firstIntersect, out double firstParameter);
            PlaneIntersectionResult secndIntersectionResult =
                secondPlane.Intersect(secondLine, 0.0, out XYZ secondIntersect, out double secndParameter);

            return firstIntersect.DirectionTo(secondIntersect);
        }
    }
}