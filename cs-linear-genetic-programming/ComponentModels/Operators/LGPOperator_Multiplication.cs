using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Multiplication : LGPOperator
    {
        public LGPOperator_Multiplication()
            : base("*")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Multiplication clone = new LGPOperator_Multiplication();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x1 = operand1.Value;
            double x2 = operand2.Value;
            destination_register.Value = x1 * x2;
           
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
