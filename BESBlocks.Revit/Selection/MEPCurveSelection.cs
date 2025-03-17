using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace BESBlocks.Revit.Selection
{
    public class MEPCurveSelection
    {
        private readonly UIDocument _uiDoc;

        public MEPCurveSelection(UIDocument uiDoc)
        {
            _uiDoc = uiDoc ?? throw new ArgumentNullException(nameof(uiDoc));
        }

        public List<MEPCurve> GetSelected()
        {
            return _uiDoc.Selection.GetElementIds()
                .Select(i => _uiDoc.Document.GetElement(i))
                .Where(i => i is MEPCurve)
                .Cast<MEPCurve>()
                .ToList();
        }

        public List<MEPCurve> SelectElements(string prompt)
        {
            return _uiDoc.Selection.PickObjects(ObjectType.Element, prompt)
                .Select(i => _uiDoc.Document.GetElement(i) as MEPCurve)
                .ToList();
        }

        public MEPCurve SelectElement(string prompt)
        {
            return _uiDoc.Document.GetElement(_uiDoc.Selection.PickObject(ObjectType.Element, prompt)) as MEPCurve;
        }
    }
}