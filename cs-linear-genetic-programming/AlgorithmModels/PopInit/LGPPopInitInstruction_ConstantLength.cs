using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.PopInit
{
    using System.Xml;
    using LinearGP.ComponentModels;

    class LGPPopInitInstruction_ConstantLength : LGPPopInitInstruction
    {
        private int mConstantProgramLength=100;

        public LGPPopInitInstruction_ConstantLength()
        {

        }

        public LGPPopInitInstruction_ConstantLength(XmlElement xml_level1)
            : base(xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "constant_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mConstantProgramLength = value;
                    }
                }
            }
        }

        public override void Initialize(LGPPop pop)
        {
            // Xianshun says:
	        // specified here is a variable length initialization that selects initial program
	        // lengths from a uniform distribution within a specified range of m_iInitialMinProgLength - m_iIinitialMaxProgLength
	        // the method is recorded in chapter 7 section 7.6 page 164 of Linear Genetic Programming 2004
	        int iPopulationSize=pop.PopulationSize;

	        // Xianshun says:
	        // the program generated in this way will have program length as small as 
	        // iMinProgLength and as large as iMaxProgLength
	        // the program length is distributed uniformly between iMinProgLength and iMaxProgLength
	        for(int i=0; i<iPopulationSize; i++)
	        {
		        LGPProgram lgp=pop.CreateProgram(mConstantProgramLength, pop.Environment);
		        pop.AddProgram(lgp);
	        }
        }

        public override LGPPopInitInstruction Clone()
        {
            LGPPopInitInstruction_ConstantLength clone = new LGPPopInitInstruction_ConstantLength();
            clone.mConstantProgramLength = mConstantProgramLength;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(">> Name: LGPPopInstruction_MaximumInitialization\n");
	        sb.AppendFormat(">> Constant Initial Program Length: {0}", mConstantProgramLength);

            return sb.ToString();
        }
    }
}
