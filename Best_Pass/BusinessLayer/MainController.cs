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
        private static MutationFactory _mutationFactory;
        private static SelectionFactory _selectionFactory;
        private static CrossingoverFactory _crossingoverFactory;

        public MainController()
        {
            _newConfig = new Configuration();
            UserSettings = new UserSettings();
            GeneticsDataSet = new DB_GeneticsDataSet();
            LaunchTableAdapter = new DB_GeneticsDataSetTableAdapters.LaunchesTableAdapter();
            PersonsTableAdapter = new DB_GeneticsDataSetTableAdapters.PersonsTableAdapter();

            _mutationFactory = new MutationFactory();
            _selectionFactory = new SelectionFactory();
            _crossingoverFactory = new CrossingoverFactory();
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

        public void CreateMutation(Configuration.AlgorithmMode algorithmMode, List<string> aliasMutations)
        {
            _newConfig.AliasMutations = aliasMutations;
            if (Configuration.AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Mutation = _mutationFactory.CreateMutation(aliasMutations[0]);
            }
            if (Configuration.AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations.ToArray());
                _newConfig.Mutation = _mutationFactory.CreateMutation("QualityCountsMutation", proxyMutationList, 0);
            }
            if (Configuration.AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyMutation> proxyMutationList = GetProxyMutationList(aliasMutations.ToArray());
                _newConfig.Mutation = _mutationFactory.CreateMutation("SearchBestMutation", proxyMutationList, 0);
            }
        }

        public void CreateSelection(Configuration.AlgorithmMode algorithmMode, List<string> aliasSelection)
        {
            _newConfig.AliasSelection = aliasSelection;
            if (Configuration.AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Selection = _selectionFactory.CreateSelection(aliasSelection[0]);
            }
            if (Configuration.AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection.ToArray());
                _newConfig.Selection = _selectionFactory.CreateSelection("QualityCountsSelection", proxySelectionList);
            }
            if (Configuration.AlgorithmMode.Search == algorithmMode)
            {
                List<ProxySelection> proxySelectionList = GetProxySelectionList(aliasSelection.ToArray());
                _newConfig.Selection = _selectionFactory.CreateSelection("SearchBestSelection", proxySelectionList);
            }
        }

        public void CreateCrossingover(Configuration.AlgorithmMode algorithmMode, List<string> aliasCrossingover)
        {
            _newConfig.AliasCrossingover = aliasCrossingover;
            if (Configuration.AlgorithmMode.Singl == algorithmMode)
            {
                _newConfig.Crossingover = _crossingoverFactory.CreateCrossingover(aliasCrossingover[0]);
            }
            if (Configuration.AlgorithmMode.Quality == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover.ToArray());
                _newConfig.Crossingover = _crossingoverFactory.CreateCrossingover("QualityCountsCrossingover", proxyCrossingoverList, 0);
            }
            if (Configuration.AlgorithmMode.Search == algorithmMode)
            {
                List<ProxyCrossingover> proxyCrossingoverList = GetProxyCrossingoverList(aliasCrossingover.ToArray());
                _newConfig.Crossingover = _crossingoverFactory.CreateCrossingover("SearchBestCrossingover", proxyCrossingoverList, 0);
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
            return aliasMutations.Select(t => new ProxyMutation(_mutationFactory.CreateMutation(t))).ToList();
        }

        private static List<ProxySelection> GetProxySelectionList(string[] aliasSelection)
        {
            return aliasSelection.Select(t => new ProxySelection(_selectionFactory.CreateSelection(t))).ToList();
        }

        private static List<ProxyCrossingover> GetProxyCrossingoverList(string[] aliasCrossingover)
        {
            return aliasCrossingover.Select(t => new ProxyCrossingover(_crossingoverFactory.CreateCrossingover(t))).ToList();
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
            sw.WriteLine((int)_newConfig.AlgMode);
            sw.WriteLine();
            sw.WriteLine(_newConfig.Graph.CountOfRibs);
            sw.WriteLine(_newConfig.Graph.CountOfNode);
            for (int i = 0; i < _newConfig.Graph.CountOfNode; i++)
            {
                for (int j = i + 1; j < _newConfig.Graph.CountOfNode; j++)
                {
                    sw.WriteLine(_newConfig.Graph.GetRibByNodes(i, j).Weight);
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
            if (_newConfig.AlgMode == Configuration.AlgorithmMode.Singl)
            {
                sw.WriteLine(_newConfig.Mutation.GetName());
                sw.WriteLine(_newConfig.Selection.GetName());
                sw.WriteLine(_newConfig.Crossingover.GetName());
            }
            if (_newConfig.AlgMode == Configuration.AlgorithmMode.Quality || _newConfig.AlgMode == Configuration.AlgorithmMode.Search)
            {
                sw.WriteLine(_newConfig.AliasMutations.Count);
                foreach (string t in _newConfig.AliasMutations)
                {
                    sw.WriteLine(t);
                }
                sw.WriteLine(_newConfig.AliasSelection.Count);
                foreach (string t in _newConfig.AliasSelection)
                {
                    sw.WriteLine(t);
                }
                sw.WriteLine(_newConfig.AliasCrossingover.Count);
                foreach (string t in _newConfig.AliasCrossingover)
                {
                    sw.WriteLine(t);
                }
            }
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
            _newConfig.AlgMode = (Configuration.AlgorithmMode)Convert.ToInt16(sr.ReadLine());
            sr.ReadLine();
            int countOfRibs = Convert.ToInt16(sr.ReadLine());
            int countOfNode = Convert.ToInt16(sr.ReadLine());

            double[] weight = new double[countOfRibs];
            for (int i = 0; i < countOfRibs; i++)
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
            if (_newConfig.AlgMode == Configuration.AlgorithmMode.Singl)
            {
                _newConfig.Mutation = _mutationFactory.CreateMutation(sr.ReadLine());
                _newConfig.Selection = _selectionFactory.CreateSelection(sr.ReadLine());
                _newConfig.Crossingover = _crossingoverFactory.CreateCrossingover(sr.ReadLine());
            }
            else
            {
                List<string> aliasMutations = GetAliasForAlgorithms(sr);
                CreateMutation(_newConfig.AlgMode, aliasMutations);
                List<string> aliasSelection = GetAliasForAlgorithms(sr);
                CreateSelection(_newConfig.AlgMode, aliasSelection);
                List<string> aliasCrossingover = GetAliasForAlgorithms(sr);
                CreateCrossingover(_newConfig.AlgMode, aliasCrossingover);
            }
            sr.ReadLine();
            _newConfig.ProbOfMutation = Convert.ToInt16(sr.ReadLine());
            _newConfig.ProbOfCrossingover = Convert.ToInt16(sr.ReadLine());
            sr.Close();
        }

        private List<string> GetAliasForAlgorithms(StreamReader sr)
        {
            int count = Convert.ToInt32(sr.ReadLine());
            List<string> alias = new List<string>();
            for (int i = 0; i < count; i++)
            {
                alias.Add(sr.ReadLine());
            }
            return alias;
        }

        public void StartGA()
        {
            AbstractTrack.ClearCount();
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
