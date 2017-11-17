using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Cos : LGPOperator
    {
        public LGPOperator_Cos()
            : base("cos")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Cos clone = new LGPOperator_Cos();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x = operand1.Value;
            destination_register.Value = System.Math.Cos(x);
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
