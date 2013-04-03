using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Crossingover;
using GeneticEngine.FitnessFunction;
using GeneticEngine.Graph;
using GeneticEngine.Mutation;
using GeneticEngine.Selection;
using GeneticEngine.Track;

namespace Best_Pass.BusinessLayer
{
    public class Configuration
    {
        public enum AlgorithmMode
        {
            Search, Quality, Singl
        };
        public AlgorithmMode AlgMode { set; get; }
        public string ConfigName { set; get; }
        public IGraph Graph { set; get; }
        public AbstractTrack[] Tracks { set; get; }
        public IMutation Mutation { set; get; }
        public ICrossingover Crossingover { set; get; }
        public ISelection Selection { set; get; }
        public IFitnessFunction FitnessFunction { set; get; }
        public double FitnessParam { set; get; }
        public int ProbOfCrossingover { set; get; }
        public int ProbOfMutation { set; get; }
        public int CountOfReplays { set; get; }
        public List<string> AliasMutations { set; get; }
        public List<string> AliasSelection { set; get; }
        public List<string> AliasCrossingover { set; get; }

        public Configuration()
        {
            AliasCrossingover = new List<string>();
            AliasMutations = new List<string>();
            AliasSelection = new List<string>();

            ConfigName = "Новая конфигурация";
            AlgMode = AlgorithmMode.Singl;
            CountOfReplays = 1;
            ProbOfCrossingover = 100;
            ProbOfMutation = 100;
            FitnessParam = 10;
            Mutation = new NotRandomMutation();
            AliasMutations.Add(Mutation.GetName());
            Crossingover = new CyclicalCrossingover();
            AliasCrossingover.Add(Crossingover.GetName());
            Selection = new TournamentSelection();
            AliasSelection.Add(Selection.GetName());
            Graph = new UndirectedConnectedGraph(10);
            FitnessFunction = new BestReps((int)FitnessParam);
            Tracks = new AbstractTrack[10];
            Random r = new Random();
            for (int i = 0; i < Tracks.Length; i++)
            {
                int[] points = new int[10];
                for (int j = 0; j < points.Length; j++)
                {
                    points[j] = -1;
                }
                int newRandomChromosome = r.Next(points.Length - 1);
                for (int j = 0; j < points.Length; j++)
                {
                    while (points.Contains(newRandomChromosome))
                    {
                        newRandomChromosome = r.Next(points.Length);
                    }
                    points[j] = newRandomChromosome;
                }
                Tracks[i] = new ClosedTrack(points, Graph);
            }
        }
    }
}
