using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    public class LGPOperator_Division : LGPOperator
    {
        public LGPOperator_Division()
            : base("/")
        {

        }

        public override LGPOperator Clone()
        {
            LGPOperator_Division clone = new LGPOperator_Division();
            clone.Copy(this);
            return clone;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            double x1 = operand1.Value;
            double x2 = operand2.Value;
            if (x2 == 0)
            {
                destination_register.Value = x1 + LGPProtectedDefinition.Instance.UNDEFINED;
            }
            else
            {
                destination_register.Value = x1 / x2;
            }
           
            return LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
        }
    }
}
