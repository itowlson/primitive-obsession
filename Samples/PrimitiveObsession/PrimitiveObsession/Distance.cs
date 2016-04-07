using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    [DebuggerDisplay("Distance {_metres}m")]
    public struct Distance : IEquatable<Distance>, IComparable<Distance>, IFormattable
    {
        private readonly double _metres;

        private Distance(double metres)
        {
            if (metres < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(metres), "distance must be non-negative");  // I hope this is right
            }
            if (Double.IsInfinity(metres) || Double.IsNaN(metres))
            {
                throw new ArgumentOutOfRangeException(nameof(metres), "come on chaps, be reasonable");
            }

            _metres = metres;
        }

        public static Distance FromMetres(double metres) => new Distance(metres);
        public static Distance FromKilometres(double km) => new Distance(km * 1000);
        public static Distance FromFeet(double feet) => new Distance(feet / FeetPerMetre);
        public static Distance FromMiles(double miles) => new Distance(miles / MilesPerMetre);

        public bool Equals(Distance other)
        {
            return _metres == other._metres;
        }

        public override bool Equals(object obj)
        {
            if (obj is Distance)
            {
                return Equals((Distance)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _metres.GetHashCode();
        }

        public override string ToString()
        {
            return _metres.ToString() + "m";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case "m": return Metres.ToString(formatProvider);
                case "km": return Kilometres.ToString(formatProvider);
                case "ft": return Feet.ToString(formatProvider);
                case "mi": return Miles.ToString(formatProvider);
                default: throw new ArgumentException(nameof(format));
            }
        }

        public int CompareTo(Distance other)
        {
            return _metres.CompareTo(other._metres);
        }

        // "Forgetters" - important for interop with other components, but very dangerous!
        public double Metres => _metres;
        public double Kilometres => _metres / 1000;
        public double Feet => _metres * FeetPerMetre;
        public double Miles => _metres * MilesPerMetre;

        private const double FeetPerMetre = 3.2808399;  // close enough for a Mars landing?  Eh, we'll find out
        private const double MilesPerMetre = 0.00062137;

        public static bool operator ==(Distance first, Distance second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Distance first, Distance second)
        {
            return !(first == second);
        }

        public static bool operator <(Distance first, Distance second)
        {
            return first.CompareTo(second) < 0;
        }

        public static bool operator <=(Distance first, Distance second)
        {
            return first.CompareTo(second) <= 0;
        }

        public static bool operator >(Distance first, Distance second)
        {
            return first.CompareTo(second) > 0;
        }

        public static bool operator >=(Distance first, Distance second)
        {
            return first.CompareTo(second) >= 0;
        }

        public static Distance operator +(Distance first, Distance second)
        {
            return new Distance(first._metres + second._metres);
        }

        public static Distance operator -(Distance first, Distance second)
        {
            return new Distance(first._metres - second._metres);
        }

        public static Distance operator *(Distance distance, double factor)
        {
            return new Distance(distance._metres * factor);
        }

        public static Distance operator /(Distance distance, double factor)
        {
            return new Distance(distance._metres / factor);
        }

        public static Area operator *(Distance first, Distance second)
        {
            return Area.FromSquareMetres(first._metres * second._metres);
        }

        public static Speed operator /(Distance distance, TimeSpan duration)
        {
            return Speed.FromMetresPerSecond(distance._metres / duration.TotalSeconds);
        }
    }
}
