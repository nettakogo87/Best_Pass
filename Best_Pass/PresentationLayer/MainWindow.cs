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
using Microsoft.Glee;
using Microsoft.Glee.Drawing;
using GleeColor = Microsoft.Glee.Drawing.Color;

namespace Best_Pass.PresentationLayer
{
    public partial class MainWindow : Form, IView
    {
        private int _points;
        private Microsoft.Glee.GraphViewerGdi.GViewer _viewer;
        private GraphController _graphController;
        public MainWindow()
        {
            _viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            InitializeComponent();
            _graphController = new GraphController();
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
        }

        private void RenderDataGridGraph()
        {
            RibsDataGridView.Rows.Clear();
            IGraph graph = _graphController.GetGraph();
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
                _graphController.ChangeWeightOfRip(e.RowIndex, Convert.ToDouble(weight));
                RenderViewGraph();
            }
        }

        private void CreateGraph()
        {
            _points = Convert.ToInt16(AddPointsTextBox.Text);
            _graphController.CreateGraph(_points);
        }

        private void RenderViewGraph()
        {
            IGraph pureGraph = _graphController.GetGraph();
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

            SavePersonsButton.Location = new System.Drawing.Point(SavePersonsButton.Location.X, this.Height - 152); // 
            LoadPersonsButton.Location = new System.Drawing.Point(LoadPersonsButton.Location.X, this.Height - 152);
            ApplyPersonsButton.Location = new System.Drawing.Point(ApplyPersonsButton.Location.X, this.Height - 152);

            PersonsDataGridView.Height = this.Height - 164;

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
                _graphController.SaveGraph(SaveFileDialog.FileName);
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
                _graphController.OpenGraph(OpenFileDialog.FileName);
            }
            RenderDataGridGraph();
            RenderViewGraph();
        }
    }
}
