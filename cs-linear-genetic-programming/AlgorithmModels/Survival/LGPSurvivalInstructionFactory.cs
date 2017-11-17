using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Survival
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public class LGPSurvivalInstructionFactory
    {
        private string mFilename;
        private LGPSurvivalInstruction mCurrentInstruction;

        public LGPSurvivalInstructionFactory(string filename)
        {
            mFilename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(mFilename);
            XmlElement doc_root = doc.DocumentElement;
            string selected_strategy = doc_root.Attributes["strategy"].Value;
            foreach (XmlElement xml_level1 in doc_root.ChildNodes)
            {
                if (xml_level1.Name == "strategy")
                {
                    string attrname = xml_level1.Attributes["name"].Value;
                    if (attrname == selected_strategy)
                    {
                        if (attrname == "compete")
                        {
                            mCurrentInstruction = new LGPSurvivalInstruction_Compete(xml_level1);
                        }
                        else if (attrname == "probablistic")
                        {
                            mCurrentInstruction = new LGPSurvivalInstruction_Probablistic(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPSurvivalInstructionFactory Clone()
        {
            LGPSurvivalInstructionFactory clone = new LGPSurvivalInstructionFactory(mFilename);
            return clone;
        }

        public virtual LGPProgram Compete(LGPPop pop, LGPProgram weak_program_in_current_pop, LGPProgram child_program)
        {
            if (mCurrentInstruction != null)
            {
                return mCurrentInstruction.Compete(pop, weak_program_in_current_pop, child_program);
            }
            else
            {
                throw new ArgumentNullException();
            }
            
        }


        public override string ToString()
        {
            if (mCurrentInstruction != null)
            {
                return mCurrentInstruction.ToString();
            }
            return "LGP Survival Instruction Factory";
        }
    }
}
