using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Graph;
using Ninject.Modules;
using GeneticEngine.Track;

namespace Best_Pass.BusinessLayer.Factorys
{
    public interface ITrackFactory
    {
        AbstractTrack CreateTrack(string name, int[] trackPoints, IGraph graph);
    }
}
