using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.RegInit
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using LinearGP.ProblemModels;

    class LGPRegInitInstruction_CompleteInputInitReg : LGPRegInitInstruction
    {

        public LGPRegInitInstruction_CompleteInputInitReg()
        {

        }

        public LGPRegInitInstruction_CompleteInputInitReg(XmlElement xml_level1)
            : base(xml_level1)
        {
            
        }

        public override void InitializeRegisters(LGPRegisterSet reg_set, LGPConstantSet constant_set, LGPFitnessCase fitness_case)
        {
            int iRegisterCount=reg_set.RegisterCount;
	        int iInputCount=fitness_case.GetInputCount();


	        int iRegisterIndex=0;
	        while(iRegisterIndex < iRegisterCount)
	        {
		        for(int j=0; j<iInputCount; ++j, ++iRegisterIndex)
		        {
			        if(iRegisterIndex >= iRegisterCount)
			        {
				        break;
			        }

			        double value;
			        fitness_case.QueryInput(j, out value);
			        reg_set.FindRegisterByIndex(iRegisterIndex).Value=value;
		        }
	        }

        }

        public override LGPRegInitInstruction Clone()
        {
            LGPRegInitInstruction_CompleteInputInitReg clone = new LGPRegInitInstruction_CompleteInputInitReg();
            return clone;
        }

        public override string ToString()
        {
            return ">> Name: LGPRegInitInstruction_CompleteInputInitReg";
        }
    }
}
