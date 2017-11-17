using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Sqrt : LGPOperator
    {
        public LGPOperator_Sqrt()
            : base("cos")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Sqrt clone = new LGPOperator_Sqrt();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x = operand1.Value;
            destination_register.Value = System.Math.Sqrt(System.Math.Abs(x));
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
