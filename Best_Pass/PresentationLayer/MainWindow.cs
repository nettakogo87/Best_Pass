using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Best_Pass.BusinessLayer;
using GeneticEngine.Graph;
using GeneticEngine.Track;
using Microsoft.Glee;
using Microsoft.Glee.Drawing;
using GleeColor = Microsoft.Glee.Drawing.Color;

namespace Best_Pass.PresentationLayer
{
    public partial class MainWindow : Form, IView
    {
        private int _points;
        private Microsoft.Glee.GraphViewerGdi.GViewer _viewer;
        private MainController _mainController;

        public MainWindow()
        {
            _viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            InitializeComponent();
            _mainController = new MainController();
            RenderDataGridGraph();
//            RenderViewGraph();
            RenderDataGridPersons();
            RenderAlgorithm();
            RenderLaunchMode();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void addPointButton_Click(object sender, EventArgs e)
        {
            CreateGraph();
            RenderDataGridGraph();
            RenderViewGraph();
            RenderDataGridPersons();
        }

        private void RenderDataGridGraph()
        {
            RibsDataGridView.Rows.Clear();
            IGraph graph = _mainController.GetGraph();
            for (int i = 0; i < graph.CountOfRibs; i++)
            {
                Rib rib = graph.GetRib(i);
                RibsDataGridView.Rows.Add(rib.StartNode, rib.EndNode, rib.Weight);
            }
        }

        private void RibsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                string weight = RibsDataGridView[2, e.RowIndex].Value.ToString();
                _mainController.ChangeWeightOfRip(e.RowIndex, Convert.ToDouble(weight));
                RenderViewGraph();
            }
        }

        private void CreateGraph()
        {
            int scopeStart = Convert.ToInt16(ScopeStartTextBox.Text);
            int scopeEnd = Convert.ToInt16(ScopeEndTextBox.Text);
            _points = Convert.ToInt16(AddPointsTextBox.Text);
            _mainController.CreateGraph(_points, scopeStart, scopeEnd);
            _mainController.CreatePersons();
        }

        private void RenderViewGraph()
        {
            IGraph pureGraph = _mainController.GetGraph();
            Graph graph = new Graph("graph");
            for (int i = 0; i < pureGraph.CountOfRibs; i++)
            {
                Rib rib = pureGraph.GetRib(i);
                string startPoint = rib.StartNode.ToString();
                string endPoint = rib.EndNode.ToString();
                int weight = Convert.ToInt16(rib.Weight.ToString());
                Microsoft.Glee.Drawing.Edge edge = graph.AddEdge(startPoint, endPoint);
                edge.Attr.Label = weight.ToString();
                edge.Attr.ArrowHeadAtSource = ArrowStyle.None;
                edge.Attr.LineWidth = weight;       // толщина показвает что ребро длинное
                edge.Attr.Color = GetGleeColor(Convert.ToInt16(startPoint) / Convert.ToInt16(endPoint) + Convert.ToInt16(startPoint)); // случайно подбираем цвет для ребер
            }
            graph.GraphAttr.AspectRatio = 1;
            _viewer.Graph = graph;
            _viewer.Dock = DockStyle.Fill;
            _viewer.NavigationVisible = false;
            _viewer.SaveButtonVisible = false;
            GraphPictureBox.Controls.Clear();
            GraphPictureBox.Controls.Add(_viewer);
        }


