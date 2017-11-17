using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.MexicanHat
{
    using LinearGP.ProblemModels;
    using LinearGP.ComponentModels;

    public class MexicanHatFitnessCase : LGPFitnessCase
    {
        private double[] mX = new double[2];
        private double mY;
        private double mPredictedY;

        public double PredictedY
        {
            get { return mPredictedY; }
        }

        public double X1
        {
            get { return mX[0]; }
            set { mX[0] = value; }
        }

        public double X2
        {
            get { return mX[1]; }
            set { mX[1] = value; }
        }

        public double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public override void RunLGPProgramCompleted(double[] result)
        {
            mPredictedY = result[0];
        }

        public override bool QueryInput(int index, out double input)
        {
            input = 0;
            if (index < mX.Length)
            {
                input = mX[index];
                return true;
            }
            
            return false;
        }


        public override int GetInputCount()
        {
            return mX.Length;
        }


    }
}
