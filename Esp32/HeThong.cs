using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esp32
{
    public class HeThong
    {
        private const int LoadCellsCount = 6;   // số cell

        double[] values = new double[LoadCellsCount];

        public double[] ReadData(double x)
        {
            double[] values =
            {
                Math.Sin(x),
                Math.Cos(x),
                Math.Sin(x * 0.5),
                Math.Cos(x * 0.5),
                Math.Sin(x * 0.2),
                Math.Cos(x * 0.2)
            };

            return values;
        }
    }
}
