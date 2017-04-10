namespace AgeGrading
{
    partial class CompareMissingMembersDlg
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnNextAgeGroup = new System.Windows.Forms.Button();
            this.labelFirstAgeGroup = new System.Windows.Forms.Label();
            this.numAgeGroup = new System.Windows.Forms.NumericUpDown();
            this.labelAgeGradingResults = new System.Windows.Forms.Label();
            this.dataGridView = new AgeGrading.MyDataGridView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dataGridViewHighlight = new System.Windows.Forms.DataGridView();
            this.dataGridViewMissingMembers = new AgeGrading.MyDataGridView();
            this.btnClose = new System.Windows.Forms.Button();
            this.labelMissingMembers = new System.Windows.Forms.Label();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnHighlightMatchingNames = new System.Windows.Forms.Button();
            this.chkLastNamesOnly = new System.Windows.Forms.CheckBox();
            this.btnReduceByAge = new System.Windows.Forms.Button();
            this.btnHighlightNextMatch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numAgeGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHighlight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMissingMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNextAgeGroup
            // 
            this.btnNextAgeGroup.AutoSize = true;
            this.btnNextAgeGroup.Location = new System.Drawing.Point(200, 12);
            this.btnNextAgeGroup.Name = "btnNextAgeGroup";
            this.btnNextAgeGroup.Size = new System.Drawing.Size(75, 23);
            this.btnNextAgeGroup.TabIndex = 32;
            this.btnNextAgeGroup.Text = "N&ext Group";
            this.btnNextAgeGroup.UseVisualStyleBackColor = true;
            this.btnNextAgeGroup.Click += new System.EventHandler(this.btnNextAgeGroup_Click);
            // 
            // labelFirstAgeGroup
            // 
            this.labelFirstAgeGroup.AutoSize = true;
            this.labelFirstAgeGroup.Location = new System.Drawing.Point(16, 17);
            this.labelFirstAgeGroup.Name = "labelFirstAgeGroup";
            this.labelFirstAgeGroup.Size = new System.Drawing.Size(78, 13);
            this.labelFirstAgeGroup.TabIndex = 31;
            this.labelFirstAgeGroup.Text = "1st Age Group:";
            // 
            // numAgeGroup
            // 
            this.numAgeGroup.Location = new System.Drawing.Point(100, 13);
            this.numAgeGroup.Minimum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numAgeGroup.Name = "numAgeGroup";
            this.numAgeGroup.Size = new System.Drawing.Size(94, 20);
            this.numAgeGroup.TabIndex = 30;
            this.numAgeGroup.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numAgeGroup.ValueChanged += new System.EventHandler(this.numAgeGroup_ValueChanged);
            // 
            // labelAgeGradingResults
            // 
            this.labelAgeGradingResults.AutoSize = true;
            this.labelAgeGradingResults.Location = new System.Drawing.Point(16, 44);
            this.labelAgeGradingResults.Name = "labelAgeGradingResults";
            this.labelAgeGradingResults.Size = new System.Drawing.Size(107, 13);
            this.labelAgeGradingResults.TabIndex = 34;
            this.labelAgeGradingResults.Text = "A&ge Grading Results:";
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(433, 437);
            this.dataGridView.TabIndex = 33;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(12, 69);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dataGridView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.dataGridViewMissingMembers);
            this.splitContainer.Size = new System.Drawing.Size(854, 437);
            this.splitContainer.SplitterDistance = 433;
            this.splitContainer.TabIndex = 35;
            this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_SplitterMoved);
            // 
            // dataGridViewHighlight
            // 
            this.dataGridViewHighlight.AllowUserToAddRows = false;
            this.dataGridViewHighlight.AllowUserToDeleteRows = false;
            this.dataGridViewHighlight.AllowUserToResizeRows = false;
            this.dataGridViewHighlight.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewHighlight.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewHighlight.Location = new System.Drawing.Point(263, 512);
            this.dataGridViewHighlight.Name = "dataGridViewHighlight";
            this.dataGridViewHighlight.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewHighlight.Size = new System.Drawing.Size(240, 23);
            this.dataGridViewHighlight.TabIndex = 33;
            // 
            // dataGridViewMissingMembers
            // 
            this.dataGridViewMissingMembers.AllowUserToAddRows = false;
            this.dataGridViewMissingMembers.AllowUserToDeleteRows = false;
            this.dataGridViewMissingMembers.AllowUserToResizeRows = false;
            this.dataGridViewMissingMembers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewMissingMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMissingMembers.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewMissingMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMissingMembers.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewMissingMembers.Name = "dataGridViewMissingMembers";
            this.dataGridViewMissingMembers.ReadOnly = true;
            this.dataGridViewMissingMembers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewMissingMembers.Size = new System.Drawing.Size(417, 437);
            this.dataGridViewMissingMembers.TabIndex = 34;
            this.dataGridViewMissingMembers.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewMissingMembers_CellContentDoubleClick);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(791, 512);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 36;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // labelMissingMembers
            // 
            this.labelMissingMembers.AutoSize = true;
            this.labelMissingMembers.Location = new System.Drawing.Point(446, 44);
            this.labelMissingMembers.Name = "labelMissingMembers";
            this.labelMissingMembers.Size = new System.Drawing.Size(114, 13);
            this.labelMissingMembers.TabIndex = 37;
            this.labelMissingMembers.Text = "&Missing Members: ({0})";
            // 
            // btnRestart
            // 
            this.btnRestart.AutoSize = true;
            this.btnRestart.Location = new System.Drawing.Point(281, 12);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(75, 23);
            this.btnRestart.TabIndex = 38;
            this.btnRestart.Text = "&Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnHighlightMatchingNames
            // 
            this.btnHighlightMatchingNames.AutoSize = true;
            this.btnHighlightMatchingNames.Location = new System.Drawing.Point(362, 12);
            this.btnHighlightMatchingNames.Name = "btnHighlightMatchingNames";
            this.btnHighlightMatchingNames.Size = new System.Drawing.Size(141, 23);
            this.btnHighlightMatchingNames.TabIndex = 39;
            this.btnHighlightMatchingNames.Text = "&Highlight Matching Names";
            this.btnHighlightMatchingNames.UseVisualStyleBackColor = true;
            this.btnHighlightMatchingNames.Click += new System.EventHandler(this.btnHighlightMatchingNames_Click);
            // 
            // chkLastNamesOnly
            // 
            this.chkLastNamesOnly.AutoSize = true;
            this.chkLastNamesOnly.Checked = true;
            this.chkLastNamesOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLastNamesOnly.Location = new System.Drawing.Point(510, 15);
            this.chkLastNamesOnly.Name = "chkLastNamesOnly";
            this.chkLastNamesOnly.Size = new System.Drawing.Size(106, 17);
            this.chkLastNamesOnly.TabIndex = 40;
            this.chkLastNamesOnly.Text = "&Last Names Only";
            this.chkLastNamesOnly.UseVisualStyleBackColor = true;
            // 
            // btnReduceByAge
            // 
            this.btnReduceByAge.AutoSize = true;
            this.btnReduceByAge.Location = new System.Drawing.Point(623, 12);
            this.btnReduceByAge.Name = "btnReduceByAge";
            this.btnReduceByAge.Size = new System.Drawing.Size(91, 23);
            this.btnReduceByAge.TabIndex = 41;
            this.btnReduceByAge.Text = "Reduce by Age";
            this.btnReduceByAge.UseVisualStyleBackColor = true;
            this.btnReduceByAge.Click += new System.EventHandler(this.btnReduceByAge_Click);
            // 
            // btnHighlightNextMatch
            // 
            this.btnHighlightNextMatch.AutoSize = true;
            this.btnHighlightNextMatch.Location = new System.Drawing.Point(720, 12);
            this.btnHighlightNextMatch.Name = "btnHighlightNextMatch";
            this.btnHighlightNextMatch.Size = new System.Drawing.Size(116, 23);
            this.btnHighlightNextMatch.TabIndex = 42;
            this.btnHighlightNextMatch.Text = "Highlight Next Match";
            this.btnHighlightNextMatch.UseVisualStyleBackColor = true;
            this.btnHighlightNextMatch.Click += new System.EventHandler(this.btnHighlightNextMatch_Click);
            // 
            // CompareMissingMembersDlg
            // 
            this.AcceptButton = this.btnNextAgeGroup;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(878, 547);
            this.Controls.Add(this.dataGridViewHighlight);
            this.Controls.Add(this.btnHighlightNextMatch);
            this.Controls.Add(this.btnReduceByAge);
            this.Controls.Add(this.chkLastNamesOnly);
            this.Controls.Add(this.btnHighlightMatchingNames);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.labelMissingMembers);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.labelAgeGradingResults);
            this.Controls.Add(this.btnNextAgeGroup);
            this.Controls.Add(this.labelFirstAgeGroup);
            this.Controls.Add(this.numAgeGroup);
            this.Name = "CompareMissingMembersDlg";
            this.Text = "Compare Missing Members";
            ((System.ComponentModel.ISupportInitialize)(this.numAgeGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHighlight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMissingMembers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNextAgeGroup;
        private System.Windows.Forms.Label labelFirstAgeGroup;
        private System.Windows.Forms.NumericUpDown numAgeGroup;
        private System.Windows.Forms.Label labelAgeGradingResults;
        private MyDataGridView dataGridView;
        private System.Windows.Forms.SplitContainer splitContainer;
        private MyDataGridView dataGridViewMissingMembers;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label labelMissingMembers;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnHighlightMatchingNames;
        private System.Windows.Forms.CheckBox chkLastNamesOnly;
        private System.Windows.Forms.Button btnReduceByAge;
        private System.Windows.Forms.Button btnHighlightNextMatch;
        private System.Windows.Forms.DataGridView dataGridViewHighlight;
    }
}