using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.SpiralClassification
{
    using System.IO;
    using System.Data;
    using LinearGP.ComponentModels;
    using LinearGP.ComponentModels.Operators;
    
    class Program
    {
        static DataTable LoadData(string filename)
        {
            DataTable table = new DataTable();
            table.Columns.Add("X");
            table.Columns.Add("Y");
            table.Columns.Add("Label");

            int line_count = 0;
            using (StreamReader reader = new StreamReader(filename))
            {
                string line=reader.ReadLine();
                int.TryParse(line, out line_count);

                while ((line = reader.ReadLine()) != null)
                {
                    string[] elements=line.Split(new char[]{'\t'});
                   
                    double x, y;
                    int label;
                    double.TryParse(elements[0].Trim(), out x);
                    double.TryParse(elements[1].Trim(), out y);
                    int.TryParse(elements[2].Trim(), out label);

                    table.Rows.Add(x, y, label);
                }
            }
            return table;
        }
        static void Main(string[] args)
        {
            DataTable table = LoadData("dataset.txt");

            LGPConfig config=new LGPConfig();

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
                    SpiralFitnessCase fitness_case = new SpiralFitnessCase();
                    fitness_case.X=double.Parse(table.Rows[index]["X"].ToString());
                    fitness_case.Y=double.Parse(table.Rows[index]["Y"].ToString());
                    fitness_case.Label = int.Parse(table.Rows[index]["Label"].ToString());

                    return fitness_case;
                };

            pop.GetFitnessCaseCount += () =>
                {
                    return table.Rows.Count;
                };

            pop.EvaluateCostFromAllCases += (fitness_cases) =>
            {
                double fitness = 0;
                for (int i = 0; i < fitness_cases.Count; i++)
                {
                    SpiralFitnessCase fitness_case=(SpiralFitnessCase)fitness_cases[i];
                    int correct_y = fitness_case.Label;
                    int computed_y = fitness_case.ComputedLabel;
                    fitness += (correct_y == computed_y) ? 0 : 1;
                }

                return fitness;
            };


            pop.BreedInitialPopulation();
           

            while (!pop.IsTerminated)
            {
                pop.Evolve();
                Console.WriteLine("Spiral Classification Generation: {0}", pop.CurrentGeneration);
                Console.WriteLine("Global Fitness: {0}\tCurrent Fitness: {1}", pop.GlobalBestProgram.Fitness, pop.FindFittestProgramInCurrentGeneration().Fitness);
            }

            Console.WriteLine(pop.GlobalBestProgram.ToString());

        }
    }
}
