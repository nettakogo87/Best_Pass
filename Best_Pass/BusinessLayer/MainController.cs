using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using Best_Pass.BusinessLayer.Modules;
using GeneticEngine;
using GeneticEngine.Crossingover;
using GeneticEngine.FitnessFunction;
using GeneticEngine.Graph;
using GeneticEngine.Mutation;
using GeneticEngine.ProxyOperation;
using GeneticEngine.Selection;
using GeneticEngine.Track;
using Ninject;
using Best_Pass.BusinessLayer.Factorys;


namespace Best_Pass.BusinessLayer
{
    public class MainController
    {
        public enum AlgorithmMode
        {
            Search, Quality, Singl
        };
        public enum TrackType
        {
            ClosedTrack, UnclosedTrack
        }
        private Configuration _newConfig;
        private GEngine _newGA;
        private Random _r = new Random();
        public DB_GeneticsDataSet GeneticsDataSet { get; private set; }
        public DB_GeneticsDataSetTableAdapters.LaunchesTableAdapter LaunchTableAdapter { get; private set; }
        public DB_GeneticsDataSetTableAdapters.PersonsTableAdapter PersonsTableAdapter { get; private set; }
        public UserSettings UserSettings { get; private set; }
        private Thread _threadRunGA;

        public MainController()
        {
            _newConfig = new Configuration();
            UserSettings = new UserSettings();
            GeneticsDataSet = new DB_GeneticsDataSet();
            LaunchTableAdapter = new DB_GeneticsDataSetTableAdapters.LaunchesTableAdapter();
            PersonsTableAdapter = new DB_GeneticsDataSetTableAdapters.PersonsTableAdapter();
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
            Rib newRib = _newConfig.Graph.GetRibByIndex(index);
            newRib.Weight = newWeight;
            _newConfig.Graph.SetRib(index, newRib);
        }

        public void CreatePersons(List<int[]> persons)
        {
            _newConfig.Tracks = new AbstractTrack[persons.Count];
            for (int i = 0; i < persons.Count; i++)
            {
                ITrackFactory trackFactory = new TrackFactory();
                _newConfig.Tracks[i] = trackFactory.CreateTrack("ClosedTrack", persons[i], _newConfig.Graph);
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

        public void CreateMutation(AlgorithmMode algorithmMode, string[] aliasMutations)
        {
            MutationFactory mutationFactory = new MutationFactory();
            if (AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Mutation = mutationFactory.CreateMutation(aliasMutations[0]);
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations);
                _newConfig.Mutation = mutationFactory.CreateMutation("QualityCountsMutation", proxyMutationList, 0);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations);
                _newConfig.Mutation = mutationFactory.CreateMutation("SearchBestMutation", proxyMutationList, 0);
            }
        }

