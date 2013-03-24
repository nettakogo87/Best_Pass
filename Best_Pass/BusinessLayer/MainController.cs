using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using GeneticEngine;
using GeneticEngine.Crossingover;
using GeneticEngine.FitnessFunction;
using GeneticEngine.Graph;
using GeneticEngine.Mutation;
using GeneticEngine.ProxyOperation;
using GeneticEngine.Selection;
using GeneticEngine.Track;


namespace Best_Pass.BusinessLayer
{
    public class MainController
    {
        public enum AlgorithmMode
        {
            Search, Quality, Singl
        };
        private Configuration _newConfig;
        private GEngine _newGA;
        private Random _r = new Random();

        public MainController()
        {
            _newConfig = new Configuration();
        }

        public void CreateGraph(int points, int  scopeStart, int scopeEnd)
        {
            List<Rib> ribs = new List<Rib>();
            for (int i = 0; i < points; i++)
            {
                for (int j = i + 1; j < points; j++)
                {
                    ribs.Add(new Rib(i, j, _r.Next(scopeStart, scopeEnd + 1)));
                }
            }
            _newConfig.Graph = new UndirectedConnectedGraph(ribs.ToArray());
        }

        public void SaveGraph(string path)
        {
            Stream stream = File.Open(path, FileMode.Create);
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(stream, _newConfig.Graph);
            stream.Close();
        }

        public void OpenGraph(string path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            SoapFormatter formatter = new SoapFormatter();
            _newConfig.Graph = (UndirectedConnectedGraph)formatter.Deserialize(stream);
            stream.Close();
        }

        public IGraph GetGraph()
        {
            return _newConfig.Graph;
        }

        public void ChangeWeightOfRip(int index, double newWeight)
        {
            Rib newRib = _newConfig.Graph.GetRib(index);
            newRib.Weight = newWeight;
            _newConfig.Graph.SetRib(index, newRib);
        }

        public void CreatePersons(List<int[]> persons)
        {
            _newConfig.Tracks = new AbstractTrack[persons.Count];
            for (int i = 0; i < persons.Count; i++)
            {
                _newConfig.Tracks[i] = new ClosedTrack(persons[i], _newConfig.Graph);
            }
        }

        public void CreatePersons()
        {
            _newConfig.Tracks = new AbstractTrack[10];
            for (int i = 0; i < _newConfig.Tracks.Length; i++)
            {
                int[] person = new int[_newConfig.Graph.CountOfNode];
                for (int j = 0; j < person.Length; j++)
                {
                    person[j] = -1;
                }
                int newRandomChromosome = _r.Next(_newConfig.Graph.CountOfNode - 1);
                for (int j = 0; j < person.Length; j++)
                {
                    while (person.Contains(newRandomChromosome))
                    {
                        newRandomChromosome = _r.Next(_newConfig.Graph.CountOfNode);
                    }
                    person[j] = newRandomChromosome;
                }
                _newConfig.Tracks[i] = new ClosedTrack(person, _newConfig.Graph);
            }
        }

        public void SavePersons(string path)
        {
            FileInfo newPersonsFile = new FileInfo(path);
            StreamWriter sw = newPersonsFile.CreateText();
            foreach (AbstractTrack track in _newConfig.Tracks)
            {
                for (int i = 0; i < track.Genotype.Length; i++)
                {
                    sw.Write(track.Genotype[i] + " ");
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public void LoadPersons(string path)
        {
            FileInfo newPersonsFile = new FileInfo(path);
            StreamReader sr = newPersonsFile.OpenText();
            List<int[]> tracks = new List<int[]>();
            char[] separator = { ' ' };
            while (!sr.EndOfStream)
            {
                string[] person = sr.ReadLine().ToString().Trim().Split(separator);
                int[] track = new int[person.Length];
                for (int i = 0; i < person.Length; i++)
                {
                    track[i] = Convert.ToInt16(person[i]);
                }
                tracks.Add(track);
            }
            sr.Close();
            _newConfig.Tracks = new AbstractTrack[tracks.Count];
            for (int i = 0; i < _newConfig.Tracks.Length; i++)
            {
                _newConfig.Tracks[i] = new ClosedTrack(tracks[i], _newConfig.Graph);
            }
        }

        public AbstractTrack[] GetPersons()
        {
            return _newConfig.Tracks;
        }

        public void CreateMutation(AlgorithmMode algorithmMode, int[] aliasMutations)
        {
            if (AlgorithmMode.Singl == algorithmMode)
            {
                switch (aliasMutations[0])
                {
                    case 1:
                        _newConfig.Mutation = new FourPointMutation();
                        break;
                    case 2:
                        _newConfig.Mutation = new TwoPointMutation();
                        break;
                    case 3:
                        _newConfig.Mutation = new NotRandomMutation();
                        break;
                    default:
                        _newConfig.Mutation = new TwoPointMutation();
                        break;
                }
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations);
                _newConfig.Mutation = new QualityCountsMutation(proxyMutationList, 0);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations);
                _newConfig.Mutation = new SearchBestMutation(proxyMutationList);
            }
        }

        public void CreateSelection(AlgorithmMode algorithmMode, int[] aliasSelection)
        {
            if (AlgorithmMode.Singl == algorithmMode)
            {
                switch (aliasSelection[0])
                {
                    case 1:
                        _newConfig.Selection = new TournamentSelection();
                        break;
                    case 2:
                        _newConfig.Selection = new RankingSelection();
                        break;
                    case 3:
                        _newConfig.Selection = new RouletteSelection();
                        break;
                    default:
                        _newConfig.Selection = new TournamentSelection();
                        break;
                }
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection);
                _newConfig.Selection = new QualityCountsSelection(proxySelectionList);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection);
                _newConfig.Selection = new SearchBestSelection(proxySelectionList);
            }
        }

