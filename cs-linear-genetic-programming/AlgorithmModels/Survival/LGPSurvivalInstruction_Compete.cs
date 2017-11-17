using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Survival
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    class LGPSurvivalInstruction_Compete : LGPSurvivalInstruction
    {
        public LGPSurvivalInstruction_Compete()
        {

        }

        public LGPSurvivalInstruction_Compete(XmlElement xml_level1)
            : base(xml_level1)
        {
            
        }

        public override LGPProgram Compete(LGPPop pop, LGPProgram weak_program_in_current_pop, LGPProgram child_program)
        {
            if (child_program.IsBetterThan(weak_program_in_current_pop))
            {
                pop.Replace(weak_program_in_current_pop, child_program);
                return weak_program_in_current_pop;
            }
            return child_program;
        }

        public override LGPSurvivalInstruction Clone()
        {
            LGPSurvivalInstruction_Compete clone = new LGPSurvivalInstruction_Compete();
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(">> Name: LGPSurvivalInstruction_Compete");

            return sb.ToString();
        }
    }
}
