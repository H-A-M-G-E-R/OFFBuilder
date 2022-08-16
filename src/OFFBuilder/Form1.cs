using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using NCalc;
using System.Text.RegularExpressions;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;

namespace OFFBuilder
{
    public partial class frmMain : Form
    {
        #region Variables
        int signType = 0;
        int permType = 0;
        char sepChar = ' ';
        int outType = 0;
        public static int coordinates = 4;

        List<CustomEntry> lstSignCustom = new List<CustomEntry>();
        List<CustomEntry> lstPermCustom = new List<CustomEntry>();

        List<SignedStringArray> output = new List<SignedStringArray>();
        Dictionary<SignedStringArray, int> hashes = new Dictionary<SignedStringArray, int>(new SignedStringArray.EqualityComparer());

        const int ROW_HEIGHT = 87;
        #endregion

        #region Controls
        public frmMain()
        {
            InitializeComponent();
        }

        //Form load.
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

        //Inserts a new set of coordinates.
        private void btnInsert_Click(object sender, EventArgs e)
        {
            //https://stackoverflow.com/a/187394
            Match m = Regex.Match(txtCoords.Text, "((" + Regex.Escape(sepChar.ToString()) + ").*?){" + coordinates + "}");

            if (m.Success)
                txtCoords.Text = txtCoords.Text.Substring(0, m.Groups[2].Captures[coordinates - 1].Index);

            sbTxt = new StringBuilder(txtOutput.Text);

            //Places the coordinates into an array.
            string[] coords_ = txtCoords.Text.Split(new char[] { sepChar }, StringSplitOptions.RemoveEmptyEntries);
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
                //Custom
                case 4:
                    AddSigns(coords, lstSignCustom);
                    break;
            }

            txtOutput.Text = sbTxt.ToString();
        }

