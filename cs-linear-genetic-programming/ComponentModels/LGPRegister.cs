using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public class LGPRegister
    {
        internal bool mIsConstant = false;
        private double mValue = 1;
        internal int mRegisterIndex;

        public int RegisterIndex
        {
            get { return mRegisterIndex; }
        }
        public bool IsConstant
        {
            get { return mIsConstant;  }
        }
        public double Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        public int IntValue
        {
            get { return (int)mValue; }
        }
        public bool BoolValue
        {
            get { return mValue > 0; }
        }
        
        public LGPRegister Clone()
        {
            LGPRegister clone = new LGPRegister();
            clone.mRegisterIndex = mRegisterIndex;
            clone.mIsConstant = mIsConstant;
            clone.mValue = mValue;
            return clone;
        }
        
        public virtual void Mutate(Statistics.Gaussian gaussian, double standard_deviation)
        {
            mValue+=gaussian.GetNormal() * standard_deviation;
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", mIsConstant ? "c" : "r", mRegisterIndex);
        }

    }
}
