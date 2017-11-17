using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public class LGPInstruction
    {
        private LGPProgram mProgram;
        private LGPOperator mOperator;
        private LGPRegister mOperand1;
        private LGPRegister mOperand2;
        private LGPRegister mDestinationRegister;
        private bool mIsStructuralIntron=false;

        public LGPInstruction(LGPProgram prog)
        {
            mProgram = prog;
        }

        public bool IsStructuralIntron
        {
            get { return mIsStructuralIntron; }
            set { mIsStructuralIntron = value; }
        }

        public virtual void Create()
        {
            mOperator = mProgram.OperatorSet.FindRandomOperator();

            double p_const = 0.5;
            double r = Statistics.DistributionModel.GetUniform();
            if (r < p_const)
            {
                mOperand1 = mProgram.ConstantSet.FindRandomRegister();
            }
            else
            {
                mOperand1 = mProgram.RegisterSet.FindRandomRegister();
            }
            if (mOperand1.IsConstant)
            {
                mOperand2 = mProgram.RegisterSet.FindRandomRegister();
            }
            else
            {
                r = Statistics.DistributionModel.GetUniform();
                if (r < p_const)
                {
                    mOperand2 = mProgram.ConstantSet.FindRandomRegister();
                }
                else
                {
                    mOperand2 = mProgram.RegisterSet.FindRandomRegister();
                }
            }
            mDestinationRegister = mProgram.RegisterSet.FindRandomRegister();
        }

        public LGPOperator.OperatorExecutionStatus Execute()
        {
            return mOperator.Execute(mOperand1, mOperand2, mDestinationRegister);
        }


        public LGPInstruction Clone()
        {
            LGPInstruction clone = new LGPInstruction(mProgram);
            clone.mProgram = mProgram;
            clone.mIsStructuralIntron = mIsStructuralIntron;
            clone.mOperator = mOperator;
            clone.mOperand1 = mOperand1;
            clone.mOperand2 = mOperand2;
            clone.mDestinationRegister = mDestinationRegister;
            return clone;
        }

        public LGPRegister DestinationRegister
        {
            set
            {
                mDestinationRegister = value;
            }
            get
            {
                return mDestinationRegister;
            }
        }

        public bool IsConditionalConstruct
        {
            get { return mOperator.IsConditionalConstruct; }
        }

        public bool IsOperand1ConstantRegister
        {
            get { return mOperand1.IsConstant; }
        }

        public bool IsOperand2ConstantRegister
        {
            get { return mOperand2.IsConstant; }
        }

        public int OperatorRegisterIndex
        {
            get { return mOperator.RegisterIndex; }
        }

        public int Operand1RegisterIndex
        {
            get { return mOperand1.RegisterIndex; }
        }

        public int Operand2RegisterIndex
        {
            get { return mOperand2.RegisterIndex; }
        }

        public int DestinationRegisterIndex
        {
            get { return mDestinationRegister.RegisterIndex; }
        }

        public LGPProgram Program
        {
            set
            {
                LGPProgram rhs = value;
                mOperator = rhs.OperatorSet.FindOperatorByIndex(OperatorRegisterIndex);
                if (IsOperand1ConstantRegister)
                {
                    mOperand1 = rhs.ConstantSet.FindRegisterByIndex(Operand1RegisterIndex);
                }
                else
                {
                    mOperand1 = rhs.RegisterSet.FindRegisterByIndex(Operand1RegisterIndex);
                }

                if (IsOperand2ConstantRegister)
                {
                    mOperand2 = rhs.ConstantSet.FindRegisterByIndex(Operand2RegisterIndex);
                }
                else
                {
                    mOperand2 = rhs.RegisterSet.FindRegisterByIndex(Operand2RegisterIndex);
                }

                mDestinationRegister = rhs.RegisterSet.FindRegisterByIndex(DestinationRegisterIndex);
                mProgram = rhs;
            }
        }

        public virtual void MutateOperator()
        {
            mOperator = mProgram.OperatorSet.FindRandomOperator();
        }

        public virtual void MutateRegister(double p_const = 0.5)
        {
            double r = Statistics.DistributionModel.GetUniform();
            if (r < 0.5)
            {
                mDestinationRegister = mProgram.RegisterSet.FindRandomRegister(mDestinationRegister);
            }
            else
            {
                r = Statistics.DistributionModel.GetUniform();
                LGPRegister arg1, arg2;
                if (r < 0.5)
                {
                    arg1 = mOperand1;
                    arg2 = mOperand2;
                }
                else
                {
                    arg1 = mOperand2;
                    arg2 = mOperand1;
                }

                if (arg2.IsConstant)
                {
                    arg1 = mProgram.RegisterSet.FindRandomRegister();
                }
                else
                {
                    r = Statistics.DistributionModel.GetUniform();
                    if (r < p_const)
                    {
                        arg1 = mProgram.ConstantSet.FindRandomRegister();
                    }
                    else
                    {
                        arg1 = mProgram.RegisterSet.FindRandomRegister();
                    }
                }

                mOperand1 = arg1;
                mOperand2 = arg2;
            }
        }

        public virtual void MutateConstant(Statistics.Gaussian gauss, double standard_deviation)
        {
            if (mOperand1.IsConstant)
            {
                mOperand1.Mutate(gauss, standard_deviation);
            }
            else
            {
                mOperand2.Mutate(gauss, standard_deviation);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}\t{1}\t{2}\t{3}>{4}", mOperator, mOperand1, mOperand2, mDestinationRegister, mIsStructuralIntron ? "(intron)" : "");

            return sb.ToString();
        }

        public LGPOperator Operator
        {
            get { return mOperator; }
        }
        public LGPRegister Operand1
        {
            get { return mOperand1; }
        }
        public LGPRegister Operand2
        {
            get { return mOperand2; }
        }
        
    }
}
