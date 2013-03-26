using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Selection;
using GeneticEngine.ProxyOperation;
using Ninject.Modules;

namespace Best_Pass.BusinessLayer.Modules
{
    public class SelectionModule : NinjectModule
    {
        private List<ProxySelection> _proxySelectionList;
        public SelectionModule()
        {
        }
        public SelectionModule(List<ProxySelection> proxySelectionList)
        {
            _proxySelectionList = proxySelectionList;
        }
        public override void Load()
        {
            Bind<ISelection>().To<RankingSelection>().Named("RankingSelection");
            Bind<ISelection>().To<RouletteSelection>().Named("RouletteSelection");
            Bind<ISelection>().To<TournamentSelection>().Named("TournamentSelection");
            Bind<ISelection>().To<QualityCountsSelection>().Named("QualityCountsSelection").WithConstructorArgument("proxySelectionList", _proxySelectionList);
            Bind<ISelection>().To<SearchBestSelection>().Named("SearchBestSelection").WithConstructorArgument("proxySelectionList", _proxySelectionList);
        }
    }
}
