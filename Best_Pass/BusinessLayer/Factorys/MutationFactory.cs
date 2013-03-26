using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Best_Pass.BusinessLayer.Modules;
using GeneticEngine.Mutation;
using GeneticEngine.ProxyOperation;
using Ninject;

namespace Best_Pass.BusinessLayer.Factorys
{
    public class MutationFactory
    {
        public IMutation CreateMutation(string mutationName)
        {
            IKernel kernel = new StandardKernel(new MutationModule());
            return kernel.Get<IMutation>(mutationName);
        }
        public IMutation CreateMutation(string mutationName, List<ProxyMutation> proxyMutationList, double bestResult)
        {
            IKernel kernel = new StandardKernel(new MutationModule(proxyMutationList, bestResult));
            return kernel.Get<IMutation>(mutationName);
        }
    }
}
