using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.PresentationLayer.Exceptions.Graph
{
    public class LimitingNumberOfNodeExceptions : Exception
    {
        public override string ToString()
        {
            return "Не верное количество вершин";
        }
        public override string Message
        {
            get
            {
                return "Количество вершин должно быть числом в пределах от 3 до 250";
            }
        }
    }
}