        public void CreateSelection(AlgorithmMode algorithmMode, string[] aliasSelection)
        {
            SelectionFactory selectionFactory = new SelectionFactory();
            if (AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Selection = selectionFactory.CreateSelection(aliasSelection[0]);
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection);
                _newConfig.Selection = selectionFactory.CreateSelection("QualityCountsSelection", proxySelectionList);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection);
                _newConfig.Selection = selectionFactory.CreateSelection("SearchBestSelection", proxySelectionList);
            }
        }

        public void CreateCrossingover(AlgorithmMode algorithmMode, string[] aliasCrossingover)
        {
            CrossingoverFactory crossingoverFactory = new CrossingoverFactory();
            if (AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Crossingover = crossingoverFactory.CreateCrossingover(aliasCrossingover[0]);
            }
            if (AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover);
                _newConfig.Crossingover = crossingoverFactory.CreateCrossingover("QualityCountsCrossingover", proxyCrossingoverList, 0);
            }
            if (AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover);
                _newConfig.Crossingover = crossingoverFactory.CreateCrossingover("SearchBestCrossingover", proxyCrossingoverList, 0);
            }
        }

        public void CreateFitnessFunction(string fitnessName, double param)
        {
            _newConfig.FitnessParam = param;
            FitnessFunctionFactory fitnessFunctionFactory = new FitnessFunctionFactory();
            _newConfig.FitnessFunction = fitnessFunctionFactory.CreateFitnessFunction(fitnessName, param);
        }

        public void SetProbabilitys(int pCrossingover, int pMutation)
        {
            _newConfig.ProbOfCrossingover = pCrossingover;
            _newConfig.ProbOfMutation = pMutation;
        }

        private static List<ProxyMutation> GetProxyMutationList(string[] aliasMutations)
        {
            MutationFactory mutationFactory = new MutationFactory();
            return aliasMutations.Select(t => new ProxyMutation(mutationFactory.CreateMutation(t))).ToList();
        }

        private static List<ProxySelection> GetProxySelectionList(string[] aliasSelection)
        {
            SelectionFactory selectionFactory = new SelectionFactory();
            return aliasSelection.Select(t => new ProxySelection(selectionFactory.CreateSelection(t))).ToList();
        }

        private static List<ProxyCrossingover> GetProxyCrossingoverList(string[] aliasCrossingover)
        {
            CrossingoverFactory crossingoverFactory = new CrossingoverFactory();
            return aliasCrossingover.Select(t => new ProxyCrossingover(crossingoverFactory.CreateCrossingover(t))).ToList();
        }

        public Configuration GetConfig()
        {
            return _newConfig;
        }

        public void SaveConfiguration( string path)
        {
            FileInfo newConfigurationFile = new FileInfo(path);
            StreamWriter sw = newConfigurationFile.CreateText();

            sw.WriteLine(_newConfig.ConfigName);
            sw.WriteLine(_newConfig.CountOfReplays);
            sw.WriteLine(_newConfig.AlgMode);
            sw.WriteLine();
            sw.WriteLine(_newConfig.Graph.CountOfRibs);
            sw.WriteLine(_newConfig.Graph.CountOfNode);
            for (int i = 0; i < _newConfig.Graph.CountOfNode; i++)
            {
                for (int j = i + 1; j < _newConfig.Graph.CountOfNode; j++)
                {
                    sw.WriteLine(_newConfig.Graph.GetRibByNodes(i, j));
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
            sw.WriteLine(_newConfig.FitnessParam);
            sw.WriteLine();
            sw.WriteLine(_newConfig.Mutation.GetName());
            sw.WriteLine(_newConfig.Selection.GetName());
            sw.WriteLine(_newConfig.Crossingover.GetName());
            sw.WriteLine();
            sw.WriteLine(_newConfig.ProbOfMutation);
            sw.WriteLine(_newConfig.ProbOfCrossingover);
            sw.Close();
        }

        public void LoadConfiguration(string path)
        {
            FileInfo newConfigurationFile = new FileInfo(path);
            StreamReader sr = newConfigurationFile.OpenText();
            _newConfig.ConfigName = sr.ReadLine();
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
            double paramOfFitness = Convert.ToDouble(sr.ReadLine());
            _newConfig.FitnessParam = paramOfFitness;
            FitnessFunctionFactory fitnessFunctionFactory = new FitnessFunctionFactory();
            _newConfig.FitnessFunction = fitnessFunctionFactory.CreateFitnessFunction(nameOfFitnessFunction, paramOfFitness);
            sr.ReadLine();
            string nameOfMutation = sr.ReadLine();
            MutationFactory mutationFactory = new MutationFactory();
            _newConfig.Mutation = mutationFactory.CreateMutation(nameOfMutation);
            string nameOfSelection = sr.ReadLine();
            SelectionFactory selectionFactory = new SelectionFactory();
            _newConfig.Selection = selectionFactory.CreateSelection(nameOfSelection);
            string nameOfCrossingover = sr.ReadLine();
            CrossingoverFactory crossingoverFactory = new CrossingoverFactory();
            _newConfig.Crossingover = crossingoverFactory.CreateCrossingover(nameOfCrossingover);
            sr.ReadLine();
            _newConfig.ProbOfMutation = Convert.ToInt16(sr.ReadLine());
            _newConfig.ProbOfCrossingover = Convert.ToInt16(sr.ReadLine());
            sr.Close();
        }

        public void StartGA()
        {
            AbstractTrack[] tracks = new AbstractTrack[_newConfig.Tracks.Length];
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i] = _newConfig.Tracks[i].Clone();
            }
            _newGA = new GEngine(tracks, _newConfig.ProbOfCrossingover, _newConfig.ProbOfMutation, _newConfig.FitnessFunction, _newConfig.Mutation, _newConfig.Crossingover, _newConfig.Selection);
            _threadRunGA = new Thread(_newGA.Run);
            _threadRunGA.Name ="LaborOfGA";
            try
            {
                _threadRunGA.Start();
            }
            catch (ThreadAbortException)
            {
                CleaningDB();
            }
        }

        public void StopGA()
        {
            _threadRunGA.Abort();
            CleaningDB();
        }

        public bool InspectGA()
        {
            return ThreadState.Stopped == _threadRunGA.ThreadState;
        }

        private void CleaningDB()
        {
            LoadGeneticsDataSet();
            for (int i = 0; i < GeneticsDataSet.Launches.Count; i++)
            {
                DB_GeneticsDataSet.LaunchesRow lr = GeneticsDataSet.Launches[i];
                if (lr.Id == _newGA.GetLaunchId())
                {
                    lr.Delete();
                    LaunchTableAdapter.Update(lr);
                    GeneticsDataSet.Launches.AcceptChanges();
                    break;
                }
            }
        }

        public void LoadGeneticsDataSet()
        {
            LaunchTableAdapter.Fill(GeneticsDataSet.Launches);
            PersonsTableAdapter.Fill(GeneticsDataSet.Persons);
        }

        public void DeleteRowFromTableOfLaunch(int index)
        {
            DB_GeneticsDataSet.LaunchesRow lr = GeneticsDataSet.Launches[index];
            lr.Delete();
            LaunchTableAdapter.Update(lr);
            GeneticsDataSet.Launches.AcceptChanges();
        }
    }
}
