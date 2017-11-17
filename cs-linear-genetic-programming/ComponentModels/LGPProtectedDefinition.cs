using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public class LGPProtectedDefinition
    {
        private static LGPProtectedDefinition mInstance = null;
        private static object mSyncObj = new object();

        private bool mUseUndefinedLow = true;
        private double mLGP_REG_POSITIVE_INF = 10000000;
        private double mLGP_REG_NEGATIVE_INF = -10000000;
        private double mUndefinedLow=1;
        private double mUndefinedHigh = 1000000;

        public double LGP_REG_POSITIVE_INF
        {
            get { return mLGP_REG_POSITIVE_INF; }
        }

        public double LGP_REG_NEGATIVE_INF
        {
            get { return mLGP_REG_NEGATIVE_INF; }
        }

        public double UNDEFINED
        {
            get
            {
                if (mUseUndefinedLow)
                {
                    return mUndefinedLow;
                }
                return mUndefinedHigh;
            }
        }

        private LGPProtectedDefinition()
        {

        }

        public static LGPProtectedDefinition Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (mSyncObj)
                    {
                        mInstance = new LGPProtectedDefinition();
                    }
                }
                return mInstance;
            }
        }


    }
}
