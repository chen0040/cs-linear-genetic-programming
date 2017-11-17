using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Log : LGPOperator
    {
        public LGPOperator_Log()
            : base("log")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Log clone = new LGPOperator_Log();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x = operand1.Value;
            if (x == 0)
            {
                destination_register.Value = x + LGPProtectedDefinition.Instance.UNDEFINED;
            }
            else
            {
                destination_register.Value = System.Math.Log(x);
            }
            
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
