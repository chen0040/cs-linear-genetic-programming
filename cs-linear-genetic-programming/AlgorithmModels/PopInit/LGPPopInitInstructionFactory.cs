using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.PopInit
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public class LGPPopInitInstructionFactory
    {
        private string mFilename;
        private LGPPopInitInstruction mCurrentInstruction;

        public LGPPopInitInstructionFactory(string filename)
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
                        if (attrname == "variable_length")
                        {
                            mCurrentInstruction = new LGPPopInitInstruction_VariableLength(xml_level1);
                        }
                        else if (attrname == "constant_length")
                        {
                            mCurrentInstruction = new LGPPopInitInstruction_ConstantLength(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPPopInitInstructionFactory Clone()
        {
            LGPPopInitInstructionFactory clone = new LGPPopInitInstructionFactory(mFilename);
            return clone;
        }

        public virtual void Initialize(LGPPop pop)
        {
            if (mCurrentInstruction != null)
            {
                mCurrentInstruction.Initialize(pop);
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
            return "LGP Pop Init Instruction Factory";
        }
    }
}
