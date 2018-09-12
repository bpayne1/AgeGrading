namespace AgeGrading
{
    partial class AgeGradingForm
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
            this.components = new System.ComponentModel.Container();
            this.txtRaceResults = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripRichText = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemRTCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRTCut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRTPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProcessColumn = new System.Windows.Forms.Button();
            this.labelAge = new System.Windows.Forms.Label();
            this.labelGender = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.cmbAge = new System.Windows.Forms.ComboBox();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.cmbTime = new System.Windows.Forms.ComboBox();
            this.cmbName = new System.Windows.Forms.ComboBox();
            this.labelName = new System.Windows.Forms.Label();
            this.numStartingLineNumber = new System.Windows.Forms.NumericUpDown();
            this.labelStartingLineNumber = new System.Windows.Forms.Label();
            this.labelColumnsLineNumber = new System.Windows.Forms.Label();
            this.numColumnsLineNumber = new System.Windows.Forms.NumericUpDown();
            this.cmbRaceTypes = new System.Windows.Forms.ComboBox();
            this.labelDistance = new System.Windows.Forms.Label();
            this.btnCalculateAgeGrade = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenRaceResults = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveRaceResults = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveRawResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelRaceInput = new System.Windows.Forms.Label();
            this.labelAgeGradingResults = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.radioFreeForm = new System.Windows.Forms.RadioButton();
            this.radioUseTabSeparatedColumns = new System.Windows.Forms.RadioButton();
            this.radioFixedWidthColumns = new System.Windows.Forms.RadioButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSetMemberTable = new System.Windows.Forms.ToolStripMenuItem();
            this.convertCSVToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addorUpdateResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rerunResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainFindNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemMainCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.labelKQRace = new System.Windows.Forms.Label();
            this.cmbKQRace = new System.Windows.Forms.ComboBox();
            this.labelFirstNameColumn = new System.Windows.Forms.Label();
            this.cmbFirstName = new System.Windows.Forms.ComboBox();
            this.numAgeGroup = new System.Windows.Forms.NumericUpDown();
            this.labelFirstAgeGroup = new System.Windows.Forms.Label();
            this.btnNextAgeGroup = new System.Windows.Forms.Button();
            this.btnCompareMissingMembers = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnMemberAges = new System.Windows.Forms.Button();
            this.chkOnlyCompletingRequirements = new System.Windows.Forms.CheckBox();
            this.btnCombineFirstAndLastNames = new System.Windows.Forms.Button();
            this.btnAddSelectedRace = new System.Windows.Forms.Button();
            this.btnSaveSelectedRace = new System.Windows.Forms.Button();
            this.contextMenuStripRichText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartingLineNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColumnsLineNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAgeGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRaceResults
            // 
            this.txtRaceResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRaceResults.ContextMenuStrip = this.contextMenuStripRichText;
            this.txtRaceResults.DetectUrls = false;
            this.txtRaceResults.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRaceResults.Location = new System.Drawing.Point(13, 252);
            this.txtRaceResults.Name = "txtRaceResults";
            this.txtRaceResults.Size = new System.Drawing.Size(907, 118);
            this.txtRaceResults.TabIndex = 0;
            this.txtRaceResults.Text = "";
            this.txtRaceResults.TextChanged += new System.EventHandler(this.txtRaceResults_TextChanged);
            this.txtRaceResults.Enter += new System.EventHandler(this.txtRaceResults_Enter);
            // 
            // contextMenuStripRichText
            // 
            this.contextMenuStripRichText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemRTCopy,
            this.toolStripMenuItemRTCut,
            this.toolStripMenuItemRTPaste});
            this.contextMenuStripRichText.Name = "contextMenuStripRichText";
            this.contextMenuStripRichText.Size = new System.Drawing.Size(145, 70);
            // 
            // toolStripMenuItemRTCopy
            // 
            this.toolStripMenuItemRTCopy.Name = "toolStripMenuItemRTCopy";
            this.toolStripMenuItemRTCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMenuItemRTCopy.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemRTCopy.Text = "Copy";
            this.toolStripMenuItemRTCopy.Click += new System.EventHandler(this.toolStripMenuItemRTCopy_Click);
            // 
            // toolStripMenuItemRTCut
            // 
            this.toolStripMenuItemRTCut.Name = "toolStripMenuItemRTCut";
            this.toolStripMenuItemRTCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.toolStripMenuItemRTCut.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemRTCut.Text = "Cut";
            this.toolStripMenuItemRTCut.Click += new System.EventHandler(this.toolStripMenuItemRTCut_Click);
            // 
            // toolStripMenuItemRTPaste
            // 
            this.toolStripMenuItemRTPaste.Name = "toolStripMenuItemRTPaste";
            this.toolStripMenuItemRTPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripMenuItemRTPaste.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemRTPaste.Text = "Paste";
            this.toolStripMenuItemRTPaste.Click += new System.EventHandler(this.toolStripMenuItemRTPaste_Click);
            // 
            // btnProcessColumn
            // 
            this.btnProcessColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcessColumn.AutoSize = true;
            this.btnProcessColumn.Enabled = false;
            this.btnProcessColumn.Location = new System.Drawing.Point(706, 223);
            this.btnProcessColumn.Name = "btnProcessColumn";
            this.btnProcessColumn.Size = new System.Drawing.Size(93, 23);
            this.btnProcessColumn.TabIndex = 1;
            this.btnProcessColumn.Text = "Process Co&lumn";
            this.btnProcessColumn.UseVisualStyleBackColor = true;
            this.btnProcessColumn.Click += new System.EventHandler(this.btnProcessColumn_Click);
            // 
            // labelAge
            // 
            this.labelAge.AutoSize = true;
            this.labelAge.Location = new System.Drawing.Point(13, 41);
            this.labelAge.Name = "labelAge";
            this.labelAge.Size = new System.Drawing.Size(67, 13);
            this.labelAge.TabIndex = 2;
            this.labelAge.Text = "&Age Column:";
            // 
            // labelGender
            // 
            this.labelGender.AutoSize = true;
            this.labelGender.Location = new System.Drawing.Point(13, 68);
            this.labelGender.Name = "labelGender";
            this.labelGender.Size = new System.Drawing.Size(83, 13);
            this.labelGender.TabIndex = 3;
            this.labelGender.Text = "&Gender Column:";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(13, 95);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(71, 13);
            this.labelTime.TabIndex = 4;
            this.labelTime.Text = "&Time Column:";
            // 
            // cmbAge
            // 
            this.cmbAge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAge.FormattingEnabled = true;
            this.cmbAge.Location = new System.Drawing.Point(132, 37);
            this.cmbAge.Name = "cmbAge";
            this.cmbAge.Size = new System.Drawing.Size(531, 21);
            this.cmbAge.TabIndex = 5;
            this.cmbAge.SelectedIndexChanged += new System.EventHandler(this.cmbAge_SelectedIndexChanged);
            // 
            // cmbGender
            // 
            this.cmbGender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Location = new System.Drawing.Point(132, 64);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(531, 21);
            this.cmbGender.TabIndex = 6;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // cmbTime
            // 
            this.cmbTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime.FormattingEnabled = true;
            this.cmbTime.Location = new System.Drawing.Point(132, 91);
            this.cmbTime.Name = "cmbTime";
            this.cmbTime.Size = new System.Drawing.Size(531, 21);
            this.cmbTime.TabIndex = 7;
            this.cmbTime.SelectedIndexChanged += new System.EventHandler(this.cmbTime_SelectedIndexChanged);
            // 
            // cmbName
            // 
            this.cmbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbName.FormattingEnabled = true;
            this.cmbName.Location = new System.Drawing.Point(132, 118);
            this.cmbName.Name = "cmbName";
            this.cmbName.Size = new System.Drawing.Size(531, 21);
            this.cmbName.TabIndex = 8;
            this.cmbName.SelectedIndexChanged += new System.EventHandler(this.cmbName_SelectedIndexChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(13, 122);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(76, 13);
            this.labelName.TabIndex = 9;
            this.labelName.Text = "&Name Column:";
            // 
            // numStartingLineNumber
            // 
            this.numStartingLineNumber.Location = new System.Drawing.Point(132, 176);
            this.numStartingLineNumber.Name = "numStartingLineNumber";
            this.numStartingLineNumber.Size = new System.Drawing.Size(120, 20);
            this.numStartingLineNumber.TabIndex = 10;
            this.numStartingLineNumber.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // labelStartingLineNumber
            // 
            this.labelStartingLineNumber.AutoSize = true;
            this.labelStartingLineNumber.Location = new System.Drawing.Point(13, 180);
            this.labelStartingLineNumber.Name = "labelStartingLineNumber";
            this.labelStartingLineNumber.Size = new System.Drawing.Size(109, 13);
            this.labelStartingLineNumber.TabIndex = 11;
            this.labelStartingLineNumber.Text = "&Starting Line Number:";
            // 
            // labelColumnsLineNumber
            // 
            this.labelColumnsLineNumber.AutoSize = true;
            this.labelColumnsLineNumber.Location = new System.Drawing.Point(13, 204);
            this.labelColumnsLineNumber.Name = "labelColumnsLineNumber";
            this.labelColumnsLineNumber.Size = new System.Drawing.Size(113, 13);
            this.labelColumnsLineNumber.TabIndex = 13;
            this.labelColumnsLineNumber.Text = "&Columns Line Number:";
            // 
            // numColumnsLineNumber
            // 
            this.numColumnsLineNumber.Location = new System.Drawing.Point(132, 202);
            this.numColumnsLineNumber.Name = "numColumnsLineNumber";
            this.numColumnsLineNumber.Size = new System.Drawing.Size(120, 20);
            this.numColumnsLineNumber.TabIndex = 12;
            // 
            // cmbRaceTypes
            // 
            this.cmbRaceTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRaceTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRaceTypes.FormattingEnabled = true;
            this.cmbRaceTypes.Location = new System.Drawing.Point(774, 27);
            this.cmbRaceTypes.Name = "cmbRaceTypes";
            this.cmbRaceTypes.Size = new System.Drawing.Size(146, 21);
            this.cmbRaceTypes.TabIndex = 15;
            this.cmbRaceTypes.SelectedIndexChanged += new System.EventHandler(this.cmbRaceTypes_SelectedIndexChanged);
            // 
            // labelDistance
            // 
            this.labelDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDistance.AutoSize = true;
            this.labelDistance.Location = new System.Drawing.Point(692, 31);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(63, 13);
            this.labelDistance.TabIndex = 16;
            this.labelDistance.Text = "Race T&ype:";
            // 
            // btnCalculateAgeGrade
            // 
            this.btnCalculateAgeGrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculateAgeGrade.AutoSize = true;
            this.btnCalculateAgeGrade.Enabled = false;
            this.btnCalculateAgeGrade.Location = new System.Drawing.Point(805, 223);
            this.btnCalculateAgeGrade.Name = "btnCalculateAgeGrade";
            this.btnCalculateAgeGrade.Size = new System.Drawing.Size(115, 23);
            this.btnCalculateAgeGrade.TabIndex = 17;
            this.btnCalculateAgeGrade.Text = "Calc&ulate Age Grade";
            this.btnCalculateAgeGrade.UseVisualStyleBackColor = true;
            this.btnCalculateAgeGrade.Click += new System.EventHandler(this.btnCalculateAgeGrade_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
            this.dataGridView.Location = new System.Drawing.Point(13, 392);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(907, 99);
            this.dataGridView.TabIndex = 18;
            this.dataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_CellFormatting);
            this.dataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView_DataBindingComplete);
            this.dataGridView.Enter += new System.EventHandler(this.dataGridView_Enter);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOpen,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItemOpenRaceResults,
            this.toolStripMenuItemSaveRaceResults,
            this.toolStripSeparator1,
            this.toolStripMenuItemFind,
            this.toolStripMenuItemFindNext,
            this.toolStripMenuItemSelectColumn,
            this.copyToolStripMenuItem,
            this.saveRawResultsToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(174, 230);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // toolStripMenuItemOpen
            // 
            this.toolStripMenuItemOpen.Name = "toolStripMenuItemOpen";
            this.toolStripMenuItemOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItemOpen.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemOpen.Text = "&Open";
            this.toolStripMenuItemOpen.Click += new System.EventHandler(this.toolStripMenuItemOpen_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItemOpenRaceResults
            // 
            this.toolStripMenuItemOpenRaceResults.Name = "toolStripMenuItemOpenRaceResults";
            this.toolStripMenuItemOpenRaceResults.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemOpenRaceResults.Text = "Open &Race Results";
            this.toolStripMenuItemOpenRaceResults.Click += new System.EventHandler(this.toolStripMenuItemOpenRaceResults_Click);
            // 
            // toolStripMenuItemSaveRaceResults
            // 
            this.toolStripMenuItemSaveRaceResults.Name = "toolStripMenuItemSaveRaceResults";
            this.toolStripMenuItemSaveRaceResults.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemSaveRaceResults.Text = "Save Race Resu&lts";
            this.toolStripMenuItemSaveRaceResults.Click += new System.EventHandler(this.toolStripMenuItemSaveRaceResults_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(170, 6);
            // 
            // toolStripMenuItemFind
            // 
            this.toolStripMenuItemFind.Name = "toolStripMenuItemFind";
            this.toolStripMenuItemFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.toolStripMenuItemFind.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemFind.Text = "&Find";
            this.toolStripMenuItemFind.Click += new System.EventHandler(this.toolStripMenuItemFind_Click);
            // 
            // toolStripMenuItemFindNext
            // 
            this.toolStripMenuItemFindNext.Name = "toolStripMenuItemFindNext";
            this.toolStripMenuItemFindNext.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.toolStripMenuItemFindNext.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemFindNext.Text = "Find Ne&xt";
            this.toolStripMenuItemFindNext.Click += new System.EventHandler(this.toolStripMenuItemFindNext_Click);
            // 
            // toolStripMenuItemSelectColumn
            // 
            this.toolStripMenuItemSelectColumn.Name = "toolStripMenuItemSelectColumn";
            this.toolStripMenuItemSelectColumn.Size = new System.Drawing.Size(173, 22);
            this.toolStripMenuItemSelectColumn.Text = "Select &Column";
            this.toolStripMenuItemSelectColumn.Click += new System.EventHandler(this.toolStripMenuItemSelectColumn_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.copyToolStripMenuItem.Text = "&Copy            Ctrl+C";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // saveRawResultsToolStripMenuItem
            // 
            this.saveRawResultsToolStripMenuItem.Name = "saveRawResultsToolStripMenuItem";
            this.saveRawResultsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.saveRawResultsToolStripMenuItem.Text = "Save Ra&w Results";
            this.saveRawResultsToolStripMenuItem.Click += new System.EventHandler(this.saveRawResultsToolStripMenuItem_Click);
            // 
            // labelRaceInput
            // 
            this.labelRaceInput.AutoSize = true;
            this.labelRaceInput.Location = new System.Drawing.Point(13, 233);
            this.labelRaceInput.Name = "labelRaceInput";
            this.labelRaceInput.Size = new System.Drawing.Size(63, 13);
            this.labelRaceInput.TabIndex = 19;
            this.labelRaceInput.Text = "&Race Input:";
            // 
            // labelAgeGradingResults
            // 
            this.labelAgeGradingResults.AutoSize = true;
            this.labelAgeGradingResults.Location = new System.Drawing.Point(13, 373);
            this.labelAgeGradingResults.Name = "labelAgeGradingResults";
            this.labelAgeGradingResults.Size = new System.Drawing.Size(107, 13);
            this.labelAgeGradingResults.TabIndex = 20;
            this.labelAgeGradingResults.Text = "A&ge Grading Results:";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.radioFreeForm);
            this.groupBox.Controls.Add(this.radioUseTabSeparatedColumns);
            this.groupBox.Controls.Add(this.radioFixedWidthColumns);
            this.groupBox.Location = new System.Drawing.Point(695, 83);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(225, 103);
            this.groupBox.TabIndex = 21;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Column Parse T&ype";
            // 
            // radioFreeForm
            // 
            this.radioFreeForm.AutoSize = true;
            this.radioFreeForm.Location = new System.Drawing.Point(13, 80);
            this.radioFreeForm.Name = "radioFreeForm";
            this.radioFreeForm.Size = new System.Drawing.Size(72, 17);
            this.radioFreeForm.TabIndex = 2;
            this.radioFreeForm.TabStop = true;
            this.radioFreeForm.Text = "&Free Form";
            this.radioFreeForm.UseVisualStyleBackColor = true;
            this.radioFreeForm.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioUseTabSeparatedColumns
            // 
            this.radioUseTabSeparatedColumns.AutoSize = true;
            this.radioUseTabSeparatedColumns.Location = new System.Drawing.Point(13, 50);
            this.radioUseTabSeparatedColumns.Name = "radioUseTabSeparatedColumns";
            this.radioUseTabSeparatedColumns.Size = new System.Drawing.Size(139, 17);
            this.radioUseTabSeparatedColumns.TabIndex = 1;
            this.radioUseTabSeparatedColumns.TabStop = true;
            this.radioUseTabSeparatedColumns.Text = "Ta&b Separated Columns";
            this.radioUseTabSeparatedColumns.UseVisualStyleBackColor = true;
            this.radioUseTabSeparatedColumns.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioFixedWidthColumns
            // 
            this.radioFixedWidthColumns.AutoSize = true;
            this.radioFixedWidthColumns.Checked = true;
            this.radioFixedWidthColumns.Location = new System.Drawing.Point(13, 20);
            this.radioFixedWidthColumns.Name = "radioFixedWidthColumns";
            this.radioFixedWidthColumns.Size = new System.Drawing.Size(124, 17);
            this.radioFixedWidthColumns.TabIndex = 0;
            this.radioFixedWidthColumns.TabStop = true;
            this.radioFixedWidthColumns.Text = "&Fixed Width Columns";
            this.radioFixedWidthColumns.UseVisualStyleBackColor = true;
            this.radioFixedWidthColumns.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(932, 24);
            this.menuStrip.TabIndex = 22;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMainSave,
            this.toolStripMenuItemMainSaveAs,
            this.toolStripMenuItemMainOpen,
            this.toolStripSeparator2,
            this.toolStripMenuItemSetMemberTable,
            this.convertCSVToXMLToolStripMenuItem,
            this.addorUpdateResultsToolStripMenuItem,
            this.rerunResultsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItemMainSave
            // 
            this.toolStripMenuItemMainSave.Name = "toolStripMenuItemMainSave";
            this.toolStripMenuItemMainSave.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemMainSave.Text = "&Save";
            this.toolStripMenuItemMainSave.Click += new System.EventHandler(this.toolStripMenuItemMainSave_Click);
            // 
            // toolStripMenuItemMainSaveAs
            // 
            this.toolStripMenuItemMainSaveAs.Name = "toolStripMenuItemMainSaveAs";
            this.toolStripMenuItemMainSaveAs.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemMainSaveAs.Text = "Save &As";
            this.toolStripMenuItemMainSaveAs.Click += new System.EventHandler(this.toolStripMenuItemMainSaveAs_Click);
            // 
            // toolStripMenuItemMainOpen
            // 
            this.toolStripMenuItemMainOpen.Name = "toolStripMenuItemMainOpen";
            this.toolStripMenuItemMainOpen.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemMainOpen.Text = "&Open";
            this.toolStripMenuItemMainOpen.Click += new System.EventHandler(this.toolStripMenuItemMainOpen_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // toolStripMenuItemSetMemberTable
            // 
            this.toolStripMenuItemSetMemberTable.Name = "toolStripMenuItemSetMemberTable";
            this.toolStripMenuItemSetMemberTable.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemSetMemberTable.Text = "Set &Member Table";
            this.toolStripMenuItemSetMemberTable.Click += new System.EventHandler(this.toolStripMenuItemSetMemberTable_Click);
            // 
            // convertCSVToXMLToolStripMenuItem
            // 
            this.convertCSVToXMLToolStripMenuItem.Name = "convertCSVToXMLToolStripMenuItem";
            this.convertCSVToXMLToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.convertCSVToXMLToolStripMenuItem.Text = "&Convert XLS to XML";
            this.convertCSVToXMLToolStripMenuItem.Click += new System.EventHandler(this.convertCSVToXMLToolStripMenuItem_Click);
            // 
            // addorUpdateResultsToolStripMenuItem
            // 
            this.addorUpdateResultsToolStripMenuItem.Name = "addorUpdateResultsToolStripMenuItem";
            this.addorUpdateResultsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addorUpdateResultsToolStripMenuItem.Text = "Add &or Update Results";
            this.addorUpdateResultsToolStripMenuItem.Click += new System.EventHandler(this.addorUpdateResultsToolStripMenuItem_Click);
            // 
            // rerunResultsToolStripMenuItem
            // 
            this.rerunResultsToolStripMenuItem.Name = "rerunResultsToolStripMenuItem";
            this.rerunResultsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.rerunResultsToolStripMenuItem.Text = "&Rerun Results";
            this.rerunResultsToolStripMenuItem.Click += new System.EventHandler(this.rerunResultsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMainFind,
            this.toolStripMenuItemMainFindNext,
            this.toolStripMenuItemMainCopy});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // toolStripMenuItemMainFind
            // 
            this.toolStripMenuItemMainFind.Name = "toolStripMenuItemMainFind";
            this.toolStripMenuItemMainFind.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemMainFind.Text = "&Find";
            this.toolStripMenuItemMainFind.Click += new System.EventHandler(this.toolStripMenuItemMainFind_Click);
            // 
            // toolStripMenuItemMainFindNext
            // 
            this.toolStripMenuItemMainFindNext.Name = "toolStripMenuItemMainFindNext";
            this.toolStripMenuItemMainFindNext.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemMainFindNext.Text = "Find &Next";
            this.toolStripMenuItemMainFindNext.Click += new System.EventHandler(this.toolStripMenuItemMainFindNext_Click);
            // 
            // toolStripMenuItemMainCopy
            // 
            this.toolStripMenuItemMainCopy.Name = "toolStripMenuItemMainCopy";
            this.toolStripMenuItemMainCopy.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItemMainCopy.Text = "&Copy";
            this.toolStripMenuItemMainCopy.Click += new System.EventHandler(this.toolStripMenuItemMainCopy_Click);
            // 
            // labelKQRace
            // 
            this.labelKQRace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelKQRace.AutoSize = true;
            this.labelKQRace.Location = new System.Drawing.Point(692, 58);
            this.labelKQRace.Name = "labelKQRace";
            this.labelKQRace.Size = new System.Drawing.Size(60, 13);
            this.labelKQRace.TabIndex = 24;
            this.labelKQRace.Text = "&K+Q Race:";
            // 
            // cmbKQRace
            // 
            this.cmbKQRace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbKQRace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKQRace.FormattingEnabled = true;
            this.cmbKQRace.Location = new System.Drawing.Point(774, 54);
            this.cmbKQRace.Name = "cmbKQRace";
            this.cmbKQRace.Size = new System.Drawing.Size(146, 21);
            this.cmbKQRace.TabIndex = 23;
            this.cmbKQRace.SelectedIndexChanged += new System.EventHandler(this.cmbKQRace_SelectedIndexChanged);
            // 
            // labelFirstNameColumn
            // 
            this.labelFirstNameColumn.AutoSize = true;
            this.labelFirstNameColumn.Location = new System.Drawing.Point(13, 149);
            this.labelFirstNameColumn.Name = "labelFirstNameColumn";
            this.labelFirstNameColumn.Size = new System.Drawing.Size(98, 13);
            this.labelFirstNameColumn.TabIndex = 26;
            this.labelFirstNameColumn.Text = "&First Name Column:";
            // 
            // cmbFirstName
            // 
            this.cmbFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFirstName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFirstName.FormattingEnabled = true;
            this.cmbFirstName.Location = new System.Drawing.Point(132, 145);
            this.cmbFirstName.Name = "cmbFirstName";
            this.cmbFirstName.Size = new System.Drawing.Size(531, 21);
            this.cmbFirstName.TabIndex = 25;
            this.cmbFirstName.SelectedIndexChanged += new System.EventHandler(this.cmbFirstName_SelectedIndexChanged);
            // 
            // numAgeGroup
            // 
            this.numAgeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numAgeGroup.Location = new System.Drawing.Point(744, 194);
            this.numAgeGroup.Minimum = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numAgeGroup.Name = "numAgeGroup";
            this.numAgeGroup.Size = new System.Drawing.Size(94, 20);
            this.numAgeGroup.TabIndex = 27;
            this.numAgeGroup.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            this.numAgeGroup.ValueChanged += new System.EventHandler(this.numAgeGroup_ValueChanged);
            // 
            // labelFirstAgeGroup
            // 
            this.labelFirstAgeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFirstAgeGroup.AutoSize = true;
            this.labelFirstAgeGroup.Location = new System.Drawing.Point(660, 198);
            this.labelFirstAgeGroup.Name = "labelFirstAgeGroup";
            this.labelFirstAgeGroup.Size = new System.Drawing.Size(78, 13);
            this.labelFirstAgeGroup.TabIndex = 28;
            this.labelFirstAgeGroup.Text = "1st Age Group:";
            // 
            // btnNextAgeGroup
            // 
            this.btnNextAgeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextAgeGroup.AutoSize = true;
            this.btnNextAgeGroup.Location = new System.Drawing.Point(844, 193);
            this.btnNextAgeGroup.Name = "btnNextAgeGroup";
            this.btnNextAgeGroup.Size = new System.Drawing.Size(75, 23);
            this.btnNextAgeGroup.TabIndex = 29;
            this.btnNextAgeGroup.Text = "N&ext Group";
            this.btnNextAgeGroup.UseVisualStyleBackColor = true;
            this.btnNextAgeGroup.Click += new System.EventHandler(this.btnNextAgeGroup_Click);
            // 
            // btnCompareMissingMembers
            // 
            this.btnCompareMissingMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompareMissingMembers.AutoSize = true;
            this.btnCompareMissingMembers.Enabled = false;
            this.btnCompareMissingMembers.Location = new System.Drawing.Point(557, 223);
            this.btnCompareMissingMembers.Name = "btnCompareMissingMembers";
            this.btnCompareMissingMembers.Size = new System.Drawing.Size(143, 23);
            this.btnCompareMissingMembers.TabIndex = 30;
            this.btnCompareMissingMembers.Text = "Compare &Missing Members";
            this.btnCompareMissingMembers.UseVisualStyleBackColor = true;
            this.btnCompareMissingMembers.Click += new System.EventHandler(this.btnCompareMissingMembers_Click);
            // 
            // btnMemberAges
            // 
            this.btnMemberAges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMemberAges.AutoSize = true;
            this.btnMemberAges.Enabled = false;
            this.btnMemberAges.Location = new System.Drawing.Point(449, 223);
            this.btnMemberAges.Name = "btnMemberAges";
            this.btnMemberAges.Size = new System.Drawing.Size(102, 23);
            this.btnMemberAges.TabIndex = 31;
            this.btnMemberAges.Text = "Get Member &Ages";
            this.btnMemberAges.UseVisualStyleBackColor = true;
            this.btnMemberAges.Click += new System.EventHandler(this.btnMemberAges_Click);
            // 
            // chkOnlyCompletingRequirements
            // 
            this.chkOnlyCompletingRequirements.AutoSize = true;
            this.chkOnlyCompletingRequirements.Location = new System.Drawing.Point(259, 178);
            this.chkOnlyCompletingRequirements.Name = "chkOnlyCompletingRequirements";
            this.chkOnlyCompletingRequirements.Size = new System.Drawing.Size(170, 17);
            this.chkOnlyCompletingRequirements.TabIndex = 32;
            this.chkOnlyCompletingRequirements.Text = "&Only Completing Requirements";
            this.chkOnlyCompletingRequirements.UseVisualStyleBackColor = true;
            this.chkOnlyCompletingRequirements.CheckedChanged += new System.EventHandler(this.chkOnlyCompletingRequirements_CheckedChanged);
            // 
            // btnCombineFirstAndLastNames
            // 
            this.btnCombineFirstAndLastNames.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCombineFirstAndLastNames.AutoSize = true;
            this.btnCombineFirstAndLastNames.Enabled = false;
            this.btnCombineFirstAndLastNames.Location = new System.Drawing.Point(341, 223);
            this.btnCombineFirstAndLastNames.Name = "btnCombineFirstAndLastNames";
            this.btnCombineFirstAndLastNames.Size = new System.Drawing.Size(102, 23);
            this.btnCombineFirstAndLastNames.TabIndex = 33;
            this.btnCombineFirstAndLastNames.Text = "Com&bine Names";
            this.btnCombineFirstAndLastNames.UseVisualStyleBackColor = true;
            this.btnCombineFirstAndLastNames.Click += new System.EventHandler(this.btnCombineFirstAndLastNames_Click);
            // 
            // btnAddSelectedRace
            // 
            this.btnAddSelectedRace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSelectedRace.AutoSize = true;
            this.btnAddSelectedRace.Location = new System.Drawing.Point(218, 223);
            this.btnAddSelectedRace.Name = "btnAddSelectedRace";
            this.btnAddSelectedRace.Size = new System.Drawing.Size(117, 23);
            this.btnAddSelectedRace.TabIndex = 34;
            this.btnAddSelectedRace.Text = "Open Se&lected Race";
            this.btnAddSelectedRace.UseVisualStyleBackColor = true;
            this.btnAddSelectedRace.Click += new System.EventHandler(this.btnAddSelectedRace_Click);
            // 
            // btnSaveSelectedRace
            // 
            this.btnSaveSelectedRace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSelectedRace.AutoSize = true;
            this.btnSaveSelectedRace.Location = new System.Drawing.Point(95, 223);
            this.btnSaveSelectedRace.Name = "btnSaveSelectedRace";
            this.btnSaveSelectedRace.Size = new System.Drawing.Size(117, 23);
            this.btnSaveSelectedRace.TabIndex = 35;
            this.btnSaveSelectedRace.Text = "Sa&ve Selected Race";
            this.btnSaveSelectedRace.UseVisualStyleBackColor = true;
            this.btnSaveSelectedRace.Click += new System.EventHandler(this.btnSaveSelectedRace_Click);
            // 
            // AgeGradingForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 503);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.btnSaveSelectedRace);
            this.Controls.Add(this.btnAddSelectedRace);
            this.Controls.Add(this.btnCombineFirstAndLastNames);
            this.Controls.Add(this.chkOnlyCompletingRequirements);
            this.Controls.Add(this.btnMemberAges);
            this.Controls.Add(this.btnCompareMissingMembers);
            this.Controls.Add(this.btnNextAgeGroup);
            this.Controls.Add(this.labelFirstAgeGroup);
            this.Controls.Add(this.numAgeGroup);
            this.Controls.Add(this.labelFirstNameColumn);
            this.Controls.Add(this.cmbFirstName);
            this.Controls.Add(this.labelKQRace);
            this.Controls.Add(this.cmbKQRace);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.labelAgeGradingResults);
            this.Controls.Add(this.labelRaceInput);
            this.Controls.Add(this.btnCalculateAgeGrade);
            this.Controls.Add(this.labelDistance);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.cmbRaceTypes);
            this.Controls.Add(this.labelColumnsLineNumber);
            this.Controls.Add(this.numColumnsLineNumber);
            this.Controls.Add(this.labelStartingLineNumber);
            this.Controls.Add(this.numStartingLineNumber);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.cmbName);
            this.Controls.Add(this.cmbTime);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.cmbAge);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelGender);
            this.Controls.Add(this.labelAge);
            this.Controls.Add(this.btnProcessColumn);
            this.Controls.Add(this.txtRaceResults);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(948, 541);
            this.Name = "AgeGradingForm";
            this.Text = "Age Grading";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AgeGradingForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AgeGradingForm_DragEnter);
            this.contextMenuStripRichText.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numStartingLineNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColumnsLineNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAgeGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtRaceResults;
        private System.Windows.Forms.Button btnProcessColumn;
        private System.Windows.Forms.Label labelAge;
        private System.Windows.Forms.Label labelGender;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.ComboBox cmbAge;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.ComboBox cmbTime;
        private System.Windows.Forms.ComboBox cmbName;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.NumericUpDown numStartingLineNumber;
        private System.Windows.Forms.Label labelStartingLineNumber;
        private System.Windows.Forms.Label labelColumnsLineNumber;
        private System.Windows.Forms.NumericUpDown numColumnsLineNumber;
        private System.Windows.Forms.ComboBox cmbRaceTypes;
        private System.Windows.Forms.Label labelDistance;
        private System.Windows.Forms.Button btnCalculateAgeGrade;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label labelRaceInput;
        private System.Windows.Forms.Label labelAgeGradingResults;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RadioButton radioUseTabSeparatedColumns;
        private System.Windows.Forms.RadioButton radioFixedWidthColumns;
        private System.Windows.Forms.RadioButton radioFreeForm;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFind;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectColumn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFindNext;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRichText;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRTCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRTCut;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemRTPaste;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetMemberTable;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainSave;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainSaveAs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainFind;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainFindNext;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMainCopy;
        private System.Windows.Forms.Label labelKQRace;
        private System.Windows.Forms.ComboBox cmbKQRace;
        private System.Windows.Forms.ToolStripMenuItem convertCSVToXMLToolStripMenuItem;
        private System.Windows.Forms.Label labelFirstNameColumn;
        private System.Windows.Forms.ComboBox cmbFirstName;
        private System.Windows.Forms.ToolStripMenuItem addorUpdateResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenRaceResults;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveRaceResults;
        private System.Windows.Forms.ToolStripMenuItem rerunResultsToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numAgeGroup;
        private System.Windows.Forms.Label labelFirstAgeGroup;
        private System.Windows.Forms.Button btnNextAgeGroup;
        private System.Windows.Forms.Button btnCompareMissingMembers;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnMemberAges;
        private System.Windows.Forms.CheckBox chkOnlyCompletingRequirements;
        private System.Windows.Forms.Button btnCombineFirstAndLastNames;
        private System.Windows.Forms.Button btnAddSelectedRace;
        private System.Windows.Forms.Button btnSaveSelectedRace;
        private System.Windows.Forms.ToolStripMenuItem saveRawResultsToolStripMenuItem;
    }
}

