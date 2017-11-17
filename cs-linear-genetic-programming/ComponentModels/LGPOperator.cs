using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    public abstract class LGPOperator
    {
        public enum OperatorExecutionStatus
        {
            LGP_EXECUTE_NEXT_INSTRUCTION,
            LGP_SKIP_NEXT_INSTRUCTION
        }

        protected string mSymbol;
        protected bool mIsConditionalConstruct=false;
        internal int mOperatorIndex;

        public bool IsConditionalConstruct
        {
            get { return mIsConditionalConstruct; }
        }

        public int RegisterIndex
        {
            get { return mOperatorIndex; }
        }

        public abstract LGPOperator Clone();

        public void Copy(LGPOperator rhs)
        {
            mOperatorIndex = rhs.mOperatorIndex;
            mIsConditionalConstruct = rhs.mIsConditionalConstruct;
            mSymbol = rhs.mSymbol;
        }

        public LGPOperator(string symbol)
        {
            mSymbol = symbol;
        }

        public abstract OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register);

        public override string ToString()
        {
            return mSymbol;
        }
    }
}
