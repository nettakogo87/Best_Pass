using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Algorithm
{
    public class RestrictionOfProbabilityException : Exception
    {
        public override string ToString()
        {
            return "Ограничение вероятности";
        }

        public override string Message
        {
            get
            {
                return "Вероятность должна быть целым чилом от 0 до 100";
            }
        }
    }
}
