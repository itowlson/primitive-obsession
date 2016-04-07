using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    public static class Checkers
    {
        public static void NothingCanPossiblyGoWrong()
        {
            // Which is it?
            DisplayTimeSeries1("Spline reticulation counts 2007-15", "STORED_TS_SPLINE_COUNTS");
            DisplayTimeSeries1("STORED_TS_SPLINE_COUNTS", "Spline reticulation counts 2007-15");

            // I sure hope this works
            SetDecelerationPoint1(distanceFromMars: 844903017);
        }

        public static void DisplayTimeSeries1(string timeSeriesId, string caption)
        {
            Console.WriteLine($"{caption}");
        }

        public static void DisplayTimeSeries2(TimeSeriesId timeSeriesId, string caption)
        {
            Console.WriteLine($"{caption}");
        }

        public static void SetDecelerationPoint1(double distanceFromMars)
        {
        }

        public static void SetDecelerationPoint2(Distance distanceFromMars)
        {
        }
    }
}