        //Clears the output.
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Text = "";
            output = new List<SignedStringArray>();
            hashes.Clear();
        }

        //Runs when either radPermCustom or radSignCustom are checked.
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

        //Changes the separation character.
        private void radio_SepCheckChanged(object sender, EventArgs e)
        {
            sepChar = (char)((RadioButton)sender).Tag;
        }

        //Changes the permutation type.
        private void radio_PermCheckChanged(object sender, EventArgs e)
        {
            permType = (int)((RadioButton)sender).Tag;
        }

        //Changes the sign type.
        private void radio_SignCheckChanged(object sender, EventArgs e)
        {
            signType = (int)((RadioButton)sender).Tag;
        }

        //Changes the output format.
        private void radio_OutCheckChanged(object sender, EventArgs e)
        {
            outType = (int)((RadioButton)sender).Tag;
            sbTxt = new StringBuilder();

            for (int i = 0; i < output.Count; i++)
                WriteCoords(output[i], false);

            txtOutput.Text = sbTxt.ToString();
        }

        //Hides caret of output.
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

        //Copies textbox to clipboard.
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
            MessageBox.Show("Text copied to clipboard!", "Success!");
        }

        //Changes the type of a custom permutation or sign change.
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

        //Changes the coordinates affected by a custom permutation or sign change.
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

        //Adds a custom permutation or sign change.
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
            TableLayoutPanel tlpCustomCell = new TableLayoutPanel();
            tlpCustomCell.ColumnCount = 2;
            tlpCustomCell.RowCount = 2;
            tlpCustomCell.Tag = tlpCustom.RowCount - 1;
            tlpCustomCell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 29));
            tlpCustomCell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpCustomCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
            tlpCustomCell.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tlpCustom.Controls.Add(tlpCustomCell, 0, tlpCustom.RowCount - 1);

            //Adds the button to remove rows.
            Button btnCustomRemove = new Button();
            btnCustomRemove.Text = "–";
            btnCustomRemove.Dock = DockStyle.Fill;
            btnCustomRemove.TextAlign = ContentAlignment.MiddleCenter;
            btnCustomRemove.Click += btnCustomRemove_Click;
            tlpCustomCell.Controls.Add(btnCustomRemove, 0, 0);

            //Adds the permutation types panel.
            Panel pnlCustomTypes = new Panel();
            pnlCustomTypes.Dock = DockStyle.Fill;
            pnlCustomTypes.AutoScroll = false;
            tlpCustomCell.Controls.Add(pnlCustomTypes, 1, 0);

            //Adds the permutation types radio buttons.            
            RadioButton radCustomAll = new RadioButton();
            RadioButton radCustomEven = new RadioButton();
            RadioButton radCustomOdd = new RadioButton();
            radCustomAll.Checked = true;
            radCustomAll.Text = "All";
            radCustomEven.Text = "Even";
            radCustomOdd.Text = "Odd";
            radCustomAll.Location = new Point(3, 3);
            radCustomEven.Location = new Point(45, 3);
            radCustomOdd.Location = new Point(101, 3);
            radCustomAll.Size = new Size(36, 17);
            radCustomEven.Size = new Size(50, 17);
            radCustomOdd.Size = new Size(45, 17);
            radCustomAll.Tag = ParityType.All;
            radCustomEven.Tag = ParityType.Even;
            radCustomOdd.Tag = ParityType.Odd;
            radCustomAll.CheckedChanged += radCustomType_CheckedChanged;
            radCustomEven.CheckedChanged += radCustomType_CheckedChanged;
            radCustomOdd.CheckedChanged += radCustomType_CheckedChanged;
            pnlCustomTypes.Controls.Add(radCustomAll);
            pnlCustomTypes.Controls.Add(radCustomEven);
            pnlCustomTypes.Controls.Add(radCustomOdd);

            //Adds the permutation index panel.
            Panel pnlCustomIndices = new Panel();
            pnlCustomIndices.Dock = DockStyle.Fill;
            pnlCustomIndices.AutoScroll = true;
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

        //Removes a custom permutation or sign change.
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

        //Clears custom permutations or sign changes.
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
            tlpCustom.Size = new Size(188, 0);
            lstCustom.Clear();

            tlpCustom.ResumeLayout(true);
        }

        //Verifies text written to the coordinates.
        Regex R = new Regex(" +");
        private void txtCoords_TextChanged(object sender, EventArgs e)
        {
            int s = txtCoords.SelectionStart;

            //Calculates where to move the caret.
            int b = 0; //Blocks of spaces.
            int c = 1; //Consecutive spaces. Stars from 1, so that leading spaces are removed.
            int d = 0; //How many spaces to the left the caret will be moved.
            for (int i = 0; i < txtCoords.Text.Length; i++)
            {
                if (txtCoords.Text[i] == ' ')
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

                    CheckBox chkCustomIndex = new CheckBox();
                    chkCustomIndex.Text = n;
                    chkCustomIndex.Location = new Point(x, 3);
                    chkCustomIndex.Size = new Size(26 + 6 * n.Length, 17); //Size depends on text length.
                    chkCustomIndex.Tag = i;
                    chkCustomIndex.Name = "chk" + i;
                    chkCustomIndex.CheckedChanged += chkCustomIndex_CheckedChanged;
                    pnl.Controls.Add(chkCustomIndex);

                    x += (32 + 6 * n.Length); //Pads each checkbox depending on text length.
                }
            }
        }

        //Exports the coordinates as an OFF file.
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (sfoExport.ShowDialog() == DialogResult.OK)
            {
                QConvex.InitProjectionMatrix(coordinates);
                QConvex.CreateOFFFile(sfoExport.FileName, output);
            }
        }
        #endregion

        #region Processing
        //Adds signs to coordinates.
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
                    //Custom
                    case 4:
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

        //Applies the appropriate permutations.
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
            SignedString t;

            //Finds the coordinates to permutate.
            List<int> y = new List<int>();
            for (int k = 0; k < coords.Length; k++)
                if (customEntries[indx].Indices[k])
                    y.Add(k);
            int[] z = y.ToArray();

            //Generates permutations from least to greatest in order.
            //Based on https://www.nayuki.io/page/next-lexicographical-permutation-algorithm

            //Bubblesort on coords[z[i]].
            for (j = 0; j < z.Length - 1; j++)
                for (i = 0; i < z.Length - 1; i++)
                    if (coords[z[i]] > coords[z[i + 1]])
                    {
                        t = coords[z[i + 1]];
                        coords[z[i + 1]] = coords[z[i]];
                        coords[z[i]] = t;
                        parity = !parity;
                    }

            //The loop will be exited whenever all permutations are traversed.
            while (true)
            {
                if (customEntries[indx].Type == ParityType.All || parity)
                    WriteCoords(coords);

                //Finds first swap position.
                i = z.Length - 1;
                while (i > 0 && coords[z[i - 1]] >= coords[z[i]])
                    i--;

                //Are we at the last permutation already?
                if (i <= 0)
                    return;

                //Finds second swap position.
                j = z.Length - 1;
                while (coords[z[j]] <= coords[z[i - 1]])
                    j--;

                //Performs the swap.
                t = coords[z[i - 1]];
                coords[z[i - 1]] = coords[z[j]];
                coords[z[j]] = t;
                parity = !parity;

                //Reverses the suffix.
                j = z.Length - 1;
                while (i < j)
                {
                    t = coords[z[i]];
                    coords[z[i]] = coords[z[j]];
                    coords[z[j]] = t;
                    i++;
                    j--;
                    parity = !parity;
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

            //Clears output, textbox.
            btnClear_Click(null, null);
            txtCoords.Text = "";

            //Adds the appropriate amounts of checkboxes.
            coordinates = (int)nudDimensions.Value;
            for (i = 0; i < tlpPermCustom.RowCount; i++)
                AddCheckBoxes((Panel)(((TableLayoutPanel)tlpPermCustom.GetControlFromPosition(0, i)).GetControlFromPosition(1, 1)));
            for (i = 0; i < tlpSignCustom.RowCount; i++)
                AddCheckBoxes((Panel)(((TableLayoutPanel)tlpSignCustom.GetControlFromPosition(0, i)).GetControlFromPosition(1, 1)));

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
                        PME.Append("Sqrt(" + i * (i + 1) / 2 + ")");

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

    //A simple class to be able to easily deal with negating expressions.
    public class SignedString
    {
        string str;
        bool sign;
        const double PHI = 1.618033988749895;

        public double Value
        {
            get;
            set;
        }

        public SignedString(string s)
        {
            str = s.Trim(new char[] { ',', ' ' });
            sign = true;

            try
            {
                Expression e = new Expression(s);
                e.Parameters["Pi"] = Math.PI;
                e.Parameters["Phi"] = PHI;
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
            if (str.IndexOf('+') == -1 && str.IndexOf('-') == -1)
                return "-" + str;
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

    //A wrapper for a SignedString[] implementing a hash function.
    public class SignedStringArray
    {
        SignedString[] Value
        {
            get;
            set;
        }

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

    //Used to state the type of either permutations or sign changes.
    public enum ParityType
    {
        None,
        All,
        Even,
        Odd
    }

    //A class to store either a single custom sign change or a single custom permutation.
    public class CustomEntry
    {
        public ParityType Type
        {
            get;
            set;
        }

        public bool[] Indices
        {
            get;
            set;
        }

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

    //A doubly-linked list node implementation.
    public class DLLNode
    {
        DLLNode lnk1 = null, lnk2 = null;

        public int Value
        {
            get;
            set;
        }

        public void LinkTo(DLLNode a)
        {
            if (this.lnk1 == null)
                this.lnk1 = a;
            this.lnk2 = a;
        }

        public void CrossLink(DLLNode a)
        {
            this.LinkTo(a);
            a.LinkTo(this);
        }

        public DLLNode(int v)
        {
            Value = v;
        }

        public int[] GetCycle()
        {
            DLLNode prevNode = this, node = this.lnk1, tempNode;
            List<int> res = new List<int>();
            res.Add(this.Value);

            while (node != this)
            {
                res.Add(node.Value);
                tempNode = node;

                if (node.lnk1 == prevNode)
                    node = node.lnk2;
                else
                    node = node.lnk1;

                prevNode = tempNode;
            }

            return res.ToArray();
        }
    }

    //A class to save OFF files.
    public static class QConvex
    {
        static string QCONVEX = Application.StartupPath + "\\qconvex.exe";
        static string TMP_FILE_IN = Application.StartupPath + "\\tmp.txt";
        static string TMP_FILE_OUT = Application.StartupPath + "\\tmp.off";
        private static string[] elementNames = new string[] { "Vertices", "Edges", "Faces", "Cells", "Tera", "Peta", "Exa", "Zetta", "Yotta" };

        static double[] M;
        static int d;

        //Has a miniscule probability of making the code fail.
        //This probably won't happen ever, though.
        public static void InitProjectionMatrix(int dim)
        {
            d = dim;
            M = new double[2 * dim];
            Random r = new Random();

            for (int i = 0; i < 2 * dim; i++)
                M[i] = r.NextDouble();
        }

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
                            sw.Write(coords[i][j].Value);

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
                        Arguments = "C0.00001 TI \"" + TMP_FILE_IN + "\" o TO \"" + TMP_FILE_OUT + "\"",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };

                process.Start();
                process.WaitForExit();

                File.Delete(TMP_FILE_IN);

                //Reads QConvex output, converts it to the Stella OFF format.
                List<int[]>[] elementList;
                double[][] vertexList;
                int dim;

                using (StreamReader sr = new StreamReader(TMP_FILE_OUT))
                {
                    string line = sr.ReadLine();
                    /* d-dimensional elements are stored in elementList[d].
                     * This means that the first two entries will be empty, but whatever.
                     * Also, polygons are stored as polyhedra with a single face. */
                    dim = Convert.ToInt32(line);
                    elementList = new List<int[]>[Math.Max(dim, 3)];
                    line = sr.ReadLine();
                    vertexList = new double[Convert.ToInt32(line.Substring(0, line.IndexOf(' ')))][];

                    //Loads vertexList with the vertex coordinates.
                    for (int i = 0; i < vertexList.Length; i++)
                    {
                        string[] vertex = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        vertexList[i] = new double[vertex.Length];

                        for (int j = 0; j < vertex.Length; j++)
                            vertexList[i][j] = Convert.ToDouble(vertex[j]);
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

                /* If the polytope is 2D, its convex hull has already been calculated by Qhull.
                 * newElements[2] is set to the vertices in order. */
                if (dim == 2)
                {
                    DLLNode[] nodes = new DLLNode[elementList[1].Count];
                    for (int i = 0; i < nodes.Length; i++)
                        nodes[i] = new DLLNode(i);
                    for (int i = 0; i < nodes.Length; i++)
                        nodes[elementList[1][i][0]].CrossLink(nodes[elementList[1][i][1]]);
                    elementList[2] = new List<int[]> { nodes[0].GetCycle() };
                }
                else
                {
                    /* Generates (d-1)-dimensional faces out of d-dimensional faces.
                     * It also generates edges, just to have an accurate count. */
                    for (int d = dim - 1; d >= 2; d--)
                    {
                        /* At the same time I find (d-1)-faces, I need to rewrite d-faces in terms of them.
                         * That is, except in the case d=2. */
                        elementList[d - 1] = new List<int[]>();
                        List<int>[] newElements = new List<int>[elementList[d].Count];
                        for (int i = 0; i < newElements.Length; i++)
                            newElements[i] = new List<int>();

                        /* If two d-dimensional elements have more than d common vertices, they form a (d-1)-face...
                         * ...as long as d ≤ 3. For d ≥ 4, there's the possibility they actually just share a 2-face.
                         * Furthermore, a (d-1)-face won't be shared by more than two d-dimensional elements. */
                        for (int i = 0; i < elementList[d].Count; i++)
                            for (int j = i + 1; j < elementList[d].Count; j++)
                            {
                                List<int> commonElements = new List<int>();
                                int m = 0, n = 0;
                                while (m < elementList[d][i].Length && n < elementList[d][j].Length)
                                {
                                    if (elementList[d][i][m] < elementList[d][j][n])
                                        m++;
                                    else if (elementList[d][i][m] > elementList[d][j][n])
                                        n++;
                                    else
                                    {
                                        commonElements.Add(elementList[d][i][m]);
                                        m++;
                                    }
                                }

                                //We need to discard the possibility that these elements lie in a (d – 2)-hyperplane.
                                if (commonElements.Count >= d && (d < 4 || Rank(vertexList, commonElements) > d - 2))
                                {
                                    commonElements.Sort();

                                    /* Checks if the face has not been added before.
                                     * The face index, old or new, is added to the corresponding newElements. */
                                    bool next;
                                    int duplicate = -1;
                                    for (int k = 0; duplicate == -1 && k < elementList[d - 1].Count; k++)
                                    {
                                        next = false;
                                        if (commonElements.Count == elementList[d - 1][k].Length)
                                        {
                                            for (int l = 0; !next && l < commonElements.Count; l++)
                                                if (commonElements[l] != elementList[d - 1][k][l])
                                                    next = true;

                                            if (!next)
                                                duplicate = k;
                                        }
                                    }

                                    if (duplicate == -1)
                                    {
                                        newElements[i].Add(elementList[d - 1].Count);
                                        newElements[j].Add(elementList[d - 1].Count);
                                        elementList[d - 1].Add(commonElements.ToArray());
                                    }
                                    else
                                    {
                                        if (!newElements[i].Contains(duplicate))
                                            newElements[i].Add(duplicate);
                                        if (!newElements[j].Contains(duplicate))
                                            newElements[j].Add(duplicate);
                                    }
                                }
                            }

                        if (d > 2)
                            for (int i = 0; i < elementList[d].Count; i++)
                                elementList[d][i] = newElements[i].ToArray();
                    }

                    /* Faces have to be in order: their convex hull is thus calculated.
                     * This is not the most efficient code to do so, but it's definitely the easiest. */
                    for (var f = 0; f < elementList[2].Count; f++)
                    {
                        //The soon-to-be list of vertices in cyclic order.
                        int[] v = new int[elementList[2][f].Length];

                        v[0] = elementList[2][f][0];

                        //Repeatedly connects the points that create the "leftmost" edges.
                        for (int i = 1; i < v.Length; i++)
                        {
                            int guess = v[0];
                            for (int j = 0; j < v.Length; j++)
                                if (guess == v[i - 1] || Left(v[i - 1], guess, elementList[2][f][j], vertexList))
                                    guess = elementList[2][f][j];
                            v[i] = guess;
                        }

                        elementList[2][f] = v;
                    }
                }

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
                        sr.Write(", " + elementNames[i]);
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
                        sr.Write(vertexList[i][0]);
                        for (int j = 1; j < dim; j++)
                            sr.Write(" " + vertexList[i][j]);
                        sr.WriteLine();
                    }
                    sr.WriteLine();

                    for (int d = 2; d < Math.Max(dim, 3); d++)
                    {
                        sr.WriteLine("# " + elementNames[d]);
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

                MessageBox.Show("File succesfully created.", "Success!");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!");
            }
        }

        //Returns if vector ab is to the "left" of vector ac, in a consistent manner, via the cross product after projecting to 2D.
        private static bool Left(int a, int b, int c, double[][] vertexList)
        {
            int dim = vertexList[0].Length;
            double[] w1 = new double[2], w2 = new double[2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    w1[i] += (vertexList[b][j] - vertexList[a][j]) * M[i * dim + j];
                    w2[i] += (vertexList[c][j] - vertexList[a][j]) * M[i * dim + j];
                }
            }

            return w1[0] * w2[1] > w1[1] * w2[0];
        }

        //Finds the dimension of the hyperplane through the vertices of v.
        private static int Rank(double[][] vertexList, List<int> v)
        {
            return Matrix<double>.Build.Dense(v.Count - 1, vertexList[0].Length, ((int i, int j) => vertexList[v[i + 1]][j] - vertexList[v[0]][j])).Rank();
        }
    }
}