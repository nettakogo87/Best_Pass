using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best_Pass.BusinessLayer
{
    public class UserSettings
    {
        private string _dbLocation;
        public UserSettings()
        {
            DbLocation = @"D:\Учеба\Универ\6 курс\Диплом\Программа\GeneticEngine\GeneticEngine";
        }

        public string DbLocation
        {
            get { return _dbLocation; }
            set 
            { 
                _dbLocation = value;
                AppDomain.CurrentDomain.SetData("DataDirectory", _dbLocation);
            }
        }
    }
}
