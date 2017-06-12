using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColumnService
{
    

    public class CircularColumn: Column
    {
        private double _radius = 0;
        private double _gross_area = 0;
        private double _cracked_NA = 0;
        //private double _effective_NA = 0;
        private double _cracked_moment_of_inertia = 0;
        private double _service_concrete_stress = 0;
        private double[,] _concrete_service_force_and_CG = new double[1, 3];

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

        private void _calc_cracked_moment_of_inertia(double modRatio)
        {
            //_calc_service_NA(modRatio);
            CircularSegment compressionBlock = new CircularSegment(_radius, _radius - _cracked_NA);
            this.calcReinf();
            _cracked_moment_of_inertia = ReinforcingInertia + ReinforcingArea * Math.Pow((_cracked_NA - ReinforcingCG), 2) +
                (compressionBlock.Cracked_I + compressionBlock.Area * Math.Pow(_cracked_NA - compressionBlock.CG, 2)) / modRatio;
        }

        private void _calc_service_stresses(double axial, double moment,double modRatio)
        {
            _service_concrete_stress =  moment * 12 * (_cracked_NA- _radius) / _cracked_moment_of_inertia / modRatio;
            _reinforcing_terms(_radius, _cracked_NA, _service_concrete_stress, modRatio, Reinforcing);
        }
        private void _calc_service_stresses(double moment, double modRatio)
        {
            _calc_service_stresses(0, moment, modRatio);
        }

        private void _calc_concrete_service_force_and_CG(double axial, double moment, double modRatio)
        {
            _concrete_service_force_and_CG =_concrete_service_terms(_radius, _cracked_NA, _service_concrete_stress, 100);
        }
        /*private void _calc_concrete_service_force_and_CG(double axial, double moment, double modRatio)
        {
            //_calc_service_concrete_stress(axial, moment, modRatio);
            CircularSegment compressionBlock = new CircularSegment();
            compressionBlock.Radius = _radius;
            int increments = 100;
            double stressIncrement = _service_concrete_stress / increments;
            double compressionDepthIncrement = (_radius - _cracked_NA) / increments;
            double force = 0;
            double CG = 0;
            double firstMoment = 0;

            for(int i = 0; i < increments; i++)
            {
                compressionBlock.CompressionDepth = (_radius - _cracked_NA) - compressionDepthIncrement * (i + 0.5);
                force += compressionBlock.Area * stressIncrement;
                firstMoment += compressionBlock.Area * compressionBlock.CG*stressIncrement;
            }
            _concrete_service_force_and_CG[0, 0] = force;
            _concrete_service_force_and_CG[0, 1] = firstMoment / force;
            _concrete_service_force_and_CG[0, 2] = firstMoment/12;
        }*/
        private void _calc_concrete_service_force_and_CG(double moment, double modRatio)
        {
            _calc_concrete_service_force_and_CG(0, moment, modRatio);
        }

        private double[,] _service_forces(double radius, List<Reinforcing> reinforcing, double modRatio, double neutralAxis, double stress)
        {
            _reinforcing_terms(radius, neutralAxis, stress, modRatio, reinforcing);
            double[,] concrete_terms = _concrete_service_terms(radius, neutralAxis, stress, 100);
            double[,] forceAndMoment = new double[1,2];
            forceAndMoment[0, 0] = concrete_terms[0, 0];
            forceAndMoment[0, 1] = concrete_terms[0, 2];
            forceAndMoment[1, 0] = ReinforcingForce;
            forceAndMoment[1, 1] = ReinforcingMoment;

            return forceAndMoment;
        }

        private void _reinforcing_terms(double radius, double neutralAxis, double stress, double modRatio, List<Reinforcing> reinforcing)
        {
            double reinfForce = 0;
            double reinfMoment = 0;
            stress *= modRatio;
            int count = 0;
            foreach (Reinforcing reinf in reinforcing)
            {
                reinf.Stress = stress * ((reinf.Y_Coordinate-neutralAxis) / (radius - neutralAxis));
                reinf.Force = reinf.Area * reinf.Stress;
                reinfForce += reinf.Force;
                reinfMoment += reinf.Force * reinf.Y_Coordinate / 12;
                count++;
            }
            ReinforcingForce = reinfForce;
            ReinforcingMoment = reinfMoment;
        }
        private double[,] _concrete_service_terms(double radius, double neutralAxis, double stress, int increments)
        {
            CircularSegment compressionBlock = new CircularSegment();
            compressionBlock.Radius = radius;
            double stressIncrement = stress / increments;
            double compressionDepthIncrement = (radius - neutralAxis) / increments;
            double force = 0;
            double firstMoment = 0;
            double[,] outputTerms = new double[1, 3];

            for (int i = 0; i < increments; i++)
            {
                compressionBlock.CompressionDepth = (radius - neutralAxis) - compressionDepthIncrement * (i + 0.5);
                force += compressionBlock.Area * stressIncrement;
                firstMoment += compressionBlock.Area * compressionBlock.CG * stressIncrement;
            }
            outputTerms[0, 0] = force;
            outputTerms[0, 1] = firstMoment / force;
            outputTerms[0, 2] = firstMoment / 12;
            return outputTerms;
        }

        private void _calc_moment_and_axial(double concreteStress, double cracked_NA)
        {

        }

        private void _calc_all(double axial, double moment, double modRatio)
        {
            _calc_service_NA(modRatio);
            _calc_cracked_moment_of_inertia(modRatio);
            _calc_service_stresses(axial, moment, modRatio);
            _calc_concrete_service_force_and_CG(axial, moment, modRatio);
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

        public double ServiceConcreteStress(double axial, double moment,double modRatio)
        {
            _calc_all(axial,moment, modRatio);
            return _service_concrete_stress;
        }
        public double ServiceConcreteStress(double moment, double modRatio)
        {
            _calc_all(0,moment, modRatio);
            return _service_concrete_stress;
        }

        public double ServiceConcreteForce(double axial, double moment, double modRatio)
        {
            _calc_all(axial, moment, modRatio);
            return _concrete_service_force_and_CG[0,0];
        }
        public double ServiceConcreteForce(double moment, double modRatio)
        {
            _calc_all(0,moment, modRatio);
            return _concrete_service_force_and_CG[0, 0];
        }

        public double ServiceConcreteCG(double axial, double moment, double modRatio)
        {
            _calc_all(axial, moment, modRatio);
            return _concrete_service_force_and_CG[0, 1];
        }
        public double ServiceConcreteCG(double moment, double modRatio)
        {
            _calc_all(0,moment, modRatio);
            return _concrete_service_force_and_CG[0, 1];
        }

        public double ServiceConcreteMoment(double axial, double moment, double modRatio)
        {
            _calc_all(axial, moment, modRatio);
            return _concrete_service_force_and_CG[0, 2]+ReinforcingMoment;
        }
        public double ServiceConcreteMoment(double moment, double modRatio)
        {
            _calc_all(0,moment, modRatio);
            return _concrete_service_force_and_CG[0, 2] + ReinforcingMoment;
        }
    }
}
