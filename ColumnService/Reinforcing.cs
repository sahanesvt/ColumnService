using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ColumnService
{
    public class Reinforcing
    {
        private double _area = 0;
        private double _yield = 0;
        private double _x_coord = 0;
        private double _y_coord = 0;
        private double _ultimate_stress;
        private double _service_stress;
        private bool _service = true;
        public double Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }
        public double Yield
        {
            get
            {
                return _yield;
            }
            set
            {
                _yield = value;
            }
        }
        public double X_Coordinate
        {
            get
            {
                return _x_coord;
            }
            set
            {
                _x_coord = value;
            }
        }
        public double Y_Coordinate
        {
            get
            {
                return _y_coord;
            }
            set
            {
                _y_coord = value;
            }
        }
        //public double Force { get; set; }


        private void _setValues(double area, double x_coord, double y_coord, double yield)
        {
            Area = area;
            Yield = yield;
            X_Coordinate = x_coord;
            Y_Coordinate = y_coord;
        }

        private void _setValues()
        {
            _setValues(_area, _x_coord, _y_coord, _yield);
        }


        public Reinforcing()
        {
            _setValues();
        }

        public Reinforcing(double area, double x_coord, double y_coord, double yield)
        {
            _setValues(area, x_coord, y_coord, yield);
        }

        /*public Reinforcing(double area, double strength, double distToSlab, Slab slab, bool distToTopOfSlab)
        {
            Area = area;
            Strength = strength;
            DistToTopOfSlab = distToTopOfSlab;
            if (distToTopOfSlab)
            {
                Location = slab.TopLocation - distToSlab;
            }
            else
            {
                Location = slab.BotLocation + distToSlab;
            }
        }
        public double Force()
        {
            return Area * Strength;
        }
        public void Add(double area, double distToSlab, Slab slab, bool distToTopOfSlab)
        {
            if (distToTopOfSlab)
            {
                Location = (Area * Location + area * (slab.TopLocation - distToSlab)) / (Area + area);
            }
            else
            {
                Location = (Area * Location + area * (slab.BotLocation + distToSlab)) / (Area + area);
            }
            Area += area;
        }*/
    }
}
