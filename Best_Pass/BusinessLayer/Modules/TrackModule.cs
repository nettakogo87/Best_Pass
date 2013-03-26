using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Graph;
using Ninject.Modules;
using GeneticEngine.Track;

namespace Best_Pass.BusinessLayer.Modules
{
    public class TrackModule : NinjectModule
    {
        private int[] _trackPoints;
        private IGraph _graph;
        public TrackModule(int[] trackPoints, IGraph graph)
        {
            _trackPoints = trackPoints;
            _graph = graph;
        }
        public override void Load()
        {
            Bind<AbstractTrack>()
                .To<ClosedTrack>()
                .Named("ClosedTrack")
                .WithConstructorArgument("trackPoints", _trackPoints).WithConstructorArgument("graph", _graph);
            Bind<AbstractTrack>().To<UnclosedTrack>().Named("UnclosedTrack").WithConstructorArgument("trackPoints", _trackPoints).WithConstructorArgument("graph", _graph);
        }
    }
}
