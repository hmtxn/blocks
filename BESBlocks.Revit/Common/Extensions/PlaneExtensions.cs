using System;
using Autodesk.Revit.DB;
using BESBlocks.Revit.Common.Enums;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class PlaneExtensions
    {
        public static XYZ GetXYZ(this Plane plane, UV uv)
        {
            return plane.Origin + uv.U * plane.XVec + uv.V * plane.YVec;
            ;
        }

        public static PlaneIntersectionResult Intersect(this Plane plane, Line line, double tolerance,
            out XYZ intersectionPoint, out double parameter)
        {
            //compute the dot prodcut and signed distance 
            double denominator = line.Direction.DotProduct(plane.Normal);
            double numerator = (plane.Origin - line.GetEndPoint(0)).DotProduct(plane.Normal);
            //check if the dot product is almost zero 

            if (Math.Abs(denominator) < tolerance)
            {
                // line is parallel to plane (could be inside or outside the plane)
                if (Math.Abs(numerator) < tolerance)
                {
                    //line is inside the plane
                    intersectionPoint = null;
                    parameter = double.NaN;

                    return PlaneIntersectionResult.Subset;
                }
                else
                {
                    // line is outside the plane                    
                    intersectionPoint = null;
                    parameter = double.NaN;

                    return PlaneIntersectionResult.Disjoint;
                }
            }
            else
            {
                // line is intersecting wih plane
                // compute the line paramemer 
                parameter = numerator / denominator;
                intersectionPoint = line.GetEndPoint(0) + parameter * line.Direction;

                return PlaneIntersectionResult.Intersecting;
            }
        }
    }
}