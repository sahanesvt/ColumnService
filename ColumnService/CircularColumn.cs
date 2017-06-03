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

        private void _calc_area(double radius,double compressionDepth)
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
            _calc_CG(_radius,compressionDepth);
        }

        private void _calc_cracked_I(double radius,double compressionDepth)
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

        public double Force(double stress)
        {
            return _area * stress;
        }

    }

    public class CircularColumn: Column
    {
        private double _radius = 0;
        private double _gross_area = 0;
        private double _cracked_NA = 0;
        private double _cracked_moment_of_inertia = 0;
        private double _service_concrete_stress = 0;

        private void _calc_properties(double radius)
        {
            _gross_area = Math.Pow(radius, 2) * Math.PI;
            GrossArea = _gross_area;
        }
        private void _calc_properties()
        {
            _calc_properties(_radius);
        }

        private void _calc_service_NA(double modRatio)
        {
            this.calcReinf();
            double crackedNA = 0;
            double compressionDepth = 1;//_radius/2*.8999633;
            double targetNA = _radius - compressionDepth;
            CircularSegment compressionBlock = new CircularSegment(_radius, compressionDepth);
            double increment = 12.321;

            while(targetNA != crackedNA)
            {
                crackedNA = (ReinforcingFirstMoment + (compressionBlock.Area * compressionBlock.CG) / modRatio) / (ReinforcingArea + compressionBlock.Area / modRatio);
                if (Math.Round(crackedNA, 6) == Math.Round(targetNA, 6))
                {
                    break;
                }
                else if (targetNA > crackedNA)
                {
                    compressionDepth +=  increment;
                }
                else
                {
                    increment *= 0.5;
                    compressionDepth -=  increment;
                }
                targetNA = _radius - compressionDepth;
                compressionBlock.CompressionDepth = compressionDepth;
            }
            _cracked_NA = crackedNA;
                      

        }

        private void _calc_service_concrete_stress(double moment,double modRatio)
        {
            _calc_service_NA(modRatio);
            _calc_cracked_moment_of_inertia(moment);
            _service_concrete_stress = moment * 12 * (_radius-_cracked_NA) / _cracked_moment_of_inertia / modRatio;
        }

        private void _calc_cracked_moment_of_inertia(double modRatio)
        {
            _calc_service_NA(modRatio);
            CircularSegment compressionBlock = new CircularSegment(_radius, _radius -_cracked_NA);
            this.calcReinf();
            _cracked_moment_of_inertia = ReinforcingInertia + ReinforcingArea * Math.Pow((_cracked_NA - ReinforcingCG), 2) + 
                (compressionBlock.Cracked_I + compressionBlock.Area * Math.Pow(_cracked_NA - compressionBlock.CG, 2))/modRatio;
        }

        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        public CircularColumn()
        {
            _calc_properties(_radius);
        }
        public CircularColumn(double radius)
        {
            _radius = radius;
            _calc_properties();
        }

        public double Service_NA(double modRatio)
        {
            _calc_service_NA(modRatio);
            return _cracked_NA;
        }

        public double CrackedMomentOfInertia(double modRatio)
        {
            _calc_cracked_moment_of_inertia(modRatio);
            return _cracked_moment_of_inertia;
        }

        public double ServiceConcreteStress(double moment,double modRatio)
        {
            _calc_service_concrete_stress(moment, modRatio);
            return _service_concrete_stress;
        }
    }
}
