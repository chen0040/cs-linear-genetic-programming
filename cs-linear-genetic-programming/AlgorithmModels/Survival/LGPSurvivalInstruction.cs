using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Survival
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public abstract class LGPSurvivalInstruction
    {
        public LGPSurvivalInstruction()
        {

        }

        public LGPSurvivalInstruction(XmlElement xml_level1)
        {

        }

        // Xianshun says:
        // this method return the pointer of the program that is to be deleted (loser in the competition for survival)
        public abstract LGPProgram Compete(LGPPop pop, LGPProgram weak_program_in_current_pop, LGPProgram child_program);
        public abstract LGPSurvivalInstruction Clone();
    }
}
