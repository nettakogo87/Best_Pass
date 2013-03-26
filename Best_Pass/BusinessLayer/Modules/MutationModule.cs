using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticEngine.Mutation;
using GeneticEngine.ProxyOperation;
using Ninject.Modules;

namespace Best_Pass.BusinessLayer.Modules
{
    public class MutationModule : NinjectModule
    {
        private List<ProxyMutation> _proxyMutationList;
        private double _bestResult;
        public MutationModule()
        {
        }
        public MutationModule(List<ProxyMutation> proxyMutationList, double bestResult)
        {
            _proxyMutationList = proxyMutationList;
            _bestResult = bestResult;
        }
        public override void Load()
        {
            Bind<IMutation>().To<FourPointMutation>().Named("FourPointMutation");
            Bind<IMutation>().To<TwoPointMutation>().Named("TwoPointMutation");
            Bind<IMutation>().To<NotRandomMutation>().Named("NotRandomMutation");
            Bind<IMutation>().To<QualityCountsMutation>().Named("QualityCountsMutation").WithConstructorArgument("proxyMutationList", _proxyMutationList).WithConstructorArgument("bestResult", _bestResult);
            Bind<IMutation>().To<SearchBestMutation>().Named("SearchBestMutation").WithConstructorArgument("proxyMutationList", _proxyMutationList);
        }
    }
}
