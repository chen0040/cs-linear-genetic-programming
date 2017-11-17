using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public class LGPRegisterSet
    {
        private List<KeyValuePair<LGPRegister, double>> mRegisters = new List<KeyValuePair<LGPRegister, double>>();
        private double mWeightSum = 0;

        public LGPRegisterSet()
        {

        }

        public LGPRegisterSet Clone()
        {
            LGPRegisterSet clone = new LGPRegisterSet();
            clone.mWeightSum = mWeightSum;
            foreach (KeyValuePair<LGPRegister, double> point in mRegisters)
            {
                clone.mRegisters.Add(new KeyValuePair<LGPRegister, double>(point.Key.Clone(), point.Value));
            }
            return clone;
        }

        public LGPRegister FindRandomRegister(LGPRegister current_register=null)
        {
            for(int attempts=0; attempts < 10; attempts++)
            {
                double r = mWeightSum * Statistics.DistributionModel.GetUniform();

                double current_sum = 0;
                foreach (KeyValuePair<LGPRegister, double> point in mRegisters)
                {
                    current_sum += point.Value;
                    if (current_sum >= r)
                    {
                        if (point.Key != current_register)
                        {
                            return point.Key;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
            }

            return current_register;
        }

        public LGPRegister FindRegisterByIndex(int register_index)
        {
            return mRegisters[register_index].Key;
        }

        public int RegisterCount
        {
            get { return mRegisters.Count; }
        }

        public void AddRegister(LGPRegister register, double weight = 1)
        {
            mRegisters.Add(new KeyValuePair<LGPRegister, double>(register, weight));
            register.mRegisterIndex = mRegisters.Count - 1;
            register.mIsConstant = false;
            mWeightSum += weight;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mRegisters.Count; ++i)
            {
                sb.AppendFormat("register[{0}]: {1}\n", i, mRegisters[i].Key);
            }
            return sb.ToString();
        }
    }
}
