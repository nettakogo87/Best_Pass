using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Persons
{
    public class RestrictionNameOfCityException : Exception
    {
        public override string ToString()
        {
            return "Введен не существующий город";
        }
    }
}