        private GleeColor GetGleeColor(int i)
        {
            switch (i)
            {
                case 1:
                    return GleeColor.YellowGreen;
                    break;
                case 2:
                    return GleeColor.Yellow;
                    break;
                case 3:
                    return GleeColor.Tomato;
                    break;
                case 4:
                    return GleeColor.Turquoise;
                    break;
                case 5:
                    return GleeColor.DarkGray;
                    break;
                case 6:
                    return GleeColor.Chocolate;
                    break;
                case 7:
                    return GleeColor.Blue;
                    break;
                case 8:
                    return GleeColor.BurlyWood;
                    break;
                case 9:
                    return GleeColor.Crimson;
                    break;
                case 10:
                    return GleeColor.Coral;
                    break;
                case 11:
                    return GleeColor.DarkGreen;
                    break;
                case 12:
                    return GleeColor.DarkKhaki;
                    break;
                case 13:
                    return GleeColor.DarkMagenta;
                    break;
                default:
                    return GleeColor.DarkOrange;
                    break;
            }
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            SettingsTabControl.Height = this.Height - 100;  // закладка
            SettingsTabControl.Width = this.Width - 40;

            ControlGraphGroupBox.Location = new System.Drawing.Point(ControlGraphGroupBox.Location.X, this.Height - 215); // нижнаяя панель

            GraphicGraphGroupBox.Height = this.Height - 227; // панель с рисованым графом
//            GraphicGraphGroupBox.Width = this.Width - 292;
             
            GraphPictureBox.Height = this.Height - 252;      // рисованный граф
//            GraphPictureBox.Width = this.Width - 304;

            TechnicalGraphGroupBox.Height = this.Height - 138;  // панель с табличным представление графа
            RibsDataGridView.Height = this.Height - 163;        // таблично представление графа

            SavePersonsButton.Location = new System.Drawing.Point(SavePersonsButton.Location.X, this.Height - 167); 
            LoadPersonsButton.Location = new System.Drawing.Point(LoadPersonsButton.Location.X, this.Height - 167);
            AddPersonsButton.Location = new System.Drawing.Point(AddPersonsButton.Location.X, this.Height - 167);
            DeletePersonsButton.Location = new System.Drawing.Point(DeletePersonsButton.Location.X, this.Height - 167);

            PersonsGroupBox.Height = this.Height - 138;
            PersonsDataGridView.Height = this.Height - 212;

            GraphViewGroupBox.Height = this.Height - 138;
            GraphViewGroupBox.Width = this.Width - 510;

            GraphViewPictureBox.Height = this.Height - 163;
            GraphViewPictureBox.Width = this.Width - 522;
        }

        private void SaveGraphButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo graphDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Graph");
            if (!graphDirectory.Exists)
            {
                graphDirectory.Create();
            }
            SaveFileDialog.InitialDirectory = graphDirectory.ToString();
            SaveFileDialog.DefaultExt = "xml";
            SaveFileDialog.Filter = "XML (*.xml)|*.xml|All files (*.*)|*.*";
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.SaveGraph(SaveFileDialog.FileName);
            }
        }

        private void LoadGraphButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo graphDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Graph");
            if (!graphDirectory.Exists)
            {
                graphDirectory.Create();
            }
            OpenFileDialog.InitialDirectory = graphDirectory.ToString();
            OpenFileDialog.DefaultExt = "xml";
            OpenFileDialog.Filter = "XML (*.xml)|*.xml|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.OpenGraph(OpenFileDialog.FileName);
            }
            RenderDataGridGraph();