        public void CreateCrossingover(AlgorithmMode algorithmMode, int[] aliasCrossingover)
        {
            if (AlgorithmMode.Singl == algorithmMode)
            {
                switch (aliasCrossingover[0])
                {
                    case 1:
                        _newConfig.Crossingover = new CyclicalCrossingover();
                        break;
                    case 2:
                        _newConfig.Crossingover = new InversionCrossingover();
                        break;
                    case 3:
                        _newConfig.Crossingover = new OnePointCrossingover();
                        break;
                    case 4:
                        _newConfig.Crossingover = new TwoPointCrossingover();
                        break;
                    default:
                        _newConfig.Crossingover = new TwoPointCrossingover();
                        break;
                }
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover);
                _newConfig.Crossingover = new QualityCountsCrossingover(proxyCrossingoverList, 0);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover);
                _newConfig.Crossingover = new SearchBestCrossingover(proxyCrossingoverList);
            }
        }

        public void CreateFitnessFunction(int fitnessFunction, double param)
        {
            _newConfig.FitnessFunctionParametr = param;
            switch (fitnessFunction)
            {
                case 1:
                    _newConfig.FitnessFunction = new GenerationCounter((int)param);
                    break;
                case 2:
                    _newConfig.FitnessFunction = new BestReps((int)param);
                    break;
                case 3:
                    _newConfig.FitnessFunction = new ReachWantedResult(param);
                    break;
                default:
                    _newConfig.FitnessFunction = new ReachWantedResult(param);
                    break;
            }
        }

        public void SetProbabilitys(int pCrossingover, int pMutation)
        {
            _newConfig.ProbabilityOfCrossingover = pCrossingover;
            _newConfig.ProbabilityOfMutation = pMutation;
        }

        private static List<ProxyMutation> GetProxyMutationList(int[] aliasMutations)
        {
            List<ProxyMutation> proxyMutationList = new List<ProxyMutation>();
            for (int i = 0; i < aliasMutations.Length; i++)
            {
                switch (aliasMutations[i])
                {
                    case 1:
                        proxyMutationList.Add(new ProxyMutation(new FourPointMutation()));
                        break;
                    case 2:
                        proxyMutationList.Add(new ProxyMutation(new TwoPointMutation()));
                        break;
                    case 3:
                        proxyMutationList.Add(new ProxyMutation(new NotRandomMutation()));
                        break;
                    default:
                        proxyMutationList.Add(new ProxyMutation(new TwoPointMutation()));
                        break;
                }
            }
            return proxyMutationList;
        }

        private static List<ProxySelection> GetProxySelectionList(int[] aliasSelection)
        {
            List<ProxySelection> proxySelectionsList = new List<ProxySelection>();
            for (int i = 0; i < aliasSelection.Length; i++)
            {
                switch (aliasSelection[i])
                {
                    case 1:
                        proxySelectionsList.Add(new ProxySelection(new TournamentSelection()));
                        break;
                    case 2:
                        proxySelectionsList.Add(new ProxySelection(new RankingSelection()));
                        break;
                    case 3:
                        proxySelectionsList.Add(new ProxySelection(new RouletteSelection()));
                        break;
                    default:
                        proxySelectionsList.Add(new ProxySelection(new TournamentSelection()));
                        break;
                }
            }
            return proxySelectionsList;
        }

        private static List<ProxyCrossingover> GetProxyCrossingoverList(int[] aliasCrossingover)
        {
            List<ProxyCrossingover> proxyCrossingoverList = new List<ProxyCrossingover>();
            for (int i = 0; i < aliasCrossingover.Length; i++)
            {
                switch (aliasCrossingover[i])
                {
                    case 1:
                        proxyCrossingoverList.Add(new ProxyCrossingover(new CyclicalCrossingover()));
                        break;
                    case 2:
                        proxyCrossingoverList.Add(new ProxyCrossingover(new InversionCrossingover()));
                        break;
                    case 3:
                        proxyCrossingoverList.Add(new ProxyCrossingover(new OnePointCrossingover()));
                        break;
                    case 4:
                        proxyCrossingoverList.Add(new ProxyCrossingover(new TwoPointCrossingover()));
                        break;
                    default:
                        proxyCrossingoverList.Add(new ProxyCrossingover(new TwoPointCrossingover()));
                        break;
                }
            }
            return proxyCrossingoverList;
        }

        public IMutation GetMutation()
        {
            return _newConfig.Mutation;
        }
        public ISelection GetSelection()
        {
            return _newConfig.Selection;
        }
        public ICrossingover GetCrossingover()
        {
            return _newConfig.Crossingover;
        }
        public IFitnessFunction GetFitnessFunction()
        {
            return _newConfig.FitnessFunction;
        }
        public int GetAlgorithmMode()
        {
            return _newConfig.AlgMode;
        }
        public int GetPMutation()
        {
            return _newConfig.ProbabilityOfMutation;
        }
        public int GetPCrossingover()
        {
            return _newConfig.ProbabilityOfCrossingover;
        }
        public double GetFitnessFunctionParametr()
        {
            return _newConfig.FitnessFunctionParametr;
        }

        public string GetConfigName()
        {
            return _newConfig.NameOfConfiguration;
        }
        public int GetCountOfReplays()
        {
            return _newConfig.CountOfReplays;
        }

        public void SetConfigName(string name)
        {
            _newConfig.NameOfConfiguration = name;
        }
        public void SetCountOfReplays(int countOfReplays)
        {
            _newConfig.CountOfReplays = countOfReplays;
        }

        public void SaveConfiguration( string path)
        {
            FileInfo newConfigurationFile = new FileInfo(path);
            StreamWriter sw = newConfigurationFile.CreateText();

            sw.WriteLine(_newConfig.NameOfConfiguration);
            sw.WriteLine(_newConfig.CountOfReplays);
            sw.WriteLine(_newConfig.AlgMode);
            sw.WriteLine();
            sw.WriteLine(_newConfig.Graph.CountOfRibs);
            sw.WriteLine(_newConfig.Graph.CountOfNode);
            for (int i = 0; i < _newConfig.Graph.CountOfNode; i++)
            {
                for (int j = i + 1; j < _newConfig.Graph.CountOfNode; j++)
                {
                    sw.WriteLine(_newConfig.Graph.GetWeightByRip(i, j));
                }
            }
            sw.WriteLine();
            sw.WriteLine(_newConfig.Tracks.Length);
            foreach (AbstractTrack track in _newConfig.Tracks)
            {
                for (int i = 0; i < track.Genotype.Length; i++)
                {
                    sw.Write(track.Genotype[i] + " ");
                }
                sw.WriteLine();
            }
            sw.WriteLine();
            sw.WriteLine(_newConfig.FitnessFunction.GetName());
            sw.WriteLine(_newConfig.FitnessFunctionParametr);
            sw.WriteLine();
            sw.WriteLine(_newConfig.Mutation.GetName());
            sw.WriteLine(_newConfig.Selection.GetName());
            sw.WriteLine(_newConfig.Crossingover.GetName());
            sw.WriteLine();
            sw.WriteLine(_newConfig.ProbabilityOfMutation);
            sw.WriteLine(_newConfig.ProbabilityOfCrossingover);
            sw.Close();
        }

        public void LoadConfiguration(string path)
        {
            FileInfo newConfigurationFile = new FileInfo(path);
            StreamReader sr = newConfigurationFile.OpenText();
            _newConfig.NameOfConfiguration = sr.ReadLine();
            _newConfig.CountOfReplays = Convert.ToInt16(sr.ReadLine());
            _newConfig.AlgMode = Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            int countOfRibs = Convert.ToInt16(sr.ReadLine());
            int countOfNode = Convert.ToInt16(sr.ReadLine());
            double[] weight = new double[_newConfig.Graph.CountOfRibs];
            for (int i = 0; i < _newConfig.Graph.CountOfRibs; i++)
            {
                weight[i] = Convert.ToDouble(sr.ReadLine());
            }
            int k = 0;
            List<Rib> ribs = new List<Rib>();
            for (int i = 0; i < countOfNode; i++)
            {
                for (int j = i + 1; j < countOfNode; j++)
                {
                    ribs.Add(new Rib(i, j, weight[k]));
                    k++;
                }
            }
            _newConfig.Graph = new UndirectedConnectedGraph(ribs.ToArray());
            sr.ReadLine();
            int countOfTracks = Convert.ToInt16(sr.ReadLine());
            List<int[]> tracks = new List<int[]>();
            char[] separator = { ' ' };
            for (int i = 0; i < countOfTracks; i++)
            {
                string[] person = sr.ReadLine().ToString().Trim().Split(separator);
                int[] track = new int[person.Length];
                for (int j = 0; j < person.Length; j++)
                {
                    track[j] = Convert.ToInt16(person[j]);
                }
                tracks.Add(track);
            }
            _newConfig.Tracks = new AbstractTrack[tracks.Count];
            for (int i = 0; i < _newConfig.Tracks.Length; i++)
            {
                _newConfig.Tracks[i] = new ClosedTrack(tracks[i], _newConfig.Graph);
            }
            sr.ReadLine();
            string nameOfFitnessFunction = sr.ReadLine();
            double paramOfFitnessFunction = Convert.ToDouble(sr.ReadLine());
            _newConfig.FitnessFunctionParametr = paramOfFitnessFunction;
            if (nameOfFitnessFunction == "BestReps")
            {
                _newConfig.FitnessFunction = new BestReps((int)paramOfFitnessFunction);
            }
            if (nameOfFitnessFunction == "GenerationCounter")
            {
                _newConfig.FitnessFunction = new GenerationCounter((int)paramOfFitnessFunction);
            }
            if (nameOfFitnessFunction == "ReachWantedResult")
            {
                _newConfig.FitnessFunction = new ReachWantedResult((int)paramOfFitnessFunction);
            }
            sr.ReadLine();
            string nameOfMutation = sr.ReadLine();
            if (nameOfMutation == "FourPointMutation")
            {
                _newConfig.Mutation = new FourPointMutation();
            }
            if (nameOfMutation == "TwoPointMutation")
            {
                _newConfig.Mutation = new TwoPointMutation();
            }
            if (nameOfMutation == "NotRandomMutation")
            {
                _newConfig.Mutation = new NotRandomMutation();
            }
            string nameOfSelection = sr.ReadLine();
            if (nameOfSelection == "RankingSelection")
            {
                _newConfig.Selection = new RankingSelection();
            }
            if (nameOfSelection == "RouletteSelection")
            {
                _newConfig.Selection = new RouletteSelection();
            }
            if (nameOfSelection == "TournamentSelection")
            {
                _newConfig.Selection = new TournamentSelection();
            }
            string nameOfCrossingover = sr.ReadLine();
            if (nameOfCrossingover == "CyclicalCrossingover")
            {
                _newConfig.Crossingover = new CyclicalCrossingover();
            }
            if (nameOfCrossingover == "InversionCrossingover")
            {
                _newConfig.Crossingover = new InversionCrossingover();
            }
            if (nameOfCrossingover == "OnePointCrossingover")
            {
                _newConfig.Crossingover = new OnePointCrossingover();
            }
            if (nameOfCrossingover == "TwoPointCrossingover")
            {
                _newConfig.Crossingover = new TwoPointCrossingover();
            }
            sr.ReadLine();
            _newConfig.ProbabilityOfMutation = Convert.ToInt16(sr.ReadLine());
            _newConfig.ProbabilityOfCrossingover = Convert.ToInt16(sr.ReadLine());
            sr.Close();
        }

        public void StartGA()
        {
            AbstractTrack[] tracks = new AbstractTrack[_newConfig.Tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i] = _newConfig.Tracks[i].Clone();
            }
            _newGA = new GEngine(tracks, _newConfig.ProbabilityOfCrossingover, _newConfig.ProbabilityOfMutation, _newConfig.FitnessFunction, _newConfig.Mutation, _newConfig.Crossingover, _newConfig.Selection);
            _newGA.Run();
        }
    }
}
