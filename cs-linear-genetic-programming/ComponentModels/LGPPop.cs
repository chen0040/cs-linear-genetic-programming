using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.ComponentModels
{
    using ProblemModels;
    using AlgorithmModels.Crossover;
    using AlgorithmModels.Mutation;
    using AlgorithmModels.PopInit;
    using AlgorithmModels.RegInit;
    using AlgorithmModels.Selection;
    using AlgorithmModels.Survival;
    using Statistics;

    /* Xianshun says:
     Algorithm 2.1 (LGP algorithm)
     1. Initialize the population with random programs and calculate their fitness value
     2. Randomly select 2  nt individuals from the pouplation without replacement
     3. Perform two fitness tournament of size nt
     4. Make temporary copies of the two tournament winners
     5. Modify the two winners by one or more variation operators with certain probabilities
     6. Evaluate the fitness of the two offspring
     7. If the current best-fit indivdiual is replaced by one of the spring validate the new best program using unknown data (only required for machine learning)  
     8. Reproduce the two tournament winnners within the population with a certain probability (This is the "Reproduction" Mechansim) by replacing the two tournament losers with the temporary copies of the winners under a certain condition (This is "Population Replacement" for steady-state LGP)
     9. Repeat Step 2 to 8 until maximum number of generations is reached.

     Note that the population size stay fixed
    */

    /* Xianshun says:
     In step 5 of Algorithm 2.1, three different variation mechanism can be used to modify the two temporary copies of the tournament winners:
      1. crossover 
      2. macro mutation
      3. micro mutation
    These three variation mechanism can be applied in combination or one at a time to modify the two temporary copies of the tournament winners.
    */

    /* Xianshun says:
     The current model is based on steady-state LGP
     Regarding what is meant by a "generation" in steady-state LGP, I quote a paragraph from "Genetic Programming" by Wolfgang Banzhaf, Peter Nordin, Robert E. Keller, Frank D. Francone
     The steady-state GP models do not have distinct generations. Steady-state GP maintains the illusion of generations 
     by stopping once very P fitness evaluations (where P is the populaton size) and measuring their online statistics. 
     This convention has proved effective and is used almost universally in steady-state GP systems. Here we will follow this convention. 
 
     The reader should recall, however, that a generaytion is no more than an agreed convention in steady-state models and any 
     online statistic could be calculated so as to reflect the reality of a steady-state system more precisely.

     Also stated in "Linear Genetic Programming": In a steady-state LGP, ... a generation (equivalent) is complete if the number of new individuals
     equals the population size.
     */
    public class LGPPop
    {
        private LGPConfig mConfig;
        private LGPEnvironment mEnvironment;
        private bool mSetup = false;
        private int mCurrentGeneration = 0;
        private LGPProgram mGlobalBestProgram = null;
        private List<LGPProgram> mPrograms=new List<LGPProgram>();

        private LGPMutationInstructionFactory mMutationInstructionFactory;
        private LGPCrossoverInstructionFactory mCrossoverInstructionFactory;
        private LGPPopInitInstructionFactory mPopInitInstructionFactory;
        private LGPSelectionInstructionFactory mReproductionSelectionInstructionFactory;
        private LGPRegInitInstructionFactory mRegInitInstructionFactory;
        private LGPSurvivalInstructionFactory mSurvivalInstructionFactory;

        private LGPOperatorSet mOperatorSet = new LGPOperatorSet();

        public delegate double EvaluateFitnessFromAllCasesHandle(List<LGPFitnessCase> cases);
        public event EvaluateFitnessFromAllCasesHandle EvaluateFitnessFromAllCases;

        public delegate double EvaluateObjectiveHandle(LGPProgram program);
        public EvaluateObjectiveHandle ObjectiveEvaluator = null;

        internal double _EvaluateFitnessFromAllCases(List<LGPFitnessCase> cases)
        {
            if (EvaluateFitnessFromAllCases != null)
            {
                return EvaluateFitnessFromAllCases(cases);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public delegate LGPFitnessCase CreateFitnessCaseHandle(int index);
        public event CreateFitnessCaseHandle CreateFitnessCase;

        public delegate int GetFitnessCaseCountHandle();
        public event GetFitnessCaseCountHandle GetFitnessCaseCount;

        private Gaussian mGaussian = new Gaussian();

        public LGPPop(LGPConfig config)
        {
            mConfig = config;
        }

        public double MicroMutateConstantRate
        {
            get
            {
                return mConfig.MicroMutateConstantRate;
            }
        }

        protected virtual void EvaluateFitness()
        {
            for (int i = 0; i < mPrograms.Count; i++)
            {
                if (!mPrograms[i].IsFitnessValid)
                {
                    mPrograms[i].EvaluateFitness();
                }
            }
        }

        public virtual bool IsTerminated
        {
            get
            {
                return mCurrentGeneration >= mConfig.MaxGenerations;
            }
        }

        public virtual void Evolve()
        {
            int iPopSize=this.mConfig.PopulationSize;
	        int program_count=0;
	        while(program_count < iPopSize)
	        {
		        KeyValuePair<LGPProgram, LGPProgram> tournament_winners=new KeyValuePair<LGPProgram, LGPProgram>();
		        KeyValuePair<LGPProgram, LGPProgram> tournament_losers=new KeyValuePair<LGPProgram, LGPProgram>();
		        mReproductionSelectionInstructionFactory.Select(this, ref tournament_winners, ref tournament_losers);
		
		        LGPProgram tp1=tournament_winners.Key.Clone();
		        LGPProgram tp2=tournament_winners.Value.Clone();

                double r=DistributionModel.GetUniform();
                if (r < this.mConfig.CrossoverRate)
                {
                    mCrossoverInstructionFactory.Crossover(this, tp1, tp2);
                }

                r = DistributionModel.GetUniform();
                if (r < this.mConfig.MacroMutationRate)
                {
                    mMutationInstructionFactory.Mutate(this, tp1);
                }

                r = DistributionModel.GetUniform();
                if (r < this.mConfig.MacroMutationRate)
                {
                    mMutationInstructionFactory.Mutate(this, tp2);
                }

		        r=DistributionModel.GetUniform();
		        if(r < this.mConfig.MicroMutationRate)
		        {
                    tp1.MicroMutate(mGaussian);
		        }
		
		        r=DistributionModel.GetUniform();
		        if(r < this.mConfig.MicroMutationRate)
		        {
                    tp2.MicroMutate(mGaussian);
		        }

		        if(! tp1.IsFitnessValid)
		        {
			        tp1.EvaluateFitness();
		        }
		        if(! tp2.IsFitnessValid)
		        {
			        tp2.EvaluateFitness();
		        }

		        if(tp1.IsBetterThan(tp2))
		        {
			        if(tp1.IsBetterThan(mGlobalBestProgram))
			        {
				        mGlobalBestProgram=tp1.Clone();
			        }
		        }
		        else
		        {
			        if(tp2.IsBetterThan(mGlobalBestProgram))
			        {
				        mGlobalBestProgram=tp2.Clone();
			        }
		        }

		        LGPProgram loser1=mSurvivalInstructionFactory.Compete(this, tournament_losers.Key, tp1); // this method returns the pointer to the loser in the competition for survival;
		        LGPProgram loser2=mSurvivalInstructionFactory.Compete(this, tournament_losers.Value, tp2);

		        if(loser1==tournament_losers.Key)
		        {
			        ++program_count;
		        }
		        if(loser2==tournament_losers.Value)
		        {
			        ++program_count;
		        }
	        }

           

	        mCurrentGeneration++;
        }

        public virtual void BreedInitialPopulation()
        {
	        Setup();

	        int iPopulationSize=mConfig.PopulationSize;

	        mPopInitInstructionFactory.Initialize(this);

	        EvaluateFitness();


	        mGlobalBestProgram=FindFittestProgramInCurrentGeneration().Clone();
        }

        public LGPProgram GlobalBestProgram
        {
            get { return mGlobalBestProgram; }
        }

        public override string ToString()
        {
	        if(mSetup==false)
	        {
		        return "Not setup yet";
	        }

	        StringBuilder sb=new StringBuilder();

	        sb.AppendLine("register initialization instruction: ");
	        sb.AppendLine(mRegInitInstructionFactory.ToString());
	        sb.AppendLine("pop init instruction: ");
	        sb.AppendLine(mPopInitInstructionFactory.ToString());
	        sb.AppendLine("macro mutation instruction: ");
	        sb.AppendLine(mMutationInstructionFactory.ToString());
	        sb.AppendLine("crossover instruction: ");
	        sb.AppendLine(mCrossoverInstructionFactory.ToString());
	        sb.AppendLine("reproduction seletion instruction: ");
	        sb.AppendLine(mReproductionSelectionInstructionFactory.ToString());
	        sb.AppendLine("survival instruction: ");
	        sb.AppendLine(mSurvivalInstructionFactory.ToString());

	        for(int i=0; i<mPrograms.Count; i++)
	        {
		        sb.AppendFormat("Program[{0}]\n{1}\n", i, mPrograms[i]);
	        }

            return sb.ToString();
        }

        protected virtual LGPEnvironment CreateEnvironment(LGPConfig lgpConfig)
        {
            LGPEnvironment environment = new LGPEnvironment(lgpConfig);
            environment.CreateFitnessCaseTriggered += (index) =>
                {
                    return CreateFitnessCase(index);
                };
            environment.GetFitnessCaseCountTriggered += () =>
                {
                    return GetFitnessCaseCount();
                };

            return environment;
        }
        
        protected virtual LGPMutationInstructionFactory CreateMutationInstructionFactory(string filename)
        {
            return new LGPMutationInstructionFactory(filename);
        }
        protected virtual LGPCrossoverInstructionFactory CreateCrossoverInstructionFactory(string filename)
        {
            return new LGPCrossoverInstructionFactory(filename);
        }
        protected virtual LGPPopInitInstructionFactory CreatePopInitInstructionFactory(string filename)
        {
            return new LGPPopInitInstructionFactory(filename);
        }
        protected virtual LGPSelectionInstructionFactory CreateReproductionSelectionInstructionFactory(string filename)
        {
            return new LGPSelectionInstructionFactory(filename);
        }
        protected virtual LGPRegInitInstructionFactory CreateRegInitInstructionFactory(string filename)
        {
            return new LGPRegInitInstructionFactory(filename);
        }
        protected virtual LGPSurvivalInstructionFactory CreateSurvivalInstructionFactory(string filename)
        {
            return new LGPSurvivalInstructionFactory(filename);
        }

        public virtual LGPProgram CreateProgram(int size, LGPEnvironment env)
        {
            LGPProgram program = new LGPProgram(this, mOperatorSet.Clone());
            program.Create(size);
            
            return program;
        }

        public LGPOperatorSet OperatorSet
        {
            get { return mOperatorSet; }
        }

        public double MicroMutateRegisterRate { get { return mConfig.MicroMutateRegisterRate; } }

        public double MicroMutateOperatorRate { get { return mConfig.MicroMutateOperatorRate; } }

        public double MicroMutateConstantStandardDeviation { get { return mConfig.MicroMutateConstantStandardDeviation; } }

        public int RegisterCount { get { return mConfig.RegisterCount; } }

        public int ConstantRegisterCount { get { return mConfig.ConstantRegisterCount; } }

        public double FindConstantRegisterValueByIndex(int i)
        {
            return mConfig.FindConstantRegisterValueByIndex(i);
        }

        public double FindConstantRegisterWeightByIndex(int i)
        {
            return mConfig.FindConstantRegisterWeightByIndex(i);
        }

        public LGPEnvironment Environment
        {
            get { return mEnvironment; }
        }

        public virtual void InitializeProgramRegisters(LGPProgram lgp, LGPFitnessCase fitness_case)
        {
            mRegInitInstructionFactory.InitializeRegisters(lgp.RegisterSet, lgp.ConstantSet, fitness_case);
        }

        public bool IsMaximization { get { return mConfig.IsMaximization; } }

        public int CurrentGeneration
        {
            get { return mCurrentGeneration; }
        }

        public int PopulationSize { get {return mConfig.PopulationSize; } }

        public void AddProgram(LGPProgram lgp)
        {
            mPrograms.Add(lgp);
        }

        public LGPProgram FindProgramByIndex(int i)
        {
            return mPrograms[i];
        }

        public int ProgramCount { get { return mPrograms.Count; } }

        public void RandomShuffle()
        {
            Shuffle(mPrograms);
        }

        public static void Shuffle(IList<LGPProgram> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                LGPProgram value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void SortFittestProgramFirst()
        {
            mPrograms=mPrograms.OrderByDescending(o => o.Fitness).ToList();
        }

        public void SortFittestProgramLast()
        {
            mPrograms = mPrograms.OrderBy(o => o.Fitness).ToList();
        }

        public LGPProgram FindFittestProgramInCurrentGeneration()
        {
            SortFittestProgramFirst();
            return mPrograms[0];
        }

        protected void Setup()
        {
            if (mSetup == false)
            {
                mEnvironment = CreateEnvironment(mConfig);
                mMutationInstructionFactory = CreateMutationInstructionFactory(mConfig.GetScript(ScriptNames.MutationInstructionFactory));
                mCrossoverInstructionFactory = CreateCrossoverInstructionFactory(mConfig.GetScript(ScriptNames.CrossoverInstructionFactory));
                mPopInitInstructionFactory = CreatePopInitInstructionFactory(mConfig.GetScript(ScriptNames.PopInitInstructionFactory));
                mReproductionSelectionInstructionFactory = CreateReproductionSelectionInstructionFactory(mConfig.GetScript(ScriptNames.ReproductionSelectionInstructionFactory));
                mRegInitInstructionFactory = CreateRegInitInstructionFactory(mConfig.GetScript(ScriptNames.RegInitInstructionFactory));
                mSurvivalInstructionFactory = CreateSurvivalInstructionFactory(mConfig.GetScript(ScriptNames.SurvivalInstructionFactory));
                mSetup = true;
            }
        }

        public void Replace(LGPProgram weak_program_in_current_pop, LGPProgram child_program)
        {
            //bool found = false;
            for (int i = 0; i < mPrograms.Count; ++i)
            {
                if (mPrograms[i] == weak_program_in_current_pop)
                {
                    //found = true;
                    mPrograms[i] = child_program;
                }
            }
            //if (found == false)
            //{
            //    throw new Exception();
            //}
        }
    }
}
