using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using GeneticEngine.Graph;

namespace Best_Pass.BusinessLayer
{
    public class GraphController
    {
        private IGraph _graph;

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

        public void ChangeWeightOfRip(int index, double newWeight)
        {
            Rib newRib = _graph.GetRib(index);
            newRib.Weight = newWeight;
            _graph.SetRib(index, newRib);
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
    }
}
