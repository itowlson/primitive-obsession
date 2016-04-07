using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PrimitiveObsession.Checkers;

namespace PrimitiveObsession
{
    static class Program
    {
        static void Main(string[] args)
        {
            var toBackOfRoom = Distance.FromMetres(10);
            Console.WriteLine($"It is {toBackOfRoom:ft} feet to the back");

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
