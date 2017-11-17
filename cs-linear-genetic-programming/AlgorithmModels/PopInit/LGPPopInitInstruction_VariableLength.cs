using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.PopInit
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    class LGPPopInitInstruction_VariableLength : LGPPopInitInstruction
    {
        private int m_iInitialMaxProgLength;
        private int m_iInitialMinProgLength;

        public LGPPopInitInstruction_VariableLength()
        {

        }

        public LGPPopInitInstruction_VariableLength(XmlElement xml_level1)
            : base(xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "InitialMinProgLength")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        m_iInitialMinProgLength = value;
                    }
                    else if (attrname == "InitialMaxProgLength")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        m_iInitialMaxProgLength = value;
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
		        int iProgLength=m_iInitialMinProgLength + DistributionModel.NextInt(m_iInitialMaxProgLength - m_iInitialMinProgLength + 1);
                //Console.WriteLine("Prog Length: {0}", iProgLength);
		        LGPProgram lgp=pop.CreateProgram(iProgLength, pop.Environment);
		        pop.AddProgram(lgp);

                //Console.WriteLine("Min Length: {0}", m_iInitialMinProgLength);
                //Console.WriteLine("LGP: {0}", lgp.InstructionCount);

                if (lgp.InstructionCount < m_iInitialMinProgLength)
                {
                    throw new ArgumentNullException();
                }
                if (lgp.InstructionCount > m_iInitialMaxProgLength)
                {
                    throw new ArgumentNullException();
                }
	        }
        }

        public override LGPPopInitInstruction Clone()
        {
            LGPPopInitInstruction_VariableLength clone = new LGPPopInitInstruction_VariableLength();
            clone.m_iInitialMaxProgLength = m_iInitialMaxProgLength;
            clone.m_iInitialMinProgLength = m_iInitialMinProgLength;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(">> Name: LGPPopInstruction_MaximumInitialization\n");
            sb.AppendFormat(">> Min Initial Program Length: {0}\n", m_iInitialMinProgLength);
            sb.AppendFormat(">> Max Initial Program Length: {0}", m_iInitialMaxProgLength);

            return sb.ToString();
        }
    }
}
