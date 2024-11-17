using NCalc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OFFBuilder
{
    public partial class frmMain : Form
    {
        #region Variables
        int signType = 0;
        int permType = 0;
        public static char sepChar = ' ';
        int outType = 0;
        public static int coordinates = 4;

        readonly List<CustomEntry> lstSignCustom = new List<CustomEntry>();
        readonly List<CustomEntry> lstPermCustom = new List<CustomEntry>();

        List<SignedStringArray> output = new List<SignedStringArray>();
        readonly Dictionary<SignedStringArray, int> hashes = new Dictionary<SignedStringArray, int>(new SignedStringArray.EqualityComparer());

        const int ROW_HEIGHT = 87;
        #endregion

        #region Controls
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            //Adds event handlers for the radio buttons.
            //These toggle specific variables depending on the selected option.
            foreach (RadioButton c in grpFormatting.Controls.OfType<RadioButton>())
                c.CheckedChanged += new EventHandler(radio_SepCheckChanged);

            foreach (RadioButton c in grpPermutation.Controls.OfType<RadioButton>())
                c.CheckedChanged += new EventHandler(radio_PermCheckChanged);

            foreach (RadioButton c in grpSign.Controls.OfType<RadioButton>())
                c.CheckedChanged += new EventHandler(radio_SignCheckChanged);

            foreach (RadioButton c in grpOutput.Controls.OfType<RadioButton>())
                c.CheckedChanged += new EventHandler(radio_OutCheckChanged);

            radSignCustom.CheckedChanged += radCustom_CheckedChanged;
            radPermCustom.CheckedChanged += radCustom_CheckedChanged;
        }

        /// <summary>
        /// Inserts a new set of coordinates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            sbTxt = new StringBuilder(txtOutput.Text);

            //Places the coordinates into an array.
            string[] coords_ = txtCoords.Text.Split(new char[] { sepChar }, StringSplitOptions.RemoveEmptyEntries);

            // Checks if there's right number of coordinates.
            if (coords_.Length == coordinates)
            {
                SignedString[] coords = new SignedString[coords_.Length];
                for (int i = 0; i < coords_.Length; i++)
                    coords[i] = coords_[i];

                switch (signType)
                {
                    //None
                    case 0:
                        AddSigns(coords, CustomEntry.None());
                        break;
                    //All
                    case 1:
                        AddSigns(coords, CustomEntry.All());
                        break;
                    //Even
                    case 2:
                        AddSigns(coords, CustomEntry.Even());
                        break;
                    //Odd
                    case 3:
                        AddSigns(coords, CustomEntry.Odd());
                        break;
                    //Full
                    case 4:
                        AddSigns(coords, CustomEntry.Full());
                        break;
                    //Custom
                    case 5:
                        AddSigns(coords, lstSignCustom);
                        break;
                }

                txtOutput.Text = sbTxt.ToString();
            }
        }

        /// <summary>
        /// Clears the output.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Text = "";
            output = new List<SignedStringArray>();
            hashes.Clear();
        }

        /// <summary>
        /// Runs when either radPermCustom or radSignCustom are checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == radPermCustom)
            {
                btnPermAdd.Enabled = radPermCustom.Checked;
                btnPermClear.Enabled = radPermCustom.Checked;
                tlpPermCustom.Enabled = radPermCustom.Checked;
            }
            else
            {
                btnSignAdd.Enabled = radSignCustom.Checked;
                btnSignClear.Enabled = radSignCustom.Checked;
                tlpSignCustom.Enabled = radSignCustom.Checked;
            }
        }

        /// <summary>
        /// Changes the separation character.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_SepCheckChanged(object sender, EventArgs e)
        {
            sepChar = (char)((RadioButton)sender).Tag;
        }

        /// <summary>
        /// Changes the permutation type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_PermCheckChanged(object sender, EventArgs e)
        {
            permType = (int)((RadioButton)sender).Tag;
        }

        /// <summary>
        /// Changes the sign type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_SignCheckChanged(object sender, EventArgs e)
        {
            signType = (int)((RadioButton)sender).Tag;
        }

        /// <summary>
        /// Changes the output format.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_OutCheckChanged(object sender, EventArgs e)
        {
            outType = (int)((RadioButton)sender).Tag;
            sbTxt = new StringBuilder();

            for (int i = 0; i < output.Count; i++)
                WriteCoords(output[i], false);

            txtOutput.Text = sbTxt.ToString();
        }

        /// <summary>
        /// Hides caret of output.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtOutput_GotFocus(object sender, EventArgs e)
        {
            HideCaret();
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        public void HideCaret()
        {
            HideCaret(txtOutput.Handle);
        }

        /// <summary>
        /// Copies textbox to clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtOutput.Text != "")
                Clipboard.SetText(txtOutput.Text);
        }

        /// <summary>
        /// Pastes clipboard to textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaste_Click(object sender, EventArgs e)
        {
            string[] clipboard = Clipboard.GetText().Split(new char[] { '\r', '\n' });
            sbTxt = new StringBuilder(txtOutput.Text);
            //Reads each line of the clipboard.
            for (int i = 0; i < clipboard.Length; i++)
            {
                if (clipboard[i] != "")
                {
                    string[] coords_ = clipboard[i].Split(new char[] { sepChar }, StringSplitOptions.RemoveEmptyEntries);
                    if (coords_.Length == coordinates)
                    {
                        SignedStringArray coords = new SignedString[coords_.Length];
                        for (int j = 0; j < coords_.Length; j++)
                            coords[j] = coords_[j];
                        //WriteCoords(coords);
                        switch (signType)
                        {
                            //None
                            case 0:
                                AddSigns(coords, CustomEntry.None());
                                break;
                            //All
                            case 1:
                                AddSigns(coords, CustomEntry.All());
                                break;
                            //Even
                            case 2:
                                AddSigns(coords, CustomEntry.Even());
                                break;
                            //Odd
                            case 3:
                                AddSigns(coords, CustomEntry.Odd());
                                break;
                            //Full
                            case 4:
                                AddSigns(coords, CustomEntry.Full());
                                break;
                            //Custom
                            case 5:
                                AddSigns(coords, lstSignCustom);
                                break;
                        }
                    }
                }
            }
            txtOutput.Text = sbTxt.ToString();
        }

        /// <summary>
        /// Changes the type of a custom permutation or sign change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radCustomType_CheckedChanged(object sender, EventArgs e)
        {
            //Gets info about the sender.
            RadioButton rad = (RadioButton)sender;
            Control tlp = rad.Parent.Parent;
            int i = (int)tlp.Tag;
            ParityType p = (ParityType)rad.Tag;

            //Checks whether the radio button is for signs or permutations.
            if (tlp.Parent == tlpPermCustom)
                lstPermCustom[i].Type = p;
            else
                lstSignCustom[i].Type = p;
        }

        /// <summary>
        /// Changes the coordinates affected by a custom permutation or sign change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void chkCustomIndex_CheckedChanged(object sender, EventArgs e)
        {
            //Gets info about the sender.
            CheckBox chk = (CheckBox)sender;
            Control tlp = chk.Parent.Parent;
            int i = (int)tlp.Tag;
            int j = (int)chk.Tag;

            //Checks whether the radio button is for signs or permutations.
            if (tlp.Parent == tlpPermCustom)
                lstPermCustom[i].Indices[j] = chk.Checked;
            else
                lstSignCustom[i].Indices[j] = chk.Checked;
        }

        /// <summary>
        /// Adds a custom permutation or sign change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomAdd_Click(object sender, EventArgs e)
        {
            TableLayoutPanel tlpCustom;
            List<CustomEntry> lst;

            if (sender == btnPermAdd)
            {
                tlpCustom = tlpPermCustom;
                grpPermutation.Height += ROW_HEIGHT;
                lst = lstPermCustom;
            }
            else
            {
                tlpCustom = tlpSignCustom;
                grpSign.Height += ROW_HEIGHT;
                lst = lstSignCustom;
            }

            tlpCustom.SuspendLayout();

            //Adds rows to tableLayoutPanel.
            tlpCustom.RowCount++;
            tlpCustom.RowStyles.Add(new RowStyle(SizeType.Absolute, ROW_HEIGHT));

            //Adds nested tableLayoutPanel.
            TableLayoutPanel tlpCustomCell = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                Tag = tlpCustom.RowCount - 1
            };
            tlpCustomCell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 29));
            tlpCustomCell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpCustomCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
            tlpCustomCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tlpCustom.Controls.Add(tlpCustomCell, 0, tlpCustom.RowCount - 1);

            //Adds the button to remove rows.
            Button btnCustomRemove = new Button
            {
                Text = "–",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnCustomRemove.Click += btnCustomRemove_Click;
            tlpCustomCell.Controls.Add(btnCustomRemove, 0, 0);

            //Adds the permutation types panel.
            Panel pnlCustomTypes = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false
            };
            tlpCustomCell.Controls.Add(pnlCustomTypes, 1, 0);

            //Adds the permutation types radio buttons.            
            RadioButton radCustomAll = new RadioButton();
            RadioButton radCustomEven = new RadioButton();
            RadioButton radCustomOdd = new RadioButton();
            RadioButton radCustomFull = new RadioButton();
            RadioButton radCustomCyclic = new RadioButton();
            radCustomAll.Checked = true;
            radCustomAll.Text = "All";
            radCustomEven.Text = "Even";
            radCustomOdd.Text = "Odd";
            radCustomFull.Text = "Full";
            radCustomCyclic.Text = "Cyclic";
            radCustomAll.Location = new Point(3, 3);
            radCustomEven.Location = new Point(40, 3);
            radCustomOdd.Location = new Point(90, 3);
            radCustomFull.Location = new Point(130, 3);
            radCustomCyclic.Location = new Point(125, 3);
            radCustomAll.Size = new Size(36, 17);
            radCustomEven.Size = new Size(50, 17);
            radCustomOdd.Size = new Size(45, 17);
            radCustomFull.Size = new Size(69420, 17);
            radCustomCyclic.Size = new Size(69420, 17);
            radCustomAll.Tag = ParityType.All;
            radCustomEven.Tag = ParityType.Even;
            radCustomOdd.Tag = ParityType.Odd;
            radCustomFull.Tag = ParityType.Full;
            radCustomCyclic.Tag = ParityType.Cyclic;
            radCustomAll.CheckedChanged += radCustomType_CheckedChanged;
            radCustomEven.CheckedChanged += radCustomType_CheckedChanged;
            radCustomOdd.CheckedChanged += radCustomType_CheckedChanged;
            radCustomFull.CheckedChanged += radCustomType_CheckedChanged;
            radCustomCyclic.CheckedChanged += radCustomType_CheckedChanged;
            pnlCustomTypes.Controls.Add(radCustomAll);
            pnlCustomTypes.Controls.Add(radCustomEven);
            pnlCustomTypes.Controls.Add(radCustomOdd);
            if (sender == btnSignAdd)
                pnlCustomTypes.Controls.Add(radCustomFull);
            if (sender == btnPermAdd)
                pnlCustomTypes.Controls.Add(radCustomCyclic);

            //Adds the permutation index panel.
            Panel pnlCustomIndices = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            tlpCustomCell.Controls.Add(pnlCustomIndices, 1, 1);

            //Adds the permutation index checkboxes.
            AddCheckBoxes(pnlCustomIndices);

            bool[] b = new bool[coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = false;

            lst.Add(new CustomEntry(ParityType.All, b));
            tlpCustom.Height += ROW_HEIGHT;
            tlpCustom.ResumeLayout(true);
        }

        /// <summary>
        /// Removes a custom permutation or sign change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomRemove_Click(object sender, EventArgs e)
        {
            //Gets the button and the enclosing TableLayoutPanel.
            Button btn = (Button)sender;
            int r = (int)btn.Parent.Tag;
            TableLayoutPanel tlpCustom = (TableLayoutPanel)btn.Parent.Parent;
            List<CustomEntry> lstCustom;

            tlpCustom.Height -= ROW_HEIGHT;
            if (tlpCustom == tlpPermCustom)
            {
                grpPermutation.Height -= ROW_HEIGHT;
                lstCustom = lstPermCustom;
            }
            else
            {
                grpSign.Height -= ROW_HEIGHT;
                lstCustom = lstSignCustom;
            }

            tlpCustom.SuspendLayout();

            //Removes control from corresponding row.
            tlpCustom.GetControlFromPosition(0, r).Dispose();
            tlpCustom.Controls.Remove(tlpCustom.GetControlFromPosition(0, r));
            lstCustom.RemoveAt(r);

            //Moves controls to the corresponding rows, updates tags.
            for (int s = r + 1; s < tlpCustom.RowCount; s++)
            {
                Control crt = tlpCustom.GetControlFromPosition(0, s);
                crt.Tag = ((int)crt.Tag) - 1;
                tlpCustom.SetCellPosition(crt, new TableLayoutPanelCellPosition(0, s - 1));
            }

            //Deletes the empty row.
            tlpCustom.RowStyles.RemoveAt(--tlpCustom.RowCount);

            tlpOptions.RowStyles[4] = new RowStyle(SizeType.Absolute, 1);
            tlpCustom.ResumeLayout(true);
        }

        /// <summary>
        /// Clears custom permutations or sign changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomClear_Click(object sender, EventArgs e)
        {
            TableLayoutPanel tlpCustom;
            List<CustomEntry> lstCustom;

            if (sender == btnPermClear)
            {
                tlpCustom = tlpPermCustom;
                lstCustom = lstPermCustom;
                grpPermutation.Height -= ROW_HEIGHT * tlpPermCustom.RowCount;
            }
            else
            {
                tlpCustom = tlpSignCustom;
                lstCustom = lstSignCustom;
                grpSign.Height -= ROW_HEIGHT * tlpSignCustom.RowCount;
            }

            tlpCustom.SuspendLayout();
            grpFormatting.Height -= ROW_HEIGHT * tlpPermCustom.RowCount;

            while (tlpCustom.Controls.Count > 0)
                tlpCustom.Controls[0].Dispose();

            tlpCustom.Controls.Clear();
            tlpCustom.RowStyles.Clear();
            tlpCustom.RowCount = 0;
            tlpCustom.Size = new Size(223, 0);
            lstCustom.Clear();

            tlpCustom.ResumeLayout(true);
        }

        //Verifies text written to the coordinates.
        readonly Regex R = new Regex(" +");
        private void txtCoords_TextChanged(object sender, EventArgs e)
        {
            int s = txtCoords.SelectionStart;

            //Calculates where to move the caret.
            int b = 0; //Blocks of spaces.
            int c = 1; //Consecutive spaces. Stars from 1, so that leading spaces are removed.
            int d = 0; //How many spaces to the left the caret will be moved.
            for (int i = 0; i < txtCoords.Text.Length; i++)
            {
                if (txtCoords.Text[i] == sepChar)
                {
                    if (i < s && ++c >= 2) //Double spaces will be deleted.
                        d++;
                    else if (++b >= coordinates) //Extra coordinates will be deleted.
                    {
                        txtCoords.Text = txtCoords.Text.Substring(0, i);
                        txtCoords.SelectionStart = Math.Min(s - d, txtCoords.Text.Length);
                        return;
                    }
                }
                else
                    c = 0;
            }

            //Removes multiple spaces.
            txtCoords.Text = R.Replace(txtCoords.Text, " ").TrimStart();
            txtCoords.SelectionStart = s - d;
        }

        private void AddCheckBoxes(Panel pnl)
        {
            //If there are excess controls, removes the unneeded ones.
            if (pnl.Controls.Count >= coordinates)
            {
                for (int i = coordinates; i < pnl.Controls.Count; i++)
                {
                    pnl.Controls["chk" + i].Dispose();
                    pnl.Controls.RemoveByKey("chk" + i);
                }
            }
            else
            {
                //A checkbox with n digits takes up a space of 32 + 6n.
                int x = 3 + 38 * Math.Min(9, pnl.Controls.Count) + 44 * Math.Max(0, pnl.Controls.Count - 10) - pnl.HorizontalScroll.Value;

                for (int i = pnl.Controls.Count; i < coordinates; i++)
                {
                    string n = (i + 1).ToString();

                    CheckBox chkCustomIndex = new CheckBox
                    {
                        Text = n,
                        Location = new Point(x, 3),
                        Size = new Size(26 + 6 * n.Length, 17), //Size depends on text length.
                        Tag = i,
                        Name = "chk" + i
                    };
                    chkCustomIndex.CheckedChanged += chkCustomIndex_CheckedChanged;
                    pnl.Controls.Add(chkCustomIndex);

                    x += 32 + 6 * n.Length; //Pads each checkbox depending on text length.
                }
            }
        }

        /// <summary>
        /// Exports the coordinates as an OFF file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (sfoExport.ShowDialog() == DialogResult.OK)
            {
                QConvex.CreateOFFFile(sfoExport.FileName, output);
            }
        }
        #endregion

        #region Processing
        /// <summary>
        /// Adds signs to coordinates.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="customEntries"></param>
        /// <param name="indx"></param>
        private void AddSigns(SignedStringArray coords, List<CustomEntry> customEntries, int indx = 0)
        {
            coords = coords.Clone();

            //If no more sign changes are to be made.
            if (indx >= customEntries.Count || customEntries[indx].Type == ParityType.None)
            {
                switch (permType)
                {
                    //None
                    case 0:
                        AddPermutations(coords, CustomEntry.None());
                        return;
                    //All
                    case 1:
                        AddPermutations(coords, CustomEntry.All());
                        return;
                    //Even
                    case 2:
                        AddPermutations(coords, CustomEntry.Even());
                        return;
                    //Odd
                    case 3:
                        AddPermutations(coords, CustomEntry.Odd());
                        return;
                    //Cyclic
                    case 4:
                        AddPermutations(coords, CustomEntry.Cyclic());
                        return;
                    //Custom
                    case 5:
                        AddPermutations(coords, lstPermCustom);
                        return;
                }
            }

            //Counter to go through all sign changes.
            int N;

            //Finds the non-zero coordinates.
            List<int> y = new List<int>();
            for (int i = 0; i < coords.Length; i++)
                if (customEntries[indx].Indices[i] && coords[i].Value != 0)
                    y.Add(i);
            int[] z = y.ToArray();

            //Applies the appropriate sign changes.
            switch (customEntries[indx].Type)
            {
                case ParityType.Full:
                    AddSigns(coords, customEntries, indx + 1);
                    for (int i = 0; i < z.Length; i++)
                        coords[z[i]].Neg();
                    AddSigns(coords, customEntries, indx + 1);
                    break;
                case ParityType.All:
                    N = 1 << z.Length;
                    //Cycles through all sign combinations.
                    //At each step i, the coordinates' signs will correspond to i's binary digits.
                    AddSigns(coords, customEntries, indx + 1);
                    for (int i = 1; i < N; i++)
                    {
                        int c = i ^ (i - 1);
                        for (int j = 0; j < coords.Length; j++)
                        {
                            if ((c & 1) == 1)
                                coords[z[j]].Neg();
                            c >>= 1;
                        }

                        AddSigns(coords, customEntries, indx + 1);
                    }
                    break;
                case ParityType.Even:
                    N = 1 << (z.Length - 1);
                    //Cycles through all even sign combinations.
                    //At each step i, the coordinates' signs will correspond to i's binary digits.
                    //The last coordinate's sign will be toggled accordingly.
                    AddSigns(coords, customEntries, indx + 1);
                    for (int i = 1; i < N; i++)
                    {
                        int c = i ^ (i - 1);
                        int k = 0;

                        for (int j = 0; j < coords.Length - 1; j++)
                        {
                            if ((c & 1) == 1)
                            {
                                coords[z[j]].Neg();
                                k++;
                            }
                            c >>= 1;
                        }
                        if ((k & 1) == 1)
                            coords[z[z.Length - 1]].Neg();

                        AddSigns(coords, customEntries, indx + 1);
                    }
                    break;
                case ParityType.Odd:
                    coords[z[0]].Neg();
                    goto case ParityType.Even;
            }
        }

        /// <summary>
        /// Applies the appropriate permutations.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="customEntries"></param>
        /// <param name="indx"></param>
        private void AddPermutations(SignedStringArray coords, List<CustomEntry> customEntries, int indx = 0)
        {
            coords = coords.Clone();

            //If no more permutations are to be made.
            if (indx >= customEntries.Count || customEntries[indx].Type == ParityType.None)
            {
                WriteCoords(coords);
                return;
            }

            int i, j;
            bool parity = customEntries[indx].Type == ParityType.Even;

            //Finds the coordinates to permutate.
            List<int> y = new List<int>();
            for (int k = 0; k < coords.Length; k++)
                if (customEntries[indx].Indices[k])
                    y.Add(k);
            int[] z = y.ToArray();

            int[] o = Enumerable.Range(0, coords.Length).ToArray();

            //cyclic permutations
            if (customEntries[indx].Type == ParityType.Cyclic)
            {
                AddPermutations(coords, customEntries, indx + 1);
                for (i = 0; i < z.Length - 1; i++)
                {
                    for (j = 0; j < z.Length - 1; j++)
                    {
                        //Performs the swap.
                        (coords[z[j + 1]], coords[z[j]]) = (coords[z[j]], coords[z[j + 1]]);
                    }
                    AddPermutations(coords, customEntries, indx + 1);
                }
            }
            else
            {
                //Generates permutations.
                //Based on https://www.nayuki.io/page/next-lexicographical-permutation-algorithm

                //The loop will be exited whenever all permutations are traversed.
                while (true)
                {
                    if (customEntries[indx].Type == ParityType.All || parity)
                        AddPermutations(coords, customEntries, indx + 1);

                    //Finds first swap position.
                    i = z.Length - 1;
                    while (i > 0 && o[z[i - 1]] >= o[z[i]])
                        i--;

                    //Are we at the last permutation already?
                    if (i <= 0)
                        return;

                    //Finds second swap position.
                    j = z.Length - 1;
                    while (o[z[j]] <= o[z[i - 1]])
                        j--;

                    //Performs the swap...yet again.
                    (coords[z[i - 1]], coords[z[j]]) = (coords[z[j]], coords[z[i - 1]]);
                    (o[z[i - 1]], o[z[j]]) = (o[z[j]], o[z[i - 1]]);
                    parity = !parity;

                    //Reverses the suffix.
                    j = z.Length - 1;
                    while (i < j)
                    {
                        (coords[z[i]], coords[z[j]]) = (coords[z[j]], coords[z[i]]);
                        (o[z[i]], o[z[j]]) = (o[z[j]], o[z[i]]);
                        i++;
                        j--;
                        parity = !parity;
                    }
                }
            }
        }

        //Writes a set of coordinates to the textbox (and to the output, if specified).
        StringBuilder sbTxt;
        private void WriteCoords(SignedStringArray coords, bool addToOutput = true)
        {
            int i;
            if (addToOutput)
            {
                if (hashes.ContainsKey(coords))
                    return;

                SignedStringArray clone = coords.Clone();
                hashes.Add(clone, 0);
                output.Add(clone);
            }

            for (i = 0; i < coords.Length; i++)
            {
                //Symbolic
                if (outType == 0)
                    sbTxt.Append(coords[i]);
                //Numeric
                else
                    sbTxt.Append(coords[i].Value);

                if (i < coords.Length - 1)
                    sbTxt.Append(sepChar);
                else
                    sbTxt.Append("\r\n");
            }
        }
        #endregion

        private void nudDimensions_ValueChanged(object sender, EventArgs e)
        {
            int i;

            //Clears output.
            btnClear_Click(null, null);

            //Adds the appropriate amounts of checkboxes.
            coordinates = (int)nudDimensions.Value;
            for (i = 0; i < tlpPermCustom.RowCount; i++)
                AddCheckBoxes((Panel)((TableLayoutPanel)tlpPermCustom.GetControlFromPosition(0, i)).GetControlFromPosition(1, 1));
            for (i = 0; i < tlpSignCustom.RowCount; i++)
                AddCheckBoxes((Panel)((TableLayoutPanel)tlpSignCustom.GetControlFromPosition(0, i)).GetControlFromPosition(1, 1));

            //Updates the CustomEntries.
            for (i = 0; i < lstPermCustom.Count; i++)
                lstPermCustom[i].ChangeIndexCount();
            for (i = 0; i < lstSignCustom.Count; i++)
                lstSignCustom[i].ChangeIndexCount();

            btnProject.Text = "Project to " + (coordinates - 1) + "D";
            btnProject.Enabled = coordinates > 2;
        }

        //Projects the vertices one dimension lower, by orthogonal projection onto the hyperplane w+x+y+z+...=0.
        private void btnProject_Click(object sender, EventArgs e)
        {
            List<SignedStringArray> outputOld = output;
            nudDimensions.Value--;
            hashes.Clear();
            sbTxt.Clear();

            //Generates projection matrix.
            string[,] PM = new string[coordinates, coordinates + 1];
            for (int i = 1; i <= coordinates; i++)
            {
                for (int j = 1; j <= coordinates + 1; j++)
                {
                    //Calculates the corresponding entry of the projection matrix.
                    StringBuilder PME = new StringBuilder();

                    //Adds the zeros of the matrix entries.
                    if (j > i + 1)
                    {
                        PM[i - 1, j - 1] = "0";
                        continue;
                    }
                    if (j <= i)
                        PME.Append("-");

                    //Numerator of the entry.
                    if (i == 1)
                        PME.Append("1");
                    else
                        PME.Append("sqrt(" + i * (i + 1) / 2 + ")");

                    //Denominator.
                    PME.Append("/");
                    if (j <= i)
                        PME.Append(i * (i + 1));
                    else
                        PME.Append(j);

                    PM[i - 1, j - 1] = PME.ToString();
                }
            }

            for (int i = 0; i < outputOld.Count; i++)
            {
                SignedStringArray project = new SignedStringArray(new SignedString[coordinates]);
                for (int j = 0; j < coordinates; j++)
                {
                    StringBuilder coord = new StringBuilder();
                    for (int k = 0; k <= coordinates; k++)
                    {
                        if (PM[j, k] != "0" && outputOld[i][k].Value != 0)
                        {
                            if (coord.Length > 0)
                                coord.Append("+");

                            if (outputOld[i][k].Value == 1)
                                coord.Append(PM[j, k]);
                            else
                                coord.Append("(" + PM[j, k] + ")*(" + outputOld[i][k].ToString() + ")");
                        }
                    }

                    if (coord.Length > 0)
                        project[j] = coord.ToString();
                    else
                        project[j] = "0";
                }

                WriteCoords(project);
            }

            txtOutput.Text = sbTxt.ToString();
        }
    }

    /// <summary>
    /// A simple class to be able to easily deal with negating expressions.
    /// </summary>
    public class SignedString
    {
        readonly string str;
        bool sign;
        const double PHI = 1.618033988749895;

        public double Value;

        public SignedString(string s)
        {
            str = s.Trim(new char[] { frmMain.sepChar });
            sign = true;

            try
            {
                Expression e = new Expression(s.ToLower(), EvaluateOptions.IgnoreCase);
                e.Parameters["pi"] = Math.PI;
                e.Parameters["phi"] = PHI;
                Value = Convert.ToDouble(e.Evaluate());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!");
                Value = double.NaN;
            }
        }

        public void Neg()
        {
            sign = !sign;
            Value = -Value;
        }

        public SignedString(SignedString s)
        {
            str = s.str;
            sign = s.sign;
            Value = s.Value;
        }

        public override string ToString()
        {
            if (sign)
                return str;

            //Very crude way of not adding unnecessary parentheses.
            if (!str.Contains('+') && !str.Contains('-'))
            {
                // v -> -v where v has no -'s or +'s
                return "-" + str;
            }
            if (str[0] == '-' && str[1] == '(' && str[str.Length - 1] == ')')
            {
                // -(v) -> v
                return str.Remove(0, 2).Remove(str.Length - 3);
            }
            if (str.Count(x => x == '-') == 1 && str[0] == '-' && !str.Contains('+'))
            {
                // -v -> v where v has no -'s or +'s
                return str.Remove(0, 1);
            }
            return "-(" + str + ")";
        }

        public static bool operator <(SignedString a, SignedString b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >(SignedString a, SignedString b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <=(SignedString a, SignedString b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator >=(SignedString a, SignedString b)
        {
            return a.Value >= b.Value;
        }

        public static implicit operator SignedString(string s)
        {
            return new SignedString(s);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    /// <summary>
    /// A wrapper for a SignedString[] implementing a hash function.
    /// </summary>
    public class SignedStringArray
    {
        readonly SignedString[] Value;

        public SignedStringArray(SignedString[] arr)
        {
            Value = arr;
        }

        public int Length
        {
            get
            {
                return Value.Length;
            }
        }

        public SignedString this[int i]
        {
            get
            {
                return Value[i];
            }
            set
            {
                Value[i] = value;
            }
        }

        public static implicit operator SignedStringArray(SignedString[] s)
        {
            return new SignedStringArray(s);
        }

        public SignedStringArray Clone()
        {
            SignedString[] clone = new SignedString[this.Length];
            for (int i = 0; i < clone.Length; i++)
                clone[i] = new SignedString(this[i]);

            return clone;
        }

        public class EqualityComparer : IEqualityComparer<SignedStringArray>
        {
            public int GetHashCode(SignedStringArray s)
            {
                int h = 0;
                for (int i = 0; i < s.Value.Length; i++)
                    h = ((double)(h + s.Value[i].GetHashCode())).GetHashCode();
                return h;
            }

            public bool Equals(SignedStringArray s, SignedStringArray t)
            {
                if (s.Length != t.Length)
                    return false;

                for (int i = 0; i < s.Value.Length; i++)
                    if (s[i].Value != t[i].Value)
                        return false;

                return true;
            }
        }
    }

    /// <summary>
    /// Used to state the type of either permutations or sign changes.
    /// </summary>
    public enum ParityType
    {
        None,
        All,
        Even,
        Odd,
        Full, //Sign changes only
        Cyclic //Permutations only
    }

    /// <summary>
    /// A class to store either a single custom sign change or a single custom permutation.
    /// </summary>
    public class CustomEntry
    {
        public ParityType Type;

        public bool[] Indices;

        public CustomEntry(ParityType t, bool[] b)
        {
            Type = t;
            Indices = b;
        }

        public static List<CustomEntry> None()
        {
            return new List<CustomEntry> { new CustomEntry(ParityType.None, null) };
        }

        public static List<CustomEntry> All()
        {
            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = true;

            return new List<CustomEntry> { new CustomEntry(ParityType.All, b) };
        }

        public static List<CustomEntry> Even()
        {
            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = true;

            return new List<CustomEntry> { new CustomEntry(ParityType.Even, b) };
        }

        public static List<CustomEntry> Odd()
        {
            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = true;

            return new List<CustomEntry> { new CustomEntry(ParityType.Odd, b) };
        }

        public static List<CustomEntry> Full()
        {
            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = true;

            return new List<CustomEntry> { new CustomEntry(ParityType.Full, b) };
        }

        public static List<CustomEntry> Cyclic()
        {
            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < b.Length; i++)
                b[i] = true;

            return new List<CustomEntry> { new CustomEntry(ParityType.Cyclic, b) };
        }

        public void ChangeIndexCount()
        {
            if (frmMain.coordinates == Indices.Length)
                return;

            bool[] b = new bool[frmMain.coordinates];
            for (int i = 0; i < Math.Min(frmMain.coordinates, Indices.Length); i++)
                b[i] = Indices[i];
            Indices = b;
        }
    }

    /// <summary>
    /// Compares faces in lexicographic order for use with a sorted dictionary used to check for duplicate faces.
    /// </summary>
    public class FaceCompare : Comparer<int[]>
    {
        public override int Compare(int[] x, int[] y)
        {
            if (x.Length == y.Length)
            {
                for (int i = 0; i < Math.Min(x.Length, y.Length); i++)
                {
                    if (x[i] != y[i])
                    {
                        return x[i] - y[i];
                    }
                }
            }
            return x.Length - y.Length;
        }
    }

    /// <summary>
    /// A class to save OFF files.
    /// </summary>
    public static class QConvex
    {
        static readonly string QCONVEX = Path.Combine(Application.StartupPath, "qconvex" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));
        static readonly string TMP_FILE_IN = Path.Combine(Application.StartupPath, "tmp.txt");
        static readonly string TMP_FILE_OUT = Path.Combine(Application.StartupPath, "tmp.off");
        private static readonly string[] elementNames = new string[] { "Vertices", "Edges", "Faces", "Cells", "Tera", "Peta", "Exa", "Zetta", "Yotta", "Xenna", "Daka", "Henda", "Doka", "Tradaka", "Tedaka", "Pedaka", "Exdaka", "Zedaka", "Yodaka", "Nedaka", "Ika", "Ikena", "Ikoda", "Iktra" };

        /// <summary>
        ///     Creates an OFF file from coordinates.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="coords"></param>
        public static void CreateOFFFile(string path, List<SignedStringArray> coords)
        {
            try
            {
                //Writes coords to a temp file.
                using (StreamWriter sw = new StreamWriter(TMP_FILE_IN))
                {
                    sw.WriteLine(coords[0].Length);
                    sw.WriteLine(coords.Count);

                    for (int i = 0; i < coords.Count; i++)
                    {
                        for (int j = 0; j < coords[i].Length; j++)
                        {
                            sw.Write(string.Format("{0:G17}", coords[i][j].Value));

                            if (j < coords[j].Length - 1)
                                sw.Write(' ');
                            else
                                sw.WriteLine();
                        }
                    }
                }

                //Runs Qconvex
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = QCONVEX,
                        Arguments = "Q14 C-0.00001 C0.00001 TI \"" + TMP_FILE_IN + "\" o TO \"" + TMP_FILE_OUT + "\"",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string err = "";
                    switch (process.ExitCode)
                    {
                        case 1:
                            err = "qh_ERRinput";
                            break;
                        case 2:
                            err = "qh_ERRsingular";
                            break;
                        case 3:
                            err = "qh_ERRprec";
                            break;
                        case 4:
                            err = "qh_qh_ERRmem";
                            break;
                        case 5:
                            err = "qh_ERRQhull";
                            break;
                        case 6:
                            err = "qh_ERRother";
                            break;
                        case 7:
                            err = "qh_ERRtopology";
                            break;
                        case 8:
                            err = "qh_ERRwide";
                            break;
                        case 9:
                            err = "qh_ERRdebug";
                            break;
                    }
                    throw new Exception("Qhull failed to create convex hull. " + err);
                }

                File.Delete(TMP_FILE_IN);

                //Reads QConvex output, converts it to the Stella OFF format.
                List<int[]>[] elementList;
                double[][] vertexList;
                string[][] vertexListString;
                int dim;

                using (StreamReader sr = new StreamReader(TMP_FILE_OUT))
                {
                    string line = sr.ReadLine();
                    /* d-dimensional elements are stored in elementList[d].
                     * This means that the first two entries will be empty, but whatever.
                     * Also, polygons are stored as polyhedra with a single face. */
                    dim = Convert.ToInt32(line);
                    elementList = new List<int[]>[Math.Max(dim + 1, 3)];
                    line = sr.ReadLine();
                    vertexList = new double[Convert.ToUInt64(line.Substring(0, line.IndexOf(' ')))][];
                    vertexListString = new string[vertexList.Length][];

                    //Loads vertexList with the vertex coordinates.
                    for (int i = 0; i < vertexList.Length; i++)
                    {
                        string[] vertex = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        vertexList[i] = new double[vertex.Length];
                        vertexListString[i] = new string[vertex.Length];

                        for (int j = 0; j < vertex.Length; j++)
                        {
                            vertexList[i][j] = Convert.ToDouble(vertex[j]);
                            vertexListString[i][j] = vertex[j].ToString();
                        }
                    }

                    //Loads facets.
                    elementList[dim - 1] = new List<int[]>();
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        string[] strValues = line.Substring(line.IndexOf(' ')).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int[] intValues = new int[strValues.Length];
                        for (int i = 0; i < intValues.Length; i++)
                            intValues[i] = Convert.ToInt32(strValues[i]);
                        Array.Sort(intValues);
                        elementList[dim - 1].Add(intValues);
                    }
                }

                File.Delete(TMP_FILE_OUT);

                Stopwatch time = new Stopwatch();
                Stopwatch commonVerticesTime = new Stopwatch();
                Stopwatch duplicateElementsTime = new Stopwatch();

                time.Start();

                /* If the polytope is 2D, its convex hull has already been calculated by Qhull.
                 * newElements[2] is set to the vertices in order. */
                if (dim == 2)
                {
                    var face = new int[elementList[1].Count];
                    var edgeUsed = new bool[elementList[1].Count];

                    // First edge
                    face[0] = elementList[1][0][0];
                    face[1] = elementList[1][0][1];

                    // Other edges
                    for (int v = 1; v < elementList[1].Count - 1; v++)
                    {
                        for (int e = 1; v < elementList[1].Count; e++)
                        {
                            if (edgeUsed[e] == false)
                            {
                                if (face[v] == elementList[1][e][0])
                                {
                                    face[v + 1] = elementList[1][e][1];
                                    edgeUsed[e] = true;
                                    break;
                                }
                                if (face[v] == elementList[1][e][1])
                                {
                                    face[v + 1] = elementList[1][e][0];
                                    edgeUsed[e] = true;
                                    break;
                                }
                            }
                        }
                    }

                    elementList[2] = new List<int[]> { face };
                }
                else
                {
                    // Maximal element.
                    elementList[dim] = new List<int[]> { Enumerable.Range(0, elementList[dim - 1].Count).ToArray() };

                    /* Generates (d-1)-dimensional faces out of d-dimensional faces.
                     * We generate edges to create the faces. */
                    for (int d = dim - 1; d >= 2; d--)
                    {
                        /* At the same time I find (d-1)-faces, I need to rewrite d-faces in terms of them.
                         * That is, except in the case d=2. */
                        elementList[d - 1] = new List<int[]>();
                        List<int>[] newElements = new List<int>[elementList[d].Count];
                        for (int i = 0; i < newElements.Length; i++)
                            newElements[i] = new List<int>();

                        SortedDictionary<int[], int> dm1Elements = new SortedDictionary<int[], int>(new FaceCompare());

                        /* Two d-faces sharing a (d-1)-face must also share a (d+1)-face.
                         * We take advantage of this to speed up the process. */
                        foreach (int[] dp1Face in elementList[d + 1])
                            for (int i = 0; i < dp1Face.Length - 1; i++)
                                for (int j = i + 1; j < dp1Face.Length; j++)
                                {
                                    commonVerticesTime.Start();

                                    //Finds common vertices.
                                    List<int> commonVertices = new List<int>(d);
                                    int m = 0, n = 0;
                                    int[] a = elementList[d][dp1Face[i]], b = elementList[d][dp1Face[j]];
                                    while (m < a.Length && n < b.Length)
                                    {
                                        if (a[m] < b[n])
                                            m++;
                                        else if (a[m] > b[n])
                                            n++;
                                        else
                                        {
                                            commonVertices.Add(a[m]);
                                            m++;
                                            n++;
                                        }
                                    }
                                    commonVerticesTime.Stop();

                                    /* If two d-dimensional elements have more than d common vertices, they form a (d-1)-face...
                                    * ...as long as d ≤ 3. For d ≥ 4, there's the possibility they actually just share a 2-face.
                                    * Furthermore, a (d-1)-face won't be shared by more than two d-dimensional elements. */
                                    if (commonVertices.Count >= d && (d < 4 || Rank(vertexList, commonVertices) == d - 1))
                                    {
                                        duplicateElementsTime.Start();

                                        /* Checks if the face has not been added before.
                                            * The face index, old or new, is added to the corresponding newElements. */
                                        if (dm1Elements.TryGetValue(commonVertices.ToArray(), out int idx))
                                        {
                                            if (!newElements[dp1Face[i]].Contains(idx))
                                                newElements[dp1Face[i]].Add(idx);
                                            if (!newElements[dp1Face[j]].Contains(idx))
                                                newElements[dp1Face[j]].Add(idx);
                                        }
                                        else
                                        {
                                            idx = dm1Elements.Count;
                                            dm1Elements.Add(commonVertices.ToArray(), idx);
                                            newElements[dp1Face[i]].Add(idx);
                                            newElements[dp1Face[j]].Add(idx);
                                            elementList[d - 1].Add(commonVertices.ToArray());
                                        }
                                        duplicateElementsTime.Stop();
                                    }
                                }

                        for (int i = 0; i < elementList[d].Count; i++)
                            elementList[d][i] = newElements[i].ToArray();
                    }

                    //Creates faces by connecting edges.
                    for (var f = 0; f < elementList[2].Count; f++)
                    {
                        var face = new int[elementList[2][f].Length];
                        var edgeUsed = new bool[elementList[2][f].Length];

                        // First edge
                        face[0] = elementList[1][elementList[2][f][0]][0];
                        face[1] = elementList[1][elementList[2][f][0]][1];

                        // Other edges
                        for (int v = 1; v < elementList[2][f].Length - 1; v++)
                        {
                            for (int e = 1; v < elementList[2][f].Length; e++)
                            {
                                if (edgeUsed[e] == false)
                                {
                                    if (face[v] == elementList[1][elementList[2][f][e]][0])
                                    {
                                        face[v + 1] = elementList[1][elementList[2][f][e]][1];
                                        edgeUsed[e] = true;
                                        break;
                                    }
                                    if (face[v] == elementList[1][elementList[2][f][e]][1])
                                    {
                                        face[v + 1] = elementList[1][elementList[2][f][e]][0];
                                        edgeUsed[e] = true;
                                        break;
                                    }
                                }
                            }
                        }

                        elementList[2][f] = face;
                    }
                }

                time.Stop();

                //Writes the path.
                using (StreamWriter sr = new StreamWriter(path))
                {
                    if (dim == 2)
                        elementNames[2] = "Components";
                    else
                        elementNames[2] = "Faces";

                    if (dim != 3)
                        sr.Write(dim);
                    sr.WriteLine("OFF");

                    //# Vertices, Faces, Edges, ...
                    sr.Write("# " + elementNames[0]);
                    sr.Write(", " + elementNames[2]);
                    if (dim >= 3)
                        sr.Write(", " + elementNames[1]);
                    for (int i = 3; i < dim; i++)
                        sr.Write(", " + ((i >= elementNames.Length) ? i + "-elements" : elementNames[i]));
                    sr.WriteLine();

                    //The actual corresponding numbers.
                    sr.Write(vertexList.Length);
                    sr.Write(" " + elementList[2].Count);
                    if (dim >= 3)
                        sr.Write(" " + elementList[1].Count);
                    for (int i = 3; i < dim; i++)
                        sr.Write(" " + elementList[i].Count);
                    sr.WriteLine();
                    sr.WriteLine();

                    sr.WriteLine("# " + elementNames[0]);
                    for (int i = 0; i < vertexList.Length; i++)
                    {
                        sr.Write(vertexListString[i][0]);
                        for (int j = 1; j < dim; j++)
                            sr.Write(" " + vertexListString[i][j]);
                        sr.WriteLine();
                    }
                    sr.WriteLine();

                    for (int d = 2; d < Math.Max(dim, 3); d++)
                    {
                        sr.WriteLine("# " + ((d >= elementNames.Length) ? d + "-elements" : elementNames[d]));
                        for (int i = 0; i < elementList[d].Count; i++)
                        {
                            int len = elementList[d][i].Length;
                            sr.Write(len);
                            for (int j = 0; j < len; j++)
                                sr.Write(" " + elementList[d][i][j]);
                            sr.WriteLine();
                        }
                        sr.WriteLine();
                    }
                }
                TimeSpan ts = time.Elapsed;
                string totalTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                TimeSpan tsc = commonVerticesTime.Elapsed;
                string commonTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", tsc.Hours, tsc.Minutes, tsc.Seconds, tsc.Milliseconds / 10);
                TimeSpan tsd = duplicateElementsTime.Elapsed;
                string duplicateTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", tsd.Hours, tsd.Minutes, tsd.Seconds, tsd.Milliseconds / 10);
                MessageBox.Show("Total time: " + totalTime + "\nCommon vertices time: " + commonTime + "\nDuplicate elements time: " + duplicateTime, "Success!");
            }
            catch (Exception e)
            {
                File.Delete(TMP_FILE_IN);
                File.Delete(TMP_FILE_OUT);
                MessageBox.Show(e.StackTrace, e.Message);
            }
        }

        /// <summary>
        /// Finds the dimension of the hyperplane through the vertices of v.
        /// </summary>
        /// <param name="vertexList"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private static int Rank(double[][] vertexList, List<int> v)
        {
            double[][] matrix = new double[v.Count - 1][];

            // Copies the other vertices to the matrix, shifted so that the first vertex is at the origin.
            for (int i = 0; i < v.Count - 1; i++)
            {
                double[] row = new double[vertexList[0].Length];
                for (int j = 0; j < vertexList[0].Length; j++)
                    row[j] = vertexList[v[i + 1]][j] - vertexList[v[0]][j];
                matrix[i] = row;
            }

            return Rank(matrix);
        }

        /// <summary>
        /// Returns the rank of the matrix via Gaussian elimination.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int Rank(double[][] matrix)
        {
            double pivotAbs, temp, div;
            int m = matrix.Length, n = matrix[0].Length, h = 0, k = 0, pivotIdx, rank = 0;
            for (; h < m && k < n; k++)
            {
                pivotAbs = Math.Abs(matrix[h][k]);
                pivotIdx = h;

                //Find pivot
                for (int i = h; i < m; i++)
                {
                    temp = Math.Abs(matrix[i][k]);
                    if (temp > pivotAbs)
                    {
                        pivotAbs = temp;
                        pivotIdx = i;
                    }
                }
                //If pivot = 0, move to next column.
                if (pivotAbs < 1e-6)
                    continue;
                else
                {
                    rank++;

                    if (h != pivotIdx)
                        //Swap rows
                        (matrix[h], matrix[pivotIdx]) = (matrix[pivotIdx], matrix[h]);
                    for (int i = h + 1; i < m; i++)
                    {
                        div = matrix[i][k] / matrix[h][k];
                        matrix[i][k] = 0;
                        for (int j = k + 1; j < n; j++)
                        {
                            matrix[i][j] -= matrix[h][j] * div;
                        }
                    }
                }
                h++;
            }
            return rank;
        }
    }
}