using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    using System.Xml;

    public class LGPConfig
    {
        private string mFilename;
        private int mRegisterCount=8;
        private int mPopulationSize = 30;
        private int mMaxGenerations = 100;
        private bool mIsMaximization = false;
        private double mMicroMutateConstantRate = 0.5;
        private double mMicroMutateRegisterRate = 0.5;
        private double mMicroMutateOperatorRate = 0.5;
        private double mMicroMutateConstantStandardDeviation = 1;
        private List<KeyValuePair<double, double>> mConstantRegisters=new List<KeyValuePair<double, double>>();
        private double mCrossoverRate = 0.5;
        private double mMacroMutationRate = 0.5;
        private double mMicroMutationRate = 0.5;
        private Dictionary<string, string> mScripts = new Dictionary<string, string>();

        public LGPConfig(string filename)
        {
            mFilename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement doc_root=doc.DocumentElement;

            foreach (XmlElement xml_level1 in doc_root.ChildNodes)
            {
                if (xml_level1.Name == "parameters")
                {
                    foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
                    {
                        if(xml_level2.Name=="param")
                        {
                            string attrname = xml_level2.Attributes["name"].Value;
                            string attrvalue = xml_level2.Attributes["value"].Value;
                            if (attrname == "NumRegisters")
                            {
                                int value=0;
                                int.TryParse(attrvalue, out value);
                                mRegisterCount = value;
                            }
                            else if (attrname == "PopulationSize")
                            {
                                int value = 0;
                                int.TryParse(attrvalue, out value);
                                mPopulationSize = value;
                            }
                            else if (attrname == "MaxGenerations")
                            {
                                int value = 0;
                                int.TryParse(attrvalue, out value);
                                mMaxGenerations = value;
                            }
                            else if (attrname == "Maximization")
                            {
                                bool value = false;
                                bool.TryParse(attrvalue, out value);
                                mIsMaximization = value;
                            }
                            else if (attrname == "CrossoverRate")
                            {
                                double value = 0;
                                double.TryParse(attrvalue, out value);
                                mCrossoverRate = value;
                            }
                            else if (attrname == "MacroMutationRate")
                            {
                                double value = 0;
                                double.TryParse(attrvalue, out value);
                                mMacroMutationRate = value;
                            }
                            else if (attrname == "MicroMutationRate")
                            {
                                double value = 0;
                                double.TryParse(attrvalue, out value);
                                mMicroMutationRate = value;
                            }
                        }
                    }
                }
                else if (xml_level1.Name == "micro_mutation")
                {
                    foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
                    {
                        if (xml_level2.Name == "mutation_type_probability")
                        {
                            double.TryParse(xml_level2.Attributes["operator_mutation"].Value, out mMicroMutateOperatorRate);
                            double.TryParse(xml_level2.Attributes["register_mutation"].Value, out mMicroMutateRegisterRate);
                            double.TryParse(xml_level2.Attributes["constant_mutation"].Value, out mMicroMutateConstantRate);
                            double sum = mMicroMutateConstantRate + mMicroMutateOperatorRate + mMicroMutateRegisterRate;
                            mMicroMutateRegisterRate /= sum;
                            mMicroMutateOperatorRate /= sum;
                            mMicroMutateConstantRate /= sum;
                        }
                        else if (xml_level2.Name == "constant_mutation")
                        {
                            double.TryParse(xml_level2.Attributes["standard_deviation"].Value, out mMicroMutateConstantStandardDeviation);
                        }
                    }
                }
                else if (xml_level1.Name == "constant_reg")
                {
                    foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
                    {
                        if(xml_level2.Name=="constant")
                        {
                            double value, weight;
                            double.TryParse(xml_level2.Attributes["value"].Value, out value);
                            double.TryParse(xml_level2.Attributes["weight"].Value, out weight);
                            mConstantRegisters.Add(new KeyValuePair<double, double>(value, weight));
                        }
                    }
                }
                else if (xml_level1.Name == "lgp_scripts")
                {
                    foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
                    {
                        if (xml_level2.Name == "script")
                        {
                            string script_name = xml_level2.Attributes["name"].Value;
                            string script_src = xml_level2.Attributes["src"].Value;
                            mScripts[script_name] = script_src;
                        }
                    }
                }
            }

        }

        public double MicroMutateConstantRate { get { return mMicroMutateConstantRate; } }

        public double MicroMutateRegisterRate { get { return mMicroMutateRegisterRate; } }

        public double MicroMutateOperatorRate { get { return mMicroMutateOperatorRate; } }

        public double MicroMutateConstantStandardDeviation { get { return mMicroMutateConstantStandardDeviation; } }

        public int RegisterCount { get { return mRegisterCount; } }

        public int ConstantRegisterCount { get { return mConstantRegisters.Count; } }

        internal double FindConstantRegisterValueByIndex(int i)
        {
            return mConstantRegisters[i].Key;
        }

        internal double FindConstantRegisterWeightByIndex(int i)
        {
            return mConstantRegisters[i].Value;
        }

        public bool IsMaximization { get { return mIsMaximization; } }

        public int PopulationSize { get { return mPopulationSize; } }

        public string GetScript(string p)
        {
            if (mScripts.ContainsKey(p))
            {
                return mScripts[p];
            }
            return null;
        }

        public int MaxGenerations { get { return mMaxGenerations; } }

        public double CrossoverRate { get { return mCrossoverRate; } }

        public double MacroMutationRate { get { return mMacroMutationRate; } }

        public double MicroMutationRate { get { return mMicroMutationRate; } }
    }
}
