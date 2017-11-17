using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    using ProblemModels;

    public class LGPProgram
    {
        LGPPop mPop;

        private LGPOperatorSet mOperatorSet;
        private LGPRegisterSet mRegisterSet;
        private LGPConstantSet mConstantSet;
        private List<LGPInstruction> mInstructions = new List<LGPInstruction>();

        private bool mSetup = false;
        private double mFitness = 0;
        private double mObjectiveValue=0;
        private bool mIsFitnessValid = false;

        public LGPProgram(LGPPop pop, LGPOperatorSet operator_set)
        {
            mPop = pop;
            mOperatorSet = operator_set;
        }

        public List<LGPInstruction> Instructions
        {
            get { return mInstructions; }
        }

        public double Fitness
        {
            get { return mFitness; }
        }

        public double ObjectiveValue
        {
            get { return mObjectiveValue; }
        }

        public void Setup()
        {
            mRegisterSet = CreateRegisterSet();
            mConstantSet = CreateConstantSet();
            mSetup = true;
        }

        protected virtual LGPRegisterSet CreateRegisterSet()
        {
            LGPRegisterSet reg_set = new LGPRegisterSet();
            int reg_count = mPop.RegisterCount;
            for (int i = 0; i < reg_count; ++i)
            {
                reg_set.AddRegister(new LGPRegister());
            }
            return reg_set;
        }

        public virtual void Create(int size)
        {
            if (!mSetup)
            {
                Setup();
            }

            // Xianshun says:
            // In this method, the instruction created is not garanteed to be structurally effective
            for (int i = 0; i < size; ++i)
            {
                LGPInstruction instruction = new LGPInstruction(this);
                instruction.Create();
                mInstructions.Add(instruction);
            }

        }

        protected virtual LGPConstantSet CreateConstantSet()
        {
            LGPConstantSet constant_set = new LGPConstantSet();
            int reg_count = mPop.ConstantRegisterCount;
            for (int i = 0; i < reg_count; ++i)
            {
                constant_set.AddConstant(mPop.FindConstantRegisterValueByIndex(i), mPop.FindConstantRegisterWeightByIndex(i));
            }

            return constant_set;
        }

        public virtual void EvaluateFitness()
        {
            //if (InstructionCount == 0)
            //{
            //    throw new ArgumentNullException();
            //}

            MarkStructuralIntrons();

            if (mPop.ObjectiveEvaluator == null)
            {
                LGPEnvironment env = mPop.Environment;
                int fitness_case_count = env.GetFitnessCaseCount();
                List<LGPFitnessCase> cases = new List<LGPFitnessCase>();
                for (int i = 0; i < fitness_case_count; ++i)
                {
                    LGPFitnessCase fitness_case = env.CreateFitnessCase(i);
                    ExecuteOnFitnessCase(fitness_case);
                    cases.Add(fitness_case);
                }
                mObjectiveValue = EvaluateFitnessFromAllCases(cases);
            }
            else
            {
                mObjectiveValue = mPop.ObjectiveEvaluator(this);
            }

            if (mPop.IsMaximization)
            {
                mFitness = mObjectiveValue;
            }
            else
            {
                mFitness = -mObjectiveValue;
            }
            mIsFitnessValid = true;
        }

        

        public double EvaluateFitnessFromAllCases(List<LGPFitnessCase> cases)
        {
            return mPop._EvaluateFitnessFromAllCases(cases);
        }

        protected void MarkStructuralIntrons()
        {
            /*
           Source: Brameier, M 2004  On Linear Genetic Programming (thesis)

           Algorithm 3.1 (detection of structural introns)
           1. Let set R_eff always contain all registers that are effective at the current program
              position. R_eff := { r | r is output register }.
              Start at the last program instruction and move backwards.
           2. Mark the next preceding operation in program with:
               destination register r_dest element-of R_eff.
              If such an instruction is not found then go to 5.
           3. If the operation directly follows a branch or a sequence of branches then mark these
              instructions too. Otherwise remove r_dest from R_eff .
           4. Insert each source (operand) register r_op of newly marked instructions in R_eff
              if not already contained. Go to 2.
           5. Stop. All unmarked instructions are introns.
           */
            int instruction_count=mInstructions.Count;
            for (int i = instruction_count - 1; i >= 0; i--)
            {
                mInstructions[i].IsStructuralIntron = true;
            }

            HashSet<int> Reff = new HashSet<int>();
            int register_count = mPop.RegisterCount;
            for (int i = 0; i < register_count; ++i)
            {
                Reff.Add(i);
            }

            LGPInstruction current_instruction = null;
            LGPInstruction prev_instruction = null;  // prev_instruction is the last visited instruction from bottom up of the program 

            for (int i = instruction_count - 1; i >= 0; i--)
            {
                prev_instruction = current_instruction;
                current_instruction = mInstructions[i];
                // prev_instruction is not an structural intron and the current_instruction
                // is a condictional construct then, the current_instruction is not structural intron either
                // this directly follows from Step 3 of Algorithm 3.1
                if (current_instruction.IsConditionalConstruct && prev_instruction != null)
                {
                    if (!prev_instruction.IsStructuralIntron)
                    {
                        current_instruction.IsStructuralIntron = false;
                    }
                }
                else
                {
                    if (Reff.Contains(current_instruction.DestinationRegisterIndex))
                    {
                        current_instruction.IsStructuralIntron = false;
                        Reff.Remove(current_instruction.DestinationRegisterIndex);

                        if (!current_instruction.IsOperand1ConstantRegister)
                        {
                            Reff.Add(current_instruction.Operand1RegisterIndex);
                        }
                        if (!current_instruction.IsOperand2ConstantRegister)
                        {
                            Reff.Add(current_instruction.Operand2RegisterIndex);
                        }
                    }
                }
            }
        }

        public void MarkStructuralIntrons(int stop_point, HashSet<int> Reff)
        {
            /*
           Source: Brameier, M 2004  On Linear Genetic Programming (thesis)

           Algorithm 3.1 (detection of structural introns)
           1. Let set R_eff always contain all registers that are effective at the current program
              position. R_eff := { r | r is output register }.
              Start at the last program instruction and move backwards.
           2. Mark the next preceding operation in program with:
               destination register r_dest element-of R_eff.
              If such an instruction is not found then go to 5.
           3. If the operation directly follows a branch or a sequence of branches then mark these
              instructions too. Otherwise remove r_dest from R_eff .
           4. Insert each source (operand) register r_op of newly marked instructions in R_eff
              if not already contained. Go to 2.
           5. Stop. All unmarked instructions are introns.
           */

            // Xianshun says:
            // this is a variant of Algorithm 3.1 that run Algorithm 3.1 until stop_point and return the Reff at that stage

            int instruction_count = mInstructions.Count;
            for (int i = instruction_count - 1; i > stop_point; i--)
            {
                mInstructions[i].IsStructuralIntron = true;
            }

            Reff.Clear();
            int register_count = mPop.RegisterCount;
            for (int i = 0; i < register_count; ++i)
            {
                Reff.Add(i);
            }

            LGPInstruction current_instruction = null;
            LGPInstruction prev_instruction = null;
            for (int i = instruction_count - 1; i > stop_point; i--)
            {
                prev_instruction = current_instruction;
                current_instruction = mInstructions[i];
                // prev_instruction is not an structural intron and the current_instruction
                // is a condictional construct then, the current_instruction is not structural intron either
                // this directly follows from Step 3 of Algorithm 3.1
                if (current_instruction.IsConditionalConstruct && prev_instruction != null)
                {
                    if (!prev_instruction.IsStructuralIntron)
                    {
                        current_instruction.IsStructuralIntron = false;
                    }
                }
                else
                {
                    if (Reff.Contains(current_instruction.DestinationRegisterIndex))
                    {
                        current_instruction.IsStructuralIntron = false;
                        Reff.Remove(current_instruction.DestinationRegisterIndex);

                        if (!current_instruction.IsOperand1ConstantRegister)
                        {
                            Reff.Add(current_instruction.Operand1RegisterIndex);
                        }
                        if (!current_instruction.IsOperand2ConstantRegister)
                        {
                            Reff.Add(current_instruction.Operand2RegisterIndex);
                        }
                    }
                }
            }
        }

        protected void ExecuteOnFitnessCase(LGPFitnessCase fitness_case)
        {
            mPop.InitializeProgramRegisters(this, fitness_case);

            LGPOperator.OperatorExecutionStatus command = LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
            LGPInstruction current_effective_instruction = null;
            LGPInstruction prev_effective_instruction = null;
            foreach (LGPInstruction instruction in mInstructions)
            {
                if (instruction.IsStructuralIntron)
                {
                    continue;
                }
                prev_effective_instruction = current_effective_instruction;
                current_effective_instruction = instruction;
                if (command == LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION)
                {
                    command = current_effective_instruction.Execute();
                    fitness_case.ReportProgress(instruction.Operator, instruction.Operand1, instruction.Operand2, instruction.DestinationRegister, RegisterSet);
                }
                else
                {
                    // Xianshun says:
                    // as suggested in Linear Genetic Programming
                    // the condictional construct is restricted to single condictional construct
                    // an example of single conditional construct would be
                    // line 1: if(register[a])
                    // line 2: <action1>
                    // line 3: <action2>
                    // if register[a]==true, then <action1> and <action2> are executed
                    // if register[a]==false, then <action1> is skipped and <action2> is executed
                    // <action1> and <action2> are restricted to effective instruction
                    if (prev_effective_instruction.IsConditionalConstruct)
                    {
                        command = LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
                    }
                }
            }

            double[] outputs = new double[mRegisterSet.RegisterCount];
            for (int i = 0; i < outputs.Length; ++i)
            {
                outputs[i] = mRegisterSet.FindRegisterByIndex(i).Value;
            }
            fitness_case.RunLGPProgramCompleted(outputs);
        }

        public virtual double[] Execute(double [] register_values)
        {
            for (int i = 0; i < mRegisterSet.RegisterCount; ++i)
            {
                mRegisterSet.FindRegisterByIndex(i).Value = register_values[i % register_values.Length];
            }

            LGPOperator.OperatorExecutionStatus command = LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
            LGPInstruction current_effective_instruction = null;
            LGPInstruction prev_effective_instruction = null;
            foreach (LGPInstruction instruction in mInstructions)
            {
                if (instruction.IsStructuralIntron)
                {
                    continue;
                }
                prev_effective_instruction = current_effective_instruction;
                current_effective_instruction = instruction;
                if (command == LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION)
                {
                    command = current_effective_instruction.Execute();
                    
                }
                else
                {
                    // Xianshun says:
                    // as suggested in Linear Genetic Programming
                    // the condictional construct is restricted to single condictional construct
                    // an example of single conditional construct would be
                    // line 1: if(register[a])
                    // line 2: <action1>
                    // line 3: <action2>
                    // if register[a]==true, then <action1> and <action2> are executed
                    // if register[a]==false, then <action1> is skipped and <action2> is executed
                    // <action1> and <action2> are restricted to effective instruction
                    if (prev_effective_instruction.IsConditionalConstruct)
                    {
                        command = LGPOperator.OperatorExecutionStatus.LGP_EXECUTE_NEXT_INSTRUCTION;
                    }
                }
            }

            double[] outputs = new double[mRegisterSet.RegisterCount];
            for (int i = 0; i < outputs.Length; ++i)
            {
                outputs[i] = mRegisterSet.FindRegisterByIndex(i).Value;
            }
            return outputs;
        }

        public int InstructionCount
        {
            get { return mInstructions.Count; }
        }

        /// <summary>
        /// this means the number of structurally effective instructions in the linear program
        /// </summary>
        public int EffectiveInstructionCount
        {
            get
            {
                int count = 0;
                foreach (LGPInstruction instruction in mInstructions)
                {
                    if (!instruction.IsStructuralIntron)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public double MicroMutateOperatorRate
        {
            get
            {
                return mPop.MicroMutateOperatorRate;
            }
        }

        public double MicroMutateRegisterRate
        {
            get { return mPop.MicroMutateRegisterRate; }
        }

        public double MicroMutateConstantRate
        {
            get { return mPop.MicroMutateConstantRate; }
        }

        /// <summary>
        /// the micro-mutation is derived from Linear Genetic Programming 2004 chapter 6 section 6.2.2
        /// three type selection probability are first determined and roulette wheel is used to decide which
        /// mutation type is to be performed
        /// 1. if micro-mutate-operator type is selected, then randomly pick an instruction and
        /// randomly select an instruction and mutate its operator to some other operator from the operator set
        /// 2. if micro-mutate-register type is selected, then randomly pick an instruction and 
        /// randomly select one of the two operands, then
        /// 2.1 with a constant selection probability p_{const}, a randomly selected constant register is assigned to the selected operand
        /// 2.2 with probability 1-p_{const}, a randomly selected variable register is assigned to the selected operand
        /// p_{const} is the proportion of instruction that holds a constant value.
        /// 3. if micro-mutate-constant type is selected, then randomly pick an effective instruction with a constant as one
        /// of its register value, mutate the constant to c+$N(0, \omega_{\mu}$
        /// </summary>
        public void MicroMutate(Statistics.Gaussian gauss)
        {
            double micro_mutate_operator_rate = MicroMutateOperatorRate;
            double micro_mutate_register_rate = MicroMutateRegisterRate;
            double micro_mutate_constant_rate = MicroMutateConstantRate;
            double operator_sector = micro_mutate_operator_rate;
            double register_sector = operator_sector + micro_mutate_register_rate;
            
            double r=Statistics.DistributionModel.GetUniform();
            if (r < operator_sector)
            {
                MutateInstructionOperator();
            }
            else if (r < register_sector)
            {
                MutateInstructionRegister();
            }
            else
            {
                MutateInstructionConstant(gauss);
            }

            TrashFitness();
        }

        public void TrashFitness()
        {
            mIsFitnessValid = false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mInstructions.Count; ++i)
            {
                sb.AppendFormat("instruction[{0}]: {1}\n", i, mInstructions[i]);
            }
            if (mIsFitnessValid)
            {
                sb.AppendFormat("objective_value: {0}\n", mObjectiveValue);
                sb.AppendFormat("fitness: {0}", mFitness);
            }
            else
            {
                sb.Append("Invalid Fitness");
            }
            return sb.ToString();
        }

        public void MutateInstructionOperator()
        {
            LGPInstruction instruction = mInstructions[Statistics.DistributionModel.NextInt(InstructionCount)];
            instruction.MutateOperator();
        }

        public void MutateInstructionRegister()
        {
            LGPInstruction selected_instruction = mInstructions[Statistics.DistributionModel.NextInt(InstructionCount)];
            double p_const = 0;
            foreach (LGPInstruction instruction in mInstructions)
            {
                if (instruction.IsOperand1ConstantRegister || instruction.IsOperand2ConstantRegister)
                {
                    p_const += 1.0;
                }
            }
            p_const /= InstructionCount;
            selected_instruction.MutateRegister(p_const);
        }

        /// <summary>
        /// this is derived from the micro mutation implementation in section
        /// 6.2.2 of Linear Genetic Programming
        /// 1. randomly select an (effective) instruction with a constant c
        /// 2. change constant c through a standard deviation from the current value
        /// c:=c + normal(mean:=0, standard_deviation)
        /// </summary>
        public void MutateInstructionConstant(Statistics.Gaussian guass)
        {
            LGPInstruction selected_instruction = null;
            foreach (LGPInstruction instruction in mInstructions)
            {
                if (!instruction.IsStructuralIntron && (instruction.IsOperand1ConstantRegister || instruction.IsOperand2ConstantRegister))
                {
                    if (selected_instruction == null)
                    {
                        selected_instruction = instruction;
                    }
                    else if (Statistics.DistributionModel.GetUniform() < 0.5)
                    {
                        selected_instruction = instruction;
                    }
                }
                
            }
            if (selected_instruction != null)
            {
                selected_instruction.MutateConstant(guass, MicroMutateConstantStandardDeviation);
            }
        }

        public LGPOperatorSet OperatorSet
        {
            get { return mOperatorSet; }
        }

        public LGPConstantSet ConstantSet
        {
            get { return mConstantSet; }
        }

        public LGPRegisterSet RegisterSet
        {
            get { return mRegisterSet; }
        }

        public virtual LGPProgram Clone()
        {
            LGPProgram clone = new LGPProgram(mPop, mOperatorSet.Clone());
            clone.Copy(this);
            return clone;
        }

        public void Copy(LGPProgram rhs)
        {
            mSetup = rhs.mSetup;
            mFitness = rhs.mFitness;
            mObjectiveValue = rhs.mObjectiveValue;
            mIsFitnessValid = rhs.mIsFitnessValid;

            mRegisterSet = rhs.mRegisterSet.Clone();
            mOperatorSet = rhs.mOperatorSet.Clone();
            mConstantSet = rhs.mConstantSet.Clone();

            mPop = rhs.mPop;

            for (int i = 0; i < rhs.mInstructions.Count; ++i)
            {
                mInstructions.Add(rhs.mInstructions[i].Clone());
                mInstructions[i].Program = this;
            }
        }

        public double MicroMutateConstantStandardDeviation
        {
            get { return mPop.MicroMutateConstantStandardDeviation;  }
        }

        public bool IsBetterThan(LGPProgram rhs)
        {
            return mFitness > rhs.Fitness;
        }

        public bool IsFitnessValid
        {
            get
            {
                return mIsFitnessValid;
            }
        }
    }
}
