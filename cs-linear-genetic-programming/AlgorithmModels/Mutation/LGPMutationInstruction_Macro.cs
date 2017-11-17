using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Mutation
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    public class LGPMutationInstruction_Macro : LGPMutationInstruction
    {
        double mMacroMutateInsertionRate;
        double mMacroMutateDeletionRate;
        int mMacroMutateMinProgramLength;
        int mMacroMutateMaxProgramLength;
        bool mEffectiveMutation;

        public LGPMutationInstruction_Macro()
        {
            mMacroMutateInsertionRate = (0.5);
            mMacroMutateDeletionRate=(0.5);
            mEffectiveMutation=(false);
            mMacroMutateMaxProgramLength=(100);
            mMacroMutateMinProgramLength=(20);
        }

        public LGPMutationInstruction_Macro(XmlElement xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "insertion_mutation_probability")
                    {
                        double value = 0;
                        double.TryParse(attrvalue, out value);
                        mMacroMutateInsertionRate = value;
                    }
                    else if (attrname == "deletion_mutation_probability")
                    {
                        double value = 0;
                        double.TryParse(attrvalue, out value);
                        mMacroMutateDeletionRate = value;
                    }
                    else if (attrname == "min_program_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMacroMutateMinProgramLength = value;
                    }
                    else if (attrname == "max_program_length")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mMacroMutateMaxProgramLength = value;
                    }
                    else if (attrname == "effective_mutation")
                    {
                        bool value = false;
                        bool.TryParse(attrvalue, out value);
                        mEffectiveMutation = value;
                    }
                }
            }

            mMacroMutateInsertionRate /= (mMacroMutateInsertionRate + mMacroMutateDeletionRate);
            mMacroMutateDeletionRate = 1 - mMacroMutateInsertionRate;
        }

        public override void Mutate(LGPPop lgpPop, LGPProgram child)
        {
            // Xianshun says:
	        // This is derived from Algorithm 6.1 (Section 6.2.1) of Linear Genetic Programming
	        // Macro instruction mutations either insert or delete a single instruction.
	        // In doing so, they change absolute program length with minimum step size on the 
	        // level of full instructions, the macro level. On the functional level , a single 
	        // node is inserted in or deleted from the program graph, together with all 
	        // its connecting edges.
	        // Exchanging an instruction or change the position of an existing instruction is not 
	        // regarded as macro mutation. Both of these variants are on average more 
	        // destructive, i.e. they imply a larger variation step size, since they include a deletion
	        // and an insertion at the same time. A further, but important argument against 
	        // substitutios of single instructions is that these do not vary program length. If
	        // single instruction would only be exchanged there would be no code growth.

	        double r=DistributionModel.GetUniform();
	        List<LGPInstruction> instructions=child.Instructions;
	        if(child.InstructionCount < mMacroMutateMaxProgramLength && ((r < mMacroMutateInsertionRate)  || child.InstructionCount == mMacroMutateMinProgramLength))
	        {
		        LGPInstruction inserted_instruction=new LGPInstruction(child);
		        inserted_instruction.Create();
		        int loc=DistributionModel.NextInt(child.InstructionCount);
		        if(loc==child.InstructionCount - 1)
		        {
			        instructions.Add(inserted_instruction);
		        }
		        else
		        {
			        instructions.Insert(loc, inserted_instruction);
		        }

		        if(mEffectiveMutation)
		        {
			        while(instructions[loc].IsConditionalConstruct && loc < instructions.Count)
			        {
				        loc++;
			        }
			        if(loc < instructions.Count)
			        {
				        HashSet<int> Reff=new HashSet<int>();
				        child.MarkStructuralIntrons(loc, Reff);
				        if(Reff.Count > 0)
				        {
                            int iRegisterIndex=-1;
					        foreach(int Reff_value in Reff)
					        {
						        if(iRegisterIndex==-1)
						        {
							        iRegisterIndex=Reff_value;
						        }
						        else if(DistributionModel.GetUniform() < 0.5)
						        {
							        iRegisterIndex=Reff_value;
						        }
					        }
					        instructions[loc].DestinationRegister=child.RegisterSet.FindRegisterByIndex(iRegisterIndex);
				        }
			        }
		        }
	        }
	        else if(child.InstructionCount > mMacroMutateMinProgramLength && ((r > mMacroMutateInsertionRate) || child.InstructionCount == mMacroMutateMaxProgramLength))
	        {
		        int loc=DistributionModel.NextInt(instructions.Count);
		        if(mEffectiveMutation)
		        {
			        for(int i=0; i<10; i++)
			        {
				        loc=DistributionModel.NextInt(instructions.Count);
				        if(! instructions[loc].IsStructuralIntron)
				        {
					        break;
				        }
			        }
		        }

		       
		        instructions.RemoveAt(loc);
	        }

	        child.TrashFitness();
        }

        public override LGPMutationInstruction Clone()
        {
            LGPMutationInstruction_Macro clone = new LGPMutationInstruction_Macro();
            clone.mMacroMutateInsertionRate = mMacroMutateInsertionRate;
            clone.mMacroMutateDeletionRate = mMacroMutateDeletionRate;
            clone.mMacroMutateMaxProgramLength = mMacroMutateMaxProgramLength;
            clone.mMacroMutateMinProgramLength = mMacroMutateMinProgramLength;
            clone.mEffectiveMutation = mEffectiveMutation;

            return clone;
        }
    }
}
