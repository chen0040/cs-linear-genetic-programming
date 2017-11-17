using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public class LGPOperatorSet
    {
        private List<KeyValuePair<LGPOperator, double>> mOperators=new List<KeyValuePair<LGPOperator, double>>();
        private double mWeightSum=0;

        public LGPOperatorSet()
        {

        }

        public LGPOperator FindRandomOperator(LGPOperator current_operator=null)
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                double r = mWeightSum * Statistics.DistributionModel.GetUniform();

                double current_sum = 0;
                foreach (KeyValuePair<LGPOperator, double> point in mOperators)
                {
                    current_sum += point.Value;
                    if (current_sum >= r)
                    {
                        if (point.Key != current_operator)
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

            return current_operator;
        }

        public LGPOperator FindOperatorByIndex(int op_index)
        {
            return mOperators[op_index].Key;
        }

        public LGPOperatorSet Clone()
        {
            LGPOperatorSet clone = new LGPOperatorSet();
            clone.mWeightSum = mWeightSum;
            foreach (KeyValuePair<LGPOperator, double> point in mOperators)
            {
                clone.mOperators.Add(new KeyValuePair<LGPOperator, double>(point.Key.Clone(), point.Value));
            }

            return clone;
        }

        public int OperatorCount
        {
            get { return mOperators.Count; }
        }

        public void AddOperator(LGPOperator op, double weight=1)
        {
            mOperators.Add(new KeyValuePair<LGPOperator, double>(op, weight));
            op.mOperatorIndex = mOperators.Count - 1;
            mWeightSum += weight;
        }

        public void AddIfltOperator(double weight = 1)
        {
            AddOperator(new Operators.LGPOperator_Iflt(), weight);
        }

        public void AddIfgtOperator(double weight = 1)
        {
            AddOperator(new Operators.LGPOperator_Ifgt(), weight);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mOperators.Count; ++i)
            {
                sb.AppendFormat("operators[{0}]: {1}\n", i, mOperators[i].Key);
            }
            return sb.ToString();
        }
    }
}
