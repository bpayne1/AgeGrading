using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.Globalization;
using System.Xml;
using HtmlAgilityPack;
using System.Threading;

namespace AgeGrading
{
	public partial class AgeGradingForm : Form
	{
		private const string kRegistryLocation = @"Software\SFTC\AgeGradingForm";
		private const string kRaceTypeKey = @"RaceType";
		private const string kColumnsLineNumberKey = @"ColumnsLineNumber";
		private const string kStartingLineNumberKey = @"StartingLineNumber";
		private const string kDefaultAgeKey = @"DefaultAge";
		private const string kDefaultNameKey = @"DefaultName";
		private const string kDefaultTimeKey = @"DefaultTime";
		private const string kDefaultSexKey = @"DefaultSex";
		private const string kColumnParseType = @"ColumnParseType";
		private const string kMemberTableKey = @"MemberTable";
		private const string kKQRaceKey = @"KQRaceKey";
		private const string kCompletedRequirementsKey = @"CompletedRequirements";
		public AgeGradingForm()
		{
			InitializeComponent();
			GetColumnParseType();
			GetUserChoices();
			KQCompetitionData = new KQCompetitionData();
			KQCompetitionData.MemberTableName = this.MemberTableName;
			KQCompetitionData.SaveTable();
			RaceInfo.BuildRaceInfo();
		}

