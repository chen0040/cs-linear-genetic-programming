using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.RegInit
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using LinearGP.ProblemModels;

    class LGPRegInitInstruction_Standard : LGPRegInitInstruction
    {
        private int mInputCopyCount=1;
        private double mDefaultRegisterValue=1;

        public LGPRegInitInstruction_Standard()
        {

        }

        public LGPRegInitInstruction_Standard(XmlElement xml_level1)
            : base(xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "input_copy_count")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mInputCopyCount = value;
                    }
                    else if (attrname == "default_register_value")
                    {
                        double value = 0;
                        double.TryParse(attrvalue, out value);
                        mDefaultRegisterValue = value;
                    }
                }
            }
        }

        public override void InitializeRegisters(LGPRegisterSet reg_set, LGPConstantSet constant_set, LGPFitnessCase fitness_case)
        {
            int iRegisterCount=reg_set.RegisterCount;
	        int iInputCount=fitness_case.GetInputCount();

        

	        int iRegisterIndex=0;
	        for(int i=0; i<mInputCopyCount; ++i)
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

		        if(iRegisterIndex >= iRegisterCount)
		        {
			        break;
		        }
	        }

	        while(iRegisterIndex < iRegisterCount)
	        {
		        reg_set.FindRegisterByIndex(iRegisterIndex).Value=mDefaultRegisterValue;
		        iRegisterIndex++;
	        }


        }

        public override LGPRegInitInstruction Clone()
        {
            LGPRegInitInstruction_Standard clone = new LGPRegInitInstruction_Standard();
            clone.mDefaultRegisterValue = mDefaultRegisterValue;
            clone.mInputCopyCount = mInputCopyCount;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb=new StringBuilder();
            sb.AppendLine(">> Name: LGPRegInitInstruction_Standard");
            sb.AppendFormat(">> input copy count: {0}\n", mInputCopyCount);
            sb.AppendFormat(">> default register value: {0}", mDefaultRegisterValue);
            return sb.ToString();
        }
    }
}
