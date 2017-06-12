using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnService
{
    internal class CircularSegment
    {
        private double _radius = 0;
        private double _area = 0;
        private double _CG = 0;
        private double _theta = 0;
        private double _cracked_I = 0;
        private double _compressionDepth = 0;

        private void _calc_theta(double radius, double compressionDepth)
        {
            _theta = 2 * Math.Acos((radius - compressionDepth) / radius);
        }
        private void _calc_theta(double compressionDepth)
        {
            _calc_theta(_radius, compressionDepth);
        }

        private void _calc_area(double radius, double compressionDepth)
        {
            _calc_theta(radius, compressionDepth);
            _area = 0.5 * Math.Pow(radius, 2) * (_theta - Math.Sin(_theta));
        }
        private void _calc_area(double compressionDepth)
        {
            _calc_area(_radius, compressionDepth);
        }

        private void _calc_CG(double radius, double compressionDepth)
        {
            _calc_theta(radius, compressionDepth);
            _CG = 4 * radius * Math.Pow(Math.Sin(0.5 * _theta), 3) / (3 * (_theta - Math.Sin(_theta)));
        }
        private void _calc_CG(double compressionDepth)
        {
            _calc_CG(_radius, compressionDepth);
        }

        private void _calc_cracked_I(double radius, double compressionDepth)
        {
            _calc_theta(radius, compressionDepth);
            _cracked_I = Math.Pow(radius, 4) / 8 * (_theta - Math.Sin(_theta) + 2 * Math.Sin(_theta) * Math.Pow(Math.Sin(_theta / 2), 2)) - _area * Math.Pow(_CG, 2);
        }
        private void _calc_cracked_I(double compressionDepth)
        {
            _calc_cracked_I(_radius, compressionDepth);
        }


        public double Area = 0;
        public double CG = 0;
        public double Cracked_I = 0;
        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                _calc_area(_compressionDepth);
                _calc_CG(_compressionDepth);
                _calc_cracked_I(_compressionDepth);

                Area = _area;
                CG = _CG;
                Cracked_I = _cracked_I;
            }
        }
        public double CompressionDepth
        {
            get
            {
                return _compressionDepth;
            }
            set
            {
                _compressionDepth = value;
                _calc_area(_compressionDepth);
                _calc_CG(_compressionDepth);
                _calc_cracked_I(_compressionDepth);

                Area = _area;
                CG = _CG;
                Cracked_I = _cracked_I;
            }
        }

        public CircularSegment(double radius, double compressionDepth)
        {
            _compressionDepth = compressionDepth;
            _radius = radius;
            _calc_area(_compressionDepth);
            _calc_CG(_compressionDepth);
            _calc_cracked_I(_compressionDepth);

            Area = _area;
            CG = _CG;
            Cracked_I = _cracked_I;
        }
        public CircularSegment()
        {
            _calc_area(_compressionDepth);
            _calc_CG(_compressionDepth);
            _calc_cracked_I(_compressionDepth);

            Area = _area;
            CG = _CG;
            Cracked_I = _cracked_I;
        }

        public double Force(double stress)
        {
            return _area * stress;
        }

    }
}