//            RenderViewGraph();
        }

        private void AddPersonsButton_Click(object sender, EventArgs e)
        {
            PersonsDataGridView.Rows.Add(PersonsDataGridView.Rows.Count, "");
        }

        private void DeletePersonsButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PersonsDataGridView.Rows.Count; i++)
            {
                if (PersonsDataGridView.Rows[i].Selected)
                {
                    PersonsDataGridView.Rows.RemoveAt(i);
                }
            }
            for (int i = 0; i < PersonsDataGridView.Rows.Count; i++)
            {
                PersonsDataGridView[0, i].Value = i;
            }
        }

        private void PersonsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (PersonsDataGridView.Rows.Count > 0)
            {
                List<int[]> persons = new List<int[]>();
                char[] separator = {' '};
                for (int i = 0; i < PersonsDataGridView.Rows.Count; i++)
                {
                    if (PersonsDataGridView[1, i].Value.ToString() != "")
                    {
                        string[] person = PersonsDataGridView[1, i].Value.ToString().Trim().Split(separator);
                        int[] per = new int[person.Length];
                        for (int j = 0; j < per.Length; j++)
                        {
                            per[j] = Convert.ToInt16(person[j]);
                        }
                        persons.Add(per);
                    }
                }
                _mainController.CreatePersons(persons);
            }
        }

        private void SavePersonsButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo graphDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Persons");
            if (!graphDirectory.Exists)
            {
                graphDirectory.Create();
            }
            SaveFileDialog.InitialDirectory = graphDirectory.ToString();
            SaveFileDialog.DefaultExt = "txt";
            SaveFileDialog.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.SavePersons(SaveFileDialog.FileName);
            }
        }

        private void LoadPersonsButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo graphDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Persons");
            if (!graphDirectory.Exists)
            {
                graphDirectory.Create();
            }
            OpenFileDialog.InitialDirectory = graphDirectory.ToString();
            OpenFileDialog.DefaultExt = "txt";
            OpenFileDialog.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.LoadPersons(OpenFileDialog.FileName);
            }
            RenderDataGridPersons();
        }

        private void RenderDataGridPersons()
        {
            PersonsDataGridView.Rows.Clear();
            AbstractTrack[] tracks = _mainController.GetPersons();
            for (int i = 0; i < tracks.Length; i++)
            {
                string person = "";
                for (int j = 0; j < tracks[i].Genotype.Length; j++)
                {
                    person += tracks[i].Genotype[j].ToString() + " ";
                }
                PersonsDataGridView.Rows.Add(i, person);
            }
        }

        private void RenderAlgorithm()
        {
            if (_mainController.GetAlgorithmMode() == (int)MainController.AlgorithmMode.Singl)
            {
                SingAlgorithmRadioButton.Checked = true;
            }
            if (_mainController.GetAlgorithmMode() == (int)MainController.AlgorithmMode.Quality)
            {
                ComparisonAlgorithmRadioButton.Checked = true;
            }
            if (_mainController.GetAlgorithmMode() == (int)MainController.AlgorithmMode.Search)
            {
                SearchBestAlgorithmRadioButton.Checked = true;
            }

            if (_mainController.GetFitnessFunction().GetName() == "BestReps")
            {
                BestRepsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetFitnessFunctionParametr().ToString();
            }
            if (_mainController.GetFitnessFunction().GetName() == "GenerationCounter")
            {
                NumberOfGenerationsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetFitnessFunctionParametr().ToString();
            }
            if (_mainController.GetFitnessFunction().GetName() == "ReachWantedResult")
            {
                NumberOfGenerationsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetFitnessFunctionParametr().ToString();
            }

            if (_mainController.GetMutation().GetName() == "FourPointMutation")
            {
                FourPointMCheckBox.Checked = true;
            }
            if (_mainController.GetMutation().GetName() == "TwoPointMutation")
            {
                TwoPointMCheckBox.Checked = true;
            }
            if (_mainController.GetMutation().GetName() == "NotRandomMutation")
            {
                NotRandomMCheckBox.Checked = true;
            }

            if (_mainController.GetSelection().GetName() == "RankingSelection")
            {
                RankingSCheckBox.Checked = true;
            }
            if (_mainController.GetSelection().GetName() == "RouletteSelection")
            {
                RouletteSCheckBox.Checked = true;
            }
            if (_mainController.GetSelection().GetName() == "TournamentSelection")
            {
                TournamentSCheckBox.Checked = true;
            }

            if (_mainController.GetCrossingover().GetName() == "CyclicalCrossingover")
            {
                CyclicalCCheckBox.Checked = true;
            }
            if (_mainController.GetCrossingover().GetName() == "InversionCrossingover")
            {
                InversionCCheckBox.Checked = true;
            }
            if (_mainController.GetCrossingover().GetName() == "OnePointCrossingover")
            {
                OnePointCCheckBox.Checked = true;
            }
            if (_mainController.GetCrossingover().GetName() == "TwoPointCrossingover")
            {
                TwoPointMCheckBox.Checked = true;
            }
            ProbablyOfMutationTextBox.Text = _mainController.GetPMutation().ToString();
            ProbablyOfSelectionTextBox.Text = _mainController.GetPSelection().ToString();
        }

        private void NumberOfGenerationsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            NumberOfGenerationsLabel.Enabled = true;
            NumberOfGenerationsTextBox.Enabled = true;
            BestRepsLabel.Enabled = false;
            BestRepsTextBox.Enabled = false;
            AchieveBetterLabel.Enabled = false;
            AchieveBetterTextBox.Enabled = false;
        }

        private void BestRepsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            NumberOfGenerationsLabel.Enabled = false;
            NumberOfGenerationsTextBox.Enabled = false;
            BestRepsLabel.Enabled = true;
            BestRepsTextBox.Enabled = true;
            AchieveBetterLabel.Enabled = false;
            AchieveBetterTextBox.Enabled = false;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void AchieveBetterRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            NumberOfGenerationsLabel.Enabled = false;
            NumberOfGenerationsTextBox.Enabled = false;
            BestRepsLabel.Enabled = false;
            BestRepsTextBox.Enabled = false;
            AchieveBetterLabel.Enabled = true;
            AchieveBetterTextBox.Enabled = true;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void SearchBestAlgorithmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProbablyOfMutationTextBox.Text = "100";
            ProbablyOfMutationTextBox.Enabled = false;
            ProbablyOfSelectionTextBox.Text = "100";
            ProbablyOfSelectionTextBox.Enabled = false;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void ComparisonAlgorithmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProbablyOfMutationTextBox.Text = "100";
            ProbablyOfMutationTextBox.Enabled = false;
            ProbablyOfSelectionTextBox.Text = "100";
            ProbablyOfSelectionTextBox.Enabled = false;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void SingAlgorithmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProbablyOfMutationTextBox.Enabled = true;
            ProbablyOfSelectionTextBox.Enabled = true;
            if (SingAlgorithmRadioButton.Checked)
            {
                FourPointMCheckBox.Checked = false;
                TwoPointMCheckBox.Checked = false;
                NotRandomMCheckBox.Checked = true;

                RankingSCheckBox.Checked = false;
                RouletteSCheckBox.Checked = false;
                TournamentSCheckBox.Checked = true;

                InversionCCheckBox.Checked = false;
                OnePointCCheckBox.Checked = false;
                TwoPointCCheckBox.Checked = false;
                CyclicalCCheckBox.Checked = true;
            }

            ApplyAlgorithmButton.Enabled = true;
        }

        private void FourPointMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && FourPointMCheckBox.Checked)
            {
                TwoPointMCheckBox.Checked = false;
                NotRandomMCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void TwoPointMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && TwoPointMCheckBox.Checked)
            {
                FourPointMCheckBox.Checked = false;
                NotRandomMCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void NotRandomMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && NotRandomMCheckBox.Checked)
            {
                TwoPointMCheckBox.Checked = false;
                FourPointMCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void TournamentSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && TournamentSCheckBox.Checked)
            {
                RankingSCheckBox.Checked = false;
                RouletteSCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void RankingSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && RankingSCheckBox.Checked)
            {
                TournamentSCheckBox.Checked = false;
                RouletteSCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void RouletteSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && RouletteSCheckBox.Checked)
            {
                TournamentSCheckBox.Checked = false;
                RankingSCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void CyclicalCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && CyclicalCCheckBox.Checked)
            {
                InversionCCheckBox.Checked = false;
                OnePointCCheckBox.Checked = false;
                TwoPointCCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void InversionCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && InversionCCheckBox.Checked)
            {
                CyclicalCCheckBox.Checked = false;
                OnePointCCheckBox.Checked = false;
                TwoPointCCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void OnePointCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && OnePointCCheckBox.Checked)
            {
                InversionCCheckBox.Checked = false;
                CyclicalCCheckBox.Checked = false;
                TwoPointCCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void TwoPointCCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (SingAlgorithmRadioButton.Checked && TwoPointCCheckBox.Checked)
            {
                InversionCCheckBox.Checked = false;
                OnePointCCheckBox.Checked = false;
                CyclicalCCheckBox.Checked = false;
            }
            ApplyAlgorithmButton.Enabled = true;
        }

        private void ApplyAlgorithmButton_Click(object sender, EventArgs e)
        {
            List<int> aliasMutant = new List<int>();
            if (FourPointMCheckBox.Checked)
            {
                aliasMutant.Add(1);
            }
            if (TwoPointMCheckBox.Checked)
            {
                aliasMutant.Add(2);
            }
            if (NotRandomMCheckBox.Checked)
            {
                aliasMutant.Add(3);
            }

            List<int> aliasSelection = new List<int>();
            if (TournamentSCheckBox.Checked)
            {
                aliasSelection.Add(1);
            }
            if (RankingSCheckBox.Checked)
            {
                aliasSelection.Add(2);
            }
            if (RouletteSCheckBox.Checked)
            {
                aliasSelection.Add(3);
            }

            List<int> aliasCrossingover = new List<int>();
            if (CyclicalCCheckBox.Checked)
            {
                aliasCrossingover.Add(1);
            }
            if (InversionCCheckBox.Checked)
            {
                aliasCrossingover.Add(2);
            }
            if (OnePointCCheckBox.Checked)
            {
                aliasCrossingover.Add(3);
            }
            if (TwoPointCCheckBox.Checked)
            {
                aliasCrossingover.Add(4);
            }

            int aliasFitnessFunction = 0;
            double param = 0;
            if (NumberOfGenerationsRadioButton.Checked)
            {
                aliasFitnessFunction = 1;
                param = Convert.ToDouble(NumberOfGenerationsTextBox.Text);
            }
            if (BestRepsRadioButton.Checked)
            {
                aliasFitnessFunction = 2;
                param = Convert.ToDouble(BestRepsTextBox.Text);
            }
            if (AchieveBetterRadioButton.Checked)
            {
                aliasFitnessFunction = 3;
                param = Convert.ToDouble(AchieveBetterTextBox.Text);
            }

            if (SingAlgorithmRadioButton.Checked)
            {
                _mainController.CreateMutation(MainController.AlgorithmMode.Singl, aliasMutant.ToArray());
                _mainController.CreateSelection(MainController.AlgorithmMode.Singl, aliasSelection.ToArray());
                _mainController.CreateCrossingover(MainController.AlgorithmMode.Singl, aliasCrossingover.ToArray());
                _mainController.CreateFitnessFunction(aliasFitnessFunction, param);
            }
            if (ComparisonAlgorithmRadioButton.Checked)
            {
                _mainController.CreateMutation(MainController.AlgorithmMode.Quality, aliasMutant.ToArray());
                _mainController.CreateSelection(MainController.AlgorithmMode.Quality, aliasSelection.ToArray());
                _mainController.CreateCrossingover(MainController.AlgorithmMode.Quality, aliasCrossingover.ToArray());
                _mainController.CreateFitnessFunction(aliasFitnessFunction, param);
            }
            if (SearchBestAlgorithmRadioButton.Checked)
            {
                _mainController.CreateMutation(MainController.AlgorithmMode.Search, aliasMutant.ToArray());
                _mainController.CreateSelection(MainController.AlgorithmMode.Search, aliasSelection.ToArray());
                _mainController.CreateCrossingover(MainController.AlgorithmMode.Search, aliasCrossingover.ToArray());
                _mainController.CreateFitnessFunction(aliasFitnessFunction, param);
            }
            int pSelection = Convert.ToInt16(ProbablyOfSelectionTextBox.Text);
            int pMutation = Convert.ToInt16(ProbablyOfMutationTextBox.Text);
            _mainController.SetProbabilitys(pSelection, pMutation);
        }

        private void RenderLaunchMode()
        {
            ConfigAlgDataGridView.Rows.Clear();
            ConfigAlgDataGridView.Rows.Add(ConfigAlgDataGridView.Rows.Count, _mainController.GetConfigName(), _mainController.GetCountOfReplays());
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            WorkToolStripProgressBar.Value = 5;
            _mainController.StartGA();
        }

        private void ConfigAlgDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                _mainController.SetConfigName(ConfigAlgDataGridView[1, e.RowIndex].Value.ToString());
                _mainController.SetCountOfReplays(Convert.ToInt16(ConfigAlgDataGridView[2, e.RowIndex].Value));
            }
        }

        private void SaveConfigButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo confDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Configuration");
            if (!confDirectory.Exists)
            {
                confDirectory.Create();
            }
            SaveFileDialog.InitialDirectory = confDirectory.ToString();
            SaveFileDialog.DefaultExt = "txt";
            SaveFileDialog.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.SaveConfiguration(SaveFileDialog.FileName);
            }
        }

        private void AddConfigButton_Click(object sender, EventArgs e)
        {
            DirectoryInfo confDirectory = new DirectoryInfo(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Configuration");
            if (!confDirectory.Exists)
            {
                confDirectory.Create();
            }
            OpenFileDialog.InitialDirectory = confDirectory.ToString();
            OpenFileDialog.DefaultExt = "txt";
            OpenFileDialog.Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mainController.LoadConfiguration(OpenFileDialog.FileName);
            }
            RenderDataGridGraph();
//            RenderViewGraph();
            RenderDataGridPersons();
            RenderAlgorithm();
            RenderLaunchMode();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {

        }
    }
}
