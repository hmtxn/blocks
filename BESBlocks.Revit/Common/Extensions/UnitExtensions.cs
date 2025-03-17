using System;

namespace BESBlocks.Revit.Common.Extensions
{
    public static class UnitExtensions
    {
        public static double DegreeToRadian(this double value)
        {
            return value * (Math.PI / 180.0);
        }

        public static double RadianToDegree(this double value)
        {
            return value * (180.0 / Math.PI);
        }

        public static double FootToInch(this double value)
        {
            return value * 12.0;
        }

        public static double InchToFoot(this double value)
        {
            return value / 12.0;
        }

        public static double MetreToFoot(this double value)
        {
            return value * 3.281;
        }

        public static double CentimetreToFoot(this double value)
        {
            return value / 30.48;
        }

        public static double MillimetreToFoot(this double value)
        {
            return value / 304.8;
        }
    }
}