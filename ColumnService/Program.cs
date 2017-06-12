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
            double _radius = 21;
            double _clear = 3.625;
            double _axial = 500*0;
            double _moment = 900;

            CircularColumn column = new CircularColumn(_radius);

            double[,] _reinforcing = new double[22,2];
            _reinforcing[0, 0] = 0;
            _reinforcing[0, 1] = _radius - _clear;
            column.Reinforcing.Add(new Reinforcing(1.56, _reinforcing[0, 0], _reinforcing[0, 1], 60));

            for (int i =1; i<22; i++)
            {
                _reinforcing[i, 0] = _reinforcing[0, 0] * Math.Cos(2 * Math.PI / 22 * i) - _reinforcing[0, 1] * Math.Sin(2 * Math.PI / 22 * i);
                _reinforcing[i, 1] = _reinforcing[0, 0] * Math.Sin(2 * Math.PI / 22 * i) + _reinforcing[0, 1] * Math.Cos(2 * Math.PI / 22 * i);
                column.Reinforcing.Add(new Reinforcing(1.56,_reinforcing[i, 0],_reinforcing[i,1],60));
               
            }

            Console.WriteLine(column.Service_NA(8));
            Console.WriteLine(column.CrackedMomentOfInertia(8));
            Console.WriteLine(column.ServiceConcreteStress(_axial,_moment,8));
            Console.WriteLine(column.ServiceConcreteForce(_axial, _moment, 8));
            Console.WriteLine(column.ServiceConcreteCG(_axial, _moment, 8));
            Console.WriteLine(column.ReinforcingForce);
            Console.WriteLine(Math.Round(column.ServiceConcreteMoment(_axial, _moment, 8),2));
            Console.ReadLine();

        }
    }
}
