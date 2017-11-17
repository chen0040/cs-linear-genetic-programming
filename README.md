# cs-linear-genetic-programming

Linear Genetic Programming implemented in C#

# Install

```bash
Install-Package cs-statistics -Version 1.0.2
Install-Package cs-linear-genetic-programming -Version 1.0.1
```

# Usage

## Symbolic Regression (Mexican Hat)

The sample codes below show how to use the LinearGP to solve the Mexican Hat symbolic regression problem:

```cs 
class Program
{
	static double FunctionXY(double x1, double x2)
	{
	   return (1 - x1 * x1 / 4 - x2 * x2 / 4) * System.Math.Exp(- x1 * x2 / 8 - x2 * x2 / 8);
	}

	static DataTable LoadData()
	{
		DataTable table = new DataTable();
		table.Columns.Add("X1");
		table.Columns.Add("X2");
		table.Columns.Add("Y");

		double lower_bound=-4;
		double upper_bound=4;
		int period=16;

		double interval=(upper_bound - lower_bound) / period;

		for(int i=0; i<period; i++)
		{
			double x1=lower_bound + interval * i;
			for(int j=0; j<period; j++)
			{
				double x2=lower_bound + interval * j;
				table.Rows.Add(x1, x2, FunctionXY(x1, x2));
			}
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
		pop.OperatorSet.AddOperator(new LGPOperator_Power());
		pop.OperatorSet.AddIfltOperator();

		pop.CreateFitnessCase += (index) =>
		{
			MexicanHatFitnessCase fitness_case = new MexicanHatFitnessCase();
			fitness_case.X1 = double.Parse(table.Rows[index]["X1"].ToString());
			fitness_case.X2 = double.Parse(table.Rows[index]["X2"].ToString());
			fitness_case.Y = double.Parse(table.Rows[index]["Y"].ToString());


			return fitness_case;
		};

		pop.GetFitnessCaseCount += () =>
		{
			return table.Rows.Count;
		};

		pop.EvaluateCostFromAllCases += (fitness_cases) =>
		{
			double cost = 0;
			for (int i = 0; i < fitness_cases.Count; i++)
			{
				MexicanHatFitnessCase fitness_case = (MexicanHatFitnessCase)fitness_cases[i];
				double correct_y = fitness_case.Y;
				double computed_y = fitness_case.PredictedY;
				cost += (correct_y - computed_y) * (correct_y - computed_y);
			}

			return cost;
		};


		pop.BreedInitialPopulation();


		while (!pop.IsTerminated)
		{
			pop.Evolve();
			Console.WriteLine("Mexican Hat Symbolic Regression Generation: {0}", pop.CurrentGeneration);
			Console.WriteLine("Global Fitness: {0}\tCurrent Fitness: {1}", pop.GlobalBestProgram.Fitness.ToString("0.000"), pop.FindFittestProgramInCurrentGeneration().Fitness.ToString("0.000"));
		}

		Console.WriteLine(pop.GlobalBestProgram.ToString());

	}
}
```

Where the fitness case MexicanHatFitnessCase is defined as follows:

```cs 
public class MexicanHatFitnessCase : LGPFitnessCase
{
	private double[] mX = new double[2];
	private double mY;
	private double mPredictedY;

	public double PredictedY
	{
		get { return mPredictedY; }
	}

	public double X1
	{
		get { return mX[0]; }
		set { mX[0] = value; }
	}

	public double X2
	{
		get { return mX[1]; }
		set { mX[1] = value; }
	}

	public double Y
	{
		get { return mY; }
		set { mY = value; }
	}

	public override void RunLGPProgramCompleted(double[] result)
	{
		mPredictedY = result[0];
	}

	public override bool QueryInput(int index, out double input)
	{
		input = 0;
		if (index < mX.Length)
		{
			input = mX[index];
			return true;
		}
		
		return false;
	}


	public override int GetInputCount()
	{
		return mX.Length;
	}


}
```

## Classification (Spiral)

The sample codes below show how to use the LinearGP to solve the Spiral classification problem:

```cs 
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
```

Where the fitness case SpiralFitnessCase is defined as follows:

```cs
public class SpiralFitnessCase : LGPFitnessCase
{
	private double[] mCoordinates = new double[2];
	private int mLabel;
	private int mComputedLabel;

	public int ComputedLabel
	{
		get { return mComputedLabel; }
	}

	public int Label
	{
		get { return mLabel; }
		set { mLabel = value; }
	}

	public double X
	{
		get { return mCoordinates[0]; }
		set { mCoordinates[0] = value; }
	}

	public double Y
	{
		get { return mCoordinates[1]; }
		set { mCoordinates[1] = value; }
	}

	public override void RunLGPProgramCompleted(double[] result)
	{
		if (result[0] < 0.5)
		{
			mComputedLabel = -1;
		}
		else
		{
			mComputedLabel = 1;
		}
	}

	public override bool QueryInput(int index, out double input)
	{
		input = 0;
		if (index < mCoordinates.Length)
		{
			input = mCoordinates[index];
			return true;
		}
		return false;
	}


	public override int GetInputCount()
	{
		return mCoordinates.Length;
	}


}
```