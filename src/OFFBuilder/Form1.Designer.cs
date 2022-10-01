namespace OFFBuilder
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblCoords = new System.Windows.Forms.Label();
            this.txtCoords = new System.Windows.Forms.TextBox();
            this.radFormSpace = new System.Windows.Forms.RadioButton();
            this.radFormComma = new System.Windows.Forms.RadioButton();
            this.grpFormatting = new System.Windows.Forms.GroupBox();
            this.grpPermutation = new System.Windows.Forms.GroupBox();
            this.btnPermClear = new System.Windows.Forms.Button();
            this.btnPermAdd = new System.Windows.Forms.Button();
            this.tlpPermCustom = new System.Windows.Forms.TableLayoutPanel();
            this.radPermNone = new System.Windows.Forms.RadioButton();
            this.radPermCustom = new System.Windows.Forms.RadioButton();
            this.radPermOdd = new System.Windows.Forms.RadioButton();
            this.radPermEven = new System.Windows.Forms.RadioButton();
            this.radPermAll = new System.Windows.Forms.RadioButton();
            this.radPermCyclic = new System.Windows.Forms.RadioButton();
            this.grpSign = new System.Windows.Forms.GroupBox();
            this.radSignFull = new System.Windows.Forms.RadioButton();
            this.btnSignClear = new System.Windows.Forms.Button();
            this.tlpSignCustom = new System.Windows.Forms.TableLayoutPanel();
            this.btnSignAdd = new System.Windows.Forms.Button();
            this.radSignNone = new System.Windows.Forms.RadioButton();
            this.radSignCustom = new System.Windows.Forms.RadioButton();
            this.radSignOdd = new System.Windows.Forms.RadioButton();
            this.radSignEven = new System.Windows.Forms.RadioButton();
            this.radSignAll = new System.Windows.Forms.RadioButton();
            this.grpOutput = new System.Windows.Forms.GroupBox();
            this.radOutSym = new System.Windows.Forms.RadioButton();
            this.radOutVal = new System.Windows.Forms.RadioButton();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnInsert = new System.Windows.Forms.Button();
            this.tlpLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.grpDimensions = new System.Windows.Forms.GroupBox();
            this.btnProject = new System.Windows.Forms.Button();
            this.nudDimensions = new System.Windows.Forms.NumericUpDown();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.sfoExport = new System.Windows.Forms.SaveFileDialog();
            this.btnPaste = new System.Windows.Forms.Button();
            this.grpFormatting.SuspendLayout();
            this.grpPermutation.SuspendLayout();
            this.grpSign.SuspendLayout();
            this.grpOutput.SuspendLayout();
            this.tlpLayout.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.grpDimensions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDimensions)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCoords
            // 
            this.lblCoords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCoords.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoords.Location = new System.Drawing.Point(12, 9);
            this.lblCoords.Name = "lblCoords";
            this.lblCoords.Size = new System.Drawing.Size(530, 17);
            this.lblCoords.TabIndex = 0;
            this.lblCoords.Text = "Coordinates:";
            this.lblCoords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCoords
            // 
            this.txtCoords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCoords.Location = new System.Drawing.Point(12, 29);
            this.txtCoords.Name = "txtCoords";
            this.txtCoords.Size = new System.Drawing.Size(530, 20);
            this.txtCoords.TabIndex = 0;
            this.txtCoords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCoords.TextChanged += new System.EventHandler(this.txtCoords_TextChanged);
            // 
            // radFormSpace
            // 
            this.radFormSpace.AutoSize = true;
            this.radFormSpace.Checked = true;
            this.radFormSpace.Location = new System.Drawing.Point(6, 19);
            this.radFormSpace.Name = "radFormSpace";
            this.radFormSpace.Size = new System.Drawing.Size(119, 17);
            this.radFormSpace.TabIndex = 3;
            this.radFormSpace.TabStop = true;
            this.radFormSpace.Tag = ' ';
            this.radFormSpace.Text = "Separate by spaces";
            this.radFormSpace.UseVisualStyleBackColor = true;
            // 
            // radFormComma
            // 
            this.radFormComma.AutoSize = true;
            this.radFormComma.Location = new System.Drawing.Point(6, 42);
            this.radFormComma.Name = "radFormComma";
            this.radFormComma.Size = new System.Drawing.Size(124, 17);
            this.radFormComma.TabIndex = 4;
            this.radFormComma.Tag = ',';
            this.radFormComma.Text = "Separate by commas";
            this.radFormComma.UseVisualStyleBackColor = true;
            // 
            // grpFormatting
            // 
            this.grpFormatting.Controls.Add(this.radFormSpace);
            this.grpFormatting.Controls.Add(this.radFormComma);
            this.grpFormatting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpFormatting.Location = new System.Drawing.Point(3, 54);
            this.grpFormatting.Name = "grpFormatting";
            this.grpFormatting.Size = new System.Drawing.Size(236, 65);
            this.grpFormatting.TabIndex = 4;
            this.grpFormatting.TabStop = false;
            this.grpFormatting.Text = "Formatting";
            // 
            // grpPermutation
            // 
            this.grpPermutation.Controls.Add(this.btnPermClear);
            this.grpPermutation.Controls.Add(this.btnPermAdd);
            this.grpPermutation.Controls.Add(this.tlpPermCustom);
            this.grpPermutation.Controls.Add(this.radPermNone);
            this.grpPermutation.Controls.Add(this.radPermCustom);
            this.grpPermutation.Controls.Add(this.radPermOdd);
            this.grpPermutation.Controls.Add(this.radPermEven);
            this.grpPermutation.Controls.Add(this.radPermAll);
            this.grpPermutation.Controls.Add(this.radPermCyclic);
            this.grpPermutation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPermutation.Location = new System.Drawing.Point(3, 293);
            this.grpPermutation.Name = "grpPermutation";
            this.grpPermutation.Size = new System.Drawing.Size(236, 162);
            this.grpPermutation.TabIndex = 5;
            this.grpPermutation.TabStop = false;
            this.grpPermutation.Text = "Permutations";
            // 
            // btnPermClear
            // 
            this.btnPermClear.Enabled = false;
            this.btnPermClear.Location = new System.Drawing.Point(101, 127);
            this.btnPermClear.Name = "btnPermClear";
            this.btnPermClear.Size = new System.Drawing.Size(23, 23);
            this.btnPermClear.TabIndex = 20;
            this.btnPermClear.Text = "×";
            this.btnPermClear.UseVisualStyleBackColor = true;
            this.btnPermClear.Click += new System.EventHandler(this.btnCustomClear_Click);
            // 
            // btnPermAdd
            // 
            this.btnPermAdd.Enabled = false;
            this.btnPermAdd.Location = new System.Drawing.Point(72, 127);
            this.btnPermAdd.Name = "btnPermAdd";
            this.btnPermAdd.Size = new System.Drawing.Size(23, 23);
            this.btnPermAdd.TabIndex = 19;
            this.btnPermAdd.Text = "+";
            this.btnPermAdd.UseVisualStyleBackColor = true;
            this.btnPermAdd.Click += new System.EventHandler(this.btnCustomAdd_Click);
            // 
            // tlpPermCustom
            // 
            this.tlpPermCustom.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpPermCustom.ColumnCount = 1;
            this.tlpPermCustom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPermCustom.Enabled = false;
            this.tlpPermCustom.Location = new System.Drawing.Point(6, 159);
            this.tlpPermCustom.Margin = new System.Windows.Forms.Padding(0);
            this.tlpPermCustom.Name = "tlpPermCustom";
            this.tlpPermCustom.Size = new System.Drawing.Size(223, 0);
            this.tlpPermCustom.TabIndex = 12;
            // 
            // radPermNone
            // 
            this.radPermNone.AutoSize = true;
            this.radPermNone.Checked = true;
            this.radPermNone.Location = new System.Drawing.Point(6, 19);
            this.radPermNone.Name = "radPermNone";
            this.radPermNone.Size = new System.Drawing.Size(102, 17);
            this.radPermNone.TabIndex = 13;
            this.radPermNone.TabStop = true;
            this.radPermNone.Tag = 0;
            this.radPermNone.Text = "No permutations";
            this.radPermNone.UseVisualStyleBackColor = true;
            // 
            // radPermCustom
            // 
            this.radPermCustom.AutoSize = true;
            this.radPermCustom.Location = new System.Drawing.Point(6, 132);
            this.radPermCustom.Name = "radPermCustom";
            this.radPermCustom.Size = new System.Drawing.Size(60, 17);
            this.radPermCustom.TabIndex = 18;
            this.radPermCustom.Tag = 5;
            this.radPermCustom.Text = "Custom";
            this.radPermCustom.UseVisualStyleBackColor = true;
            // 
            // radPermOdd
            // 
            this.radPermOdd.AutoSize = true;
            this.radPermOdd.Location = new System.Drawing.Point(6, 86);
            this.radPermOdd.Name = "radPermOdd";
            this.radPermOdd.Size = new System.Drawing.Size(108, 17);
            this.radPermOdd.TabIndex = 16;
            this.radPermOdd.Tag = 3;
            this.radPermOdd.Text = "Odd permutations";
            this.radPermOdd.UseVisualStyleBackColor = true;
            // 
            // radPermEven
            // 
            this.radPermEven.AutoSize = true;
            this.radPermEven.Location = new System.Drawing.Point(6, 63);
            this.radPermEven.Name = "radPermEven";
            this.radPermEven.Size = new System.Drawing.Size(113, 17);
            this.radPermEven.TabIndex = 15;
            this.radPermEven.Tag = 2;
            this.radPermEven.Text = "Even permutations";
            this.radPermEven.UseVisualStyleBackColor = true;
            // 
            // radPermAll
            // 
            this.radPermAll.AutoSize = true;
            this.radPermAll.Location = new System.Drawing.Point(6, 40);
            this.radPermAll.Name = "radPermAll";
            this.radPermAll.Size = new System.Drawing.Size(99, 17);
            this.radPermAll.TabIndex = 14;
            this.radPermAll.Tag = 1;
            this.radPermAll.Text = "All permutations";
            this.radPermAll.UseVisualStyleBackColor = true;
            // 
            // radPermCyclic
            // 
            this.radPermCyclic.AutoSize = true;
            this.radPermCyclic.Location = new System.Drawing.Point(6, 109);
            this.radPermCyclic.Name = "radPermCyclic";
            this.radPermCyclic.Size = new System.Drawing.Size(116, 17);
            this.radPermCyclic.TabIndex = 17;
            this.radPermCyclic.Tag = 4;
            this.radPermCyclic.Text = "Cyclic permutations";
            this.radPermCyclic.UseVisualStyleBackColor = true;
            // 
            // grpSign
            // 
            this.grpSign.Controls.Add(this.radSignFull);
            this.grpSign.Controls.Add(this.btnSignClear);
            this.grpSign.Controls.Add(this.tlpSignCustom);
            this.grpSign.Controls.Add(this.btnSignAdd);
            this.grpSign.Controls.Add(this.radSignNone);
            this.grpSign.Controls.Add(this.radSignCustom);
            this.grpSign.Controls.Add(this.radSignOdd);
            this.grpSign.Controls.Add(this.radSignEven);
            this.grpSign.Controls.Add(this.radSignAll);
            this.grpSign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSign.Location = new System.Drawing.Point(3, 125);
            this.grpSign.Name = "grpSign";
            this.grpSign.Size = new System.Drawing.Size(236, 162);
            this.grpSign.TabIndex = 6;
            this.grpSign.TabStop = false;
            this.grpSign.Text = "Sign changes";
            // 
            // radSignFull
            // 
            this.radSignFull.AutoSize = true;
            this.radSignFull.Location = new System.Drawing.Point(6, 111);
            this.radSignFull.Name = "radSignFull";
            this.radSignFull.Size = new System.Drawing.Size(107, 17);
            this.radSignFull.TabIndex = 9;
            this.radSignFull.Tag = 4;
            this.radSignFull.Text = "Full sign changes";
            this.radSignFull.UseVisualStyleBackColor = true;
            // 
            // btnSignClear
            // 
            this.btnSignClear.Enabled = false;
            this.btnSignClear.Location = new System.Drawing.Point(101, 129);
            this.btnSignClear.Name = "btnSignClear";
            this.btnSignClear.Size = new System.Drawing.Size(23, 23);
            this.btnSignClear.TabIndex = 12;
            this.btnSignClear.Text = "×";
            this.btnSignClear.UseVisualStyleBackColor = true;
            this.btnSignClear.Click += new System.EventHandler(this.btnCustomClear_Click);
            // 
            // tlpSignCustom
            // 
            this.tlpSignCustom.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlpSignCustom.ColumnCount = 1;
            this.tlpSignCustom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSignCustom.Enabled = false;
            this.tlpSignCustom.Location = new System.Drawing.Point(6, 135);
            this.tlpSignCustom.Margin = new System.Windows.Forms.Padding(0);
            this.tlpSignCustom.Name = "tlpSignCustom";
            this.tlpSignCustom.Size = new System.Drawing.Size(223, 0);
            this.tlpSignCustom.TabIndex = 15;
            // 
            // btnSignAdd
            // 
            this.btnSignAdd.Enabled = false;
            this.btnSignAdd.Location = new System.Drawing.Point(72, 129);
            this.btnSignAdd.Name = "btnSignAdd";
            this.btnSignAdd.Size = new System.Drawing.Size(23, 23);
            this.btnSignAdd.TabIndex = 11;
            this.btnSignAdd.Text = "+";
            this.btnSignAdd.UseVisualStyleBackColor = true;
            this.btnSignAdd.Click += new System.EventHandler(this.btnCustomAdd_Click);
            // 
            // radSignNone
            // 
            this.radSignNone.AutoSize = true;
            this.radSignNone.Checked = true;
            this.radSignNone.Location = new System.Drawing.Point(6, 19);
            this.radSignNone.Name = "radSignNone";
            this.radSignNone.Size = new System.Drawing.Size(105, 17);
            this.radSignNone.TabIndex = 5;
            this.radSignNone.TabStop = true;
            this.radSignNone.Tag = 0;
            this.radSignNone.Text = "No sign changes";
            this.radSignNone.UseVisualStyleBackColor = true;
            // 
            // radSignCustom
            // 
            this.radSignCustom.AutoSize = true;
            this.radSignCustom.Location = new System.Drawing.Point(6, 134);
            this.radSignCustom.Name = "radSignCustom";
            this.radSignCustom.Size = new System.Drawing.Size(60, 17);
            this.radSignCustom.TabIndex = 10;
            this.radSignCustom.Tag = 5;
            this.radSignCustom.Text = "Custom";
            this.radSignCustom.UseVisualStyleBackColor = true;
            // 
            // radSignOdd
            // 
            this.radSignOdd.AutoSize = true;
            this.radSignOdd.Location = new System.Drawing.Point(6, 88);
            this.radSignOdd.Name = "radSignOdd";
            this.radSignOdd.Size = new System.Drawing.Size(111, 17);
            this.radSignOdd.TabIndex = 8;
            this.radSignOdd.Tag = 3;
            this.radSignOdd.Text = "Odd sign changes";
            this.radSignOdd.UseVisualStyleBackColor = true;
            // 
            // radSignEven
            // 
            this.radSignEven.AutoSize = true;
            this.radSignEven.Location = new System.Drawing.Point(6, 65);
            this.radSignEven.Name = "radSignEven";
            this.radSignEven.Size = new System.Drawing.Size(116, 17);
            this.radSignEven.TabIndex = 7;
            this.radSignEven.Tag = 2;
            this.radSignEven.Text = "Even sign changes";
            this.radSignEven.UseVisualStyleBackColor = true;
            // 
            // radSignAll
            // 
            this.radSignAll.AutoSize = true;
            this.radSignAll.Location = new System.Drawing.Point(6, 42);
            this.radSignAll.Name = "radSignAll";
            this.radSignAll.Size = new System.Drawing.Size(102, 17);
            this.radSignAll.TabIndex = 6;
            this.radSignAll.Tag = 1;
            this.radSignAll.Text = "All sign changes";
            this.radSignAll.UseVisualStyleBackColor = true;
            // 
            // grpOutput
            // 
            this.grpOutput.Controls.Add(this.radOutSym);
            this.grpOutput.Controls.Add(this.radOutVal);
            this.grpOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOutput.Location = new System.Drawing.Point(3, 461);
            this.grpOutput.Name = "grpOutput";
            this.grpOutput.Size = new System.Drawing.Size(236, 65);
            this.grpOutput.TabIndex = 7;
            this.grpOutput.TabStop = false;
            this.grpOutput.Text = "Output";
            // 
            // radOutSym
            // 
            this.radOutSym.AutoSize = true;
            this.radOutSym.Checked = true;
            this.radOutSym.Location = new System.Drawing.Point(6, 19);
            this.radOutSym.Name = "radOutSym";
            this.radOutSym.Size = new System.Drawing.Size(153, 17);
            this.radOutSym.TabIndex = 21;
            this.radOutSym.TabStop = true;
            this.radOutSym.Tag = 0;
            this.radOutSym.Text = "Show symbolic expressions";
            this.radOutSym.UseVisualStyleBackColor = true;
            // 
            // radOutVal
            // 
            this.radOutVal.AutoSize = true;
            this.radOutVal.Location = new System.Drawing.Point(6, 42);
            this.radOutVal.Name = "radOutVal";
            this.radOutVal.Size = new System.Drawing.Size(134, 17);
            this.radOutVal.TabIndex = 22;
            this.radOutVal.Tag = 1;
            this.radOutVal.Text = "Show numerical values";
            this.radOutVal.UseVisualStyleBackColor = true;
            // 
            // txtOutput
            // 
            this.txtOutput.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(268, 3);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(259, 343);
            this.txtOutput.TabIndex = 27;
            this.txtOutput.GotFocus += new System.EventHandler(this.txtOutput_GotFocus);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(12, 426);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 23;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInsert.Location = new System.Drawing.Point(467, 426);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(75, 23);
            this.btnInsert.TabIndex = 26;
            this.btnInsert.Text = "Insert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // tlpLayout
            // 
            this.tlpLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpLayout.ColumnCount = 2;
            this.tlpLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 265F));
            this.tlpLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLayout.Controls.Add(this.txtOutput, 1, 0);
            this.tlpLayout.Controls.Add(this.pnlOptions, 0, 0);
            this.tlpLayout.Location = new System.Drawing.Point(12, 55);
            this.tlpLayout.Name = "tlpLayout";
            this.tlpLayout.RowCount = 1;
            this.tlpLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLayout.Size = new System.Drawing.Size(530, 349);
            this.tlpLayout.TabIndex = 11;
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.Controls.Add(this.tlpOptions);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(3, 3);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(259, 343);
            this.pnlOptions.TabIndex = 9;
            // 
            // tlpOptions
            // 
            this.tlpOptions.AutoSize = true;
            this.tlpOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpOptions.ColumnCount = 1;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.Controls.Add(this.grpDimensions, 0, 0);
            this.tlpOptions.Controls.Add(this.grpOutput, 0, 4);
            this.tlpOptions.Controls.Add(this.grpFormatting, 0, 1);
            this.tlpOptions.Controls.Add(this.grpPermutation, 0, 3);
            this.tlpOptions.Controls.Add(this.grpSign, 0, 2);
            this.tlpOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpOptions.Location = new System.Drawing.Point(0, 0);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 5;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.Size = new System.Drawing.Size(242, 529);
            this.tlpOptions.TabIndex = 13;
            // 
            // grpDimensions
            // 
            this.grpDimensions.Controls.Add(this.btnProject);
            this.grpDimensions.Controls.Add(this.nudDimensions);
            this.grpDimensions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDimensions.Location = new System.Drawing.Point(3, 3);
            this.grpDimensions.Name = "grpDimensions";
            this.grpDimensions.Size = new System.Drawing.Size(236, 45);
            this.grpDimensions.TabIndex = 14;
            this.grpDimensions.TabStop = false;
            this.grpDimensions.Text = "Dimensions";
            // 
            // btnProject
            // 
            this.btnProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnProject.Location = new System.Drawing.Point(142, 16);
            this.btnProject.Name = "btnProject";
            this.btnProject.Size = new System.Drawing.Size(88, 23);
            this.btnProject.TabIndex = 2;
            this.btnProject.Text = "Project to 3D";
            this.btnProject.UseVisualStyleBackColor = true;
            this.btnProject.Click += new System.EventHandler(this.btnProject_Click);
            // 
            // nudDimensions
            // 
            this.nudDimensions.Location = new System.Drawing.Point(6, 19);
            this.nudDimensions.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudDimensions.Name = "nudDimensions";
            this.nudDimensions.Size = new System.Drawing.Size(88, 20);
            this.nudDimensions.TabIndex = 1;
            this.nudDimensions.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudDimensions.ValueChanged += new System.EventHandler(this.nudDimensions_ValueChanged);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(305, 426);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 25;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(224, 426);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 24;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // sfoExport
            // 
            this.sfoExport.DefaultExt = "off";
            this.sfoExport.Filter = "OFF files (*.off)|*.off|All files (*.*)|*.*";
            this.sfoExport.Title = "Save OFF file";
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPaste.Location = new System.Drawing.Point(386, 426);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 27;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnInsert;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 461);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.tlpLayout);
            this.Controls.Add(this.btnInsert);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtCoords);
            this.Controls.Add(this.lblCoords);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(570, 500);
            this.Name = "frmMain";
            this.Text = "OFF Builder";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grpFormatting.ResumeLayout(false);
            this.grpFormatting.PerformLayout();
            this.grpPermutation.ResumeLayout(false);
            this.grpPermutation.PerformLayout();
            this.grpSign.ResumeLayout(false);
            this.grpSign.PerformLayout();
            this.grpOutput.ResumeLayout(false);
            this.grpOutput.PerformLayout();
            this.tlpLayout.ResumeLayout(false);
            this.tlpLayout.PerformLayout();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.grpDimensions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudDimensions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCoords;
        private System.Windows.Forms.TextBox txtCoords;
        private System.Windows.Forms.RadioButton radFormSpace;
        private System.Windows.Forms.RadioButton radFormComma;
        private System.Windows.Forms.GroupBox grpFormatting;
        private System.Windows.Forms.GroupBox grpPermutation;
        private System.Windows.Forms.RadioButton radPermEven;
        private System.Windows.Forms.RadioButton radPermAll;
        private System.Windows.Forms.RadioButton radPermOdd;
        private System.Windows.Forms.RadioButton radPermCyclic;
        private System.Windows.Forms.RadioButton radPermCustom;
        private System.Windows.Forms.GroupBox grpSign;
        private System.Windows.Forms.RadioButton radSignCustom;
        private System.Windows.Forms.RadioButton radSignOdd;
        private System.Windows.Forms.RadioButton radSignEven;
        private System.Windows.Forms.RadioButton radSignAll;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.RadioButton radPermNone;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.RadioButton radSignNone;
        private System.Windows.Forms.GroupBox grpOutput;
        private System.Windows.Forms.RadioButton radOutSym;
        private System.Windows.Forms.RadioButton radOutVal;
        private System.Windows.Forms.TableLayoutPanel tlpLayout;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TableLayoutPanel tlpPermCustom;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.Button btnPermAdd;
        private System.Windows.Forms.Button btnSignAdd;
        private System.Windows.Forms.TableLayoutPanel tlpSignCustom;
        private System.Windows.Forms.Button btnSignClear;
        private System.Windows.Forms.Button btnPermClear;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog sfoExport;
        private System.Windows.Forms.GroupBox grpDimensions;
        private System.Windows.Forms.NumericUpDown nudDimensions;
        private System.Windows.Forms.Button btnProject;
        private System.Windows.Forms.RadioButton radSignFull;
        private System.Windows.Forms.Button btnPaste;
    }
}

