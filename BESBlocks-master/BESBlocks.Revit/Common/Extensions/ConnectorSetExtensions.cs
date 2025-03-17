using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class ConnectorSetExtensions
    {
        public static List<Connector> ToList(this ConnectorSet connectors)
        {
            List<Connector> list = new List<Connector>(connectors.Size);

            foreach (Connector con in connectors)
                list.Add(con);

            return list;
        }

        public static ConnectorSet GetSideConnectors(this ConnectorSet connectors)
        {
            ConnectorSet result = new ConnectorSet();

            foreach (Connector con in connectors)
            {
                if (con.ConnectorType != ConnectorType.End)
                    continue;

                if (Math.Round(con.CoordinateSystem.BasisZ.Z, 4) == 0.0)
                    result.Insert(con);
            }

            return result;
        }

        public static ConnectorSet GetWithoutRefs(this ConnectorSet connectors)
        {
            ConnectorSet result = new ConnectorSet();

            foreach (Connector con in connectors)
            {
                if (con.ConnectorType != ConnectorType.End)
                    continue;

                if (con.AllRefs.Size == 0)
                    result.Insert(con);
            }

            return result;
        }

        public static ConnectorSet GetEqualsToDirection(this ConnectorSet connectors, XYZ direction)
        {
            ConnectorSet result = new ConnectorSet();

            foreach (Connector con in connectors)
            {
                if (con.ConnectorType != ConnectorType.End)
                    continue;

                if (con.CoordinateSystem.BasisZ.IsEqual(direction, 4))
                    result.Insert(con);
            }

            return result;
        }

        public static Connector NearestTo(this ConnectorSet connectors, XYZ point)
        {
            Connector targetConnector = null;
            double targetDistance = double.MaxValue;

            foreach (Connector connector in connectors)
            {
                double distance = point.DistanceTo(connector.Origin);
                if (targetDistance > distance)
                {
                    targetConnector = connector;
                    targetDistance = distance;
                }
            }

            return targetConnector;
        }

        public static Connector NearestTo(this ConnectorSet firstConnectors, ConnectorSet secondConnectors)
        {
            Connector targetConnector = null;
            double targetDistance = double.MaxValue;

            foreach (Connector firstCon in firstConnectors)
            {
                foreach (Connector secondCon in secondConnectors)
                {
                    double distance = firstCon.Origin.DistanceTo(secondCon.Origin);

                    if (targetDistance > distance)
                    {
                        targetConnector = firstCon;
                        targetDistance = distance;
                    }
                }
            }

            return targetConnector;
        }

        public static Connector DistantTo(this ConnectorSet connectors, XYZ point)
        {
            Connector targetConnector = null;
            double targetDistance = double.MinValue;

            foreach (Connector connector in connectors)
            {
                double distance = point.DistanceTo(connector.Origin);
                if (targetDistance < distance)
                {
                    targetConnector = connector;
                    targetDistance = distance;
                }
            }

            return targetConnector;
        }

        public static Connector DistantTo(this ConnectorSet firstConnectors, ConnectorSet secondConnectors)
        {
            Connector targetConnector = null;
            double targetDistance = double.MinValue;

            foreach (Connector firstCon in firstConnectors)
            {
                foreach (Connector secondCon in secondConnectors)
                {
                    double distance = firstCon.Origin.DistanceTo(secondCon.Origin);
                    if (targetDistance < distance)
                    {
                        targetConnector = firstCon;
                        targetDistance = distance;
                    }
                }
            }

            return targetConnector;
        }
    }
}