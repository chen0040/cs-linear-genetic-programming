using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ProblemModels
{
    public abstract class LGPFitnessCase
    {
        public LGPFitnessCase()
        {

        }

        public abstract int GetInputCount();

        public abstract bool QueryInput(int index, out double input);

        public virtual void ReportProgress(ComponentModels.LGPOperator mOperator, ComponentModels.LGPRegister mOperand1, ComponentModels.LGPRegister mOperand2, ComponentModels.LGPRegister mDestinationRegister, ComponentModels.LGPRegisterSet lGPRegisterSet)
        {

        }

        public abstract void RunLGPProgramCompleted(double[] outputs);
    }
}
