using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    public struct Speed
    {
        private readonly double _mps;

        private Speed(double mps)
        {
            // error checking elided
            _mps = mps;
        }

        public static Speed FromMetresPerSecond(double mps) => new Speed(mps);
    }
}
