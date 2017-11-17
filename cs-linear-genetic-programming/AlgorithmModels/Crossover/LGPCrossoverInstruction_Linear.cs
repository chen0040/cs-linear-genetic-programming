using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Crossover
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    // Xianshun says:
    // this is derived from Algorithm 5.1 of Section 5.7.1 of Linear Genetic Programming
    // this linear crossover can also be considered as two-point crossover
    public class LGPCrossoverInstruction_Linear : LGPCrossoverInstruction
    {
        private int mMaxProgramLength;
        private int mMinProgramLength;
        private int mMaxSegmentLength;
        private int mMaxDistanceOfCrossoverPoints;
        private int mMaxDifferenceOfSegmentLength;

        public LGPCrossoverInstruction_Linear()
            : base()
        {
            mMaxProgramLength = 100;
            mMinProgramLength = 20;
            mMaxSegmentLength = 10;
            mMaxDistanceOfCrossoverPoints = 10;
            mMaxDifferenceOfSegmentLength = 5;
        }

        public LGPCrossoverInstruction_Linear(XmlElement xml_level1)
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
                    else if (attrname == "max_segment_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMaxSegmentLength = value;
                    }
                    else if (attrname == "max_distance_of_crossover_points") 
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMaxDistanceOfCrossoverPoints = value;
                    }
                    else if (attrname == "max_difference_in_segment_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMaxDifferenceOfSegmentLength = value;
                    }
                }
            }
        }

        public override LGPCrossoverInstruction Clone()
        {
            LGPCrossoverInstruction_Linear clone = new LGPCrossoverInstruction_Linear();
            clone.mMaxDifferenceOfSegmentLength = mMaxDifferenceOfSegmentLength;
            clone.mMaxDistanceOfCrossoverPoints = mMaxDistanceOfCrossoverPoints;
            clone.mMaxProgramLength = mMaxProgramLength;
            clone.mMaxSegmentLength = mMaxSegmentLength;
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

            // select i1 from gp1 and i2 from gp2 such that abs(i1-i2) <= max_crossover_point_distance
            // max_crossover_point_distance=min{length(gp1) - 1, m_max_distance_of_crossover_points}
            int i1 = DistributionModel.NextInt(gp1.InstructionCount);
            int i2 = DistributionModel.NextInt(gp2.InstructionCount);
            int cross_point_distance = (i1 > i2) ? (i1 - i2) : (i2 - i1);
            int max_crossover_point_distance = (gp1.InstructionCount - 1 > mMaxDistanceOfCrossoverPoints ? mMaxDistanceOfCrossoverPoints : gp1.InstructionCount - 1);
            while (cross_point_distance > max_crossover_point_distance)
            {
                i1 = DistributionModel.NextInt(gp1.InstructionCount);
                i2 = DistributionModel.NextInt(gp2.InstructionCount);
                cross_point_distance = (i1 > i2) ? (i1 - i2) : (i2 - i1);
            }

            int s1_max = (gp1.InstructionCount - i1) > mMaxDifferenceOfSegmentLength ? mMaxDifferenceOfSegmentLength : (gp1.InstructionCount - i1);
            int s2_max = (gp2.InstructionCount - i2) > mMaxDifferenceOfSegmentLength ? mMaxDifferenceOfSegmentLength : (gp2.InstructionCount - i2);

            // select s1 from gp1 (start at i1) and s2 from gp2 (start at i2)
            // such that length(s1) <= length(s2)
            // and abs(length(s1) - length(s2)) <= m_max_difference_of_segment_length)
            int ls1 = 1 + DistributionModel.NextInt(s1_max);
            int ls2 = 1 + DistributionModel.NextInt(s2_max);
            int lsd = (ls1 > ls2) ? (ls1 - ls2) : (ls2 - ls1);
            while ((ls1 > ls2) && (lsd > mMaxDifferenceOfSegmentLength))
            {
                ls1 = 1 + DistributionModel.NextInt(s1_max);
                ls2 = 1 + DistributionModel.NextInt(s2_max);
                lsd = (ls1 > ls2) ? (ls1 - ls2) : (ls2 - ls1);
            }

            if(((gp2.InstructionCount - (ls2-ls1)) < mMinProgramLength || ((gp1.InstructionCount+(ls2-ls1)) > mMaxProgramLength)))
            {
                if(DistributionModel.GetUniform()<0.5)
                {
                    ls2=ls1;
                }
                else
                {
                    ls1=ls2;
                }
                if((i1+ls1) > gp1.InstructionCount)
                {
                    ls1=ls2=gp1.InstructionCount-1;
                }
            }

            List<LGPInstruction> instructions1=gp1.Instructions;
            List<LGPInstruction> instructions2=gp2.Instructions;

            List<LGPInstruction> instructions1_1=new List<LGPInstruction>();
            List<LGPInstruction> instructions1_2=new List<LGPInstruction>();
            List<LGPInstruction> instructions1_3=new List<LGPInstruction>();

            List<LGPInstruction> instructions2_1=new List<LGPInstruction>();
            List<LGPInstruction> instructions2_2=new List<LGPInstruction>();
            List<LGPInstruction> instructions2_3=new List<LGPInstruction>();

            for(int i=0; i < i1; ++i)
            {
                instructions1_1.Add(instructions1[i]);
            }
            for(int i=i1; i < i1+ls1; ++i)
            {
                instructions1_2.Add(instructions1[i]);
            }
            for(int i=i1+ls1; i < instructions1.Count; ++i)
            {
                 instructions1_3.Add(instructions1[i]);
            }

            for(int i=0; i < i2; ++i)
            {
                instructions2_1.Add(instructions2[i]);
            }
            for(int i=i2; i < i2+ls2; ++i)
            {
                instructions2_2.Add(instructions2[i]);
            }
            for(int i=i2+ls2; i < instructions2.Count; ++i)
            {
                 instructions2_3.Add(instructions2[i]);
            }

            instructions1.Clear();
            instructions2.Clear();

            for(int i=0; i < i1; ++i)
            {
                instructions1.Add(instructions1_1[i]);
            }
            for(int i=0; i < ls2; ++i)
            {
                instructions1.Add(instructions2_2[i]);
                instructions2_2[i].Program=gp1;
            }
            for(int i=0; i < instructions1_3.Count; ++i)
            {
                instructions1.Add(instructions1_3[i]);
            }

            for(int i=0; i < i2; ++i)
            {
                instructions2.Add(instructions2_1[i]);
            }
            for(int i=0; i < ls1; ++i)
            {
                instructions2.Add(instructions1_2[i]);
                instructions1_2[i].Program=gp2;
            }
            for(int i=0; i < instructions2_3.Count; ++i)
            {
                instructions2.Add(instructions2_3[i]);
            }

            gp1.TrashFitness();
            gp2.TrashFitness();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(">> Name: LGPCrossoverInstruction_Linear\n");
            sb.AppendFormat(">> Max Program Length: {0}\n", mMaxProgramLength);
            sb.AppendFormat(">> Min Program Length: {0}\n", mMinProgramLength);
            sb.AppendFormat(">> Max Distance of Crossover Points: {0}\n", mMaxDistanceOfCrossoverPoints);
            sb.AppendFormat(">> Max Difference in Segment Length: {0}", mMaxDifferenceOfSegmentLength);

            return sb.ToString();
        }
    }
}
