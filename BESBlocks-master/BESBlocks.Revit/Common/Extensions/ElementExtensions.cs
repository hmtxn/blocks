using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class ElementExtensions
    {
        public static ConnectorSet GetConnectorSet(this Element element)
        {
            ConnectorSet connectorSet = null;

            if (element is MEPCurve mepCurve)
            {
                connectorSet = mepCurve.ConnectorManager.Connectors;
            }
            else if (element is FamilyInstance familyInstance)
            {
                MEPModel mepModel = familyInstance.MEPModel;

                if (mepModel != null)
                    connectorSet = mepModel.ConnectorManager.Connectors;
            }

            return connectorSet ?? throw new InvalidOperationException();
        }

        public static bool HasConnectorSet(this Element element)
        {
            bool result = false;

            if (element is MEPCurve mepCurve)
            {
                result = mepCurve.ConnectorManager != null && mepCurve.ConnectorManager.Connectors != null;
            }
            else if (element is FamilyInstance familyInstance)
            {
                result = familyInstance.MEPModel != null && familyInstance.MEPModel.ConnectorManager != null &&
                         familyInstance.MEPModel.ConnectorManager.Connectors != null;
            }

            return result;
        }

        public static bool TryGetConnectorSet(this Element element, out ConnectorSet connectorSet)
        {
            bool result = false;
            connectorSet = null;

            if (element is MEPCurve mepCurve)
            {
                if (mepCurve.ConnectorManager != null && mepCurve.ConnectorManager.Connectors != null)
                {
                    connectorSet = mepCurve.ConnectorManager.Connectors;
                    result = true;
                }
            }
            else if (element is FamilyInstance familyInstance)
            {
                if (familyInstance.MEPModel != null && familyInstance.MEPModel.ConnectorManager != null &&
                    familyInstance.MEPModel.ConnectorManager.Connectors != null)
                {
                    connectorSet = familyInstance.MEPModel.ConnectorManager.Connectors;
                    result = true;
                }
            }

            return result;
        }

        public static BoundingBoxXYZ GetBoundingBoxXYZ(this Element element, ViewDetailLevel detailLevel,
            Transform transform)
        {
            Options options = new Options()
            {
                DetailLevel = detailLevel,
            };
            GeometryElement gElement = element.get_Geometry(options);
            gElement = gElement.GetTransformed(transform);

            BoundingBoxXYZ box = gElement.GetBoundingBox();

            return box;
        }

        public static BoundingBoxXYZ GetBoundingBoxXYZ(this IEnumerable<Element> elements, ViewDetailLevel detailLevel,
            Transform transform)
        {
            BoundingBoxXYZ mainBox = null;

            foreach (Element element in elements)
            {
                BoundingBoxXYZ box = element.GetBoundingBoxXYZ(detailLevel, Transform.Identity);

                if (mainBox == null)
                {
                    mainBox = box;
                }
                else
                {
                    mainBox.ExpandToContain(box.Min);
                    mainBox.ExpandToContain(box.Max);
                }
            }

            return mainBox;
        }

        public static List<Solid> GetSolids(this Element baseElement, ViewDetailLevel viewDetailLevel,
            Transform transform)
        {
            Options options = new Options()
            {
                DetailLevel = viewDetailLevel,
            };
            GeometryElement gElement = baseElement.get_Geometry(options);
            gElement = gElement.GetTransformed(transform);

            List<Solid> solids = new List<Solid>();

            foreach (GeometryObject gObj in gElement)
            {
                if (gObj is Solid solid)
                    solids.Add(solid);
            }

            return solids;
        }

        public static void TransferTextParametersTo(this Element baseElement, Element element)
        {
            foreach (Parameter baseParameter in baseElement.Parameters)
            {
                if (baseParameter.StorageType != StorageType.String || baseParameter.IsReadOnly)
                    continue;

                string baseValue = baseParameter.AsString();

                if (baseValue == null || baseValue == string.Empty)
                    continue;

                foreach (Parameter parameter in element.Parameters)
                {
                    if (parameter.StorageType != StorageType.String || parameter.IsReadOnly)
                        continue;

                    if (baseParameter.Id == parameter.Id)
                    {
                        parameter.Set(baseValue);
                        break;
                    }
                }
            }
        }

        public static void TransferWorksetToElement(this Element baseElement, Element element)
        {
            Parameter baseWorksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

            if (baseWorksetParam != null && !baseWorksetParam.IsReadOnly)
            {
                int baseWorkset = baseWorksetParam.AsInteger();

                Parameter worksetParam = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                if (worksetParam != null && !worksetParam.IsReadOnly)
                    worksetParam.Set(baseWorkset);
            }
        }
    }
}