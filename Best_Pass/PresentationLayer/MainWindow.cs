using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Best_Pass.BusinessLayer;
using Best_Pass.PresentationLayer.Exceptions.Algorithm;
using Best_Pass.PresentationLayer.Exceptions.Graph;
using Best_Pass.PresentationLayer.Exceptions.Persons;
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
        Timer _wTimer;

        public MainWindow()
        {
            InitializeComponent();

            _wTimer = new Timer();
            _wTimer.Interval = 500;
            _wTimer.Tick += new EventHandler(wTimer_Tick);

            _mainController = new MainController();
            _viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            RenderDataGridGraph();
//            RenderViewGraph();
            RenderDataGridPersons();
            RenderAlgorithm();
            RenderLaunchMode();
            RenderTableOfLaunch();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void addPointButton_Click(object sender, EventArgs e)
        {
            int countOfNode = 0;
            int scopeStart = 0;
            int scopeEnd = 0;
            try
            {
                countOfNode = Convert.ToInt16(AddPointsTextBox.Text.Trim());
                scopeStart = Convert.ToInt32(ScopeStartTextBox.Text.Trim());
                scopeEnd = Convert.ToInt32(ScopeEndTextBox.Text.Trim());
                if (1 > scopeStart || 2000000 < scopeEnd || scopeStart > scopeEnd)
                {
                    throw new ConstraintLengthOfRibException();
                }
                if (3 > countOfNode || 250 < countOfNode)
                {
                    throw new LimitingNumberOfNodeExceptions();
                }
                CreateGraph();
                RenderDataGridGraph();
                //            RenderViewGraph();
                RenderDataGridPersons();
            }
            catch (LimitingNumberOfNodeExceptions ex)
            {
                var result = MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            catch (ConstraintLengthOfRibException ex)
            {
                var result = MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            catch
            {
                var result = MessageBox.Show(@"Вы что-то ввели не правильно!", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void RenderDataGridGraph()
        {
            RibsDataGridView.Rows.Clear();
            IGraph graph = _mainController.GetGraph();
            for (int i = 0; i < graph.CountOfRibs; i++)
            {
                Rib rib = graph.GetRibByIndex(i);
                RibsDataGridView.Rows.Add(rib.StartNode, rib.EndNode, rib.Weight);
            }
        }

        private void RibsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                try
                {
                    string weight = RibsDataGridView[2, e.RowIndex].Value.ToString();
                    if (1 > Convert.ToInt32(weight) || 2000000 < Convert.ToInt32(weight))
                    {
                        throw new ConstraintLengthOfRibException();
                    }
                    _mainController.ChangeWeightOfRip(e.RowIndex, Convert.ToDouble(weight));
                    RenderViewGraph();
                }
                catch (ConstraintLengthOfRibException ex)
                {
                    var result = MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK,
                                                 MessageBoxIcon.Question);
                    RibsDataGridView[e.ColumnIndex, e.RowIndex].Value =
                        RibsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText;
                }
                catch
                {
                    var result = MessageBox.Show(@"Вы что-то ввели не правильно!", @"Ошибка", MessageBoxButtons.OK,
                                                 MessageBoxIcon.Question);
                    RibsDataGridView[e.ColumnIndex, e.RowIndex].Value =
                        RibsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText;
                }
                finally
                {
                    RibsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText = String.Empty;
                }
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
                Rib rib = pureGraph.GetRibByIndex(i);
                string startPoint = rib.StartNode.ToString();
                string endPoint = rib.EndNode.ToString();
                int weight = Convert.ToInt32(rib.Weight.ToString());
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

            TableOfLaunchgGroupBox.Width = this.Width - 60;
            TableOfLaunchDataGridView.Width = this.Width - 72;
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
            RenderDataGridPersons();
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
                try
                {
                    for (int i = 0; i < PersonsDataGridView.Rows.Count; i++)
                    {

                        if (PersonsDataGridView[1, i].Value.ToString() != "")
                        {
                            string[] person = PersonsDataGridView[1, i].Value.ToString().Trim().Split(separator);
                            int[] per = new int[person.Length];
                            if (per.Length != _mainController.GetGraph().CountOfNode)
                            {
                                throw new LimitingNumberOfCitiesException();
                            }
                            for (int j = 0; j < per.Length; j++)
                            {
                                per[j] = -1;
                            }
                            for (int j = 0; j < per.Length; j++)
                            {
                                if (0 > Convert.ToInt16(person[j]) || _mainController.GetGraph().CountOfNode < Convert.ToInt16(person[j]) || per.Contains(Convert.ToInt16(person[j])))
                                {
                                    throw new RestrictionNameOfCityException();
                                }
                                per[j] = Convert.ToInt16(person[j]);
                            }
                            persons.Add(per);
                        }
                    }
                    _mainController.CreatePersons(persons);
                }
                catch (LimitingNumberOfCitiesException ex)
                {
                    var result =
                        MessageBox.Show(
                            @"Количесто городов должно быть равным " + _mainController.GetGraph().CountOfNode + "!",
                            ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Question);
                    PersonsDataGridView[e.ColumnIndex, e.RowIndex].Value =
                        PersonsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText;
                }
                catch (RestrictionNameOfCityException ex)
                {
                    var result =
                        MessageBox.Show(
                            @"Номер города должен быть в пределах от 0 до " + (_mainController.GetGraph().CountOfNode - 1) + @", и не должет повторяться!",
                            ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Question);
                    PersonsDataGridView[e.ColumnIndex, e.RowIndex].Value =
                        PersonsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText;
                }
                catch
                {
                    var result = MessageBox.Show(@"Вы что-то ввели не правильно!", @"Ошибка", MessageBoxButtons.OK,
                                                 MessageBoxIcon.Question);
                    PersonsDataGridView[e.ColumnIndex, e.RowIndex].Value =
                        PersonsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText;
                }
                finally
                {
                    PersonsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText = String.Empty;
                }
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
            if (_mainController.GetConfig().AlgMode == (int)MainController.AlgorithmMode.Singl)
            {
                SingAlgorithmRadioButton.Checked = true;
            }
            if (_mainController.GetConfig().AlgMode == (int)MainController.AlgorithmMode.Quality)
            {
                ComparisonAlgorithmRadioButton.Checked = true;
            }
            if (_mainController.GetConfig().AlgMode == (int)MainController.AlgorithmMode.Search)
            {
                SearchBestAlgorithmRadioButton.Checked = true;
            }

            if (_mainController.GetConfig().FitnessFunction.GetName() == "BestReps")
            {
                BestRepsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetConfig().FitnessParam.ToString();
            }
            if (_mainController.GetConfig().FitnessFunction.GetName() == "GenerationCounter")
            {
                NumberOfGenerationsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetConfig().FitnessParam.ToString();
            }
            if (_mainController.GetConfig().FitnessFunction.GetName() == "ReachWantedResult")
            {
                NumberOfGenerationsRadioButton.Checked = true;
                BestRepsTextBox.Text = _mainController.GetConfig().FitnessParam.ToString();
            }

            if (_mainController.GetConfig().Mutation.GetName() == "FourPointMutation")
            {
                FourPointMCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Mutation.GetName() == "TwoPointMutation")
            {
                TwoPointMCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Mutation.GetName() == "NotRandomMutation")
            {
                NotRandomMCheckBox.Checked = true;
            }

            if (_mainController.GetConfig().Selection.GetName() == "RankingSelection")
            {
                RankingSCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Selection.GetName() == "RouletteSelection")
            {
                RouletteSCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Selection.GetName() == "TournamentSelection")
            {
                TournamentSCheckBox.Checked = true;
            }

            if (_mainController.GetConfig().Crossingover.GetName() == "CyclicalCrossingover")
            {
                CyclicalCCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Crossingover.GetName() == "InversionCrossingover")
            {
                InversionCCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Crossingover.GetName() == "OnePointCrossingover")
            {
                OnePointCCheckBox.Checked = true;
            }
            if (_mainController.GetConfig().Crossingover.GetName() == "TwoPointCrossingover")
            {
                TwoPointMCheckBox.Checked = true;
            }
            ProbablyOfMutationTextBox.Text = _mainController.GetConfig().ProbOfMutation.ToString();
            ProbablyOfCrossingoverTextBox.Text = _mainController.GetConfig().ProbOfCrossingover.ToString();
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
            ProbablyOfCrossingoverTextBox.Text = "100";
            ProbablyOfCrossingoverTextBox.Enabled = false;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void ComparisonAlgorithmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProbablyOfMutationTextBox.Text = "100";
            ProbablyOfMutationTextBox.Enabled = false;
            ProbablyOfCrossingoverTextBox.Text = "100";
            ProbablyOfCrossingoverTextBox.Enabled = false;

            ApplyAlgorithmButton.Enabled = true;
        }

        private void SingAlgorithmRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ProbablyOfMutationTextBox.Enabled = true;
            ProbablyOfCrossingoverTextBox.Enabled = true;
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
            try
            {
                int pCrossingover = Convert.ToInt16(ProbablyOfCrossingoverTextBox.Text);
                int pMutation = Convert.ToInt16(ProbablyOfMutationTextBox.Text);
                if (0 > pMutation || 100 < pMutation || 0 > pCrossingover || 100 < pCrossingover)
                {
                    throw new RestrictionOfProbabilityException();
                }
                _mainController.SetProbabilitys(pCrossingover, pMutation);
            }
            catch (RestrictionOfProbabilityException ex)
            {
                var result = MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK,
                             MessageBoxIcon.Question);
                ProbablyOfCrossingoverTextBox.Text = "100";
                ProbablyOfMutationTextBox.Text = "100";
            }
            catch
            {
                var result = MessageBox.Show(@"Вы что-то ввели не правильно!", @"Ошибка", MessageBoxButtons.OK,
                             MessageBoxIcon.Question);
                ProbablyOfCrossingoverTextBox.Text = "100";
                ProbablyOfMutationTextBox.Text = "100";
            }

            List<string> aliasMutant = new List<string>();
            if (FourPointMCheckBox.Checked)
            {
                aliasMutant.Add("FourPointMutation");
            }
            if (TwoPointMCheckBox.Checked)
            {
                aliasMutant.Add("TwoPointMutation");
            }
            if (NotRandomMCheckBox.Checked)
            {
                aliasMutant.Add("NotRandomMutation");
            }

            List<string> aliasSelection = new List<string>();
            if (TournamentSCheckBox.Checked)
            {
                aliasSelection.Add("TournamentSelection");
            }
            if (RankingSCheckBox.Checked)
            {
                aliasSelection.Add("RankingSelection");
            }
            if (RouletteSCheckBox.Checked)
            {
                aliasSelection.Add("RouletteSelection");
            }

            List<string> aliasCrossingover = new List<string>();
            if (CyclicalCCheckBox.Checked)
            {
                aliasCrossingover.Add("CyclicalCrossingover");
            }
            if (InversionCCheckBox.Checked)
            {
                aliasCrossingover.Add("InversionCrossingover");
            }
            if (OnePointCCheckBox.Checked)
            {
                aliasCrossingover.Add("OnePointCrossingover");
            }
            if (TwoPointCCheckBox.Checked)
            {
                aliasCrossingover.Add("TwoPointCrossingover");
            }

            string aliasFitnessFunction = "BestReps";
            double param = 0;
            try
            {
                if (NumberOfGenerationsRadioButton.Checked)
                {
                    aliasFitnessFunction = "GenerationCounter";
                    param = Convert.ToDouble(NumberOfGenerationsTextBox.Text);
                }
                if (BestRepsRadioButton.Checked)
                {
                    aliasFitnessFunction = "BestReps";
                    param = Convert.ToDouble(BestRepsTextBox.Text);
                }
                if (AchieveBetterRadioButton.Checked)
                {
                    aliasFitnessFunction = "ReachWantedResult";
                    param = Convert.ToDouble(AchieveBetterTextBox.Text);
                }
                if (param.CompareTo(0) == -1 || param.CompareTo(2000000) == 1)
                {
                    throw new ConstraintFitnessFunctionParameterException();
                }
            }
            catch (ConstraintFitnessFunctionParameterException ex)
            {
                var result = MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK,
                         MessageBoxIcon.Question);
                NumberOfGenerationsTextBox.Text = "10";
                BestRepsTextBox.Text = "10";
                AchieveBetterTextBox.Text = "10";
            }
            catch
            {
                var result = MessageBox.Show(@"Вы что-то ввели не правильно!", @"Ошибка", MessageBoxButtons.OK,
                         MessageBoxIcon.Question);
                NumberOfGenerationsTextBox.Text = "10";
                BestRepsTextBox.Text = "10";
                AchieveBetterTextBox.Text = "10";
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
        }

        private void RenderLaunchMode()
        {
            ConfigAlgDataGridView.Rows.Clear();
            ConfigAlgDataGridView.Rows.Add(ConfigAlgDataGridView.Rows.Count, _mainController.GetConfig().ConfigName, _mainController.GetConfig().CountOfReplays);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartButton.Enabled = false;
            StopButton.Enabled = true;
            WorkToolStripStatusLabel.Text = "Выполнение";

            _mainController.StartGA();
            _wTimer.Start();

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            _mainController.StopGA();
            StopWorkingPrepare();
        }

        private void wTimer_Tick(object sender, EventArgs e)
        {
            if (!_mainController.InspectGA())
            {
                if (WProgressBar.Maximum == WProgressBar.Value)
                {
                    WProgressBar.Value = 0;
                }
                WProgressBar.PerformStep();
            }
            else
            {
                StopWorkingPrepare();
            }
        }

        private void StopWorkingPrepare()
        {
            _wTimer.Stop();
            WProgressBar.Value = 0;
            WorkToolStripStatusLabel.Text = "Ожидание запуска";
            StartButton.Enabled = true;
            StopButton.Enabled = false;
            RenderTableOfLaunch();
        }

        private void ConfigAlgDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                _mainController.GetConfig().ConfigName = ConfigAlgDataGridView[1, e.RowIndex].Value.ToString();
                _mainController.GetConfig().CountOfReplays = Convert.ToInt16(ConfigAlgDataGridView[2, e.RowIndex].Value);
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

        private void PersonsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            PersonsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText =
                PersonsDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void RibsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            RibsDataGridView[e.ColumnIndex, e.RowIndex].ErrorText =
                RibsDataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void RenderTableOfLaunch()
        {
            _mainController.LoadGeneticsDataSet();
            TableOfLaunchDataGridView.Rows.Clear();
            for (int i = 0; i < _mainController.GeneticsDataSet.Launches.Rows.Count; i++)
            {
                DB_GeneticsDataSet.LaunchesRow lr = _mainController.GeneticsDataSet.Launches[i];
                TableOfLaunchDataGridView.Rows.Add(lr.StartTime, lr.EndTime, lr.OperationTime, lr.TypeOfCrossingover,
                                                   lr.TypeOfMutation, lr.TypeOfSelection, lr.FitnessFunction,
                                                   lr.NumberOfGenerations, lr.BestResult, lr.Id);
            }
            
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(_mainController.UserSettings);
            settingsWindow.ShowDialog();
            RenderTableOfLaunch();
        }

        private void DeleteLaunchButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < TableOfLaunchDataGridView.Rows.Count; i++)
            {
                if (TableOfLaunchDataGridView.Rows[i].Selected)
                {
                    _mainController.DeleteRowFromTableOfLaunch(i);
                }
            }
            RenderTableOfLaunch();
        }

        private void DetailPersonsButton_Click(object sender, EventArgs e)
        {
            Guid id = _mainController.GeneticsDataSet.Launches[0].Id;
            for (int i = 0; i < TableOfLaunchDataGridView.Rows.Count; i++)
            {
                if (TableOfLaunchDataGridView.Rows[i].Selected)
                {
                    id = new Guid(TableOfLaunchDataGridView[9, i].Value.ToString());
                }
            }
            PersonsWindow personsWindow = new PersonsWindow(_mainController.GeneticsDataSet, id);
            personsWindow.Show();
        }
    }
}
