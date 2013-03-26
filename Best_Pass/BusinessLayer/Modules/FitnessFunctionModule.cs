using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.FitnessFunction;
using Ninject.Modules;

namespace Best_Pass.BusinessLayer.Modules
{
    public class FitnessFunctionModule : NinjectModule
    {
        private double _param;
        public FitnessFunctionModule(double param)
        {
            _param = param;
        }
        public override void Load()
        {
            Bind<IFitnessFunction>().To<BestReps>().Named("BestReps").WithConstructorArgument("countOfReps", (int)_param);
            Bind<IFitnessFunction>().To<GenerationCounter>().Named("GenerationCounter").WithConstructorArgument("specifiedCountOfReps", (int)_param);
            Bind<IFitnessFunction>().To<ReachWantedResult>().Named("ReachWantedResult").WithConstructorArgument("wantedBestResult", _param);
        }
    }
}
