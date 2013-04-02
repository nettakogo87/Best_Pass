namespace Best_Pass.PresentationLayer
{
    partial class SettingsWindow
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
            this.ApplyButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.DBLocationLabel = new System.Windows.Forms.Label();
            this.DBLocationTextBox = new System.Windows.Forms.TextBox();
            this.DBLocationOpenButton = new System.Windows.Forms.Button();
            this.DbLocationFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(416, 229);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 0;
            this.ApplyButton.Text = "Применить";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(497, 229);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Отменить";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // DBLocationLabel
            // 
            this.DBLocationLabel.AutoSize = true;
            this.DBLocationLabel.Location = new System.Drawing.Point(12, 13);
            this.DBLocationLabel.Name = "DBLocationLabel";
            this.DBLocationLabel.Size = new System.Drawing.Size(158, 13);
            this.DBLocationLabel.TabIndex = 2;
            this.DBLocationLabel.Text = "Расположение Базы Данных:";
            // 
            // DBLocationTextBox
            // 
            this.DBLocationTextBox.Location = new System.Drawing.Point(173, 10);
            this.DBLocationTextBox.Name = "DBLocationTextBox";
            this.DBLocationTextBox.Size = new System.Drawing.Size(318, 20);
            this.DBLocationTextBox.TabIndex = 3;
            // 
            // DBLocationOpenButton
            // 
            this.DBLocationOpenButton.Location = new System.Drawing.Point(497, 8);
            this.DBLocationOpenButton.Name = "DBLocationOpenButton";
            this.DBLocationOpenButton.Size = new System.Drawing.Size(75, 23);
            this.DBLocationOpenButton.TabIndex = 4;
            this.DBLocationOpenButton.Text = "Открыть";
            this.DBLocationOpenButton.UseVisualStyleBackColor = true;
            this.DBLocationOpenButton.Click += new System.EventHandler(this.DBLocationOpenButton_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton;
            this.ClientSize = new System.Drawing.Size(584, 264);
            this.Controls.Add(this.DBLocationOpenButton);
            this.Controls.Add(this.DBLocationTextBox);
            this.Controls.Add(this.DBLocationLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.ApplyButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.Text = "Настройки";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label DBLocationLabel;
        private System.Windows.Forms.TextBox DBLocationTextBox;
        private System.Windows.Forms.Button DBLocationOpenButton;
        private System.Windows.Forms.FolderBrowserDialog DbLocationFolderBrowserDialog;
    }
}