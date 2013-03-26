using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Crossingover;
using GeneticEngine.ProxyOperation;
using Ninject.Modules;

namespace Best_Pass.BusinessLayer.Modules
{
    public class CrossingoverModule : NinjectModule
    {
        private List<ProxyCrossingover> _proxyCrossingoverList;
        private double _bestResult;
        public CrossingoverModule()
        {
        }
        public CrossingoverModule(List<ProxyCrossingover> proxyCrossingoverList, double bestResult)
        {
            _proxyCrossingoverList = proxyCrossingoverList;
            _bestResult = bestResult;
        }
        public override void Load()
        {
            Bind<ICrossingover>().To<CyclicalCrossingover>().Named("CyclicalCrossingover");
            Bind<ICrossingover>().To<InversionCrossingover>().Named("InversionCrossingover");
            Bind<ICrossingover>().To<OnePointCrossingover>().Named("OnePointCrossingover");
            Bind<ICrossingover>().To<TwoPointCrossingover>().Named("TwoPointCrossingover");
            Bind<ICrossingover>().To<QualityCountsCrossingover>().Named("QualityCountsCrossingover").WithConstructorArgument("proxyCrossingoverList", _proxyCrossingoverList).WithConstructorArgument("bestResult", _bestResult);
            Bind<ICrossingover>().To<SearchBestCrossingover>().Named("SearchBestCrossingover").WithConstructorArgument("proxyCrossingoverList", _proxyCrossingoverList);
        }
    }
}
