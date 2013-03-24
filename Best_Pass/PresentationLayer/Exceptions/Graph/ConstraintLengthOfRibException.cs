using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Graph
{
    public class ConstraintLengthOfRibException : Exception
    {
        public override string ToString()
        {
            return "Не допустимые размеры ребра";
        }
        public override string Message
        {
            get
            {
                return "Размер ребра должен быть от 1 до 2 000 000";
            }
        }
    }
}
