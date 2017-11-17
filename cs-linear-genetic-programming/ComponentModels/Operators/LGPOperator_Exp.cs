using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Exp : LGPOperator
    {
        public LGPOperator_Exp()
            : base("exp")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Exp clone = new LGPOperator_Exp();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x = operand1.Value;
            destination_register.Value = System.Math.Exp(x);
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
