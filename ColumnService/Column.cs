using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnService
{
    public abstract class Column
    {
        public double ReinforcingArea;
        public double ReinforcingFirstMoment;
        public double ReinforcingCG;
        public double ReinforcingInertia;
        public virtual double GrossArea{get; protected set;}
        public List<Reinforcing> Reinforcing = new List<Reinforcing>();
        


        protected void calcReinf()
        {
            double _area = 0;
            double _NA = 0;
            double _first_moment = 0;
            double _inertia = 0;
            foreach(Reinforcing reinforcing in Reinforcing)
            {
                _area += reinforcing.Area;
                _first_moment += reinforcing.Area * reinforcing.Y_Coordinate;
            }
            _NA = _first_moment / _area;
            foreach(Reinforcing reinforcing in Reinforcing)
            {
                _inertia += reinforcing.Area * Math.Pow(_NA - reinforcing.Y_Coordinate, 2);
            }
            ReinforcingArea = _area;
            ReinforcingFirstMoment = _first_moment;
            ReinforcingCG = _NA;
            ReinforcingInertia = _inertia;
        }
    }

}
