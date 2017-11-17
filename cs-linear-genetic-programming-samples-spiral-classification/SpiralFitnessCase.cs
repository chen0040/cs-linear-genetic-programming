using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.SpiralClassification
{
    using LinearGP.ProblemModels;
    using LinearGP.ComponentModels;

    public class SpiralFitnessCase : LGPFitnessCase
    {
        private double[] mCoordinates = new double[2];
        private int mLabel;
        private int mComputedLabel;

        public int ComputedLabel
        {
            get { return mComputedLabel; }
        }

        public int Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        public double X
        {
            get { return mCoordinates[0]; }
            set { mCoordinates[0] = value; }
        }

        public double Y
        {
            get { return mCoordinates[1]; }
            set { mCoordinates[1] = value; }
        }

        public override void RunLGPProgramCompleted(double[] result)
        {
            if (result[0] < 0.5)
            {
                mComputedLabel = -1;
            }
            else
            {
                mComputedLabel = 1;
            }
        }

        public override bool QueryInput(int index, out double input)
        {
            input = 0;
            if (index < mCoordinates.Length)
            {
                input = mCoordinates[index];
                return true;
            }
            return false;
        }


        public override int GetInputCount()
        {
            return mCoordinates.Length;
        }


    }
}
