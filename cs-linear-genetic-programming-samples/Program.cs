using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.SymbolicRegression
{
    using System.IO;
    using System.Data;
    using LinearGP.ComponentModels;
    using LinearGP.ComponentModels.Operators;

    class Program
    {
        static double FunctionXY(double x)
        {
            return x * x + x + 1;
        }

        static DataTable LoadData()
        {
            DataTable table = new DataTable();
            table.Columns.Add("X");
            table.Columns.Add("Y");

            double lower_bound=-10;
	        double upper_bound=10;
	        int period=100;

	        double interval=(upper_bound - lower_bound) / period;

	        for(int i=0; i<period; i++)
	        {
		        double x=lower_bound + interval * i;
		        table.Rows.Add(x, FunctionXY(x));
	        }
            

            return table;
        }
        static void Main(string[] args)
        {
            DataTable table = LoadData();

            LGPConfig config = new LGPConfig();

            LGPPop pop = new LGPPop(config);
            pop.OperatorSet.AddOperator(new LGPOperator_Plus());
            pop.OperatorSet.AddOperator(new LGPOperator_Minus());
            pop.OperatorSet.AddOperator(new LGPOperator_Division());
            pop.OperatorSet.AddOperator(new LGPOperator_Multiplication());
            pop.OperatorSet.AddOperator(new LGPOperator_Sin());
            pop.OperatorSet.AddOperator(new LGPOperator_Cos());
            pop.OperatorSet.AddIfgtOperator();

            pop.CreateFitnessCase += (index) =>
            {
                SymRegFitnessCase fitness_case = new SymRegFitnessCase();
                fitness_case.X = double.Parse(table.Rows[index]["X"].ToString());
                fitness_case.Y = double.Parse(table.Rows[index]["Y"].ToString());
                

                return fitness_case;
            };

            pop.GetFitnessCaseCount += () =>
            {
                return table.Rows.Count;
            };

            pop.EvaluateFitnessFromAllCases += (fitness_cases) =>
            {
                double fitness = 0;
                for (int i = 0; i < fitness_cases.Count; i++)
                {
                    SymRegFitnessCase fitness_case = (SymRegFitnessCase)fitness_cases[i];
                    double correct_y = fitness_case.Y;
                    double computed_y = fitness_case.ComputedY;
                    fitness += (correct_y - computed_y) * (correct_y - computed_y);
                }

                return fitness;
            };


            pop.BreedInitialPopulation();


            while (!pop.IsTerminated)
            {
                pop.Evolve();
                Console.WriteLine("Symbolic Regression Generation: {0}", pop.CurrentGeneration);
                Console.WriteLine("Global Fitness: {0}\tCurrent Fitness: {1}", pop.GlobalBestProgram.Fitness.ToString("0.000"), pop.FindFittestProgramInCurrentGeneration().Fitness.ToString("0.000"));
            }

            Console.WriteLine(pop.GlobalBestProgram.ToString());

        }
    }
}
