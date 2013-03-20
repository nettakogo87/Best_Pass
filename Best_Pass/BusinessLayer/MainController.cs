using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using GeneticEngine.Graph;
using GeneticEngine.Track;

namespace Best_Pass.BusinessLayer
{
    public class MainController
    {
        private IGraph _graph;
        private AbstractTrack[] _tracks;

        public void CreateGraph(int points)
        {
            List<Rib> ribs = new List<Rib>();
            for (int i = 0; i < points; i++)
            {
                for (int j = i + 1; j < points; j++)
                {
                    ribs.Add(new Rib(i, j, 1));
                }
            }
            _graph = new UndirectedConnectedGraph(ribs.ToArray());
        }

        public void SaveGraph(string path)
        {
            Stream stream = File.Open(path, FileMode.Create);
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(stream, _graph);
            stream.Close();
        }

        public void OpenGraph(string path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            SoapFormatter formatter = new SoapFormatter();
            _graph = (UndirectedConnectedGraph)formatter.Deserialize(stream);
            stream.Close();
        }

        public IGraph GetGraph()
        {
            return _graph;
        }

        public void ChangeWeightOfRip(int index, double newWeight)
        {
            Rib newRib = _graph.GetRib(index);
            newRib.Weight = newWeight;
            _graph.SetRib(index, newRib);
        }

        public void CreatePersons(List<int[]> persons)
        {
            _tracks = new AbstractTrack[persons.Count];
            for (int i = 0; i < persons.Count; i++)
            {
                _tracks[i] = new ClosedTrack( persons[i], _graph);
            }
        }

        public void SavePersons(string path)
        {
            FileInfo newPersonsFile = new FileInfo(path);
            StreamWriter sw = newPersonsFile.CreateText();
            foreach (AbstractTrack track in _tracks)
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
            _tracks = new AbstractTrack[tracks.Count];
            for (int i = 0; i < _tracks.Length; i++)
            {
                _tracks[i] = new ClosedTrack(tracks[i], _graph);
            }
        }

        public AbstractTrack[] GetPersons()
        {
            return _tracks;
        }

    }
}
