using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Best_Pass.BusinessLayer.Modules;
using GeneticEngine.FitnessFunction;
using Ninject;

namespace Best_Pass.BusinessLayer.Factorys
{
    public class FitnessFunctionFactory
    {
        public IFitnessFunction CreateFitnessFunction(string fitnessFunctionName, double param)
        {
            IKernel kernel = new StandardKernel(new FitnessFunctionModule(param));
            return kernel.Get<IFitnessFunction>(fitnessFunctionName);
        }
    }
}
