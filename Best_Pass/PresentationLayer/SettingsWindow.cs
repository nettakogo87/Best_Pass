using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Best_Pass.BusinessLayer;

namespace Best_Pass.PresentationLayer
{
    public partial class SettingsWindow : Form
    {
        private UserSettings _userUserSettings;
        public SettingsWindow(UserSettings userUserSettings)
        {
            InitializeComponent();
            _userUserSettings = userUserSettings;
            DBLocationTextBox.Text = _userUserSettings.DbLocation;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            _userUserSettings.DbLocation = DBLocationTextBox.Text;
            this.Close();
        }

        private void DBLocationOpenButton_Click(object sender, EventArgs e)
        {
            DbLocationFolderBrowserDialog.SelectedPath = DBLocationTextBox.Text;
            if (DbLocationFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DBLocationTextBox.Text = DbLocationFolderBrowserDialog.SelectedPath;
            }
        }
    }
}
