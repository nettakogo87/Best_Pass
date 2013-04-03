namespace Best_Pass.PresentationLayer
{
    partial class PersonsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PersonsTableViewGroupBox = new System.Windows.Forms.GroupBox();
            this.PersonsImageViewGroupBox = new System.Windows.Forms.GroupBox();
            this.PersonsTableDataGridView = new System.Windows.Forms.DataGridView();
            this.Track = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lenght = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeOfCrossingover = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeOfMutation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeOfSelection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberOfGeneration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstParent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecondParent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BestRib = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorstRib = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeOfTrack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PersonsTableViewGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PersonsTableDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // PersonsTableViewGroupBox
            // 
            this.PersonsTableViewGroupBox.Controls.Add(this.PersonsTableDataGridView);
            this.PersonsTableViewGroupBox.Location = new System.Drawing.Point(12, 12);
            this.PersonsTableViewGroupBox.Name = "PersonsTableViewGroupBox";
            this.PersonsTableViewGroupBox.Size = new System.Drawing.Size(760, 278);
            this.PersonsTableViewGroupBox.TabIndex = 0;
            this.PersonsTableViewGroupBox.TabStop = false;
            this.PersonsTableViewGroupBox.Text = "Табличное придставление";
            // 
            // PersonsImageViewGroupBox
            // 
            this.PersonsImageViewGroupBox.Location = new System.Drawing.Point(12, 296);
            this.PersonsImageViewGroupBox.Name = "PersonsImageViewGroupBox";
            this.PersonsImageViewGroupBox.Size = new System.Drawing.Size(760, 256);
            this.PersonsImageViewGroupBox.TabIndex = 1;
            this.PersonsImageViewGroupBox.TabStop = false;
            this.PersonsImageViewGroupBox.Text = "Визуальное представление";
            // 
            // PersonsTableDataGridView
            // 
            this.PersonsTableDataGridView.AllowUserToAddRows = false;
            this.PersonsTableDataGridView.AllowUserToDeleteRows = false;
            this.PersonsTableDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PersonsTableDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Track,
            this.Lenght,
            this.TypeOfCrossingover,
            this.TypeOfMutation,
            this.TypeOfSelection,
            this.NumberOfGeneration,
            this.Index,
            this.FirstParent,
            this.SecondParent,
            this.BestRib,
            this.WorstRib,
            this.TypeOfTrack});
            this.PersonsTableDataGridView.Location = new System.Drawing.Point(6, 19);
            this.PersonsTableDataGridView.Name = "PersonsTableDataGridView";
            this.PersonsTableDataGridView.ReadOnly = true;
            this.PersonsTableDataGridView.RowHeadersVisible = false;
            this.PersonsTableDataGridView.Size = new System.Drawing.Size(748, 253);
            this.PersonsTableDataGridView.TabIndex = 0;
            // 
            // Track
            // 
            this.Track.HeaderText = "Путь по вершинам";
            this.Track.Name = "Track";
            this.Track.ReadOnly = true;
            this.Track.Width = 180;
            // 
            // Lenght
            // 
            this.Lenght.HeaderText = "Длина";
            this.Lenght.Name = "Lenght";
            this.Lenght.ReadOnly = true;
            // 
            // TypeOfCrossingover
            // 
            this.TypeOfCrossingover.HeaderText = "Тип кроссинговера";
            this.TypeOfCrossingover.Name = "TypeOfCrossingover";
            this.TypeOfCrossingover.ReadOnly = true;
            this.TypeOfCrossingover.Width = 120;
            // 
            // TypeOfMutation
            // 
            this.TypeOfMutation.HeaderText = "Тип мутации";
            this.TypeOfMutation.Name = "TypeOfMutation";
            this.TypeOfMutation.ReadOnly = true;
            this.TypeOfMutation.Width = 120;
            // 
            // TypeOfSelection
            // 
            this.TypeOfSelection.HeaderText = "Тип селекции";
            this.TypeOfSelection.Name = "TypeOfSelection";
            this.TypeOfSelection.ReadOnly = true;
            this.TypeOfSelection.Width = 120;
            // 
            // NumberOfGeneration
            // 
            this.NumberOfGeneration.HeaderText = "Номер поколения";
            this.NumberOfGeneration.Name = "NumberOfGeneration";
            this.NumberOfGeneration.ReadOnly = true;
            // 
            // Index
            // 
            this.Index.HeaderText = "Порядковый номер";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // FirstParent
            // 
            this.FirstParent.HeaderText = "Первый родитель";
            this.FirstParent.Name = "FirstParent";
            this.FirstParent.ReadOnly = true;
            // 
            // SecondParent
            // 
            this.SecondParent.HeaderText = "Второй родитель";
            this.SecondParent.Name = "SecondParent";
            this.SecondParent.ReadOnly = true;
            // 
            // BestRib
            // 
            this.BestRib.HeaderText = "Лучшее ребро";
            this.BestRib.Name = "BestRib";
            this.BestRib.ReadOnly = true;
            // 
            // WorstRib
            // 
            this.WorstRib.HeaderText = "Худшее ребро";
            this.WorstRib.Name = "WorstRib";
            this.WorstRib.ReadOnly = true;
            // 
            // TypeOfTrack
            // 
            this.TypeOfTrack.HeaderText = "Тип особи";
            this.TypeOfTrack.Name = "TypeOfTrack";
            this.TypeOfTrack.ReadOnly = true;
            // 
            // PersonsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 564);
            this.Controls.Add(this.PersonsImageViewGroupBox);
            this.Controls.Add(this.PersonsTableViewGroupBox);
            this.Name = "PersonsWindow";
            this.Text = "Дерево поколений";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SizeChanged += new System.EventHandler(this.PersonsWindow_SizeChanged);
            this.PersonsTableViewGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PersonsTableDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox PersonsTableViewGroupBox;
        private System.Windows.Forms.GroupBox PersonsImageViewGroupBox;
        private System.Windows.Forms.DataGridView PersonsTableDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Track;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lenght;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeOfCrossingover;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeOfMutation;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeOfSelection;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberOfGeneration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstParent;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecondParent;
        private System.Windows.Forms.DataGridViewTextBoxColumn BestRib;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorstRib;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeOfTrack;
    }
}