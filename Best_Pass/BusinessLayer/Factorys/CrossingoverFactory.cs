using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Best_Pass.BusinessLayer.Modules;
using GeneticEngine.Crossingover;
using GeneticEngine.ProxyOperation;
using Ninject;

namespace Best_Pass.BusinessLayer.Factorys
{
    public class CrossingoverFactory
    {
        public ICrossingover CreateCrossingover(string crossingoverName)
        {
            IKernel kernel = new StandardKernel(new CrossingoverModule());
            return kernel.Get<ICrossingover>(crossingoverName);
        }
        public ICrossingover CreateCrossingover(string crossingoverName, List<ProxyCrossingover> proxyCrossingoverList, double bestResult)
        {
            IKernel kernel = new StandardKernel(new CrossingoverModule(proxyCrossingoverList, bestResult));
            return kernel.Get<ICrossingover>(crossingoverName);
        }
    }
}
