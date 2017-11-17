using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.RegInit
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using LinearGP.ProblemModels;

    public class LGPRegInitInstructionFactory
    {
        private string mFilename;
        private LGPRegInitInstruction mCurrentInstruction;

        public LGPRegInitInstructionFactory(string filename)
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
                        if (attrname == "initialize_register_with_input")
                        {
                            mCurrentInstruction = new LGPRegInitInstruction_Standard(xml_level1);
                        }
                        else if (attrname == "complete_initialization_of_register_with_input")
                        {
                            mCurrentInstruction = new LGPRegInitInstruction_CompleteInputInitReg(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPRegInitInstructionFactory Clone()
        {
            LGPRegInitInstructionFactory clone = new LGPRegInitInstructionFactory(mFilename);
            return clone;
        }

        public virtual void InitializeRegisters(LGPRegisterSet reg_set, LGPConstantSet constant_set, LGPFitnessCase fitness_case)
        {
            if (mCurrentInstruction != null)
            {
                mCurrentInstruction.InitializeRegisters(reg_set, constant_set, fitness_case);
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
            return "LGP Reg Init Instruction Factory";
        }
    }
}
