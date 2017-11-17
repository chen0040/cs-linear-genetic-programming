using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Sin : LGPOperator
    {
        public LGPOperator_Sin()
            : base("sin")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Sin clone = new LGPOperator_Sin();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x = operand1.Value;
            destination_register.Value = System.Math.Sin(x);
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
