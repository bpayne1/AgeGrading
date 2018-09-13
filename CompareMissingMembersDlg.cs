using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AgeGrading
{
    public partial class CompareMissingMembersDlg : Form
    {
        public CompareMissingMembersDlg()
        {
            InitializeComponent();
            btnReduceByAge.Enabled = false;
            btnHighlightNextMatch.Enabled = btnReduceByAge.Enabled;
            dataGridViewHighlight.Visible = false;
        }

        private string mMissingMembersFile;
        public string MissingMembersFile
        {
            get { return mMissingMembersFile; }
            set { mMissingMembersFile = value; }
        }

        private DataTable mAgeGradedResults;
        public DataTable AgeGradedResults
        {
            get { return mAgeGradedResults; }
            set { mAgeGradedResults = value; }
        }

        private string mAgeColumnName;
        public string AgeColumnName
        {
            get
            {
                return mAgeColumnName;
            }
            set { mAgeColumnName = value; }
        }

        private string mNameColumnName;
        public string NameColumnName
        {
            get { return mNameColumnName; }
            set { mNameColumnName = value; }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            AgeGradingForm.SavePositionInformation(this.Name, this.Location, this.Size, this.WindowState);
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                Point location = this.Location;
                Size size = this.Size;
                FormWindowState state = this.WindowState;
                AgeGradingForm.GetPositionInformation(this.Name, ref location, ref size, ref state);
                this.Location = location;
                this.Size = size;
                if (state != FormWindowState.Minimized) this.WindowState = state;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            base.OnLoad(e);
            if (String.IsNullOrEmpty(MissingMembersFile) || !File.Exists(MissingMembersFile))
            {
                MessageBox.Show(this, "Invalid or non-existent Missing Member Names File", "Error");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return;
            }
            if (String.IsNullOrEmpty(AgeColumnName))
            {
                MessageBox.Show(this, "Invalid or non-existent Age Column text", "Error");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return;
            }
            if (String.IsNullOrEmpty(NameColumnName))
            {
                MessageBox.Show(this, "Invalid or non-existent Name Column text", "Error");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return;
            }
            if (AgeGradedResults == null)
            {
                MessageBox.Show(this, "Invalid or non-existent Age Graded Results Table", "Error");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return;
            }
            labelMissingMembers.Left = splitContainer.Left + splitContainer.SplitterDistance;
            dataGridView.DataSource = this.AgeGradedResults;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (0 == String.Compare(column.Name, NameColumnName, true)
                    || 0 == String.Compare(column.Name, AgeColumnName, true)) continue;
                column.Visible = false;
            }
            RestartAgeGroup = true;
            FillMissingMemberTable();
        }

        private void FillMissingMemberTable()
        {
            string fileName = Path.GetFileNameWithoutExtension(MissingMembersFile);
            labelMissingMembers.Text = String.Format(labelMissingMembers.Text, fileName);
            string[] textLines = System.IO.File.ReadAllLines(MissingMembersFile);
            if (textLines == null || textLines.Length <= 0) return;
            AgeGradeTextReader reader = new AgeGradeTextReader();
            List<List<string>> lines = lines = reader.GetDelimitedLines(textLines, 1, 1, "\t");
            if (lines == null || lines.Count <= 0) return;
            DataTable table = new DataTable();
            AgeGradingForm.AddDataColumn(table, NameColumnName, typeof(string));
            AgeGradingForm.AddDataColumn(table, AgeColumnName, typeof(int));
            foreach (List<string> line in lines)
            {
                if (line == null || line.Count < 2) continue;
                string name = line[0];
                if (!Valid(ref name)) continue;
                if (name.StartsWith("SID", StringComparison.OrdinalIgnoreCase))
                {

                }
                name = CapitalizeName(name);
                string ageString = line[1];
                if (!Valid(ref ageString)) continue;
                int age = 0;
                if (!int.TryParse(ageString, out age)) continue;
                DataRow dataRow = table.NewRow();
                dataRow[0] = name;
                dataRow[1] = age;
                table.Rows.Add(dataRow);
            }
            dataGridViewMissingMembers.DataSource = table;
        }

        private string CapitalizeName(string name)
        {
            if (String.IsNullOrEmpty(name)) return name;
            string[] split = name.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length <= 0) return name;
            StringBuilder str = new StringBuilder();
            foreach (string item in split)
            {
                if (String.IsNullOrEmpty(item)) continue;
                if (str.Length > 0) str.Append(" ");
                if (item.Length < 1)
                {
                    str.Append(item);
                    break;
                }
                string temp = char.ToUpper(item[0]) + item.Substring(1).ToLower();
                str.Append(temp);
            }
            if (str.Length <= 0) return name;
            return str.ToString();
        }

        private bool Valid(ref string str)
        {
            if (String.IsNullOrEmpty(str)) return false;
            str = str.Trim();
            if (String.IsNullOrEmpty(str)) return false;
            return true;
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            labelMissingMembers.Left = splitContainer.Left + splitContainer.SplitterDistance;
        }

        private int mLastAgeGroup;
        private int mLastStartingRow;
        private int mLastStartingMissingMemberRow;
        private void btnNextAgeGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewHighlight.Visible) dataGridViewHighlight.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                NextGroup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void Sort(DataGridView dataGridView, string name)
        {
            if (dataGridView == null || dataGridView.Rows.Count <= 0 || dataGridView.Columns.Count <= 0) return;
            if (String.IsNullOrEmpty(name)) return;
            DataGridViewColumn column = dataGridView.Columns[name];
            if (column == null) return;
            dataGridView.Sort(column, ListSortDirection.Ascending);
        }

        private void NextGroup()
        {
            btnReduceByAge.Enabled = false;
            btnHighlightNextMatch.Enabled = btnReduceByAge.Enabled;
            if (dataGridView.Rows.Count <= 0) return;
            // Sort the list by age.
            string ageColumn = AgeColumnName;
            if (String.IsNullOrEmpty(ageColumn)) return;
            if (RestartAgeGroup)
            {
                DataGridViewColumn column = dataGridView.Columns[ageColumn];
                if (column == null) return;
                Sort(dataGridView, ageColumn);
                Sort(dataGridViewMissingMembers, ageColumn);
                mLastAgeGroup = Convert.ToInt32(numAgeGroup.Value);
                mLastStartingRow = 0;
                mLastStartingMissingMemberRow = 0;
                RestartAgeGroup = false;
                ResetVisibility(dataGridView);
                ResetVisibility(dataGridViewMissingMembers);
                ResetBackColor(dataGridView);
            }

            ResetBackColor(dataGridViewMissingMembers);
            int highestAge = int.MinValue;
            while (true)
            {
                if (mLastStartingRow >= dataGridView.Rows.Count) break;
                if (mLastStartingMissingMemberRow >= dataGridViewMissingMembers.Rows.Count) break;
                while (true)
                {
                    dataGridView.ClearSelection();
                    dataGridViewMissingMembers.ClearSelection();
                    int missingHighestAge = FindAge(dataGridViewMissingMembers, ref mLastStartingMissingMemberRow, highestAge);
                    highestAge = missingHighestAge;
                    if (mLastAgeGroup > highestAge) break;
                    int currentAge = mLastAgeGroup;
                    mLastAgeGroup++;
                    if (dataGridViewMissingMembers.SelectedCells.Count <= 0) continue;
                    bool foundAge = FindAge(dataGridView, ref mLastStartingRow, currentAge, AgeColumnName);
                    if (!foundAge) continue;
                    break;
                }
                if (mLastStartingRow >= dataGridView.Rows.Count) break;
                if (mLastStartingMissingMemberRow >= dataGridViewMissingMembers.Rows.Count) break;
                if (mLastAgeGroup > highestAge) break;
                if (dataGridView.SelectedCells.Count > 0) break;
                if (dataGridViewMissingMembers.SelectedCells.Count > 0) break;
            }

            foreach (DataGridViewCell cell1 in dataGridViewMissingMembers.SelectedCells)
            {
                if (cell1 == null) continue;
                int rowIndex1 = cell1.RowIndex;
                if (rowIndex1 <= 0) continue;
                DataGridViewCell cell = dataGridViewMissingMembers.Rows[rowIndex1].Cells[NameColumnName];
                if (cell == null) continue;
                string name1 = (cell.Value != null ? cell.Value.ToString() : null);
                if (String.IsNullOrEmpty(name1)) continue;
                foreach (DataGridViewCell cell2 in dataGridView.SelectedCells)
                {
                    if (cell2 == null) continue;
                    int rowIndex2 = cell2.RowIndex;
                    if (rowIndex2 <= 0) continue;
                    DataGridViewCell cell3 = dataGridView.Rows[rowIndex2].Cells[NameColumnName];
                    if (cell3 == null) continue;
                    string name2 = (cell3.Value != null ? cell3.Value.ToString() : null);
                    if (String.IsNullOrEmpty(name2)) continue;
                    if (ContainsName(name2, name1) || ContainsName(name1, name2))
                    {
                        cell.Style.BackColor = Color.Orange;
                        cell3.Style.BackColor = Color.Green;
                    }
                }
            }
        }

        private static char[] mSeparator;
        private bool ContainsName(string name1, string name2)
        {
            if (String.IsNullOrEmpty(name1)) return false;
            if (String.IsNullOrEmpty(name2)) return false;
            int matchValue = 0;
            if (mSeparator == null) mSeparator = new char[] { ' ' };
            char[] separator = mSeparator;
            name1 = name1.ToLower();
            string[] split = name1.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length <= 0) return false;
            name2 = name2.ToLower();
            foreach (string item in split)
            {
                if (name2.Contains(item))
                {
                    matchValue = LevenshteinDistance(name1, name2);
                    const string kNameToMatch = "seiferth";
                    if (name1.Contains(kNameToMatch) && name2.Contains(kNameToMatch))
                    {
                    }
                    if (matchValue > 6) return false;
                    if (!SeparatedNamesAreCloseMatch(name1, name2)) return false;
                    return true;
                }
            }
            return false;
        }

        private bool SeparatedNamesAreCloseMatch(string name1, string name2)
        {
            if (String.Equals(name1, name2)) return true;
            if (String.IsNullOrWhiteSpace(name1)) return true;
            if (String.IsNullOrWhiteSpace(name2)) return true;
            if (mSeparator == null) mSeparator = new char[] { ' ' };
            string[] split1 = name1.Split(mSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (split1 == null) return true;
            string[] split2 = name2.Split(mSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (split2 == null) return true;
            if (split1.Length != split2.Length) return true;
            string firstName1 = split1[0];
            string firstName2 = split2[0];
            string lastName1 = split1[split1.Length - 1];
            string lastName2 = split2[split2.Length - 1];
            if (0 == String.Compare(firstName1, firstName2, true))
            {
                if (lastName1.StartsWith(lastName2[0].ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            if (0 == String.Compare(lastName1, lastName2, true))
            {
                if (firstName1.StartsWith(firstName2[0].ToString(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            int lastIndex1 = split1.Length - 1;
            int lastIndex2 = split2.Length - 1;
            for (int ii = 0; ii < split1.Length; ii++)
            {
                string separtedName1 = split1[ii];
                string separtedName2 = split2[ii];
                int distance = LevenshteinDistance(separtedName1, separtedName2);
                int allowedDistance = Math.Min(separtedName1.Length, separtedName2.Length);
                if (distance >= allowedDistance) return false;
            }
            return true;
        }

        private void ResetBackColor(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            if (dataGridView.Rows.Count <= 0) return;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row == null) continue;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = Color.White;
                }
            }
        }

        private void ResetVisibility(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            if (dataGridView.Rows.Count <= 0) return;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row == null) continue;
                row.Visible = true;
            }
        }

        private int FindAge(DataGridView dataGridView, ref int lastStartingRow, int highestAge)
        {
            for (int ii = lastStartingRow; ii < dataGridView.Rows.Count; ii++)
            {
                DataGridViewRow row = dataGridView.Rows[ii];
                if (row == null) continue;
                DataGridViewCell cell = row.Cells[AgeColumnName];
                if (cell == null) continue;
                int age = (int)cell.Value;
                if (age > highestAge) highestAge = age;
                if (age == mLastAgeGroup)
                {
                    if (dataGridView.SelectedCells.Count <= 0) dataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                    cell.Selected = true;
                    lastStartingRow = ii;
                }
            }
            return highestAge;
        }

        private static bool FindAge(DataGridView dataGridView, ref int lastStartingRow, int currentAge, string ageColumnName)
        {
            bool foundAge = false;
            for (int ii = lastStartingRow; ii < dataGridView.Rows.Count; ii++)
            {
                DataGridViewRow row = dataGridView.Rows[ii];
                if (row == null) continue;
                DataGridViewCell cell = row.Cells[ageColumnName];
                if (cell == null) continue;
                int age = 0;
                if (!int.TryParse(cell.Value.ToString(), out age)) age = currentAge + 1;
                if (age > currentAge) break;
                if (age == currentAge)
                {
                    if (dataGridView.SelectedCells.Count <= 0) dataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                    cell.Selected = true;
                    lastStartingRow = ii;
                    foundAge = true;
                }
            }
            return foundAge;
        }

        private bool mRestartAgeGroup;
        public bool RestartAgeGroup
        {
            get { return mRestartAgeGroup; }
            set { mRestartAgeGroup = value; }
        }

        private void numAgeGroup_ValueChanged(object sender, EventArgs e)
        {
            RestartAgeGroup = true;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewHighlight.Visible) dataGridViewHighlight.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                RestartAgeGroup = true;
                NextGroup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private bool BelowAgeLimit(object age)
        {
            if (age == null) return true;
            if (age is int)
            {
                if ((int)age < 14) return true;
                return false;
            }
            string str = age as string;
            if (String.IsNullOrEmpty(str)) return true;
            try
            {
                int temp = 0;
                if (!int.TryParse(str, out temp)) return true;
                if (temp < 14) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        private void HighlightMatchingNames()
        {
            mStartingHighlightIndex = -1;
            if (dataGridView.Rows.Count <= 0) return;
            // Sort the list by age.
            string ageColumn = NameColumnName;
            if (String.IsNullOrEmpty(ageColumn)) return;
            DataGridViewColumn column = dataGridView.Columns[ageColumn];
            if (column == null) return;
            RestartAgeGroup = true;
            Sort(dataGridView, ageColumn);
            Sort(dataGridViewMissingMembers, ageColumn);
            ClearAllHighlights(false);

            dataGridView.CurrentCell = null;
            dataGridViewMissingMembers.CurrentCell = null;

            HashSet<int> dataGridViewMatchingRows = new HashSet<int>();
            HashSet<int> dataGridViewMissingMembersMatchingRows = new HashSet<int>();
            for (int rowIndex1 = 0; rowIndex1 < dataGridViewMissingMembers.RowCount; rowIndex1++)
            {
                DataGridViewCell ageCell = dataGridViewMissingMembers.Rows[rowIndex1].Cells[AgeColumnName];
                if (ageCell == null || ageCell.Value == null) continue;
                if (BelowAgeLimit(ageCell.Value))
                {
                    dataGridViewMissingMembers.Rows[rowIndex1].Visible = false;
                    continue;
                }
                DataGridViewCell cell1 = dataGridViewMissingMembers.Rows[rowIndex1].Cells[NameColumnName];
                if (cell1 == null) continue;
                string name1 = (cell1.Value != null ? cell1.Value.ToString() : null);
                if (String.IsNullOrEmpty(name1)) continue;
                for (int rowIndex2 = 0; rowIndex2 < dataGridView.RowCount; rowIndex2++)
                {
                    ageCell = dataGridView.Rows[rowIndex2].Cells[AgeColumnName];
                    if (ageCell == null || ageCell.Value == null) continue;
                    if (BelowAgeLimit(ageCell.Value))
                    {
                        dataGridView.Rows[rowIndex2].Visible = false;
                        continue;
                    }
                    DataGridViewCell cell2 = dataGridView.Rows[rowIndex2].Cells[NameColumnName];
                    if (cell2 == null) continue;
                    string name2 = (cell2.Value != null ? cell2.Value.ToString() : null);
                    if (String.IsNullOrEmpty(name2)) continue;
                    if (name1.Contains("Dausman") && name2.Contains("Dausman"))
                    {
                    }
                    if (ContainsName(name1, name2, chkLastNamesOnly.Checked))
                    {
                        cell1.Style.BackColor = Color.Orange;
                        cell2.Style.BackColor = Color.Green;
                        dataGridViewMatchingRows.Add(rowIndex2);
                        dataGridViewMissingMembersMatchingRows.Add(rowIndex1);
                    }
                }
            }
            SetRowVisibility(dataGridView, dataGridViewMatchingRows);
            SetRowVisibility(dataGridViewMissingMembers, dataGridViewMissingMembersMatchingRows);
        }

        private void ClearAllHighlights(bool ignoreMissingMembers)
        {
            ResetVisibility(dataGridView);
            if (!ignoreMissingMembers) ResetVisibility(dataGridViewMissingMembers);
            ResetBackColor(dataGridView);
            ResetBackColor(dataGridViewMissingMembers);
            dataGridView.ClearSelection();
            dataGridViewMissingMembers.ClearSelection();
        }

        private void SetRowVisibility(DataGridView dataGridView, HashSet<int> dataGridViewMatchingRows)
        {
            if (dataGridView == null || dataGridView.RowCount <= 0) return;
            if (dataGridViewMatchingRows == null) return;
            CurrencyManager cm = (CurrencyManager)BindingContext[dataGridView.DataSource];
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                try
                {
                    if (!dataGridViewMatchingRows.Contains(row.Index))
                    {
                        try
                        {
                            cm.SuspendBinding();
                            row.Visible = false;
                        }
                        finally
                        {
                            cm.ResumeBinding();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private bool ContainsName(string name1, string name2, bool lastNameOnly)
        {
            if (String.IsNullOrEmpty(name1)) return false;
            if (String.IsNullOrEmpty(name2)) return false;
            if (!lastNameOnly) return String.Equals(name1, name2, StringComparison.OrdinalIgnoreCase);
            string[] split1 = name1.ToLower().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split1 == null || split1.Length <= 0) return false;
            string[] split2 = name2.ToLower().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split2 == null || split2.Length <= 0) return false;
            if (split1.Length == split2.Length)
            {
                int lastIndex = split1.Length - 1;
                return String.Equals(split1[lastIndex], split2[lastIndex], StringComparison.OrdinalIgnoreCase);
            }
            string[] outer = (split1.Length > split2.Length ? split1 : split2);
            string[] inner = (split1.Length < split2.Length ? split1 : split2);
            for (int ii = outer.Length - 1; ii >= 1; ii--)
            {
                for (int jj = inner.Length - 1; jj >= 1; jj--)
                {
                    if (outer[ii].Contains(inner[jj])) return true;
                }
            }
            return false;
        }


        private void btnHighlightMatchingNames_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewHighlight.Visible) dataGridViewHighlight.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                btnReduceByAge.Enabled = true;
                btnHighlightNextMatch.Enabled = btnReduceByAge.Enabled;
                HighlightMatchingNames();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void dataGridViewMissingMembers_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridViewMissingMembers.Rows[e.RowIndex].Cells[NameColumnName];
            ClearAllHighlights(true);
            HashSet<int> dataGridViewMatchingRows = new HashSet<int>();
            FindMatchingAgeRows(e.RowIndex, cell, ref dataGridViewMatchingRows);
            UpdateHighlightGrid(dataGridViewMatchingRows);
            //SetRowVisibility(dataGridView, dataGridViewMatchingRows);
        }

        private bool FindMatchingAgeRows(int cellRowIndex, DataGridViewCell cell, ref HashSet<int> dataGridViewMatchingRows)
        {
            if (dataGridViewMatchingRows == null) dataGridViewMatchingRows = new HashSet<int>();
            if (cell == null) return false;
            string name1 = (cell.Value != null ? cell.Value.ToString() : null);
            if (name1 == null) return false;
            if (cellRowIndex < 0 || cellRowIndex >= dataGridViewMissingMembers.RowCount) return false;
            bool foundMatchingCell = false;
            int minimumAge = KQCompetitionData.kMinimumEligibilityAge - 1;
            for (int rowIndex = 0; rowIndex < dataGridView.RowCount; rowIndex++)
            {
                DataGridViewRow row = dataGridView.Rows[rowIndex];
                DataGridViewCell cell2 = row.Cells[NameColumnName];
                if (cell2 == null) continue;
                string name2 = (cell2.Value != null ? cell2.Value.ToString() : null);
                if (String.IsNullOrEmpty(name2)) continue;
                if (ContainsName(name1, name2))
                {
                    DataGridViewCell cellAge1 = dataGridViewMissingMembers.Rows[cellRowIndex].Cells[AgeColumnName];
                    DataGridViewCell cellAge2 = row.Cells[AgeColumnName];
                    if (cellAge1 != null && cellAge1.Value != null && cellAge2 != null && cellAge2.Value != null)
                    {
                        int age1 = (int)cellAge1.Value;
                        if (age1 < minimumAge) continue;
                        int age2 = (int)cellAge2.Value;
                        if (age2 < minimumAge) continue;
                        const int kAgeDeadband = 1;
                        if (Math.Abs(age1 - age2) > kAgeDeadband) continue;
                    }
                    foundMatchingCell = true;
                    cell.Style.BackColor = Color.Orange;
                    cell2.Style.BackColor = Color.Green;
                    dataGridViewMatchingRows.Add(rowIndex);
                }
            }
            return foundMatchingCell;
        }

        private void btnReduceByAge_Click(object sender, EventArgs e)
        {
            int count = 0;
            try
            {
                if (dataGridViewHighlight.Visible) dataGridViewHighlight.Visible = false;
                this.Cursor = Cursors.WaitCursor;
                ClearAllHighlights(false);
                HashSet<int> dataGridViewMatchingRows = new HashSet<int>();
                HashSet<int> dataGridViewMissingMembersMatchingRows = new HashSet<int>();
                for (int rowIndex = 0; rowIndex < dataGridViewMissingMembers.RowCount; rowIndex++)
                {
                    DataGridViewCell cell = dataGridViewMissingMembers.Rows[rowIndex].Cells[NameColumnName];
                    if (cell == null) continue;
                    if (FindMatchingAgeRows(rowIndex, cell, ref dataGridViewMatchingRows))
                    {
                        dataGridViewMissingMembersMatchingRows.Add(rowIndex);
                        count++;
                    }
                }
                SetRowVisibility(dataGridView, dataGridViewMatchingRows);
                SetRowVisibility(dataGridViewMissingMembers, dataGridViewMissingMembersMatchingRows);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Cursor = Cursors.Default;
                Debug.WriteLine(String.Format("reduce by age count = {0}", count));
            }
        }

        private int FindFirstVisibleRow(int startingIndex, DataGridView dataGridView)
        {
            if (dataGridView == null) return -1;
            if (startingIndex < 0) startingIndex = 0;
            for (int ii = startingIndex; ii < dataGridView.RowCount; ii++)
            {
                DataGridViewRow row = dataGridView.Rows[ii];
                if (row == null || !row.Visible) continue;
                return ii;
            }
            return -1;
        }

        private int mStartingHighlightIndex = -1;
        private void btnHighlightNextMatch_Click(object sender, EventArgs e)
        {
            CurrencyManager cm = null;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                cm = (CurrencyManager)BindingContext[this.dataGridView.DataSource];
                if (cm != null) cm.SuspendBinding();
                mStartingHighlightIndex++;
                if (mStartingHighlightIndex < 0 || mStartingHighlightIndex >= dataGridViewMissingMembers.RowCount) return;
                mStartingHighlightIndex = FindFirstVisibleRow(mStartingHighlightIndex, dataGridViewMissingMembers);
                if (mStartingHighlightIndex < 0 || mStartingHighlightIndex >= dataGridViewMissingMembers.RowCount) return;
                DataGridViewCell cell = dataGridViewMissingMembers.Rows[mStartingHighlightIndex].Cells[NameColumnName];
                if (cell == null) return;
                dataGridViewMissingMembers.FirstDisplayedScrollingRowIndex = cell.RowIndex;

                ClearAllHighlights(true);
                cell.Style.BackColor = Color.Orange;
                HashSet<int> dataGridViewMatchingRows = new HashSet<int>();
                FindMatchingAgeRows(mStartingHighlightIndex, cell, ref dataGridViewMatchingRows);
                //SetRowVisibility(dataGridView, dataGridViewMatchingRows);
                UpdateHighlightGrid(dataGridViewMatchingRows);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void UpdateHighlightGrid(HashSet<int> dataGridViewMatchingRows)
        {
            if (dataGridView == null) return;
            if (dataGridViewHighlight == null) return;
            if (dataGridViewMatchingRows == null || dataGridViewMatchingRows.Count <= 0) return;
            if (!dataGridView.Parent.Controls.Contains(dataGridViewHighlight))
                dataGridView.Parent.Controls.Add(dataGridViewHighlight);
            dataGridViewHighlight.AutoSizeRowsMode = dataGridView.AutoSizeRowsMode;
            dataGridViewHighlight.AutoSizeColumnsMode = dataGridView.AutoSizeColumnsMode;
            dataGridViewHighlight.Location = dataGridView.Location;
            dataGridViewHighlight.Size = dataGridView.Size;
            dataGridViewHighlight.BringToFront();
            dataGridViewHighlight.Columns.Clear();
            dataGridViewHighlight.Visible = true;
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                if (item == null) continue;
                DataGridViewColumn column1 = item.Clone() as DataGridViewColumn;
                if (column1 == null) continue;
                dataGridViewHighlight.Columns.Add(column1);
            }
            dataGridViewHighlight.Rows.Clear();
            foreach (int index in dataGridViewMatchingRows)
            {
                DataGridViewRow row = dataGridView.Rows[index];
                if (row == null) continue;
                DataGridViewRow row1 = row.Clone() as DataGridViewRow;
                if (row1 == null) continue;
                List<object> objects = new List<object>();
                foreach (DataGridViewCell oldCell in row.Cells)
                {
                    objects.Add(oldCell.Value);
                }
                row1.SetValues(objects.ToArray<object>());
                dataGridViewHighlight.Rows.Add(row1);
            }
        }

        /// <summary>
        /// Determines a qualitative value of how possible string is a close match to text string. 
        /// </summary>
        /// <param name="text">The text to test possible against.</param>
        /// <param name="possible">The possible string to test.</param>
        /// <returns>A smaller value is a closer match.</returns>
        internal static int LevenshteinDistance(string text, string possible)
        {
            int textLength = text.Length;
            int possibleLength = possible.Length;
            int[,] costArray = new int[textLength + 1, possibleLength + 1];
            if (textLength == 0) return possibleLength;
            if (possibleLength == 0) return textLength;
            // Initialize the array.
            for (int ii = 0; ii <= textLength; ii++)
            {
                costArray[ii, 0] = ii;
            }
            // Initialize the array.
            for (int jj = 0; jj <= possibleLength; jj++)
            {
                costArray[0, jj] = jj;
            }
            for (int ii = 1; ii <= textLength; ii++)
            {
                for (int jj = 1; jj <= possibleLength; jj++)
                {
                    int cost = ((possible[jj - 1] == text[ii - 1]) ? 0 : 1);
                    costArray[ii, jj] = Math.Min(Math.Min(costArray[ii - 1, jj] + 1, costArray[ii, jj - 1] + 1), costArray[ii - 1, jj - 1] + cost);
                }
            }
            return costArray[textLength, possibleLength];
        }

    }

    internal class DrawingControl
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }
    }

    public class MyDataGridView : DataGridView
    {
        protected override void OnSorted(EventArgs e)
        {
            base.OnSorted(e);
        }

        public override void Sort(DataGridViewColumn dataGridViewColumn, ListSortDirection direction)
        {
            base.Sort(dataGridViewColumn, direction);
        }

        public override void Sort(System.Collections.IComparer comparer)
        {
            base.Sort(comparer);
        }

        private HashSet<int> mHiddenColumns;
        public void HideColumns()
        {
            if (mHiddenColumns != null) return;
            mHiddenColumns = new HashSet<int>();
            for (int ii = 0; ii < this.Columns.Count; ii++)
            {
                DataGridViewColumn column = this.Columns[ii];
                if (column == null || !column.Visible) continue;
                mHiddenColumns.Add(ii);
            }
            if (mHiddenColumns.Count <= 0) mHiddenColumns = null;
        }

        public void RestoreHiddenColumns()
        {
            if (mHiddenColumns == null) return;
            if (mHiddenColumns.Count <= 0)
            {
                mHiddenColumns = null;
                return;
            }
            foreach (int index in mHiddenColumns)
            {
                if (index < 0 || index >= this.Columns.Count) continue;
                DataGridViewColumn column = this.Columns[index];
                if (column == null) continue;
                column.Visible = true;
            }
            mHiddenColumns = null;
        }

        protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
        {
            if (SortSuspended())
            {
                e.Value = String.Empty;
                return;
            }
            base.OnCellValueNeeded(e);
        }

        private bool SortSuspended()
        {
            if (mSuspendedColumns != null) return true;
            return false;
        }

        private Dictionary<int, DataGridViewColumnSortMode> mSuspendedColumns;
        public void SuspendSort()
        {
            if (mSuspendedColumns != null) return;
            mSuspendedColumns = new Dictionary<int, DataGridViewColumnSortMode>();
            for (int ii = 0; ii < this.Columns.Count; ii++)
            {
                DataGridViewColumn column = this.Columns[ii];
                if (column == null) continue;
                mSuspendedColumns[ii] = column.SortMode;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (mSuspendedColumns.Count <= 0) mSuspendedColumns = null;
        }

        public void RestoreSort()
        {
            if (mSuspendedColumns == null) return;
            if (mSuspendedColumns.Count <= 0)
            {
                mSuspendedColumns = null;
                return;
            }
            foreach (KeyValuePair<int, DataGridViewColumnSortMode> item in mSuspendedColumns)
            {
                if (item.Key < 0 || item.Key >= this.Columns.Count) continue;
                DataGridViewColumn column = this.Columns[item.Key];
                if (column == null) continue;
                column.SortMode = item.Value;
            }
            mSuspendedColumns = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
