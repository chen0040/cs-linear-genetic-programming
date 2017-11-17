using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Crossover
{
    using System.Xml;
    using LinearGP.ComponentModels;

    public class LGPCrossoverInstructionFactory
    {
        private LGPCrossoverInstruction mCurrentCrossover;
        private string mFilename;

        public LGPCrossoverInstructionFactory(string filename)
        {
            mFilename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement doc_root = doc.DocumentElement;
            string selected_strategy = doc_root.Attributes["strategy"].Value;
            foreach(XmlElement xml_level1 in doc_root.ChildNodes)
            {
                if (xml_level1.Name == "strategy")
                {
                    string attrname = xml_level1.Attributes["name"].Value;
                    if (attrname == selected_strategy)
                    {
                        if (attrname == "linear")
                        {
                            mCurrentCrossover = new LGPCrossoverInstruction_Linear(xml_level1);
                        }
                        else if (attrname == "one_point")
                        {
                            mCurrentCrossover = new LGPCrossoverInstruction_OnePoint(xml_level1);
                        }
                        else if (attrname == "one_seg")
                        {
                            mCurrentCrossover = new LGPCrossoverInstruction_OneSegment(xml_level1);
                        }
                    }
                }
            }
        }

        public virtual LGPCrossoverInstructionFactory Clone()
        {
            LGPCrossoverInstructionFactory clone = new LGPCrossoverInstructionFactory(mFilename);
            return clone;
        }

        public void Crossover(LGPPop pop, LGPProgram child1, LGPProgram child2)
        {
            if (mCurrentCrossover != null)
            {
                mCurrentCrossover.Crossover(pop, child1, child2);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public override string ToString()
        {
            return mCurrentCrossover.ToString();
        }
    }
}
