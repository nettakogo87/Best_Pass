using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Best_Pass.PresentationLayer
{
    public partial class PersonsWindow : Form
    {
        private DB_GeneticsDataSet _geneticsDataSet;
        private Guid _launchId;
        public PersonsWindow(DB_GeneticsDataSet geneticsDataSet, Guid launchId)
        {
            InitializeComponent();
            _geneticsDataSet = geneticsDataSet;
            _launchId = launchId;

            foreach (DB_GeneticsDataSet.PersonsRow pr in geneticsDataSet.Persons)
            {
                if (pr.Launch == _launchId)
                {
                    PersonsTableDataGridView.Rows.Add(pr.Track, pr.Length, pr.TypeOfCrossingover, pr.TypeOfMutation,
                                                      pr.TypeOfSelection, pr.NumberOfGeneration, pr.Item, pr.FirstParent,
                                                      pr.SecondParent, pr.BestRip, pr.WorstRip, pr.TypeOfTrack);
                }
            }
            
//            Graphics dc = CreateGraphics();
//            Show();
//            Pen bluePen = new Pen(Color.Blue, 3);
//            dc.DrawRectangle(bluePen, 0, 0, 50, 50);
//            Pen redPen = new Pen(Color.Red, 2);
//            dc.DrawEllipse(redPen, 0, 50, 80, 60);
        }

        private void PersonsWindow_SizeChanged(object sender, EventArgs e)
        {
            PersonsTableViewGroupBox.Width = Width - 40;
            PersonsTableViewGroupBox.Height = Height/2 - 40;
            PersonsTableDataGridView.Width = Width - 52;
            PersonsTableDataGridView.Height = Height/2 - 65;

            PersonsImageViewGroupBox.Width = Width - 40;
            PersonsImageViewGroupBox.Location = new Point(PersonsImageViewGroupBox.Location.X, Height/2 - 25);
            PersonsImageViewGroupBox.Height = Height / 2 - 20;
        }
    }
}
