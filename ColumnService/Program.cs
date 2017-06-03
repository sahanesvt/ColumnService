using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnService
{
    class Program
    {
        static void Main(string[] args)
        {
            CircularColumn column = new CircularColumn(36);

            double[,] _reinforcing = new double[22,2];
            _reinforcing[0, 0] = 0;
            _reinforcing[0, 1] = 32.375;

            for(int i =1; i<22; i++)
            {
                _reinforcing[i, 0] = _reinforcing[0, 0] * Math.Cos(2 * Math.PI / 22 * i) - _reinforcing[0, 1] * Math.Sin(2 * Math.PI / 22 * i);
                _reinforcing[i, 1] = _reinforcing[0, 0] * Math.Sin(2 * Math.PI / 22 * i) + _reinforcing[0, 1] * Math.Cos(2 * Math.PI / 22 * i);
                column.Reinforcing.Add(new Reinforcing(1.56,_reinforcing[i, 0],_reinforcing[i,1],60));
               
            }

            Console.WriteLine(column.Service_NA(8));
            Console.WriteLine(column.CrackedMomentOfInertia(8));
            Console.WriteLine(column.ServiceConcreteStress(900,8));
            Console.ReadLine();

        }
    }
}
