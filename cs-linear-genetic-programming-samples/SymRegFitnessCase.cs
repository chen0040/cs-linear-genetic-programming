using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.SymbolicRegression
{
    using LinearGP.ProblemModels;
    using LinearGP.ComponentModels;

    public class SymRegFitnessCase : LGPFitnessCase
    {
        private double mX;
        private double mY;
        private double mComputedY;

        public double ComputedY
        {
            get { return mComputedY; }
        }

        public double X
        {
            get { return mX; }
            set { mX = value; }
        }

        public double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public override void RunLGPProgramCompleted(double[] result)
        {
            mComputedY = result[0];
        }

        public override bool QueryInput(int index, out double input)
        {
            input = 0;
            if (index == 0)
            {
                input = mX;
                return true;
            }
            
            return false;
        }


        public override int GetInputCount()
        {
            return 1;
        }


    }
}
