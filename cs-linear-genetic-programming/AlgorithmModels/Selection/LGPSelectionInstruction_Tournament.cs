using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearGP.AlgorithmModels.Selection
{
    using System.Xml;
    using LinearGP.ComponentModels;
    using Statistics;

    class LGPSelectionInstruction_Tournament : LGPSelectionInstruction
    {
        private int mTournamentSize=5;

        public LGPSelectionInstruction_Tournament()
        {

        }

        public LGPSelectionInstruction_Tournament(XmlElement xml_level1)
            : base(xml_level1)
        {
            foreach (XmlElement xml_level2 in xml_level1.ChildNodes)
            {
                if (xml_level2.Name == "param")
                {
                    string attrname = xml_level2.Attributes["name"].Value;
                    string attrvalue = xml_level2.Attributes["value"].Value;
                    if (attrname == "tournament_size")
                    {
                        int value = 0;
                        int.TryParse(attrvalue, out value);
                        mTournamentSize = value;
                    }
                }
            }
        }

        public override LGPProgram Select(LGPPop pop)
        {
            HashSet<LGPProgram> tournament=new HashSet<LGPProgram>();
	        while(tournament.Count < mTournamentSize)
	        {
		        int r=DistributionModel.NextInt(pop.ProgramCount);
		        tournament.Add(pop.FindProgramByIndex(r));
	        }

	        List<LGPProgram> programs=tournament.ToList();

            programs=programs.OrderByDescending(o => o.Fitness).ToList();

	        return programs[0];
        }

        public override void Select(LGPPop pop, ref KeyValuePair<LGPProgram, LGPProgram> best_pair, ref KeyValuePair<LGPProgram, LGPProgram> worst_pair)
        {
            List<LGPProgram> tournament1=new List<LGPProgram>();
	        List<LGPProgram> tournament2=new List<LGPProgram>();
	        int tournament_size2=mTournamentSize * 2;
	        if(tournament_size2 > pop.ProgramCount)
	        {
		        tournament_size2=pop.ProgramCount;
		        int tournament_size=tournament_size2 / 2;
		        pop.RandomShuffle();
		        for(int i=0; i<tournament_size; i++)
		        {
			        tournament1.Add(pop.FindProgramByIndex(i));
		        }
		        for(int i=tournament_size; i<tournament_size2; i++)
		        {
			        tournament2.Add(pop.FindProgramByIndex(i));
		        }
	        }
	        else
	        {
		        pop.RandomShuffle();
		        for(int i=0; i<mTournamentSize; i++)
		        {
			        tournament1.Add(pop.FindProgramByIndex(i));
		        }
		        for(int i=mTournamentSize; i<tournament_size2; i++)
		        {
			        tournament2.Add(pop.FindProgramByIndex(i));
		        }
	        }	

            tournament1=tournament1.OrderByDescending(o=>o.Fitness).ToList();
            tournament2=tournament2.OrderByDescending(o=>o.Fitness).ToList();

            //Console.WriteLine("tournament 1: {0}", tournament1.Count);
            //Console.WriteLine("tournament 2: {0}", tournament2.Count);

	        best_pair=new KeyValuePair<LGPProgram, LGPProgram>(tournament1[0], tournament2[0]);
	        worst_pair=new KeyValuePair<LGPProgram, LGPProgram>(tournament1[tournament1.Count-1], tournament2[tournament2.Count-1]);
        }

        public override LGPSelectionInstruction Clone()
        {
            LGPSelectionInstruction_Tournament clone = new LGPSelectionInstruction_Tournament();
            clone.mTournamentSize = mTournamentSize;
            return clone;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(">> Name: LGPSelectionInstruction_Tournament");
	        sb.AppendFormat(">> Tournament Size: {0}", mTournamentSize);

            return sb.ToString();
        }
    }
}
