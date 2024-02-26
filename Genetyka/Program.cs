using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytm
{
    class Program
    {
        static void Main(string[] args)
        {
            AlgorytmGenetyczny algGen = new AlgorytmGenetyczny();
            algGen.PrzeprowadzObliczenia(6, 25, 0.1, 0.7, -1, 50, 20, 40);
        }
    }
}
