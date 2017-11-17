using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels.Operators
{
    /// <summary>
    ///  this operator implements the "If greater than" instruction
    /// Ifgt(operand1 > operand2)
    /// {
    ///     execute next line
    /// }
    /// else
    /// {
    ///     skip next line
    /// }
    /// </summary>
    public class LGPOperator_Ifgt : LGPOperator
    {
        public LGPOperator_Ifgt()
            : base("If>")
        {
            mIsConditionalConstruct = true;
        }

        public override OperatorExecutionStatus Execute(LGPRegister operand1, LGPRegister operand2, LGPRegister destination_register)
        {
            if (operand1.Value > operand2.Value)
            {
                return OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
            }
            else
            {
                return OperatorExecutionStatus.LGP_SKIP_NEXT_INSTRUCTION;
            }
        }

        public override LGPOperator Clone()
        {
            LGPOperator_Ifgt clone = new LGPOperator_Ifgt();
            clone.Copy(this);
            return clone;
        }
    }
}