		 public const string kYear = "2022";
		 private static int mYear;
		 public static int Year
		 {
			 get
			 {
				 if (mYear > 0) return mYear;
				 int.TryParse(kYear, out mYear);
				 return mYear;
			 }
		 }
internal static string GetFolderPath()
		{
			String skydrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), String.Format(@"SkyDrive\Documents\{0}", kYear));
			if (Directory.Exists(skydrivePath)) return skydrivePath;
			skydrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), String.Format(@"OneDrive\Documents\{0}", kYear));
			if (Directory.Exists(skydrivePath)) return skydrivePath;
			string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return String.Format(@"{0}\{1}", myDocuments, kYear);
		}

		private KQCompetitionData mKQCompetitionData;
		protected KQCompetitionData KQCompetitionData
		{
			get { return mKQCompetitionData; }
			set { mKQCompetitionData = value; }
		}

		internal static void SavePositionInformation(string key, Point location, Size size, FormWindowState windowState)
		{
			string sSubKey = String.IsNullOrEmpty(key) ? kRegistryLocation : (kRegistryLocation + @"\" + key);
			try
			{
				using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(sSubKey))
				{
					if (windowState != FormWindowState.Maximized)
					{
						regKey.SetValue("X", location.X);
						regKey.SetValue("Y", location.Y);
						regKey.SetValue("Width", size.Width);
						regKey.SetValue("Height", size.Height);
					}
					if (windowState != FormWindowState.Maximized && windowState != FormWindowState.Normal) windowState = FormWindowState.Normal;
					regKey.SetValue("WindowState", (int)windowState);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		internal static void GetPositionInformation(string key, ref Point location, ref Size size, ref FormWindowState windowState)
		{
			string sSubKey = String.IsNullOrEmpty(key) ? kRegistryLocation : (kRegistryLocation + @"\" + key);
			try
			{
				using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(sSubKey))
				{
					if (regKey != null)
					{
						location.X = Convert.ToInt32(regKey.GetValue("X", location.X), CultureInfo.InvariantCulture);
						location.Y = Convert.ToInt32(regKey.GetValue("Y", location.Y), CultureInfo.InvariantCulture);
						size.Width = Convert.ToInt32(regKey.GetValue("Width", size.Width), CultureInfo.InvariantCulture);
						size.Height = Convert.ToInt32(regKey.GetValue("Height", size.Height), CultureInfo.InvariantCulture);
						int iWindowState = (int)windowState;
						windowState = (FormWindowState)Convert.ToInt32(regKey.GetValue("WindowState", iWindowState), CultureInfo.InvariantCulture);
					}
					else
					{
						SavePositionInformation(key, location, size, windowState);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
			SavePositionInformation(this.Name, this.Location, this.Size, this.WindowState);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			SaveUserChoices();
			ResultCalculator.SaveBooleanListsFromFile();
			base.OnClosing(e);
		}

		private int ColumnsLineNumber
		{
			get
			{
				int columnsLineNumber = Convert.ToInt32(numColumnsLineNumber.Value);
				return columnsLineNumber;
			}
		}

		private int StartingLineNumber
		{
			get
			{
				int statingLineNumber = Convert.ToInt32(numStartingLineNumber.Value);
				return statingLineNumber;
			}
		}

		private void SaveColumnParseType()
		{
			string radioName = radioFixedWidthColumns.Name;
			if (radioFreeForm.Checked) radioName = radioFreeForm.Name;
			else if (radioUseTabSeparatedColumns.Checked) radioName = radioUseTabSeparatedColumns.Name;
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(kRegistryLocation))
			{
				if (key != null)
				{
					key.SetValue(kColumnParseType, radioName);
				}
			}
		}

		private Control FindControl(string name, Control.ControlCollection controlCollection)
		{
			Control[] controls = controlCollection.Find(name, false);
			if (controls != null && controls.Length == 1) return controls[0];
			foreach (Control item in controlCollection)
			{
				if (item == null) continue;
				if (item.Name.Equals(name)) return item;
			}
			return null;
		}

		private Control FindControl(string name)
		{
			Control[] controls = Controls.Find(name, false);
			if (controls != null && controls.Length == 1) return controls[0];
			foreach (Control item in Controls)
			{
				if (item == null) continue;
				if (item.Name.Equals(name)) return item;
				Control control = FindControl(name, item.Controls);
				if (control != null) return control;
			}
			return null;
		}

		private void GetColumnParseType()
		{
			string radioName = radioFixedWidthColumns.Name;
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(kRegistryLocation))
			{
				if (key != null)
				{
					radioName = key.GetValue(kColumnParseType, radioName) as string;
				}
			}
			RadioButton radio = FindControl(radioName) as RadioButton;
			if (radio == null) radio = radioFixedWidthColumns;
			radio.Checked = true;
		}

		private void SaveUserChoices()
		{
			string raceType = GetComboBoxSelection(cmbRaceTypes);
			int columnsLineNumber = ColumnsLineNumber;
			int statingLineNumber = StartingLineNumber;
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(kRegistryLocation))
			{
				if (key != null)
				{
					key.SetValue(kRaceTypeKey, raceType);
					key.SetValue(kColumnsLineNumberKey, columnsLineNumber);
					key.SetValue(kStartingLineNumberKey, statingLineNumber);
					key.SetValue(kDefaultAgeKey, DefaultAge);
					key.SetValue(kDefaultNameKey, DefaultName);
					key.SetValue(kDefaultSexKey, DefaultSex);
					key.SetValue(kDefaultTimeKey, DefaultTime);
				}
			}
		}

		private void GetUserChoices()
		{
			if (cmbRaceTypes.Items.Count <= 0)
			{
				string raceType = "5kmRoad";
				decimal columnsLineNumber = 0;
				decimal startingLineNumber = 2;
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(kRegistryLocation))
				{
					if (key != null)
					{
						try
						{
							raceType = key.GetValue(kRaceTypeKey) as string;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							columnsLineNumber = Convert.ToDecimal(key.GetValue(kColumnsLineNumberKey));
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							startingLineNumber = Convert.ToDecimal(key.GetValue(kStartingLineNumberKey));
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							DefaultAge = key.GetValue(kDefaultAgeKey, DefaultAge) as string;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							DefaultName = key.GetValue(kDefaultNameKey, DefaultName) as string;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							DefaultSex = key.GetValue(kDefaultSexKey, DefaultSex) as string;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
						try
						{
							DefaultTime = key.GetValue(kDefaultTimeKey, DefaultTime) as string;
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.ToString());
						}
					}
				}
				FillComboBox(cmbRaceTypes, AgeGradingTables.GetRaceTypes(), raceType);
				numStartingLineNumber.Value = startingLineNumber;
				numColumnsLineNumber.Value = columnsLineNumber;
				LoadRacesCombo();
			}
		}

		private const string kDefaultKQRace = "<Use Race Type Choice>";
		private string mKQRace;
		protected string KQRace
		{
			get
			{
				if (mKQRace == null)
					mKQRace = GetRegistryString(kRegistryLocation, kKQRaceKey, kDefaultKQRace);
				return mKQRace;
			}
			set { mKQRace = value; }
		}

		private bool mInitializingKQRaces;
		protected bool InitializingKQRaces
		{
			get { return mInitializingKQRaces; }
			set { mInitializingKQRaces = value; }
		}

		private string GetRaceFileName(RaceInfo raceInfo)
		{
			if (raceInfo == null) return String.Empty;
			string xmlName = String.Empty;
			if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
			{
				xmlName = raceInfo.Name;
				xmlName = String.Format("{0}.{1}", xmlName, kXMLExtension);
			}
			return xmlName;
		}

		private string EnsureWhiteSpaceMatch(string text)
		{
			if (text == null) return string.Empty;
			string[] temp = text.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (temp == null) return string.Empty;
			StringBuilder str = new StringBuilder();
			foreach (var item in temp)
			{
				if (String.IsNullOrWhiteSpace(item)) continue;
				if (str.Length > 0)
					str.Append(" ");
				str.Append(item);
			}
			return str.ToString();
		}

		private bool CloseMatchToPrimary(string primary, string text)
		{
			if (string.IsNullOrWhiteSpace(primary)) return false;
			if (string.IsNullOrWhiteSpace(text)) return false;
			string[] temp = primary.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (temp == null) return false;
			int matchingCount = 0;
			text = text.ToLower();
			foreach (var item in temp)
			{
				if (text.Contains(item.ToLower()))
					matchingCount++;
			}
			if (matchingCount < 1) return false;
			double matchedPercentage = (100 * matchingCount) / temp.Length;
			return (matchedPercentage >= 66);
		}

		private RaceInfo VerifyMatch(string raceName, RaceInfo matching)
		{
			if (String.IsNullOrWhiteSpace(raceName)) return null;
			if (matching == null) return null;
			// We need to verify the name actually is close enough to the original to
			// consider it a valid match.
			if (CloseMatchToPrimary(matching.Name, raceName))
				return matching;
			return cmbKQRace.Items[0] as RaceInfo;
		}

		private RaceInfo FindMatchingRace(string raceName)
		{
			if (String.IsNullOrWhiteSpace(raceName)) return null;
			string year = Year.ToString();
			if (raceName.StartsWith(year))
				raceName = raceName.Substring(year.Length);
			List<RaceInfo> races = RaceInfo.GetRaces();
			string[] temp = raceName.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			List<string> words = new List<string>(temp);
			List<RaceInfo> matching = new List<RaceInfo>(races);
			foreach (var item in words)
			{
				matching = FindMatchingRaces(item, matching);
				if (matching != null)
				{
					if (matching.Count == 1) return VerifyMatch(raceName, matching[0]);
					else if (matching.Count < 1) matching = new List<RaceInfo>(races);
				}
			}
			return null;
		}

		private List<RaceInfo> FindMatchingRaces(string word, List<RaceInfo> matchingSoFar)
		{
			word = word.ToLower();
			List<RaceInfo> matching = new List<RaceInfo>();
			foreach (var item in matchingSoFar)
			{
				if (item == null) continue;
				if (item.Name.ToLower().Contains(word))
					matching.Add(item);
			}
			return matching;
		}

		private void LoadRacesCombo(bool reload = false)
		{
			if (!reload && cmbKQRace.Items.Count > 0) return;
			try
			{
				cmbKQRace.Items.Clear();
				InitializingKQRaces = true;
				string folder = GetFolderPath();
				List<RaceInfo> races = RaceInfo.GetRaces();
				if (races == null || races.Count <= 0) return;
				foreach (RaceInfo race in races)
				{
					if (!String.IsNullOrEmpty(folder))
					{
						string path = Path.Combine(folder, GetRaceFileName(race));
						if (File.Exists(path)) race.FileExists = true;
					}
					cmbKQRace.Items.Add(race);
				}
				cmbKQRace.Items.Insert(0, kDefaultKQRace);
				int index = cmbKQRace.FindString(KQRace);
				if (index < 0) index = 0;
				cmbKQRace.SelectedIndex = index;
				SetRaceToolTip();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				InitializingKQRaces = false;
			}
		}

		private void SetToRace(RaceInfo raceInfo)
		{
			if (raceInfo == null)
			{
				cmbKQRace.SelectedIndex = 0;
				return;
			}
			string raceName = raceInfo.Name.ToLower();
			int index = -1;
			for (int ii = 0; ii < cmbKQRace.Items.Count; ii++)
			{
				object item = cmbKQRace.Items[ii];
				if (item == null) continue;
				string name = item.ToString().ToLower();
				if (name.EndsWith(raceName))
				{
					index = ii;
					break;
				}
			}
			if (index < 0) return;
			cmbKQRace.SelectedIndex = index;
			SetRaceToolTip();
		}

		private void SetRaceToolTip()
		{
			if (cmbKQRace.SelectedIndex < 0) return;
			RaceInfo race = cmbKQRace.Items[cmbKQRace.SelectedIndex] as RaceInfo;
			if (race == null) return;
			toolTip.SetToolTip(cmbKQRace, race.RaceDate.ToString("dddd, MMMM d, yyyy"));
		}

		private void SetRacesCombo(string raceName)
		{
			if (String.IsNullOrEmpty(raceName)) return;
			SetCombo(cmbKQRace, raceName, false);
		}

		private static bool SetCombo(ComboBox combo, string item, bool addMissingItem)
		{
			if (combo == null) return false;
			if (String.IsNullOrEmpty(item)) return false;
			if (combo.Items.Count <= 0) return false;
			int index = combo.FindStringExact(item);
			if (index >= 0) combo.SelectedIndex = index;
			else if (addMissingItem)
			{
				index = combo.Items.Add(item);
				combo.SelectedIndex = index;
			}
			return (index >= 0);
		}

		internal class ColumnCoordinates
		{
			public int StartingIndex { get; set; }
			public int Count { get; set; }
			public ColumnCoordinates(int startingIndex, int count)
			{
				StartingIndex = startingIndex;
				Count = count;
			}

			public override string ToString()
			{
				return String.Format("startingIndex = {0} count {1}", StartingIndex, Count);
			}
		}

		private List<string> mColumns;
		private List<ColumnCoordinates> mColumnWidths;
		private void GetFixedColumnWidths(string columns)
		{
			AgeGradeTextReader textReader = new AgeGradeTextReader();
			List<List<string>> temp = textReader.GetFixedColumnLines(txtRaceResults.Lines, ColumnsLineNumber, StartingLineNumber);
			foreach (List<string> line in temp)
			{
				foreach (string item in line)
				{
					Debug.Write(item);
				}
				Debug.WriteLine(String.Empty);
			}
			mColumns = new List<string>();
			mColumnWidths = new List<ColumnCoordinates>();
			if (String.IsNullOrEmpty(columns)) return;
			int count = 0;
			int startingIndex = 0;
			bool spaceSeen = false;
			StringBuilder columnHeading = new StringBuilder();
			for (int ii = 0; ii < columns.Length; ii++)
			{
				char character = columns[ii];
				if (!spaceSeen)
				{
					if (char.IsWhiteSpace(character)) spaceSeen = true;
					else columnHeading.Append(character);
					count++;
				}
				else if (!char.IsWhiteSpace(character))
				{
					mColumns.Add(columnHeading.ToString());
					mColumnWidths.Add(new ColumnCoordinates(startingIndex, count));
					columnHeading = new StringBuilder();
					columnHeading.Append(character);
					spaceSeen = false;
					startingIndex = ii;
					count = 1;
				}
				else count++;
			}
#if SHOW_COLUMN_WIDTHS
			for (int ii = 0; ii < mColumns.Count; ii++)
			{
				Debug.WriteLine(String.Format("Column {0}::coordinates = {1}",
					mColumns[ii], mColumnWidths[ii]));
			}
#endif
		}

		private AgeGradeTextReader mTextReader;
		private List<List<string>> mRowColumns;
		private List<string> GetTabSeparatedColumns(string columns)
		{
			if (String.IsNullOrEmpty(columns)) return null;
			List<string> list = new List<string>();
			mTextReader = new AgeGradeTextReader();
			string columnList = "\t";
			mRowColumns = mTextReader.GetDelimitedLines(txtRaceResults.Lines, ColumnsLineNumber, StartingLineNumber, columnList);
			list = mTextReader.ColumnHeadings;
			return list;
		}

		private List<string> GetFreeFormColumns(string columns)
		{
			if (String.IsNullOrEmpty(columns)) return null;
			List<string> list = new List<string>();
			mTextReader = new AgeGradeTextReader();
			string columnList = "\t ";
			mRowColumns = mTextReader.GetDelimitedLines(txtRaceResults.Lines, ColumnsLineNumber, StartingLineNumber, columnList);
			list = mTextReader.ColumnHeadings;
			return list;
		}

		private void GetRows()
		{
		}

		private bool mProcessedFirstPass;
		private void ProcessText()
		{
			try
			{
				if (String.IsNullOrEmpty(txtRaceResults.Text)) return;
				int columnsLineNumber = Convert.ToInt32(numColumnsLineNumber.Value);
				string columnHeader = txtRaceResults.Lines[columnsLineNumber];
				if (!mProcessedFirstPass)
				{
					mProcessedFirstPass = true;
					// Try to determine if the header contains a tab.
					int index = columnHeader.IndexOf('\t');
					if (index >= 0) radioUseTabSeparatedColumns.Checked = true;
				}
				if (radioFixedWidthColumns.Checked)
				{
					if (mTextReader == null) mTextReader = new AgeGradeTextReader();
					mRowColumns = mTextReader.GetFixedColumnLines(txtRaceResults.Lines, ColumnsLineNumber, StartingLineNumber);
					mColumns = mTextReader.ColumnHeadings;

					//GetFixedColumnWidths(columnHeader);
				}
				else if (radioFreeForm.Checked)
					mColumns = GetFreeFormColumns(columnHeader);
				else
					mColumns = GetTabSeparatedColumns(columnHeader);
				FillCombos();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private const string kDefaultAge = "Age";
		private string mDefaultAge = kDefaultAge;
		protected string DefaultAge
		{
			get { return mDefaultAge; }
			set { mDefaultAge = value; }
		}

		private const string kDefaultSex = "Sex";
		private string mDefaultSex = kDefaultSex;
		protected string DefaultSex
		{
			get { return mDefaultSex; }
			set { mDefaultSex = value; }
		}

		private const string kDefaultName = "Name";
		private string mDefaultName = kDefaultName;
		protected string DefaultName
		{
			get { return mDefaultName; }
			set { mDefaultName = value; }
		}

		private const string kDefaultFirstName = "<None>";
		private string mDefaultFirstName = kDefaultFirstName;
		protected string DefaultFirstName
		{
			get { return mDefaultFirstName; }
			set { mDefaultFirstName = value; }
		}

		private const string kDefaultTime = "Time";
		private string mDefaultTime = kDefaultTime;
		protected string DefaultTime
		{
			get { return mDefaultTime; }
			set { mDefaultTime = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			try
			{
				Point location = this.Location;
				Size size = this.Size;
				FormWindowState state = this.WindowState;
				GetPositionInformation(this.Name, ref location, ref size, ref state);
				this.Location = location;
				this.Size = size;
				if (state != FormWindowState.Minimized) this.WindowState = state;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			base.OnLoad(e);
			FillDefaultCombos();
			chkOnlyCompletingRequirements.Checked = GetRegistryBool(kRegistryLocation, kCompletedRequirementsKey, chkOnlyCompletingRequirements.Checked);
		}

		private List<string> GetSingleList(string str)
		{
			List<string> list = new List<string>();
			if (!String.IsNullOrEmpty(str)) list.Add(str);
			return list;
		}

		private void FillDefaultCombos()
		{
			FillComboBox(cmbAge, GetSingleList(DefaultAge), DefaultAge);
			FillComboBox(cmbGender, GetSingleList(DefaultSex), DefaultSex);
			FillComboBox(cmbName, GetSingleList(DefaultName), DefaultName);
			FillComboBox(cmbTime, GetSingleList(DefaultTime), DefaultTime);
			FillComboBox(cmbFirstName, GetSingleList(DefaultFirstName), DefaultFirstName, true);
			AddGenderOverride();
		}

		private const string kMaleOverride = "<Male>";
		private const string kFemaleOverride = "<Female>";

		private void AddGenderOverride()
		{
			cmbGender.Items.Add(kMaleOverride);
			cmbGender.Items.Add(kFemaleOverride);
		}

		private void FillCombos()
		{
			cmbAge.Items.Clear();
			cmbGender.Items.Clear();
			cmbName.Items.Clear();
			cmbTime.Items.Clear();
			if (mColumns == null || mColumns.Count <= 0) return;
			FillComboBox(cmbAge, mColumns, DefaultAge);
			FillComboBox(cmbGender, mColumns, DefaultSex);
			FillComboBox(cmbName, mColumns, DefaultName);
			FillComboBox(cmbTime, mColumns, DefaultTime);
			FillComboBox(cmbFirstName, mColumns, DefaultFirstName, true);
			AddGenderOverride();
		}

		private const string kUnknownComboItem = "UNKNOWN";
		private string GetComboBoxSelection(ComboBox combo)
		{
			if (combo == null || combo.SelectedIndex < 0) return kUnknownComboItem;
			RaceInfo race = combo.Items[combo.SelectedIndex] as RaceInfo;
			if (race != null) return race.Name;
			string selection = combo.Items[combo.SelectedIndex].ToString();
			if (String.IsNullOrEmpty(selection)) selection = kUnknownComboItem;
			return selection;
		}

		private RaceInfo GetKQRaceSelection()
		{
			ComboBox combo = cmbKQRace;
			if (combo == null || combo.SelectedIndex < 0) return null;
			RaceInfo raceInfo = combo.Items[combo.SelectedIndex] as RaceInfo;
			return raceInfo;
		}

		private void FillComboBox(ComboBox combo, List<string> list, string initialSelection)
		{
			FillComboBox(combo, list, initialSelection, false);
		}

		private void FillComboBox(ComboBox combo, List<string> list, string initialSelection, bool insertInitialSelection)
		{
			if (combo == null) return;
			combo.Items.Clear();
			if (list == null || list.Count <= 0) return;
			string foundInitialString = null;
			foreach (string item in list)
			{
				if (!String.IsNullOrEmpty(item))
				{
					if (foundInitialString == null)
					{
						int index = item.IndexOf(initialSelection, StringComparison.OrdinalIgnoreCase);
						if (index >= 0) foundInitialString = item;
					}
					combo.Items.Add(item);
				}
			}
			if (foundInitialString == null) foundInitialString = initialSelection;
			if (!String.IsNullOrEmpty(initialSelection))
			{
				int index = combo.FindStringExact(foundInitialString);
				if (index < 0)
				{
					index = 0;
					if (insertInitialSelection) combo.Items.Insert(0, initialSelection);
				}
				combo.SelectedIndex = index;
			}
		}

		private bool mRestartAgeGroup;
		public bool RestartAgeGroup
		{
			get { return mRestartAgeGroup; }
			set { mRestartAgeGroup = value; }
		}

		private void btnProcessColumn_Click(object sender, EventArgs e)
		{
			ProcessText();
			btnCalculateAgeGrade.Enabled = btnProcessColumn.Enabled;
			btnNextAgeGroup.Enabled = btnCalculateAgeGrade.Enabled;
			btnMemberAges.Enabled = btnNextAgeGroup.Enabled;
			numAgeGroup.Value = numAgeGroup.Minimum;
			RestartAgeGroup = true;
		}

		private DataTable mDataTable;
		internal static void AddDataColumn(DataTable dataTable, string name, Type dataType)
		{
			if (dataTable == null) return;
			if (String.IsNullOrEmpty(name)) return;
			if (dataType == null) return;
			DataColumn column = dataTable.Columns[name];
			if (column != null) return;
			DataColumn dataColumn = dataTable.Columns.Add(name);
			dataColumn.DataType = dataType;
		}

		private const string kPlaceColumnName = "Place";
		private const string kAgeAdjustedColumnName = "Age Adjusted";
		private const string kNormalizedTimeColumnName = "Normalized Time";
		private const string kWorldPercentColumnName = "World Percent";
		private DataTable GetDataTable()
		{
			mDataTable = new DataTable("Finishers");
			AddDataColumn(mDataTable, kPlaceColumnName, typeof(int));
			AddDataColumn(mDataTable, GetComboBoxSelection(cmbName), typeof(string));
			AddDataColumn(mDataTable, GetComboBoxSelection(cmbAge), typeof(int));
			AddDataColumn(mDataTable, GetComboBoxSelection(cmbGender), typeof(string));
			AddDataColumn(mDataTable, GetComboBoxSelection(cmbTime), typeof(string));
			AddDataColumn(mDataTable, kAgeAdjustedColumnName, typeof(string));
			AddDataColumn(mDataTable, kNormalizedTimeColumnName, typeof(string));
			AddDataColumn(mDataTable, kWorldPercentColumnName, typeof(double));
			return mDataTable;
		}

		private const int kMaleOverrideIndex = -100;
		private const int kFemaleOverrideIndex = kMaleOverrideIndex - 1;
		string mNameColumnName;
		string mFirstNameColumnName;
		string mAgeColumnName;
		string mGenderColumnName;
		string mTimeColumnName;
		int mNameIndex;
		int mFirstNameIndex;
		int mAgeIndex;
		int mGenderIndex;
		int mTimeIndex;
		private void InitializeColumnSearch()
		{
			mFirstNameColumnName = null;
			mFirstNameIndex = -1;
			mNameColumnName = GetComboBoxSelection(cmbName);
			mFirstNameColumnName = GetComboBoxSelection(cmbFirstName);
			mAgeColumnName = GetComboBoxSelection(cmbAge);
			mGenderColumnName = GetComboBoxSelection(cmbGender);
			mTimeColumnName = GetComboBoxSelection(cmbTime);
			mNameIndex = mColumns.FindIndex(s => s == mNameColumnName);
			if (!String.IsNullOrEmpty(mFirstNameColumnName)) mFirstNameIndex = mColumns.FindIndex(s => s == mFirstNameColumnName);
			mAgeIndex = mColumns.FindIndex(s => s == mAgeColumnName);
			mGenderIndex = mColumns.FindIndex(s => s == mGenderColumnName);
			if (mGenderIndex < 0)
			{
				if (String.Equals(mGenderColumnName, kMaleOverride, StringComparison.Ordinal)) mGenderIndex = kMaleOverrideIndex;
				else if (String.Equals(mGenderColumnName, kFemaleOverride, StringComparison.Ordinal)) mGenderIndex = kFemaleOverrideIndex;
			}
			mTimeIndex = mColumns.FindIndex(s => s == mTimeColumnName);
		}

		public static void ShowErrorMessage(string text)
		{
			if (text == null) text = string.Empty;
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void btnCalculateAgeGrade_Click(object sender, EventArgs e)
		{
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo == null)
			{
				ShowErrorMessage($"Unable to find current race's information");
				return;
			}
			int currentYear = DateTime.Now.Year;
			if (raceInfo.RaceDate.Year != currentYear)
			{
				ShowErrorMessage($"The race date for '{raceInfo.Name}' hasn't been set or is incorrect: Race Date '{raceInfo.RaceDate}'");
				return;
			}
			if (mRowColumns == null || mRowColumns.Count <= 0) return;
			GetDataTable();
			if (mDataTable == null) return;
			InitializeColumnSearch();
			int place = 0;
			for (int ii = 0; ii < mRowColumns.Count; ii++)
			{
				try
				{
					place++;
					DataRow dataRow = GetDataRow(mRowColumns[ii], place);
					if (dataRow != null)
					{
						mDataTable.Rows.Add(dataRow);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
			dataGridView.DataSource = mDataTable;
			dataGridView.Focus();
		}

		private string GetColumnContents(List<string> fields, int columnIndex, out bool failed)
		{
			failed = true;
			if (fields == null || fields.Count <= columnIndex) return null;
			string column = fields[columnIndex];
			failed = false;
			return column;
		}

		private string GetColumnContents(int lineNumber, int columnIndex, string line, out bool failed)
		{
			failed = false;
			string column = null;
			if (mRowColumns != null && lineNumber < mRowColumns.Count)
			{
				List<string> row = mRowColumns[lineNumber];
				if (row == null || row.Count <= columnIndex) return null;
				column = row[columnIndex];
			}
			else failed = true;
			return column;
		}

		private char[] mColonCharArray = ":".ToCharArray();
		private string AdjustTimeSpan(string span)
		{
			if (String.IsNullOrEmpty(span)) return span;
			const char kColon = ':';
			string[] split = span.Split(mColonCharArray, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder firstPass = new StringBuilder();
			for (int ii = 0; ii < split.Length; ii++)
			{
				string item = split[ii].Trim();
				if (firstPass.Length > 0) firstPass.Append(kColon);
				int field = 0;
				if (!int.TryParse(item, out field)) return span;
				if (field >= 60)
				{
					int mod = field % 60;
					int div = field / 60;
					firstPass.Append(String.Format("{0}{1}", div, kColon));
					firstPass.Append(String.Format("{0}", mod));
					continue;
				}
				firstPass.Append(item);
			}
			span = firstPass.ToString();
			int colonCount = 0;
			foreach (char item in span)
			{
				if (kColon == item) colonCount++;
			}
			StringBuilder str = new StringBuilder();
			for (int ii = colonCount; ii < 2; ii++)
			{
				str.Append("00:");
			}
			if (span.Length > 2 && span[1] == kColon) str.Append('0');
			str.Append(span);
			return str.ToString();
		}

		private static char[] mMillisecond = ".".ToCharArray();
		private static char[] mColon = ":".ToCharArray();
		private static TimeSpan GetTimeSpan(string timeContents)
		{
			if (String.IsNullOrWhiteSpace(timeContents)) return TimeSpan.Zero;
			string[] split = timeContents.Split(mColon);
			if (split == null || split.Length <= 0) return TimeSpan.Zero;
			if (split.Length > 4) return TimeSpan.Zero;
			const string kTimeSeparator = @"\:";
			int index = 0;
			string format = String.Empty;
			if (split.Length == 4)
			{
				const string kDay = "d";
				StringBuilder str = new StringBuilder();
				for (int ii = 0; ii < split[index].Length; ii++)
				{
					str.Append(kDay);
				}
				format += str.ToString();
				index++;
			}
			if (split.Length >= 3)
			{
				const string kHour = "h";
				StringBuilder str = new StringBuilder();
				if (index > 0) str.Append(kTimeSeparator);
				for (int ii = 0; ii < split[index].Length; ii++)
				{
					str.Append(kHour);
				}
				format += str.ToString();
				index++;
			}
			if (split.Length >= 2)
			{
				const string kMinute = "m";
				StringBuilder str = new StringBuilder();
				if (index > 0) str.Append(kTimeSeparator);
				for (int ii = 0; ii < split[index].Length; ii++)
				{
					str.Append(kMinute);
				}
				format += str.ToString();
				index++;
			}
			if (split.Length >= 1)
			{
				const string kSecond = "s";
				string[] secondSplit = split[index].Split(mMillisecond);
				StringBuilder str = new StringBuilder();
				if (index > 0) str.Append(kTimeSeparator);
				for (int ii = 0; ii < secondSplit[0].Length; ii++)
				{
					str.Append(kSecond);
				}
				format += str.ToString();
				if (secondSplit.Length == 2)
				{
					// We have milliseconds.
					const string kMillisecond = "f";
					str = new StringBuilder();
					str.Append(@"\.");
					for (int ii = 0; ii < secondSplit[1].Length; ii++)
					{
						str.Append(kMillisecond);
					}
					format += str.ToString();
				}
			}
			TimeSpan span = TimeSpan.Zero;
			if (!TimeSpan.TryParseExact(timeContents, format, null, out span))
			{
				return TimeSpan.Zero;
			}
			return span;
		}

		private char[] mColonArray = ":".ToCharArray();
		private DataRow GetDataRow(List<string> fields, int place)
		{
			if (fields == null || fields.Count <= 0) return null;
			DataRow dataRow = null;
			try
			{
				bool failed = true;
				string name = GetColumnContents(fields, mNameIndex, out failed);
				if (failed) return null;
				string[] split = name.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				StringBuilder str = new StringBuilder();
				foreach (string item in split)
				{
					if (str.Length > 0) str.Append(' ');
					str.Append(item);
				}
				name = str.ToString();
				//name = CombineFirstAndLastName(fields, name, ref failed);
				string ageContents = GetColumnContents(fields, mAgeIndex, out failed);
				if (failed) return null;
				int age = 0;
				if (!int.TryParse(ageContents, out age)) age = 0;
				string gender = null;
				if (mGenderIndex == kMaleOverrideIndex)
					gender = AgeGradingTables.AgeAdjustResult.kMale;
				else if (mGenderIndex == kFemaleOverrideIndex)
					gender = AgeGradingTables.AgeAdjustResult.kFemale;
				else
					gender = GetColumnContents(fields, mGenderIndex, out failed);
				if (failed) return null;
				string timeContents = AdjustTimeSpan(GetColumnContents(fields, mTimeIndex, out failed));
				if (failed) return null;
				//int index = timeContents.LastIndexOf('.');
				//if (index >= 0)
				//{
				//    timeContents = timeContents.Substring(0, index);
				//    string[] split = timeContents.Split(mColonArray);
				//    if (split != null && split.Length == 2) timeContents = "00:" + timeContents;
				//}
				TimeSpan time = GetTimeSpan(timeContents);
				dataRow = mDataTable.NewRow();
				dataRow[0] = place;
				dataRow[mNameColumnName] = name;
				dataRow[mAgeColumnName] = age;
				dataRow[mGenderColumnName] = gender;
				dataRow[mTimeColumnName] = FormatTimeSpan(AgeGrading.AgeGradingTables.RoundMilliseconds(time), false);
				string raceType = GetComboBoxSelection(cmbRaceTypes);
				RaceInfo raceInfo = GetKQRaceSelection();
				if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.AgeGradeTable))
					raceType = raceInfo.AgeGradeTable;
				AgeGrading.AgeGradingTables.AgeAdjustResult result =
					new AgeGradingTables.AgeAdjustResult(age, gender, time, raceType);
				dataRow[kAgeAdjustedColumnName] = FormatTimeSpan(result.AgeAdjusted, false);
				dataRow[kNormalizedTimeColumnName] = FormatTimeSpan(result.NormalizedTime, false);
				dataRow[kWorldPercentColumnName] = result.WorldPercent;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return dataRow;
		}

		private string CombineFirstAndLastName(List<string> fields, string name, ref bool failed)
		{
			string combined = name;
			if (mFirstNameIndex >= 0)
			{
				string firstName = GetColumnContents(fields, mFirstNameIndex, out failed);
				if (!String.IsNullOrEmpty(firstName)) combined = String.Format("{0} {1}", firstName, name);
			}
			return combined;
		}

		private DataRow GetDataRow(int lineIndex, int place)
		{
			if (mDataTable == null) return null;
			if (txtRaceResults.Lines == null || lineIndex < 0 || txtRaceResults.Lines.Length <= lineIndex) return null;
			string line = txtRaceResults.Lines[lineIndex];
			if (String.IsNullOrEmpty(line)) return null;
			bool failed = true;
			string name = GetColumnContents(lineIndex, mNameIndex, line, out failed);
			if (failed) return null;
			string ageContents = GetColumnContents(lineIndex, mAgeIndex, line, out failed);
			if (failed) return null;
			int age = 0;
			if (!int.TryParse(ageContents, out age)) age = 0;
			string gender = GetColumnContents(lineIndex, mGenderIndex, line, out failed);
			if (failed) return null;
			string timeContents = AdjustTimeSpan(GetColumnContents(lineIndex, mTimeIndex, line, out failed));
			if (failed) return null;
			int index = timeContents.LastIndexOf('.');
			if (index >= 0) timeContents = timeContents.Substring(0, index);
			TimeSpan time = new TimeSpan();
			if (!TimeSpan.TryParse(timeContents, out time)) time = new TimeSpan(6, 2, 2);
			DataRow dataRow = mDataTable.NewRow();
			dataRow[0] = place;
			dataRow[mNameColumnName] = name;
			dataRow[mAgeColumnName] = age;
			dataRow[mGenderColumnName] = gender;
			dataRow[mTimeColumnName] = AgeGrading.AgeGradingTables.RoundMilliseconds(time);
			string raceType = GetComboBoxSelection(cmbRaceTypes);
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.AgeGradeTable))
				raceType = raceInfo.AgeGradeTable;
			AgeGrading.AgeGradingTables.AgeAdjustResult result =
				new AgeGradingTables.AgeAdjustResult(age, gender, time, raceType);
			dataRow[kAgeAdjustedColumnName] = result.AgeAdjusted;
			dataRow[kNormalizedTimeColumnName] = result.NormalizedTime;
			dataRow[kWorldPercentColumnName] = result.WorldPercent;
			return dataRow;
		}

		private void txtRaceResults_TextChanged(object sender, EventArgs e)
		{
			btnProcessColumn.Enabled = (txtRaceResults.Lines != null && txtRaceResults.Lines.Length > 0);
			if (mDataTable != null)
			{
				mDataTable.Dispose();
				mDataTable = null;
			}
		}

		private void cmbRaceTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			SaveUserChoices();
		}

		private void cmbAge_SelectedIndexChanged(object sender, EventArgs e)
		{
			DefaultAge = GetComboBoxSelection(cmbAge);
		}

		private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
		{
			DefaultSex = GetComboBoxSelection(cmbGender);
		}

		private void cmbTime_SelectedIndexChanged(object sender, EventArgs e)
		{
			DefaultTime = GetComboBoxSelection(cmbTime);
		}

		private void cmbName_SelectedIndexChanged(object sender, EventArgs e)
		{
			DefaultName = GetComboBoxSelection(cmbName);
		}

		private void cmbFirstName_SelectedIndexChanged(object sender, EventArgs e)
		{
			DefaultFirstName = GetComboBoxSelection(cmbFirstName);
			btnCombineFirstAndLastNames.Enabled = false;
			if (mRowColumns == null || mRowColumns.Count <= 0) return;
			btnCombineFirstAndLastNames.Enabled = !String.Equals(DefaultFirstName, kDefaultFirstName, StringComparison.OrdinalIgnoreCase);
		}

		private void radio_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			if (radio == null) return;
			if (radio.Checked) SaveColumnParseType();
		}

		private string mLastFindText;
		public string LastFindText
		{
			get { return mLastFindText; }
			set { mLastFindText = value; }
		}

		private int mStartingRowIndex = 0;
		private int mStartingColumnIndex = 0;
		private void DoSearch(string findText)
		{
			if (String.IsNullOrEmpty(findText)) return;
			if (dataGridView.Rows.Count <= 0) return;
			if (dataGridView.Columns.Count <= 0) return;
			if (mStartingColumnIndex >= dataGridView.Columns.Count)
			{
				mStartingRowIndex++;
				mStartingColumnIndex = 0;
			}
			if (mStartingRowIndex < 0) mStartingRowIndex = 0;
			if (mStartingRowIndex >= dataGridView.Rows.Count) return;
			if (mStartingColumnIndex < 0) mStartingColumnIndex = 0;
			dataGridView.ClearSelection();
			int startingColumnIndex = mStartingColumnIndex;
			findText = findText.ToLower();
			for (int ii = mStartingRowIndex; ii < dataGridView.Rows.Count; ii++)
			{
				DataGridViewRow row = dataGridView.Rows[ii];
				for (int jj = startingColumnIndex; jj < dataGridView.Columns.Count; jj++)
				{
					DataGridViewCell cell = row.Cells[jj];
					if (cell.Value == null) continue;
					string value = cell.Value.ToString().ToLower();
					if (value.Contains(findText))
					{
						mStartingRowIndex = ii;
						mStartingColumnIndex = jj;
						cell.Selected = true;
						dataGridView.FirstDisplayedCell = cell;
						return;
					}
				}
				startingColumnIndex = 0;
			}
			// We didn't find anything so try from the beginning.
			if (mStartingRowIndex > 0)
			{
				int lastRowToSearch = mStartingRowIndex;
				for (int ii = 0; ii < lastRowToSearch; ii++)
				{
					DataGridViewRow row = dataGridView.Rows[ii];
					for (int jj = 0; jj < dataGridView.Columns.Count; jj++)
					{
						DataGridViewCell cell = row.Cells[jj];
						if (cell.Value == null) continue;
						string value = cell.Value.ToString().ToLower();
						if (value.Contains(findText))
						{
							mStartingRowIndex = ii;
							mStartingColumnIndex = jj;
							cell.Selected = true;
							dataGridView.FirstDisplayedCell = cell;
							return;
						}
					}
					startingColumnIndex = 0;
				}
			}
			dataGridView.Focus();
		}

		private void toolStripMenuItemFind_Click(object sender, EventArgs e)
		{
			if (dataGridView == null || dataGridView.Rows.Count <= 0) return;
			using (FindDialog dlg = new FindDialog())
			{
				dlg.FindText = mLastFindText;
				DialogResult dr = dlg.ShowDialog(this);
				if (dr == System.Windows.Forms.DialogResult.OK)
				{
					LastFindText = dlg.FindText;
					mStartingRowIndex = 0;
					mStartingColumnIndex = 0;
					DoSearch(LastFindText);
				}
			}
		}

		private void toolStripMenuItemSelectColumn_Click(object sender, EventArgs e)
		{
			try
			{
				if (mMouseLocation == Point.Empty) return;
				DataGridView.HitTestInfo info = dataGridView.HitTest(mMouseLocation.X, mMouseLocation.Y);
				if (info != null && info.ColumnIndex >= 0)
				{
					dataGridView.ClearSelection();
					foreach (DataGridViewRow row in dataGridView.Rows)
					{
						DataGridViewCell cell = row.Cells[info.ColumnIndex];
						if (cell != null) cell.Selected = true;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private Point mMouseLocation = Point.Empty;
		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			mMouseLocation = dataGridView.PointToClient(Control.MousePosition);
			DataGridView.HitTestInfo info = dataGridView.HitTest(mMouseLocation.X, mMouseLocation.Y);
		}

		private void toolStripMenuItemFindNext_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(LastFindText)) return;
			if (dataGridView.SelectedCells.Count != 1)
			{
				mStartingColumnIndex++;
				DoSearch(LastFindText);
			}
			else
			{
				DataGridViewCell cell = dataGridView.SelectedCells[0];
				if (cell != null)
				{
					if (cell.ColumnIndex == mStartingColumnIndex) mStartingColumnIndex++;
					else mStartingColumnIndex = cell.ColumnIndex + 1;
					mStartingRowIndex = cell.RowIndex;
					DoSearch(LastFindText);
					if (dataGridView.SelectedCells.Count <= 0) cell.Selected = true;
				}
			}
		}

		internal string GetRegistryString(string sKey, string sName, string sDefaultValue)
		{
			string sValue = sDefaultValue;
			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(sKey);
			if (regKey != null)
			{
				string sTemp = (string)regKey.GetValue(sName);
				if (sTemp != null) sValue = sTemp;
			}
			return sValue;
		}

		internal void SaveRegistryString(string sKey, string sName, string sValue)
		{
			Debug.Assert(!String.IsNullOrEmpty(sKey));
			if (String.IsNullOrEmpty(sKey)) return;

			using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(sKey))
			{
				regKey.SetValue(sName, sValue);
			}
		}

		internal bool GetRegistryBool(string sKey, string sName, bool sDefaultValue)
		{
			bool sValue = sDefaultValue;
			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(sKey);
			if (regKey != null)
			{
				bool sTemp = Convert.ToBoolean(regKey.GetValue(sName, sDefaultValue));
				sValue = sTemp;
			}
			return sValue;
		}

		internal void SaveRegistryBool(string sKey, string sName, bool sValue)
		{
			Debug.Assert(!String.IsNullOrEmpty(sKey));
			if (String.IsNullOrEmpty(sKey)) return;

			using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(sKey))
			{
				regKey.SetValue(sName, sValue);
			}
		}

		private string mLastDataTableFileName;
		private const string kXMLExtension = "xml";
		private const string kXLSExtension = "xlsx";
		private const string kCSVExtension = "csv";
		protected const string kInitialDirectoryKey = "InitialDirectory";
		protected const string kLastOpenedDirectoryKey = "LastOpenedDirectoryKey";
		protected const string kLastRaceDirectoryKey = "LastRaceDirectoryKey";
		protected const string kXMLFileFilter = "XML files (*.xml)|*.xml";
		protected const string kXLSFileFilter = "xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
		protected const string kCSVFileFilter = "CSV files (*.csv)|*.csv";
		private string mLastDirectory;
		private string mLastOpenedDirectory;
		private string mLastRaceDirectory;
		private string GetXMLFileName()
		{
			if (String.IsNullOrEmpty(mLastDataTableFileName))
			{
				string selectedText = null;
				if (cmbRaceTypes.SelectedIndex >= 0) selectedText = cmbRaceTypes.SelectedText;
				if (String.IsNullOrEmpty(selectedText)) selectedText = "Race";
				mLastDataTableFileName = selectedText;
			}
			return mLastDataTableFileName;
		}

		private string GetLastDirectory()
		{
			if (!String.IsNullOrEmpty(mLastDirectory)) return mLastDirectory;
			mLastDirectory = GetFolderPath();
			return mLastDirectory;
		}

		private string GetLastOpenedDirectory()
		{
			if (!String.IsNullOrEmpty(mLastOpenedDirectory)) return mLastOpenedDirectory;
			mLastOpenedDirectory = GetFolderPath();
			return mLastOpenedDirectory;
		}

		private string GetLastRaceDirectory()
		{
			string userPath = AgeGradingForm.GetFolderPath();
			mLastRaceDirectory = GetFolderPath();
			return mLastRaceDirectory;
		}

		private void SaveDataTable(string fileName)
		{
			DataTable dataTable = dataGridView.DataSource as DataTable;
			if (dataTable == null) return;
			SaveDataTable(dataTable, fileName, false, String.Empty, String.Empty);
		}

		private void SaveDataTable(DataTable dataTable, string fileName, bool saveImmediately, string comment, string initialDirectory)
		{
			if (dataTable == null) return;
			SaveFileDialog dlg = null;
			try
			{
				dlg = new SaveFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = false;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kXMLExtension;
				dlg.Filter = kXMLFileFilter;
				if (!String.IsNullOrEmpty(fileName))
					dlg.FileName = fileName;
				if (String.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory)) initialDirectory = GetLastDirectory();
				if (Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				// Use the user's documents directory if the passed directory doesn't exist.
				if (saveImmediately && !Directory.Exists(initialDirectory)) initialDirectory = AgeGradingForm.GetFolderPath();
				DialogResult dr = DialogResult.Cancel;
				if (!saveImmediately)
				{
					fileName = String.Empty;
					dr = dlg.ShowDialog(this);
					if (dr == System.Windows.Forms.DialogResult.OK) fileName = dlg.FileName;
				}
				else
				{
					dr = DialogResult.OK;
					fileName = String.Format("{0}{1}{2}", initialDirectory, Path.DirectorySeparatorChar, fileName);
				}
				if (dr == System.Windows.Forms.DialogResult.OK)
				{
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
					{
						xmlWriter.WriteComment(comment);
						dataTable.WriteXml(xmlWriter, XmlWriteMode.WriteSchema);
					}
					if (File.Exists(fileName))
					{
						if (!saveImmediately)
						{
							mLastDirectory = Path.GetDirectoryName(fileName);
							SaveRegistryString(kRegistryLocation, kInitialDirectoryKey, mLastDirectory);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataTable dataTable = dataGridView.DataSource as DataTable;
			if (dataTable == null) return;
			string xmlName = GetXMLFileName();
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
				xmlName = raceInfo.Name;
			string fileName = String.Format("{0}.{1}", xmlName, kXMLExtension);
			SaveDataTable(fileName);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DataTable dataTable = dataGridView.DataSource as DataTable;
			if (dataTable == null) return;
			SaveDataTable(String.Empty);
		}

		private void toolStripMenuItemOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = null;
			try
			{
				dlg = new OpenFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kXMLExtension;
				dlg.Filter = kXMLFileFilter;
				string initialDirectory = GetLastOpenedDirectory();
				if (!Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (File.Exists(dlg.FileName))
					{
						DataTable dataTable = new DataTable();
						dataTable.ReadXml(dlg.FileName);
						dataGridView.DataSource = dataTable;
						mLastOpenedDirectory = Path.GetDirectoryName(dlg.FileName);
						SaveRegistryString(kRegistryLocation, kInitialDirectoryKey, mLastOpenedDirectory);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private void CopyGrid()
		{
			if (dataGridView.Rows.Count > 0)
			{
				try
				{
					const string kSeparator = "\t";
					StringBuilder str = new StringBuilder();
					foreach (DataGridViewColumn column in dataGridView.Columns)
					{
						if (column != null)
						{
							if (str.Length > 0) str.Append(kSeparator);
							str.Append(column.HeaderText);
						}
					}
					if (str.Length > 0) str.AppendLine();
					foreach (DataGridViewRow row in dataGridView.Rows)
					{
						if (row == null) continue;
						StringBuilder temp = new StringBuilder();
						foreach (DataGridViewCell cell in row.Cells)
						{
							if (cell != null)
							{
								if (temp.Length > 0) temp.Append(kSeparator);
								string text = (cell.Value != null ? cell.Value.ToString() : String.Empty);
								temp.Append(text);
							}
						}
						if (temp.Length > 0)
						{
							str.AppendLine(temp.ToString());
						}
					}
					Clipboard.SetText(str.ToString());
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyGrid();
		}

		private void toolStripMenuItemRTCopy_Click(object sender, EventArgs e)
		{
			txtRaceResults.Copy();
		}

		private void toolStripMenuItemRTPaste_Click(object sender, EventArgs e)
		{
			txtRaceResults.Paste();
		}

		private void toolStripMenuItemRTCut_Click(object sender, EventArgs e)
		{
			txtRaceResults.Cut();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == (Keys.C | Keys.Control))
			{
				if (txtRaceResults.ContainsFocus)
				{
					txtRaceResults.Copy();
					return true;
				}
				else
				{
					CopyGrid();
					return true;
				}
			}
			if (keyData == (Keys.L | Keys.Control | Keys.Alt))
			{
				try
				{
					RaceInfo raceInfo = GetKQRaceSelection();
					string xmlName = GetXMLFileName();
					if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
						xmlName = raceInfo.Name;
					string raceDirectory = GetLastRaceDirectory();
					string fileName = String.Format("{0}{1}{2}.{3}", raceDirectory, Path.DirectorySeparatorChar, xmlName, kXMLExtension);
					if (File.Exists(fileName))
					{
						DataTable dataTable = OpenResults(fileName);
						if (dataTable != null)
						{
							AddOrUpdateResults(dataTable);
						}
					}
					return true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
			return base.ProcessDialogKey(keyData);
		}

		private static string FormatTimeSpan(TimeSpan span, bool showSign)
		{
			string sign = String.Empty;
			if (showSign && (span > TimeSpan.Zero))
				sign = "+";
			StringBuilder str = new StringBuilder();
			str.Insert(0, span.Seconds.ToString("00"));
			str.Insert(0, ":");
			str.Insert(0, span.Minutes.ToString("00"));
			if (span.Hours > 0)
			{
				str.Insert(0, ":");
				str.Insert(0, span.Hours.ToString("00"));
			}
			return str.ToString();
		}

		private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (!(e.Value is TimeSpan) || e.DesiredType != typeof(string)) return;
			TimeSpan time = (TimeSpan)e.Value;
			e.Value = FormatTimeSpan(time, false);
		}

		private void toolStripMenuItemMainSave_Click(object sender, EventArgs e)
		{
			saveToolStripMenuItem_Click(sender, e);
		}

		private void toolStripMenuItemMainSaveAs_Click(object sender, EventArgs e)
		{
			saveAsToolStripMenuItem_Click(sender, e);
		}

		private void toolStripMenuItemMainOpen_Click(object sender, EventArgs e)
		{
			toolStripMenuItemOpen_Click(sender, e);
		}

		private void toolStripMenuItemMainFind_Click(object sender, EventArgs e)
		{
			toolStripMenuItemFind_Click(sender, e);
		}

		private void toolStripMenuItemMainFindNext_Click(object sender, EventArgs e)
		{
			toolStripMenuItemFindNext_Click(sender, e);
		}

		private void toolStripMenuItemMainCopy_Click(object sender, EventArgs e)
		{
			if (Control.Equals(mLastFocused, txtRaceResults))
				toolStripMenuItemRTCopy_Click(sender, e);
			else copyToolStripMenuItem_Click(sender, e);
		}

		private Control mLastFocused;
		private void txtRaceResults_Enter(object sender, EventArgs e)
		{
			mLastFocused = txtRaceResults;
		}

		private void dataGridView_Enter(object sender, EventArgs e)
		{
			mLastFocused = dataGridView;
		}

		private string mMemberTableName;
		protected string MemberTableName
		{
			get
			{
				if (mMemberTableName == null)
					mMemberTableName = GetRegistryString(kRegistryLocation, kMemberTableKey, String.Empty);
				return mMemberTableName;
			}
			set
			{
				if (value == null) return;
				mMemberTableName = value;
				SaveRegistryString(kRegistryLocation, kMemberTableKey, value);
			}
		}

		private string GetMemberTableName()
		{
			if (mMemberTableName == null)
			{
				mMemberTableName = GetRegistryString(kRegistryLocation, kMemberTableKey, String.Empty);
			}
			return mMemberTableName;
		}


		private void toolStripMenuItemSetMemberTable_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = null;
			try
			{
				dlg = new OpenFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kXMLExtension;
				dlg.Filter = kXMLFileFilter;
				string initialDirectory = GetLastOpenedDirectory();
				string memberTableName = MemberTableName;
				string fileName = null;
				if (File.Exists(memberTableName))
				{
					initialDirectory = Path.GetDirectoryName(MemberTableName);
					fileName = Path.GetFileName(memberTableName);
				}
				if (!Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				if (!String.IsNullOrEmpty(fileName)) dlg.FileName = fileName;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (File.Exists(dlg.FileName))
					{
						DataTable dataTable = new DataTable();
						dataTable.ReadXml(dlg.FileName);
						dataGridView.DataSource = dataTable;
						mLastOpenedDirectory = Path.GetDirectoryName(dlg.FileName);
						SaveRegistryString(kRegistryLocation, kInitialDirectoryKey, mLastOpenedDirectory);
						MemberTableName = dlg.FileName;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private void ConvertCSVToXML()
		{
			OpenFileDialog dlg = null;
			try
			{
				dlg = new OpenFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kCSVExtension;
				dlg.Filter = kCSVFileFilter;
				string initialDirectory = GetLastOpenedDirectory();
				if (!Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (File.Exists(dlg.FileName))
					{
						string columnList = ",";
						List<string> columns = new List<string>();
						AgeGradeTextReader textReader = new AgeGradeTextReader();
						List<List<string>> rowColumns = textReader.GetDelimitedLines(dlg.FileName, 0, 1, columnList);
						columns = textReader.ColumnHeadings;
						if (rowColumns != null && rowColumns.Count > 0 && columns != null && columns.Count > 0)
						{
							string fileName = Path.GetFileName(dlg.FileName);
							using (DataTable dataTable = new DataTable(fileName))
							{
								int columnCount = 0;
								foreach (string column in columns)
								{
									string temp = column.Trim();
									if (String.IsNullOrEmpty(temp)) continue;
									columnCount++;
									AddDataColumn(dataTable, column, typeof(string));
								}

								for (int ii = 0; ii < rowColumns.Count; ii++)
								{
									//if (rowColumns[ii].Count != dataTable.Columns.Count) throw new ArgumentException("The count of items in the row doesn't equal the column count");
									try
									{
										DataRow dataRow = dataTable.NewRow();
										int count = Math.Min(columnCount, rowColumns[ii].Count);
										List<string> line = rowColumns[ii];
										for (int jj = 0; jj < count; jj++)
										{
											string column = columns[jj];
											string item = line[jj];
											dataRow[column] = item;
										}
										dataTable.Rows.Add(dataRow);
									}
									catch (Exception ex)
									{
										Debug.WriteLine(ex.ToString());
										Debug.Assert(false, ex.ToString());
									}
								}
								string xmlFileName = dlg.FileName.Replace(kCSVExtension, kXMLExtension);
								dataTable.WriteXml(xmlFileName, XmlWriteMode.WriteSchema);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private string GetCSVFile(string xlsPath)
		{
			if (String.IsNullOrWhiteSpace(xlsPath)) return String.Empty;
			if (!File.Exists(xlsPath)) return String.Empty;
			string csvPath = String.Empty;
			Microsoft.Office.Interop.Excel.Application app = null;
			Microsoft.Office.Interop.Excel.Workbook excelWorkbook = null;
			try
			{
				app = new Microsoft.Office.Interop.Excel.Application();

				// While saving, it asks for the user confirmation, whether we want to save or not.
				// By setting DisplayAlerts to false, we just skip this alert.
				app.DisplayAlerts = false;

				// Now we open the upload file in Excel Workbook. 
				excelWorkbook = app.Workbooks.Open(xlsPath);
				string directory = Path.GetDirectoryName(xlsPath);
				string fileName = Path.GetFileNameWithoutExtension(xlsPath);
				string newFileName = Path.Combine(directory, $"{fileName}.{kCSVExtension}");
				// Now save this file as CSV file.
				excelWorkbook.SaveAs(newFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV);
				csvPath = newFileName;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (excelWorkbook != null)
				{
					excelWorkbook.Close();
					excelWorkbook = null;
				}
				if (app != null)
				{
					// Close the Workbook and Quit the Excel Application at the end. 
					app.Quit();
					app = null;
				}
			}
			return csvPath;
		}

		private void ConvertExcelToXML()
		{
			OpenFileDialog dlg = null;
			try
			{
				dlg = new OpenFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kXLSExtension;
				dlg.Filter = kXLSFileFilter;
				string initialDirectory = GetLastOpenedDirectory();
				if (!Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string path = GetCSVFile(dlg.FileName);
					if (!String.IsNullOrWhiteSpace(path) && File.Exists(path))
					{
						string columnList = ",";
						List<string> columns = new List<string>();
						AgeGradeTextReader textReader = new AgeGradeTextReader();
						List<List<string>> rowColumns = textReader.GetDelimitedLines(path, 0, 1, columnList);
						columns = textReader.ColumnHeadings;
						if (rowColumns != null && rowColumns.Count > 0 && columns != null && columns.Count > 0)
						{
							string fileName = Path.GetFileName(path);
							using (DataTable dataTable = new DataTable(fileName))
							{
								int columnCount = 0;
								foreach (string column in columns)
								{
									string temp = column.Trim();
									if (String.IsNullOrEmpty(temp)) continue;
									columnCount++;
									AddDataColumn(dataTable, column, typeof(string));
								}

								for (int ii = 0; ii < rowColumns.Count; ii++)
								{
									//if (rowColumns[ii].Count != dataTable.Columns.Count) throw new ArgumentException("The count of items in the row doesn't equal the column count");
									try
									{
										DataRow dataRow = dataTable.NewRow();
										int count = Math.Min(columnCount, rowColumns[ii].Count);
										List<string> line = rowColumns[ii];
										for (int jj = 0; jj < count; jj++)
										{
											string column = columns[jj];
											string item = line[jj];
											dataRow[column] = item;
										}
										dataTable.Rows.Add(dataRow);
									}
									catch (Exception ex)
									{
										Debug.WriteLine(ex.ToString());
										Debug.Assert(false, ex.ToString());
									}
								}
								string xmlFileName = path.Replace(kCSVExtension, kXMLExtension);
								dataTable.WriteXml(xmlFileName, XmlWriteMode.WriteSchema);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private void convertCSVToXMLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ConvertExcelToXML();
		}

		private void AddOrUpdateResults(DataTable dataTable)
		{
			if (KQCompetitionData == null) return;
			if (dataTable == null) return;
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo == null) return;
			KQCompetitionData.AddOrUpdateMemberInfo(MemberTableName);
			string nameColumn = GetComboBoxSelection(cmbName);
			string ageColumn = GetComboBoxSelection(cmbAge);
			string genderColumn = GetComboBoxSelection(cmbGender);
			string raceTimeColumn = GetComboBoxSelection(cmbTime);
			string firstNameColumn = GetFirstNameColumn();
			if (String.IsNullOrEmpty(firstNameColumn)) firstNameColumn = nameColumn;
			List<KQCompetitionData.MemberInfo> missingNames = KQCompetitionData.AddOrUpdateResults(dataTable, raceInfo, MemberTableName, nameColumn, firstNameColumn, ageColumn, genderColumn,
				raceTimeColumn, kAgeAdjustedColumnName, kNormalizedTimeColumnName, kWorldPercentColumnName, chkOnlyCompletingRequirements.Checked);
			if (missingNames != null && missingNames.Count > 0)
			{
				string fileName = GetMissingMemberFileName(raceInfo);
				if (String.IsNullOrEmpty(fileName)) return;
				using (TextWriter writer = File.CreateText(fileName))
				{
					writer.WriteLine(raceInfo.Name);
					foreach (KQCompetitionData.MemberInfo item in missingNames)
					{
						if (item != null) writer.WriteLine(item.ToString());
					}
					btnCompareMissingMembers.Enabled = true;
				}
			}
		}

		private string GetMissingMemberFileName(RaceInfo raceInfo)
		{
			string raceDirectory = GetLastRaceDirectory();
			if (String.IsNullOrEmpty(raceDirectory) || !Directory.Exists(raceDirectory)) return String.Empty;
			string fileName = String.Format("{0}{1}{2}.txt", raceDirectory, Path.DirectorySeparatorChar, raceInfo.Name);
			return fileName;
		}

		private void addorUpdateResultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddOrUpdateResults(mDataTable);
#if USE_OLD_METHOD
			if (KQCompetitionData == null) return;
			if (mDataTable == null) return;
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo == null) return;
			KQCompetitionData.AddOrUpdateMemberInfo(MemberTableName);
			string nameColumn = GetComboBoxSelection(cmbName);
			string ageColumn = GetComboBoxSelection(cmbAge);
			string genderColumn = GetComboBoxSelection(cmbGender);
			string firstName = null;
			string lastName = null;
			string raceTimeColumn = GetComboBoxSelection(cmbTime);
			foreach (DataRow row in mDataTable.Rows)
			{
				if (row == null) continue;
				string name = row[nameColumn].ToString();
				if (!SplitNames(name, out firstName, out lastName)) continue;
				string ageString = row[ageColumn].ToString();
				string gender = row[genderColumn].ToString();
				DataRow foundRow = KQCompetitionData.FindFinisher(firstName, lastName, gender, ageString);
				if (!KQCompetitionData.ValidMember(firstName, lastName, gender, raceInfo.RaceDate)) continue;
				if (foundRow != null)
				{
					UpdateRaceInfo(raceInfo, row, foundRow, raceTimeColumn);
					//Debug.WriteLine(String.Format("firstName='{0}' lastName='{1}' gender='{2}' ageString='{3}'", firstName, lastName, gender, ageString));
				}
				else
				{
					Debug.WriteLine(String.Format("EMPTY ROW:: firstName='{0}' lastName='{1}' gender='{2}' ageString='{3}'", firstName, lastName, gender, ageString));
				}
			}
			KQCompetitionData.SaveTable();
			KQCompetitionData.BuildKQResults();
#endif
		}

		private void UpdateRaceInfo(RaceInfo raceInfo, DataRow row, DataRow foundRow, string raceTimeColumn)
		{
			if (raceInfo == null || row == null || foundRow == null) return;
			try
			{
				if (String.IsNullOrEmpty(raceTimeColumn))
				{
					Debug.Assert(false, "Invalid column name");
					return;
				}
				object value = row[raceTimeColumn];
				string raceTime = (value != null ? value.ToString() : String.Empty);
				value = row[kNormalizedTimeColumnName];
				string normalizedTime = (value != null ? value.ToString() : String.Empty);
				value = row[kWorldPercentColumnName];
				string worldPercentage = (value != null ? value.ToString() : String.Empty);
				string race = KQCompetitionData.BuildRaceString(raceTime, normalizedTime, worldPercentage);
				foundRow[raceInfo.Name] = race;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void cmbKQRace_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (InitializingKQRaces) return;
			KQRace = GetComboBoxSelection(cmbKQRace);
			SaveRegistryString(kRegistryLocation, kKQRaceKey, KQRace);
			SetRaceToolTip();
		}

		private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			try
			{
				foreach (DataGridViewColumn column in dataGridView.Columns)
				{
					if (column == null) continue;
					column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private string GetComment(XmlReader reader)
		{
			if (reader == null) return String.Empty;
			try
			{
				string comment = String.Empty;
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Attribute:
							break;
						case XmlNodeType.CDATA:
							break;
						case XmlNodeType.Comment:
							comment = reader.Value;
							break;
						case XmlNodeType.Document:
							break;
						case XmlNodeType.DocumentFragment:
							break;
						case XmlNodeType.DocumentType:
							break;
						case XmlNodeType.Element:
							break;
						case XmlNodeType.EndElement:
							break;
						case XmlNodeType.EndEntity:
							break;
						case XmlNodeType.Entity:
							break;
						case XmlNodeType.EntityReference:
							break;
						case XmlNodeType.None:
							break;
						case XmlNodeType.Notation:
							break;
						case XmlNodeType.ProcessingInstruction:
							break;
						case XmlNodeType.SignificantWhitespace:
							break;
						case XmlNodeType.Text:
							break;
						case XmlNodeType.Whitespace:
							break;
						case XmlNodeType.XmlDeclaration:
							break;
						default:
							break;
					}
					if (!String.IsNullOrEmpty(comment)) return comment;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			return String.Empty;
		}

		private DataTable OpenResults(string theFileName)
		{
			DataTable dataTable = null;
			if (File.Exists(theFileName))
			{
				dataTable = new DataTable();
				string comment = String.Empty;
				using (XmlReader xmlReader = XmlReader.Create(theFileName))
				{
					comment = GetComment(xmlReader);
					dataTable.ReadXml(theFileName);
				}
				dataGridView.DataSource = dataTable;
				mLastRaceDirectory = Path.GetDirectoryName(theFileName);
				SaveRegistryString(kRegistryLocation, kLastRaceDirectoryKey, mLastRaceDirectory);
				string fileName = Path.GetFileNameWithoutExtension(theFileName);
				SetRacesCombo(fileName);
				if (!String.IsNullOrEmpty(comment)) RestoreCombos(comment);
				RestartAgeGroup = true;
			}
			return dataTable;
		}

		private void RerunResults()
		{
			try
			{
				List<RaceInfo> races = RaceInfo.GetUpToDateRaces();
				if (races == null || races.Count <= 0) return;
				string lastRaceDirectory = GetLastRaceDirectory();
				if (!Directory.Exists(lastRaceDirectory)) return;
				foreach (RaceInfo raceInfo in races)
				{
					string xmlName = String.Empty;
					if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
						xmlName = raceInfo.Name;
					string fileName = String.Format("{0}{1}{2}.xml", lastRaceDirectory, Path.DirectorySeparatorChar, xmlName);
					if (File.Exists(fileName))
					{
						DataTable dataTable = OpenResults(fileName);
						if (dataTable != null)
						{
							AddOrUpdateResults(dataTable);
						}
					}
				}
				btnCompareMissingMembers.Enabled = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void toolStripMenuItemOpenRaceResults_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = null;
			try
			{
				dlg = new OpenFileDialog();
				dlg.AddExtension = false;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = kXMLExtension;
				dlg.Filter = kXMLFileFilter;
				string raceFileName = GetRaceFileName();
				if (!String.IsNullOrEmpty(raceFileName)) dlg.FileName = raceFileName;
				string initialDirectory = GetLastRaceDirectory();
				if (!Directory.Exists(initialDirectory)) initialDirectory = GetLastOpenedDirectory();
				if (Directory.Exists(initialDirectory)) dlg.InitialDirectory = initialDirectory;
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (File.Exists(dlg.FileName))
					{
						DataTable dataTable = OpenResults(dlg.FileName);
						if (dataTable != null)
						{
							mDataTable = dataTable;
							btnCompareMissingMembers.Enabled = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				if (dlg != null)
				{
					dlg.Dispose();
					dlg = null;
				}
			}
		}

		private const char kComboRestoreDelimiter = ';';
		private string GetComboRestoreString(DataTable dataTable)
		{
			string age = GetComboBoxSelection(cmbAge);
			string gender = GetComboBoxSelection(cmbGender);
			string time = GetComboBoxSelection(cmbTime);
			string name = GetComboBoxSelection(cmbName);
			string firstName = GetComboBoxSelection(cmbFirstName);
			StringBuilder str = new StringBuilder();
			str.Append(age);
			str.Append(kComboRestoreDelimiter);
			str.Append(gender);
			str.Append(kComboRestoreDelimiter);
			str.Append(time);
			str.Append(kComboRestoreDelimiter);
			str.Append(name);
			str.Append(kComboRestoreDelimiter);
			if (dataTable != null && dataTable.Columns.Contains(firstName)) str.Append(firstName);
			else str.Append(kDefaultFirstName);
			return str.ToString();
		}

		private string GetFirstNameColumn()
		{
			string firstName = GetComboBoxSelection(cmbFirstName);
			if (String.Equals(firstName, kDefaultFirstName, StringComparison.OrdinalIgnoreCase))
				return String.Empty;
			return firstName;
		}

		private void RestoreCombos(string str)
		{
			string[] split = str.Split(kComboRestoreDelimiter);
			if (split == null || split.Length != 5) return;
			SetCombo(cmbAge, split[0], true);
			SetCombo(cmbGender, split[1], true);
			SetCombo(cmbTime, split[2], true);
			SetCombo(cmbName, split[3], true);
			if (!SetCombo(cmbFirstName, split[4], true) && cmbFirstName.Items.Count > 0)
				cmbFirstName.SelectedIndex = 0;
		}

		private string GetRaceFileName()
		{
			string xmlName = String.Empty;
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
			{
				xmlName = raceInfo.Name;
				xmlName = String.Format("{0}.{1}", xmlName, kXMLExtension);
			}
			return xmlName;
		}

		private void toolStripMenuItemSaveRaceResults_Click(object sender, EventArgs e)
		{
			SaveRaceResults();
		}

		private void SaveRaceResults()
		{
			DataTable dataTable = dataGridView.DataSource as DataTable;
			if (dataTable == null) return;
			try
			{
				string comboRestoreString = GetComboRestoreString(dataTable);
				string comment = comboRestoreString;
				string xmlName = GetXMLFileName();
				RaceInfo raceInfo = GetKQRaceSelection();
				if (raceInfo != null && !String.IsNullOrEmpty(raceInfo.Name))
					xmlName = raceInfo.Name;
				string fileName = String.Format("{0}.{1}", xmlName, kXMLExtension);
				SaveDataTable(dataTable, fileName, true, comment, GetLastRaceDirectory());
				btnCompareMissingMembers.Enabled = (!String.IsNullOrEmpty(fileName) && File.Exists(fileName));
				LoadRacesCombo(true);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		private void rerunResultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RerunResults();
		}

		private int mLastAgeGroup;
		private int mLastStartingRow;
		private void btnNextAgeGroup_Click(object sender, EventArgs e)
		{
			if (dataGridView.Rows.Count <= 0) return;
			// Sort the list by age.
			string ageColumn = GetComboBoxSelection(cmbAge);
			if (String.IsNullOrEmpty(ageColumn)) return;
			if (RestartAgeGroup)
			{
				DataGridViewColumn column = dataGridView.Columns[ageColumn];
				if (column == null) return;
				dataGridView.Sort(column, ListSortDirection.Ascending);
				mLastAgeGroup = Convert.ToInt32(numAgeGroup.Value);
				mLastStartingRow = 0;
				RestartAgeGroup = false;
			}
			dataGridView.ClearSelection();

			int highestAge = int.MinValue;
			while (true)
			{
				if (mLastStartingRow >= dataGridView.Rows.Count) break;
				for (int ii = mLastStartingRow; ii < dataGridView.Rows.Count; ii++)
				{
					DataGridViewRow row = dataGridView.Rows[ii];
					if (row == null) continue;
					DataGridViewCell cell = row.Cells[ageColumn];
					if (cell == null) continue;
					int age = (int)cell.Value;
					if (age > highestAge) highestAge = age;
					if (age == mLastAgeGroup)
					{
						if (dataGridView.SelectedCells.Count <= 0) dataGridView.FirstDisplayedScrollingRowIndex = row.Index;
						cell.Selected = true;
						mLastStartingRow = ii;
					}
				}
				mLastAgeGroup++;
				if (mLastAgeGroup > highestAge) break;
				if (dataGridView.SelectedCells.Count > 0) break;
			}
		}

		private void numAgeGroup_ValueChanged(object sender, EventArgs e)
		{
			RestartAgeGroup = true;
		}

		private void btnCompareMissingMembers_Click(object sender, EventArgs e)
		{
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo == null)
			{
				MessageBox.Show(this, "K&Q Race info is invalid", "Error");
				return;
			}
			string fileName = GetMissingMemberFileName(raceInfo);
			if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
			{
				MessageBox.Show(this, "Invalid or non-existent Missing Member Names File", "Error");
				return;
			}
			DataTable dataTable = dataGridView.DataSource as DataTable;
			if (dataTable == null)
			{
				MessageBox.Show(this, "Current data table is invalid or empty", "Error");
				return;
			}
			using (CompareMissingMembersDlg dlg = new CompareMissingMembersDlg())
			{
				dlg.MissingMembersFile = fileName;
				dlg.AgeColumnName = GetComboBoxSelection(cmbAge);
				dlg.NameColumnName = GetComboBoxSelection(cmbName);
				dlg.AgeGradedResults = dataTable;
				dlg.ShowDialog(this);
			}
		}

		private void btnMemberAges_Click(object sender, EventArgs e)
		{
			if (mDataTable == null) return;
			GetNameList();
		}

		private List<RaceNames> GetNameList()
		{
			if (mDataTable == null || mDataTable.Rows.Count <= 0 || mDataTable.Columns.Count <= 0) return null;
			if (KQCompetitionData == null) return null;
			RaceInfo raceInfo = GetKQRaceSelection();
			if (raceInfo == null) return null;
			KQCompetitionData.AddOrUpdateMemberInfo(MemberTableName);
			string nameColumn = GetComboBoxSelection(cmbName);
			string ageColumn = GetComboBoxSelection(cmbAge);
			string genderColumn = GetComboBoxSelection(cmbGender);
			string raceTimeColumn = GetComboBoxSelection(cmbTime);
			string firstNameColumn = GetFirstNameColumn();
			if (String.IsNullOrEmpty(firstNameColumn)) firstNameColumn = nameColumn;
			List<RaceNames> foundMembers = KQCompetitionData.FindMatchingMemberInfo(mDataTable, raceInfo, MemberTableName, nameColumn, firstNameColumn, ageColumn, genderColumn);
			if (foundMembers != null)
			{
				foreach (RaceNames raceName in foundMembers)
				{
					string temp  = raceName.DataRow[raceTimeColumn].ToString();
					TimeSpan time;
					KQCompetitionData.GetTimeSpan(temp, out time);
					raceName.Time = time;
				}
				foundMembers.Sort(new RaceNamesComparer());
				foreach (RaceNames raceName in foundMembers)
				{
					Debug.WriteLine(String.Format("{0} {1}", raceName.LastName, raceName.Time));
				}
				foundMembers.Sort(new RaceNamesComparer());
				StringBuilder str = new StringBuilder();
				str.Append(String.Format("{0}\t", nameColumn));
				str.Append(String.Format("{0}\t", ageColumn));
				str.Append(String.Format("{0}\t", genderColumn));
				str.Append(String.Format("{0}", raceTimeColumn));
				str.AppendLine();
				Decimal value = numStartingLineNumber.Value;
				value -= 1;
				while (value > 0)
				{
					str.AppendLine();
					value -= 1;
				}
				
				foreach (RaceNames raceName in foundMembers)
				{
					str.Append(String.Format("{0} {1}\t", raceName.FirstName, raceName.LastName));
					str.Append(String.Format("{0}\t", raceName.Age));
					str.Append(String.Format("{0}\t", raceName.Gender));
					str.Append(String.Format("{0}", raceName.DataRow[raceTimeColumn]));
					str.AppendLine();
				}
				txtRaceResults.Text = str.ToString();
				txtRaceResults.SelectionStart = 0;
				txtRaceResults.SelectionLength = 0;
				txtRaceResults.ScrollToCaret();
				radioUseTabSeparatedColumns.Checked = true;
				btnProcessColumn.PerformClick();
				btnCalculateAgeGrade.PerformClick();
			}
			return null;
		}

		internal class RaceNames
		{
			private string mFirstName;
			public string FirstName
			{
				get { return mFirstName; }
				set { mFirstName = value; }
			}

			private DataRow mDataRow;
			public DataRow DataRow
			{
				get { return mDataRow; }
				set { mDataRow = value; }
			}

			private string mLastName;
			public string LastName
			{
				get { return mLastName; }
				set { mLastName = value; }
			}

			private string mGender;
			public string Gender
			{
				get { return mGender; }
				set { mGender = value; }
			}

			private int mAge;
			public int Age
			{
				get { return mAge; }
				set { mAge = value; }
			}

			private TimeSpan mTime;
			public TimeSpan Time
			{
				get { return mTime; }
				set { mTime = value; }
			}

			public RaceNames(string firstName, string lastName)
			{
				FirstName = firstName;
				LastName = lastName;
			}
		}

		internal class RaceNamesComparer : IComparer<RaceNames>
		{
			private bool mByName;
			public bool ByName
			{
				get { return mByName; }
				set { mByName = value; }
			}

			public RaceNamesComparer()
			{
			}

			public RaceNamesComparer(bool byName)
			{
				this.ByName = byName;
			}

			public int Compare(RaceNames x, RaceNames y)
			{
				if (x == null && y == null) return 0;
				if (x == null) return -1;
				if (y == null) return 1;
				int result = 0;
				result = TimeSpan.Compare(x.Time, y.Time);
				if (result != 0) return result;
				if (!ByName)
				{
					result = x.Age - y.Age;
					if (result != 0) return result;
				}
				result = String.CompareOrdinal(x.LastName, y.LastName);
				if (result != 0) return result;
				result = String.CompareOrdinal(x.FirstName, y.FirstName);
				if (result != 0) return result;
				if (ByName)
				{
					result = x.Age - y.Age;
					if (result != 0) return result;
				}
				return result;
			}
		}

		private void chkOnlyCompletingRequirements_CheckedChanged(object sender, EventArgs e)
		{
			SaveRegistryBool(kRegistryLocation, kCompletedRequirementsKey, chkOnlyCompletingRequirements.Checked);
			if (KQCompetitionData != null) KQCompetitionData.ClearVolunteerPoints();
		}

		private string GetSecondLine(string field)
		{
			if (String.IsNullOrEmpty(field)) return String.Empty;
			StringBuilder str = new StringBuilder();
			foreach (char item in field)
			{
				str.Append('=');
			}
			return str.ToString();
		}

		private void btnCombineFirstAndLastNames_Click(object sender, EventArgs e)
		{
			if (mRowColumns == null || mRowColumns.Count <= 0) return;
			if (mColumns == null) return;
			try
			{
				string firstNameColumn = GetComboBoxSelection(cmbFirstName);
				string nameColumn = GetComboBoxSelection(cmbName);
				if (String.Equals(DefaultFirstName, kDefaultFirstName, StringComparison.OrdinalIgnoreCase)) return;
				StringBuilder str = new StringBuilder();
				StringBuilder secondLine = new StringBuilder();
				int nameIndex = -1;
				int firstNameIndex = -1;
				for (int ii = 0; ii < mColumns.Count; ii++)
				{
					string column = mColumns[ii];
					if (String.Equals(column, firstNameColumn, StringComparison.OrdinalIgnoreCase))
					{
						firstNameIndex = ii;
						continue;
					}
					if (String.Equals(column, nameColumn, StringComparison.OrdinalIgnoreCase)) nameIndex = ii;
					if (str.Length > 0) str.Append('\t');
					str.Append(column);
					if (secondLine.Length > 0) secondLine.Append('\t');
					secondLine.Append(GetSecondLine(column));
				}
				if (firstNameIndex < 0) return;
				StringBuilder fullText = new StringBuilder();
				fullText.AppendLine(str.ToString());
				if (StartingLineNumber == 2) fullText.AppendLine(secondLine.ToString());
				for (int ii = 0; ii < mRowColumns.Count; ii++)
				{
					str = new StringBuilder();
					List<string> row = mRowColumns[ii];
					if (row == null || row.Count <= 0) continue;
					string firstName = row[firstNameIndex];
					for (int jj = 0; jj < row.Count; jj++)
					{
						string field = row[jj];
						if (firstNameIndex == jj) continue;
						if (nameIndex == jj) field = String.Format("{0} {1}", firstName, field);
						if (str.Length > 0) str.Append('\t');
						str.Append(field);
					}
					fullText.AppendLine(str.ToString());
				}
				fullText.AppendLine(str.ToString());
				txtRaceResults.Text = fullText.ToString();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				SetCombo(cmbFirstName, kDefaultFirstName, false);
			}
		}

		private void btnAddSelectedRace_Click(object sender, EventArgs e)
		{
			string raceFileName = GetRaceFileName();
			string path = GetFolderPath();
			string racePath = Path.Combine(path, raceFileName);
			if (File.Exists(racePath))
			{
				DataTable dataTable = OpenResults(racePath);
				if (dataTable != null)
				{
					mDataTable = dataTable;
					btnCompareMissingMembers.Enabled = true;
				}
			}
			else
			{
				MessageBox.Show(this, String.Format("Race file ({0}) doesn't exist", racePath), "Error: Race File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSaveSelectedRace_Click(object sender, EventArgs e)
		{
			SaveRaceResults();
		}

		public class TableColumn
		{
			private int _MaximumWidth;
			public int MaximumWidth
			{
				get { return _MaximumWidth; }
				set { _MaximumWidth = value; }
			}

			private List<string> _Contents;
			public List<string> Contents
			{
				get { return _Contents; }
				private set { _Contents = value; }
			}

			public void AddContent(string content)
			{
				if (content == null) content = String.Empty;
				content = content.Trim();
				if (_Contents == null) _Contents = new List<string>();
				Contents.Add(content);
			}

			private string _Name;
			public string Name
			{
				get { return _Name; }
				set { _Name = value; }
			}

			public TableColumn(string name)
			{
				this.Name = name;
			}
		}

		private HashSet<string> _columnsToSkip;
		private bool SkipColumn(string name)
		{
			if (String.IsNullOrWhiteSpace(name)) return true;
			if (_columnsToSkip == null)
			{
				_columnsToSkip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
					kAgeAdjustedColumnName,
					kNormalizedTimeColumnName,
					kWorldPercentColumnName,
				};
			}
			return _columnsToSkip.Contains(name);
		}

		private string PadWithSpaces(string text, int padCount, char padCharacter)
		{
			if (text == null) text = String.Empty;
			int count = padCount - text.Length;
			if (count > 0)
			{
				for (int ii = 0; ii < count; ii++)
				{
					text += padCharacter;
				}
			}
			return text;
		}

		private void saveRawResultsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string raceFileName = GetRaceFileName();
			string path = GetFolderPath();
			string racePath = Path.Combine(path, raceFileName);
			if (File.Exists(racePath))
			{
				DataTable dataTable = OpenResults(racePath);
				if (dataTable != null)
				{
					List<TableColumn> tableColumns = new List<TableColumn>();
					for (int ii = 0; ii < dataTable.Columns.Count; ii++)
					{
						DataColumn column = dataTable.Columns[ii];
						if (column == null) continue;
						if (SkipColumn(column.Caption)) continue;

						TableColumn tableColumn = new TableColumn(column.Caption);
						tableColumn.MaximumWidth = column.Caption.Length;
						for (int jj = 0; jj < dataTable.Rows.Count; jj++)
						{
							DataRow row = dataTable.Rows[jj];
							if (row == null) continue;
							object value = row.ItemArray[ii];
							if (value == null) value = String.Empty;
							string content = value.ToString();
							if (content.Length > tableColumn.MaximumWidth)
								tableColumn.MaximumWidth = content.Length;
							tableColumn.AddContent(content);
						}
						tableColumns.Add(tableColumn);
					}
					string directory = Path.GetDirectoryName(racePath);
					string nameWithoutExtension = Path.GetFileNameWithoutExtension(racePath) + "Results" + ".txt";
					string resultName = Path.Combine(directory, nameWithoutExtension);
					int rowCount = tableColumns[0].Contents.Count;
					List<StringBuilder> lines = new List<StringBuilder>();
					for (int ii = 0; ii < rowCount; ii++)
					{
						lines.Add(new StringBuilder());
					}
					for (int ii = 0; ii < rowCount; ii++)
					{
						StringBuilder cell = lines[ii];
						foreach (var item in tableColumns)
						{
							string columnValue = item.Contents[ii];
							cell.Append(PadWithSpaces(columnValue, item.MaximumWidth + 1, ' '));
						}
					}

					StringBuilder firstRow = new StringBuilder();
					StringBuilder secondRow = new StringBuilder();
					foreach (var item in tableColumns)
					{
						firstRow.Append(PadWithSpaces(item.Name, item.MaximumWidth + 1, ' '));
						string delimiters = PadWithSpaces(String.Empty, item.MaximumWidth, '=');
						secondRow.Append(PadWithSpaces(delimiters, item.MaximumWidth + 1, ' '));
					}
					lines.Insert(0, secondRow);
					lines.Insert(0, firstRow);
					using (TextWriter writer = File.CreateText(resultName))
					{
						foreach (StringBuilder item in lines)
						{
							if (item != null) writer.WriteLine(item.ToString());
						}
					}
				}
			}
			else
			{
				MessageBox.Show(this, String.Format("Race file ({0}) doesn't exist", racePath), "Error: Race File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AgeGradingForm_DragDrop(object sender, DragEventArgs e)
		{
			string url = string.Empty;
			if (e.Data.GetDataPresent("HTML Format"))
			{
				url = e.Data.GetData(typeof(string)) as string;
			}
			if (!String.IsNullOrWhiteSpace(url))
			{
				LoadHtml(url);
			}
		}

		private void AgeGradingForm_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("HTML Format"))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void LoadHtml(string url)
		{
			HtmlWeb htmlWeb = new HtmlWeb();

			// Creates an HtmlDocument object from an URL

			try
			{
				HtmlAgilityPack.HtmlDocument document = htmlWeb.Load(url);
				HtmlNode article = document.DocumentNode.SelectSingleNode("//article");
				if (article == null) return;
				HtmlNode title = article.SelectSingleNode("//h2");
				RaceInfo raceInfo = FindMatchingRace(title.InnerText);
				SetToRace(raceInfo);
				HtmlNode raceResults = document.DocumentNode.SelectSingleNode("//pre");
				if (raceResults != null)
				{
					StringBuilder str = new StringBuilder();
					string results = raceResults.InnerText;
					string[] lines = results.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					foreach (var item in lines)
					{
						str.AppendLine(item);
					}
					txtRaceResults.Text = str.ToString();
				}
				if (raceInfo == null)
				{
					using (var bgw = new BackgroundWorker())
					{
						bgw.DoWork += (_, __) =>
						{
							  Thread.Sleep(500);
						};
						bgw.RunWorkerCompleted += (_, __) =>
						{
							MessageBox.Show(this, $"Failed to find matching race for '{title.InnerText}'", "Error: Unknown Race", MessageBoxButtons.OK, MessageBoxIcon.Error);
						};
						bgw.RunWorkerAsync();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, $"Failed to open URL link: {ex.Message}", "Error: Html Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
