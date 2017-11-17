using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Selection
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public class LGPSelectionInstructionFactory
    {
        private string mFilename;
        private LGPSelectionInstruction mCurrentInstruction;

        public LGPSelectionInstructionFactory(string filename)
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
                        if (attrname == "tournament")
                        {
                            mCurrentInstruction = new LGPSelectionInstruction_Tournament(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPSelectionInstructionFactory Clone()
        {
            LGPSelectionInstructionFactory clone = new LGPSelectionInstructionFactory(mFilename);
            return clone;
        }

        public virtual void Select(LGPPop pop, ref KeyValuePair<LGPProgram, LGPProgram> best_pair, ref KeyValuePair<LGPProgram, LGPProgram> worst_pair)
        {
            if (mCurrentInstruction != null)
            {
                mCurrentInstruction.Select(pop, ref best_pair, ref worst_pair);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public virtual LGPProgram Select(LGPPop pop)
        {
            if (mCurrentInstruction != null)
            {
                return mCurrentInstruction.Select(pop);
            }
            return null;
        }

        public override string ToString()
        {
            if (mCurrentInstruction != null)
            {
                return mCurrentInstruction.ToString();
            }
            return "LGP Selection Instruction Factory";
        }
    }
}
