using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Algorithm
{
    public class ConstraintFitnessFunctionParameterException : Exception
    {
        public override string ToString()
        {
            return "Ограничение параметра фитнес-функции";
        }

        public override string Message
        {
            get
            {
                return "Параметр фитнес функции должен быть положительным числом, не более 2 000 000";
            }
        }
    }
}
