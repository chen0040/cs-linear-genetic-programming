using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Survival
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    class LGPSurvivalInstruction_Probablistic : LGPSurvivalInstruction
    {
        private double m_reproduction_probability = 1;

        public LGPSurvivalInstruction_Probablistic()
        {

        }

        public LGPSurvivalInstruction_Probablistic(XmlElement xml_level1)
            : base(xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "reproduction_probability")
                    {
                        double value = 0;
                        double.TryParse(attrvalue, out value);
                        m_reproduction_probability = value;
                    }
                }
            }
        }

        public override LGPProgram Compete(LGPPop pop, LGPProgram weak_program_in_current_pop, LGPProgram child_program)
        {
            double r = DistributionModel.GetUniform();

            if (r < m_reproduction_probability)
            {
                //Console.WriteLine("replacing...");
                pop.Replace(weak_program_in_current_pop, child_program);
                return weak_program_in_current_pop;
            }

            return child_program;
        }

        public override LGPSurvivalInstruction Clone()
        {
            LGPSurvivalInstruction_Probablistic clone = new LGPSurvivalInstruction_Probablistic();
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(">> Name: LGPSurvivalInstruction_Probablistic");
            sb.AppendFormat(">> Reproduction Probability: {0}", m_reproduction_probability);

            return sb.ToString();
        }
    }
}
