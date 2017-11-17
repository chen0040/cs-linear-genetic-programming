using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Crossover
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    /* Xianshun says:
       This operator is derived from Algorithm 5.2 in Section 5.7.2 of Linear Genetic Programming
    */
    public class LGPCrossoverInstruction_OnePoint : LGPCrossoverInstruction
    {
        private int mMaxProgramLength;
        private int mMinProgramLength;
        private int mMaxDistanceOfCrossoverPoints;

        public LGPCrossoverInstruction_OnePoint()
            : base()
        {
            mMaxProgramLength = 100;
            mMinProgramLength = 1;
            mMaxDistanceOfCrossoverPoints = 1;
        }

        public LGPCrossoverInstruction_OnePoint(XmlElement xml_level1)
        {
            foreach(XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if(xml_level2.Name=="param")
                {
                    string attrname=xml_level2.Attributes["name"].Value;
                    string attrvalue=xml_level2.Attributes["value"].Value;
                    if(attrname=="max_program_length")
                    {
                        int value=0;
                        int.TryParse(attrvalue, out value);
                        mMaxProgramLength=value;
                    }
                    else if (attrname == "min_program_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMinProgramLength = value;
                    }
                    else if (attrname == "max_distance_of_crossover_points") 
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMaxDistanceOfCrossoverPoints = value;
                    }
                }
            }
        }

        public override LGPCrossoverInstruction Clone()
        {
            LGPCrossoverInstruction_OnePoint clone = new LGPCrossoverInstruction_OnePoint();
            clone.mMaxDistanceOfCrossoverPoints = mMaxDistanceOfCrossoverPoints;
            clone.mMaxProgramLength = mMaxProgramLength;
            clone.mMinProgramLength = mMinProgramLength;
            return clone;
        }

        public override void Crossover(LGPPop pop, LGPProgram child1, LGPProgram child2)
        {
            // Xianshun says:
            // this implementation is derived from Algorithm 5.1 in Section 5.7.1 of Linear
            // Genetic Programming

            LGPProgram gp1 = child1;
            LGPProgram gp2 = child2;

            // length(gp1) <= length(gp2)
            if (gp1.InstructionCount > gp2.InstructionCount)
            {
                gp1 = child2;
                gp2 = child1;
            }

            int max_distance_of_crossover_points = (gp1.InstructionCount - 1) < mMaxDistanceOfCrossoverPoints ? mMaxDistanceOfCrossoverPoints : (gp1.InstructionCount - 1);

            int i1 = DistributionModel.NextInt(gp1.InstructionCount);
            int i2 = DistributionModel.NextInt(gp2.InstructionCount);

            int crossover_point_distance = (i1 > i2) ? (i1 - i2) : (i2 - i1);

            int ls1 = gp1.InstructionCount - i1;
            int ls2 = gp2.InstructionCount - i2;

            // 1. assure abs(i1-i2) <= max_distance_of_crossover_points
	        // 2. assure l(s1) <= l(s2)
	        bool not_feasible=true;
            while (not_feasible)
            {
                not_feasible = false;
                // ensure that the maximum distance between two crossover points is not exceeded
                if (crossover_point_distance > max_distance_of_crossover_points)
                {
                    not_feasible = true;
                    i1 = DistributionModel.NextInt(gp1.InstructionCount);
                    i2 = DistributionModel.NextInt(gp2.InstructionCount);
                    crossover_point_distance = (i1 > i2) ? (i1 - i2) : (i2 - i1);
                }
                else
                {
                    ls1 = gp1.InstructionCount - i1;
                    ls2 = gp2.InstructionCount - i2;
                    // assure than l(s1) <= l(s2)
                    if (ls1 > ls2)
                    {
                        not_feasible = true;
                        i1 = DistributionModel.NextInt(gp1.InstructionCount);
                        i2 = DistributionModel.NextInt(gp2.InstructionCount);
                        crossover_point_distance = (i1 > i2) ? (i1 - i2) : (i2 - i1);
                    }
                    else
                    {
                        // assure the length of the program after crossover do not exceed the maximum program length or below minimum program length
                        if ((gp2.InstructionCount - (ls2 - ls1)) < mMinProgramLength || (gp1.InstructionCount + (ls2 - ls1)) > mMaxProgramLength)
                        {
                            not_feasible = true;
                            // when the length constraint is not satisfied, make the segments to be exchanged the same length
                            if (gp1.InstructionCount >= gp2.InstructionCount)
                            {
                                i1 = i2;
                            }
                            else
                            {
                                i2 = i1;
                            }
                            crossover_point_distance = 0;
                        }
                        else
                        {
                            not_feasible = false;
                        }
                    }
                }

                List<LGPInstruction> instructions1 = gp1.Instructions;
                List<LGPInstruction> instructions2 = gp2.Instructions;

                List<LGPInstruction> instructions1_1 = new List<LGPInstruction>();
                List<LGPInstruction> instructions1_2 = new List<LGPInstruction>();

                List<LGPInstruction> instructions2_1 = new List<LGPInstruction>();
                List<LGPInstruction> instructions2_2 = new List<LGPInstruction>();

                for (int i = 0; i < i1; ++i)
                {
                    instructions1_1.Add(instructions1[i]);
                }
                for (int i = i1; i < instructions1.Count; ++i)
                {
                    instructions1_2.Add(instructions1[i]);
                }

                for (int i = 0; i < i2; ++i)
                {
                    instructions2_1.Add(instructions2[i]);
                }
                for (int i = i2; i < instructions2.Count; ++i)
                {
                    instructions2_2.Add(instructions2[i]);
                }

                instructions1.Clear();
                instructions2.Clear();

                for (int i = 0; i < i1; ++i)
                {
                    instructions1.Add(instructions1_1[i]);
                }
                for (int i = 0; i < instructions2_2.Count; ++i)
                {
                    instructions1.Add(instructions2_2[i]);
                    instructions2_2[i].Program = gp1;
                }

                for (int i = 0; i < i2; ++i)
                {
                    instructions2.Add(instructions2_1[i]);
                }

                for (int i = 0; i < instructions1_2.Count; ++i)
                {
                    instructions2.Add(instructions1_2[i]);
                    instructions1_2[i].Program = gp2;
                }

                gp1.TrashFitness();
                gp2.TrashFitness();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> Name: LGPCrossoverInstruction_Linear\n");
            sb.AppendFormat(">> Max Program Length: {0}\n", mMaxProgramLength);
            sb.AppendFormat(">> Min Program Length: {0}\n", mMinProgramLength);
            sb.AppendFormat(">> Max Distance of Crossover Points: {0}", mMaxDistanceOfCrossoverPoints);

            return sb.ToString();
        }
    }
}
