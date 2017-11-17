using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Mutation
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public class LGPMutationInstructionFactory
    {
        private string mFilename;
        private LGPMutationInstruction mCurrentMacroMutation;

        public LGPMutationInstructionFactory(string filename)
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
                        if (attrname == "macro_mutation")
                        {
                            mCurrentMacroMutation = new LGPMutationInstruction_Macro(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPMutationInstructionFactory Clone()
        {
            LGPMutationInstructionFactory clone = new LGPMutationInstructionFactory(mFilename);
            return clone;
        }

        public void Mutate(LGPPop pop, LGPProgram child1, LGPProgram child2)
        {
            if (mCurrentMacroMutation != null)
            {
                mCurrentMacroMutation.Mutate(pop, child1, child2);
            }
        }

        public void Mutate(LGPPop pop, LGPProgram child)
        {
            if (mCurrentMacroMutation != null)
            {
                mCurrentMacroMutation.Mutate(pop, child);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public override string ToString()
        {
            if (mCurrentMacroMutation != null)
            {
                return mCurrentMacroMutation.ToString();
            }
            return "Mutation Instruction Factory";
        }

    }
}
