using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Power : LGPOperator
    {
        public LGPOperator_Power()
            : base("^")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Power clone = new LGPOperator_Power();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x1 =operand1.Value;
            double x2 = operand2.Value;
            if ( System.Math.Abs(x1) < 10)
            {
                destination_register.Value = System.Math.Pow( System.Math.Abs(x1), x2);
            }
            else
            {
                destination_register.Value = x1 + x2 + LGPProtectedDefinition.Instance.UNDEFINED;
            }

            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
