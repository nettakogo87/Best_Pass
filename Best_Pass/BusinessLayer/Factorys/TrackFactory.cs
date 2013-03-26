using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Graph;
using GeneticEngine.Track;
using Best_Pass.BusinessLayer.Modules;
using Ninject;

namespace Best_Pass.BusinessLayer.Factorys
{
    public class TrackFactory : ITrackFactory
    {
        public AbstractTrack CreateTrack(string trackName, int[] trackPoints, IGraph graph)
        {
            IKernel kernel = new StandardKernel(new TrackModule(trackPoints, graph));
            return kernel.Get<AbstractTrack>(trackName);
        }
    }
}
