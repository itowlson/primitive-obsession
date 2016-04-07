using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitiveObsession
{
    public struct Area
    {
        private double _msq;

        private Area(double msq)
        {
            // error checking elided
            _msq = msq;
        }

        public static Area FromSquareMetres(double msq) => new Area(msq);
    }
}
