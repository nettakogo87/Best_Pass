using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Best_Pass.BusinessLayer.Modules;
using GeneticEngine.Selection;
using GeneticEngine.ProxyOperation;
using Ninject;

namespace Best_Pass.BusinessLayer.Factorys
{
    public class SelectionFactory
    {
        public ISelection CreateSelection(string selectionName)
        {
            IKernel kernel = new StandardKernel(new SelectionModule());
            return kernel.Get<ISelection>(selectionName);
        }
        public ISelection CreateSelection(string selectionName, List<ProxySelection> proxySelectionList)
        {
            IKernel kernel = new StandardKernel(new SelectionModule(proxySelectionList));
            return kernel.Get<ISelection>(selectionName);
        }
    }
}
