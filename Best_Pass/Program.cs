using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Best_Pass.PresentationLayer;

namespace Best_Pass
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainWindow view = new MainWindow();
            Application.Run(view);
        }
    }
}
