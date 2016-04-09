using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    public static class Checkers
    {
        public static void MakeRequest()
        {
            var wr = System.Net.WebRequest.CreateHttp("http://example.com");
            wr.Timeout = 10 * 1000;  // 10 seconds (10000 ms)?  10000 seconds?  1/10000 second (10000 ticks)?
        }

        public static Grid CreateGrid()
        {
            return GridProvider.Create(
                true,
                false,
                false,
                true,
                true,
                20,
                10,
                70,
                50,
                false,
                false);
        }

        public static void EnterMarsOrbit()
        {
            decimal distanceFromMars = Astrophysics.GetDistanceFromMars();
            decimal distanceRequiredToDecelerate = Engine.GetDecelerationDistance();
            decimal orbitalHeight = FlightPlanning.GetOrbitalHeight();

            if (distanceRequiredToDecelerate < distanceFromMars - orbitalHeight)
            {
                Engine.FireTheRetroThrustersCaptain();
            }
        }

        public static void StopIt_StopItNow()
        {
            string timeSeriesId = "STORED_TS_SPLINE_COUNTS";
            string whatDoesThisEvenMean = timeSeriesId + "_CUMULATIVE";
        }

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
            Console.WriteLine($"Distance in feet is {distanceFromMars:ft}");
        }

        public class Grid { }

        private static class GridProvider
        {
            public static Grid Create(
                bool showVerticalGridlines,
                bool showHorizontalGridlines,
                bool invertColours,
                bool showHeaderRow,
                bool showAlternateRowColours,
                int cellMarginX,
                int cellMarginY,
                int minCellWidth,
                int minCellHeight,
                bool autoStretch,
                bool autoWrap
                )
            {
                throw new NotImplementedException();
            }
        }

        private static class Astrophysics
        {
            public static decimal GetDistanceFromMars() { return 0; }
        }

        private static class Engine
        {
            public static decimal GetDecelerationDistance() { return 0; }
            public static decimal FireTheRetroThrustersCaptain() { return 0; }
        }

        private static class FlightPlanning
        {
            public static decimal GetOrbitalHeight() { return 0; }
        }
    }
}
