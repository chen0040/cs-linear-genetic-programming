using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Selection
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public abstract class LGPSelectionInstruction
    {
        public LGPSelectionInstruction()
        {

        }

        public LGPSelectionInstruction(XmlElement xml_level1)
        {

        }

        public abstract LGPProgram Select(LGPPop pop);
        public abstract void Select(LGPPop lgpPop, ref KeyValuePair<LGPProgram, LGPProgram> best_pair, ref KeyValuePair<LGPProgram, LGPProgram> worst_pair);
        public abstract LGPSelectionInstruction Clone();
    }
}
