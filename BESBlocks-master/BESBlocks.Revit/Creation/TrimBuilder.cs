using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common;
using BESBlocks.Revit.Common.Extensions;

namespace BESBlocks.Revit.Creation
{
    public class TrimBuilder
    {
        private readonly Document _doc;

        public TrimBuilder(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        }
        
        public void Create(IEnumerable<MEPCurvePair> pairs)
        {
            foreach (MEPCurvePair pair in pairs)
            {
                Create(pair);
            }
        }

        public void Create(MEPCurvePair pair)
        {
            if (pair.IsParallel)
                throw new OperationCanceledException("Elements is parallel.");

            if (pair.Offset > MEPCurveSorter.OFFSET_TOLERANCE)
                throw new OperationCanceledException("The offset of elements needs to be zero.");

            pair.FirstNearest.Origin = pair.FirstIntersectionPoint;
            pair.SecondNearest.Origin = pair.SecondIntersectionPoint;

            FamilyInstance elbow = _doc.Create.NewElbowFitting(pair.FirstNearest, pair.SecondNearest);

            pair.First.TransferTextParametersTo(elbow);
            pair.First.TransferWorksetToElement(elbow);
        }
    }
}