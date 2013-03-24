using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Persons
{
    public class LimitingNumberOfCitiesException : Exception
    {
        public override string ToString()
        {
            return "Не верное количество городов";
        }
    }
}
