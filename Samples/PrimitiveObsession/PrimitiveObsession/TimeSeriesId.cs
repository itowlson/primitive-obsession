using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    [DebuggerDisplay("TimeSeriesId {_impl}")]
    public struct TimeSeriesId : IEquatable<TimeSeriesId>
    {
        private readonly string _impl;

        public TimeSeriesId(string impl)
            : this()
        {
            if (impl == null)
            {
                throw new ArgumentNullException(nameof(impl));
            }

            _impl = impl;
        }

        public bool Equals(TimeSeriesId other)
        {
            return _impl == other._impl;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimeSeriesId)
            {
                return Equals((TimeSeriesId)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _impl == null ? 0 : _impl.GetHashCode();
        }

        public override string ToString()
        {
            return _impl;
        }

        public static bool operator ==(TimeSeriesId first, TimeSeriesId second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(TimeSeriesId first, TimeSeriesId second)
        {
            return !(first == second);
        }
    }
}
