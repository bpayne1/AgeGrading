using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace AgeGrading
{
    public class KQCompetitionData
    {
        public KQCompetitionData()
        {
            try 
	        {	        
                BuildKQRaces();
                string fileName = GetFileName();
                if (File.Exists(fileName))
                {
                    mResultsTable = new DataTable(kTableName);
                    mResultsTable.ReadXml(fileName);
                }
                else
                {
                    mResultsTable = GetCompetitionDataTable();
                }
            }
	        catch (Exception ex)
	        {
		        Debug.WriteLine(ex.ToString());
            }
#if FIND_INVALID_RACETYPES
            List<string> raceTypes = AgeGradingTables.GetRaceTypes();
            foreach (KeyValuePair<string, RaceTypeToDistance> pair in mKingAndQueenRaces)
            {
                if (!raceTypes.Contains(pair.Value.RaceType))
                {
                    Debug.WriteLine(pair.Value.RaceType + " not found");
                }
            }
#endif
        }

        private const string kMemberLastName = "Last Name";
        private const string kMemberFirstName = "First Name";
        private const string kMemberGender = "Gender";
        private const string kMemberDOB = "DOB";
        private const string kMemberLastNameAliases = "Last Name Aliases";
        private const string kMemberFirstNameAliases = "First Name Aliases";
        private const string kMemberEligibilityDate = "Eligibility Date";
        // K&Q Table
        private const string kLastName = "Last Name";
        private const string kFirstName = "First Name";
        private const string kFirstNameAliases = "First Name Aliases";
        private const string kLastNameAliases = "Last Name Aliases";
        private const string kGender = "Gender";
        private const string kDateOfBirth = "Date Of Birth";
        private const string kTotalMilesForSixRaces = "Total Miles For Six Races";
        private const string kAverageNormalizeTimesForSixRaces = "Average Normalize Times For Six Races";
        private const string kAverageWorldPercentageForSixRaces = "Average World Percentage For Six Races";
        private DataTable mMemberTable;
        internal const int kMinimumEligibilityAge = 15;
        internal DataTable MemberTable
        {
            get { return mMemberTable; }
            set { mMemberTable = value; }
        }

        private static string mAddOrUpdateStopOn = "Dean";
        internal List<MemberInfo> AddOrUpdateResults(DataTable dataTable
            , RaceInfo raceInfo
            , string memberTableName
            , string nameColumn
            , string firstNameColumn
            , string ageColumn
            , string genderColumn
            , string raceTimeColumn
            , string kAgeAdjustedColumnName
            , string kNormalizedTimeColumnName
            , string kWorldPercentColumnName
            , bool onlyCompletingRequirements
            )
        {
            if (dataTable == null) return null;
            if (raceInfo == null) return null;
            AddOrUpdateMemberInfo(memberTableName);
            DataTable memberTable = MemberTable;
            if (memberTable == null) return null;
            List<MemberInfo> missingNames = new List<MemberInfo>();
            DateTime defaultRaceDate = RaceInfo.GetDefaultRaceDate();
            foreach (DataRow row in memberTable.Rows)
            {
                if (row == null) continue;
                string eligibility = row[kMemberEligibilityDate].ToString();
                string lastName = row[kMemberLastName].ToString();
                if (String.IsNullOrEmpty(eligibility)) continue;
                DateTime date = DateTime.Parse(eligibility);
                DateTime raceDate = (raceInfo.RaceDate > defaultRaceDate ? raceInfo.RaceDate : defaultRaceDate);
                if (date > raceDate) continue;
                string firstName = row[kMemberFirstName].ToString();
                string firstNameAliases = row[kMemberFirstNameAliases].ToString();
                string lastNameAliases = row[kMemberLastNameAliases].ToString();
                string gender = row[kMemberGender].ToString();
                string DOB = row[kMemberDOB].ToString();
                if (String.Equals(lastName, mAddOrUpdateStopOn, StringComparison.OrdinalIgnoreCase))
                {
                }
                if (0 == String.Compare("Payne", lastName, true))
                {

                }
                List<DataRow> matchingRows = null;
                try
                {
                    matchingRows = FindMatchingRows(dataTable, nameColumn, firstNameColumn, genderColumn, firstName, lastName, firstNameAliases, lastNameAliases, gender, matchingRows, ageColumn, DOB, raceInfo.RaceDate);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
                if (matchingRows != null && matchingRows.Count == 1)
                {
                    DataRow resultRow = FindResultRow(firstName, lastName, gender);
                    Debug.Assert(resultRow != null, "Couldn't find member in results");
                    if (resultRow != null)
                    {
                        UpdateRaceInfo(raceInfo, matchingRows[0], resultRow, raceTimeColumn, kNormalizedTimeColumnName, kWorldPercentColumnName);
                        //Debug.WriteLine(String.Format("firstName='{0}' lastName='{1}' gender='{2}' ", firstName, lastName, gender));
                    }
                }
                else
                {
                    string birth = row[kMemberDOB].ToString();
                    DateTime birthDate = GetDateTime(birth);
                    int years = GetYears(birthDate, raceInfo.RaceDate);
                    MemberInfo memberInfo = new MemberInfo(firstName, lastName, years, TimeSpan.Zero);
                    missingNames.Add(memberInfo);
                }
            }
            SaveTable();
            BuildKQResults(onlyCompletingRequirements);
            missingNames.Sort(new MemberInfoComparer());
            return missingNames;
        }

        internal List<AgeGrading.AgeGradingForm.RaceNames> FindMatchingMemberInfo(DataTable dataTable
            , RaceInfo raceInfo
            , string memberTableName
            , string nameColumn
            , string firstNameColumn
            , string ageColumn
            , string genderColumn
            )
        {
            if (dataTable == null) return null;
            if (raceInfo == null) return null;
            AddOrUpdateMemberInfo(memberTableName);
            DataTable memberTable = MemberTable;
            if (memberTable == null) return null;
            List<AgeGrading.AgeGradingForm.RaceNames> foundMember = new List<AgeGrading.AgeGradingForm.RaceNames>();
            DateTime defaultRaceDate = RaceInfo.GetDefaultRaceDate();
            foreach (DataRow row in memberTable.Rows)
            {
                if (row == null) continue;
                string eligibility = row[kMemberEligibilityDate].ToString();
                if (String.IsNullOrEmpty(eligibility)) continue;
                DateTime date = DateTime.Parse(eligibility);
                DateTime raceDate = (raceInfo.RaceDate > defaultRaceDate ? raceInfo.RaceDate : defaultRaceDate);
                if (date > raceDate) continue;
                string firstName = row[kMemberFirstName].ToString();
                string lastName = row[kMemberLastName].ToString();
                string firstNameAliases = row[kMemberFirstNameAliases].ToString();
                string lastNameAliases = row[kMemberLastNameAliases].ToString();
                string gender = row[kMemberGender].ToString();
                string DOB = row[kMemberDOB].ToString();
                if (String.Equals(lastName, mAddOrUpdateStopOn, StringComparison.OrdinalIgnoreCase))
                {
                }
                List<DataRow> matchingRows = null;
                try
                {
                    matchingRows = FindMatchingRows(dataTable, nameColumn, firstNameColumn, genderColumn, firstName, lastName, firstNameAliases, lastNameAliases, gender, matchingRows, ageColumn, DOB, raceInfo.RaceDate);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
                if (matchingRows != null && matchingRows.Count == 1)
                {
                    DataRow resultRow = FindResultRow(firstName, lastName, gender);
                    Debug.Assert(resultRow != null, "Couldn't find member in results");
                    if (resultRow == null) continue;
                    string birth = row[kMemberDOB].ToString();
                    DateTime birthDate = GetDateTime(birth);
                    int years = GetYears(birthDate, raceInfo.RaceDate);
                    AgeGrading.AgeGradingForm.RaceNames memberInfo = new AgeGrading.AgeGradingForm.RaceNames(firstName, lastName);
                    memberInfo.Age = years;
                    memberInfo.Gender = gender;
                    memberInfo.DataRow = resultRow;
                    memberInfo.DataRow = matchingRows[0];
                    foundMember.Add(memberInfo);
                }
            }
            return foundMember;
        }

        private static List<DataRow> FindMatchingRows(DataTable dataTable, string nameColumn, string firstNameColumn, string genderColumn, string firstName, string lastName, string firstNameAliases,
            string lastNameAliases, string gender, List<DataRow> matchingRows, string ageColumn, string DOB, DateTime raceDate)
        {
            try
            {
                List<DataRow> rows = FindMatchingRows(dataTable.Rows, lastName, nameColumn, true);
                if (rows != null && rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(firstNameColumn) && dataTable.Columns.Contains(firstNameColumn))
                    {
                        matchingRows = FindMatchingRows(rows, firstName, firstNameColumn, true);
                        if ((matchingRows == null || matchingRows.Count <= 0) && !String.IsNullOrEmpty(firstNameAliases)) matchingRows = FindMatchingRows(rows, firstNameAliases, firstNameColumn, true);
                        if (matchingRows != null && matchingRows.Count > 0) matchingRows = FindMatchingRows(matchingRows, gender, genderColumn, false);
                    }
                }
                if (matchingRows != null && matchingRows.Count > 1)
                {
                    if (!String.IsNullOrEmpty(DOB))
                    {
                        // Try to match the age to eliminate the duplicates.
                        try
                        {
                            DateTime birthDate = GetDateTime(DOB);
                            if (!DateTime.Equals(birthDate, DateTime.MinValue))
                            {
                                DataRow dataRow = null;
                                int years = GetYears(birthDate, raceDate);
                                foreach (DataRow item in matchingRows)
                                {
                                    int age = (int)item[ageColumn];
                                    if (age == years && dataRow == null) dataRow = item;
                                    else if (age == years)
                                    {
                                        // Still can't determine which data row to use. Punt.
                                        dataRow = null;
                                        break;
                                    }
                                }
                                if (dataRow != null)
                                {
                                    matchingRows = new List<DataRow>();
                                    matchingRows.Add(dataRow);
                                    return matchingRows;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                    throw new Exception("More than one name matches in the race results");
                }
                if ((matchingRows == null || matchingRows.Count <= 0) && !String.IsNullOrEmpty(lastNameAliases))
                {
                    rows = FindMatchingRows(dataTable.Rows, lastNameAliases, nameColumn, true);
                    if (rows != null && rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(firstNameColumn) && dataTable.Columns.Contains(firstNameColumn))
                        {
                            if (String.Equals("HURST", lastName, StringComparison.OrdinalIgnoreCase))
                            {

                            }
                            matchingRows = FindMatchingFirstNameRows(rows, firstName, firstNameColumn);
                            if ((matchingRows == null || matchingRows.Count <= 0) && !String.IsNullOrEmpty(firstNameAliases)) matchingRows = FindMatchingRows(rows, firstNameAliases, firstNameColumn, true);
                            if (matchingRows != null && matchingRows.Count > 0) matchingRows = FindMatchingRows(matchingRows, gender, genderColumn, false);
                        }
                    }
                }
                if (matchingRows != null && matchingRows.Count > 1)
                {
                    // Try to match the age to eliminate the duplicates.
                    try
                    {
                        DateTime birthDate = GetDateTime(DOB);
                        if (!DateTime.Equals(birthDate, DateTime.MinValue))
                        {
                            DataRow dataRow = null;
                            int years = GetYears(birthDate, DateTime.Today);
                            foreach (DataRow item in matchingRows)
                            {
                                int age = (int)item[ageColumn];
                                if (age == years && dataRow == null) dataRow = item;
                                else if (age == years)
                                {
                                    // Still can't determine which data row to use. Punt.
                                    dataRow = null;
                                    break;
                                }
                            }
                            if (dataRow != null)
                            {
                                matchingRows = new List<DataRow>();
                                matchingRows.Add(dataRow);
                                return matchingRows;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    throw new Exception("More than one name matches in the race results");
                }
            }
            catch
            {
                throw;
            }
            return matchingRows;
        }

        private static int GetYears(DateTime birthDate, DateTime onThisDate)
        {
            int age = onThisDate.Year - birthDate.Year;
            if (onThisDate.Month < birthDate.Month || (onThisDate.Month == birthDate.Month && onThisDate.Day < birthDate.Day)) age--;
            return age;
        }
        
        private static DateTime GetDateTime(string date)
        {
            if (String.IsNullOrEmpty(date)) return DateTime.MinValue;
            DateTime dateTime = DateTime.Parse(date);
            if (!DateTime.TryParse(date, out dateTime)) return DateTime.MinValue;
            return dateTime;
        }

        public class MemberInfo
        {
            public MemberInfo(string firstName, string lastName, int age, TimeSpan normalizedTime)
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
                NormalizedTime = normalizedTime;
            }

            private int mAge;
            public int Age
            {
                get { return mAge; }
                set { mAge = value; }
            }

            private string mFirstName;
            public string FirstName
            {
                get
                {
                    if (mFirstName == null) mFirstName = String.Empty;
                    return mFirstName;
                }
                set { mFirstName = value; }
            }

            private string mLastName;
            public string LastName
            {
                get
                {
                    if (mLastName == null) mLastName = String.Empty;
                    return mLastName;
                }
                set { mLastName = value; }
            }

            private TimeSpan mNormalizedTime;
            public TimeSpan NormalizedTime
            {
                get { return mNormalizedTime; }
                set { mNormalizedTime = value; }
            }

            public override string ToString()
            {
                return String.Format("{0} {1}\t{2}", FirstName, LastName, Age);
            }
        }

        public class MemberInfoComparer : IComparer<MemberInfo>
        {
            private bool mByName;
            public bool ByName
            {
                get { return mByName; }
                set { mByName = value; }
            }
            
            public MemberInfoComparer()
            {
            }

            public MemberInfoComparer(bool byName)
            {
                this.ByName = byName;
            }

            public int Compare(MemberInfo x, MemberInfo y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                int result = 0;
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

        private void UpdateRaceInfo(RaceInfo raceInfo, DataRow row, DataRow foundRow, string raceTimeColumn
            , string kNormalizedTimeColumnName
            , string kWorldPercentColumnName
            )
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

        private DataRow FindResultRow(string firstName, string lastName, string gender)
        {
            if (String.IsNullOrEmpty(firstName)) return null;
            if (String.IsNullOrEmpty(lastName)) return null;
            if (String.IsNullOrEmpty(gender)) return null;
            List<DataColumn> dataColumns = new List<DataColumn>();
            dataColumns.Add(mResultsTable.Columns[kLastName]);
            dataColumns.Add(mResultsTable.Columns[kFirstName]);
            dataColumns.Add(mResultsTable.Columns[kGender]);
            DataRow foundRow = FindFinisher(firstName, lastName, gender, dataColumns);
            return foundRow;
        }

        public static DataRow FindMemberInRace(DataTable mResultsTable, string firstName, string lastName, string firstNameAliases, string lastNameAliases, string gender, string ageString)
        {
            if (String.IsNullOrEmpty(firstName)) return null;
            if (String.IsNullOrEmpty(lastName)) return null;
            gender = AgeGrading.AgeGradingTables.AgeAdjustResult.GetGender(gender);
            if (String.IsNullOrEmpty(gender)) return null;
            if (String.IsNullOrEmpty(ageString)) return null;
            int age = 0;
            if (!int.TryParse(ageString, out age)) return null;
            List<DataColumn> dataColumns = new List<DataColumn>();
            dataColumns.Add(mResultsTable.Columns[kLastName]);
            dataColumns.Add(mResultsTable.Columns[kFirstName]);
            dataColumns.Add(mResultsTable.Columns[kGender]);
            if (String.Equals(lastName, "Wyatt", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("");
            }
            return null;
        }

        private string mMemberTableName;
        public string MemberTableName
        {
            get { return mMemberTableName; }
            set { mMemberTableName = value; }
        }

        private DataTable GetMemberTable()
        {
            if (MemberTable != null) return MemberTable;
            MemberTable = new DataTable();
            MemberTable.ReadXml(MemberTableName);
            return MemberTable;
        }

        public void AddOrUpdateMemberInfo(string memberFileName)
        {
            if (String.IsNullOrEmpty(memberFileName)) return;
            if (!File.Exists(memberFileName)) return;
            MemberTable = null;
            try
            {
                GetMemberTable();
                foreach (DataRow row in MemberTable.Rows)
                {
                    if (row == null) continue;
                    string lastName = row[kMemberLastName].ToString();
                    if (String.Equals("BENTLEY", lastName, StringComparison.OrdinalIgnoreCase))
                    {

                    }
                    string firstName = row[kMemberFirstName].ToString();
                    string firstNameAliases = row[kMemberFirstNameAliases].ToString();
                    string gender = row[kMemberGender].ToString();
                    string DOB = row[kMemberDOB].ToString();
                    if (String.IsNullOrEmpty(DOB)) DOB = DateTime.MinValue.ToString();
                    string tableGender = AgeGrading.AgeGradingTables.AgeAdjustResult.GetGender(gender);
                    if (String.IsNullOrEmpty(tableGender)) gender = String.Empty;
                    DataRow foundRow = mResultsTable.Rows.Find(new object[] { lastName, firstName, gender });
                    if (foundRow == null && !String.IsNullOrEmpty(firstNameAliases))
                        foundRow = mResultsTable.Rows.Find(new object[] { lastName, firstNameAliases, gender });
                    if (foundRow == null)
                    {
                        firstNameAliases = row[kMemberFirstNameAliases].ToString();
                        string lastNameAliases = row[kMemberLastNameAliases].ToString();
                        DataRow dataRow = AddDataRow(mResultsTable, lastName, firstName, firstNameAliases, lastNameAliases, gender, DateTime.Parse(DOB));
                        if (dataRow != null) mResultsTable.Rows.Add(dataRow);
                    }
                    else
                    {
                        // Need to update the row here. Paticularly the DOB and aliases.
                        foundRow[kDateOfBirth] = DOB;
                        foundRow[kFirstNameAliases] = row[kFirstNameAliases];
                        foundRow[kLastNameAliases] = row[kLastNameAliases];
                    }
                }
                //List<DataColumn> dataColumns = new List<DataColumn>();
                //dataColumns.Add(MemberTable.Columns[kMemberLastName]);
                //dataColumns.Add(MemberTable.Columns[kMemberFirstName]);
                //dataColumns.Add(MemberTable.Columns[kMemberGender]);
                //MemberTable.PrimaryKey = dataColumns.ToArray<DataColumn>();
                SaveTable();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public bool ValidMember(string firstName, string lastName, string gender, DateTime raceDate)
        {
            if (MemberTable == null) return false;
            DataRow foundRow = MemberTable.Rows.Find(new object[] { lastName, firstName, gender });
            if (foundRow == null) return false;
            string eligibility = foundRow[kMemberEligibilityDate].ToString();
            if (String.IsNullOrEmpty(eligibility)) return false;
            DateTime date = DateTime.Parse(eligibility);
            if (date > raceDate) return false;
            return true;
        }

        private DataRow FindFinisher(string firstName, string lastName, string gender, List<DataColumn> primaryKey)
        {
            if (mResultsTable == null) return null;
            if (primaryKey == null || primaryKey.Count <= 0) return null;
            mResultsTable.PrimaryKey = primaryKey.ToArray<DataColumn>();
            DataRow foundRow = mResultsTable.Rows.Find(new object[] { lastName, firstName, gender });
            return foundRow;
        }

        private List<DataRow> FindLastName(string lastName, string gender, string lastNameColumn)
        {
            if (mResultsTable == null) return null;
            if (String.IsNullOrEmpty(lastNameColumn)) return null;
            List<DataRow> foundRows = null;
            foreach (DataRow row in mResultsTable.Rows)
            {
                if (row == null) continue;
                object value = row[lastNameColumn];
                if (value == null) continue;
                string theLastName = value.ToString();
                if (String.IsNullOrEmpty(theLastName)) continue;
                int index = theLastName.IndexOf(lastName, StringComparison.OrdinalIgnoreCase);
                if (index < 0) continue;
                // Now match the gender.
                value = row[kGender];
                if (value == null) continue;
                if (!AgeGradingTables.GendersMatch(value.ToString(), gender)) continue;
                if (foundRows == null) foundRows = new List<DataRow>();
                foundRows.Add(row);
            }
            return foundRows;
        }

        private static DataRow FindMatch(IEnumerable rows, string operand, string columnName, bool checkContains)
        {
            if (rows == null) return null;
            if (String.IsNullOrEmpty(operand)) return null;
            if (String.IsNullOrEmpty(columnName)) return null;
            foreach (DataRow row in rows)
            {
                if (row == null) continue;
                object value = row[columnName];
                if (value == null) continue;
                string temp = value.ToString();
                if (String.IsNullOrEmpty(temp)) continue;
                if (!checkContains && String.Equals(temp, operand, StringComparison.OrdinalIgnoreCase)) return row;
                else if (checkContains)
                {
                    int index = operand.IndexOf(temp, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0) return row;
                }
            }
            return null;
        }

        private const string kFindMatchingRowStopOn = "MCELRAVEY";
        private static List<DataRow> FindMatchingRows(IEnumerable rows, string operand, string columnName, bool checkContains)
        {
            if (rows == null) return null;
            if (String.IsNullOrEmpty(operand)) return null;
            string[] split = operand.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length <= 0) return null;
            if (String.IsNullOrEmpty(columnName)) return null;
            HashSet<DataRow> matchingRows = new HashSet<DataRow>();
            foreach (DataRow row in rows)
            {
                if (row == null) continue;
                object value = row[columnName];
                if (value == null) continue;
                string temp = value.ToString();
                if (String.IsNullOrEmpty(temp)) continue;
                int tempIndex = temp.IndexOf(kFindMatchingRowStopOn, StringComparison.OrdinalIgnoreCase);
                if (tempIndex >= 0)
                {
                }
                foreach (string operandValue in split)
                {
                    string theOperandValue = operandValue.Trim();
                    if (matchingRows.Contains(row)) break;
                    if (!checkContains && String.Equals(temp, theOperandValue, StringComparison.OrdinalIgnoreCase)) matchingRows.Add(row);
                    else if (checkContains)
                    {
                        int index = temp.IndexOf(theOperandValue, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0) matchingRows.Add(row);
                    }
                }
            }
            return matchingRows.ToList<DataRow>();
        }

        private static List<DataRow> FindMatchingFirstNameRows(IEnumerable rows, string firstName, string columnName)
        {
            if (rows == null) return null;
            if (String.IsNullOrEmpty(firstName)) return null;
            if (String.IsNullOrEmpty(columnName)) return null;
            HashSet<DataRow> matchingRows = new HashSet<DataRow>();
            char[] space = new char[] { ' ' }; 
            foreach (DataRow row in rows)
            {
                if (row == null) continue;
                if (matchingRows.Contains(row)) continue;
                object value = row[columnName];
                if (value == null) continue;
                string temp = value.ToString();
                if (String.IsNullOrEmpty(temp)) continue;
                string[] split = temp.Split(space, StringSplitOptions.RemoveEmptyEntries);
                if (String.Equals(firstName, split[0], StringComparison.OrdinalIgnoreCase)) matchingRows.Add(row);
                else if (split.Length == 3)
                {
                    string[] splitName = firstName.Split(space, StringSplitOptions.RemoveEmptyEntries);
                    bool firstMatch = String.Equals(splitName[0], split[0], StringComparison.OrdinalIgnoreCase);
                    if (splitName.Length == 2 && firstMatch)
                    {
                        matchingRows.Add(row);
                    }
                    else if (splitName.Length == 3
                        && firstMatch
                        && String.Equals(splitName[1], split[1], StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        matchingRows.Add(row);
                    }
                }
            }
            return matchingRows.ToList<DataRow>();
        }

        public DataRow FindFinisher(string firstName, string lastName, string gender, string ageString)
        {
            if (mResultsTable == null) return null;
            if (String.IsNullOrEmpty(firstName)) return null;
            if (String.IsNullOrEmpty(lastName)) return null;
            gender = AgeGrading.AgeGradingTables.AgeAdjustResult.GetGender(gender);
            if (String.IsNullOrEmpty(gender)) return null;
            if (String.IsNullOrEmpty(ageString)) return null;
            int age = 0;
            if (!int.TryParse(ageString, out age)) return null;
            List<DataColumn> dataColumns = new List<DataColumn>();
            dataColumns.Add(mResultsTable.Columns[kLastName]);
            dataColumns.Add(mResultsTable.Columns[kFirstName]);
            dataColumns.Add(mResultsTable.Columns[kGender]);
            if (String.Equals(lastName, "Wyatt", StringComparison.OrdinalIgnoreCase))
            {
            }
            DataRow foundRow = FindFinisher(firstName, lastName, gender, dataColumns);
            if (foundRow != null) return foundRow;
            // Check if there's a matching last name
            List<DataRow> foundRows = FindLastName(lastName, gender, kLastName);
            if (foundRows != null && foundRows.Count > 0)
            {
                foundRow = FindMatch(foundRows, firstName, kFirstName, false);
                if (foundRow != null) return foundRow;
                foundRow = FindMatch(foundRows, firstName, kFirstNameAliases, true);
                if (foundRow != null) return foundRow;
            }
            // Check if there's a matching last name to the aliases
            foundRows = FindLastName(lastName, gender, kLastNameAliases);
            if (foundRows != null && foundRows.Count > 0)
            {
                foundRow = FindMatch(foundRows, firstName, kFirstName, true);
                if (foundRow != null) return foundRow;
            }
            return null;
        }

        private string GetFileName()
        {
            string folder = AgeGradingForm.GetFolderPath();
            string fileName = String.Format(@"{0}\{1}", folder, kKQResultsFileName);
            return fileName;
        }

        private const string kAgeGroupFileName = "AgeGrouping";
        internal static string GetAgeGroupFileName()
        {
            string folder = AgeGradingForm.GetFolderPath();
            string fileName = String.Format("{0}{1}{2}.html", folder, Path.DirectorySeparatorChar, kAgeGroupFileName);
            return fileName;
        }

        private const string kMrMsMileageFileName = "MrMsMileage";
        internal static string GetMrMsMileageFileName()
        {
            string folder = AgeGradingForm.GetFolderPath();
            string fileName = String.Format("{0}{1}{2}.html", folder, Path.DirectorySeparatorChar, kMrMsMileageFileName);
            return fileName;
        }

        private const char kRaceDelimiter = '@';
        public static string BuildRaceString(string raceTime, string normalizedTime, string worldPercentage)
        {
            if (String.IsNullOrEmpty(raceTime)) return String.Empty;
            if (String.IsNullOrEmpty(normalizedTime)) return String.Empty;
            if (String.IsNullOrEmpty(worldPercentage)) return String.Empty;
            string race = String.Format("{0}{1}{2}{3}{4}",
                raceTime,
                kRaceDelimiter,
                normalizedTime,
                kRaceDelimiter,
                worldPercentage
            );
            return race;
        }

        private bool GetRaceTimes(string race, out string raceTime, out string normalizedTime, out string worldPercentage)
        {
            raceTime = String.Empty;
            normalizedTime = String.Empty;
            worldPercentage = String.Empty;
            if (String.IsNullOrEmpty(race)) return false;
            string[] split = race.Split(new char[] { kRaceDelimiter });
            if (split == null || split.Length != 3) return false;
            raceTime = split[0].Trim();
            normalizedTime = split[1].Trim();
            worldPercentage = split[2].Trim();
            return true;
        }

        internal static bool GetTimeSpan(string spanString, out TimeSpan span)
        {
            span = TimeSpan.Zero;
            if (String.IsNullOrEmpty(spanString)) return false;
            string[] split = spanString.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length <= 0) return false;
            double totalSeconds = 0.0;
            double temp = 0.0;
            int index = split.Length - 1;
            if (!double.TryParse(split[index], out temp)) return false;
            totalSeconds += temp;
            index--;
            if (index >= 0)
            {
                if (!double.TryParse(split[index], out temp)) return false;
                totalSeconds += 60 * temp;
                index--;
            }
            if (index >= 0)
            {
                if (!double.TryParse(split[index], out temp)) return false;
                totalSeconds += 3600 * temp;
            }
            span = TimeSpan.FromSeconds(totalSeconds);
            return (!TimeSpan.Equals(TimeSpan.Zero, span));
        }

        private string CapitalizeFirstLetter(string name)
        {
            if (String.IsNullOrEmpty(name)) return name;
            string[] split = name.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split == null || split.Length <= 0) return name;
            StringBuilder str = new StringBuilder();
            foreach (string item in split)
            {
                if (str.Length > 0) str.Append(" ");
                if (item.Length > 1) str.Append(item.Substring(0, 1).ToUpper() + item.Substring(1).ToLower());
                else str.Append(item.ToUpper());
            }
            if (str.Length <= 0) return name;
            return str.ToString();
        }

        public void BuildKQResults(bool onlyCompletingRequirements)
        {
            if (mResultsTable == null) return;
            try
            {
                if (onlyCompletingRequirements) InitializeVolunteePoints();
                List<string> KQRaces = GetKQRaces();
                List<ResultRow> table = new List<ResultRow>();
                List<ResultRow> validResultsTable = new List<ResultRow>();
                List<AgeGradeResult> ageGradeResults = new List<AgeGradeResult>();
                StringBuilder str = new StringBuilder();

                HtmlTableRow row;
                HtmlTableCell cell;
                string lastestRace = String.Empty;
                DateTime endOfYear = new DateTime(DateTime.Now.Year, 12, 31);
                foreach (DataRow item in mResultsTable.Rows)
                {
                    if (item == null) continue;
                    List<string> rows = null;
                    double totalMiles = 0;
                    TimeSpan averageNormalizedTime = TimeSpan.Zero;
                    double averageWorldPercent = 0.0;
                    List<ResultCalculator.RaceResult> raceResults = null;
                    string firstName = item[kFirstName].ToString();
                    if (firstName != null) firstName = firstName.ToUpper();
                    string lastName = item[kLastName].ToString();
                    if (lastName != null) lastName = lastName.ToUpper();
                    if (String.Equals(firstName, "DAVID", StringComparison.OrdinalIgnoreCase) && String.Equals(lastName, "BAXTER", StringComparison.OrdinalIgnoreCase))
                    {
                    }
                    string raceInfo = GetRaces(item, KQRaces, out rows, out totalMiles, out averageNormalizedTime, out averageWorldPercent, out raceResults);
                    if (!String.IsNullOrEmpty(raceInfo))
                    {
                        if (onlyCompletingRequirements && raceResults.Count < ResultCalculator.RequiredRaceCount) continue;
                        bool validRaceResults = false;
                        List<ResultCalculator.RaceResult> raceLists = null;
                        double lowestNormalizeTime = 0.0;
                        double totalRaceDistance = 0.0;
                        double averageWorldPercentage = 0.0;
                        if (onlyCompletingRequirements)
                        {
                            VolunteerPoints volunteerPoints = FindVolunteerPoints(lastName, firstName);
                            if (volunteerPoints == null || (volunteerPoints != null && volunteerPoints.TotalPoints < 5))
                            {
#if SHOW_MISSING
                                if (volunteerPoints == null)
                                    Debug.WriteLine(String.Format("{0} {1} wasn't found in volunteer points", firstName, lastName));
                                else
                                    Debug.WriteLine(String.Format("{0} {1} points {2} didn't qualify volunteer points", firstName, lastName, volunteerPoints.TotalPoints));
#endif
                                continue;
                            }
                        }
                        if (String.Equals(lastName, mAddOrUpdateStopOn, StringComparison.OrdinalIgnoreCase))
                        {
                        }
                        string birth = item[kDateOfBirth].ToString();
                        DateTime birthDate = GetDateTime(birth);
                        int ageAtEndOfYear = GetYears(birthDate, endOfYear);
                        if (ageAtEndOfYear < kMinimumEligibilityAge) continue;
                        TimeSpan lowestNormalizeTimeSpan = TimeSpan.Zero;
                        int raceCount = (raceResults != null ? raceResults.Count : 0);
                        if (raceCount >= ResultCalculator.RequiredRaceCount)
                        {
                            int tempIndex = lastName.IndexOf(kFindMatchingRowStopOn, StringComparison.OrdinalIgnoreCase);
                            if (tempIndex >= 0)
                            {
                            }
                            raceLists = ResultCalculator.GetAllCombination(raceResults, ResultCalculator.RequiredRaceCount,
                                out lowestNormalizeTime, out totalRaceDistance, out averageWorldPercentage);
                            if (onlyCompletingRequirements && totalRaceDistance < ResultCalculator.MinimumRaceDistance) continue;
                            if (raceLists != null)
                            {
                                validRaceResults = true;
                                lowestNormalizeTimeSpan = TimeSpan.FromSeconds(lowestNormalizeTime);
                            }
                        }
                        row = new HtmlTableRow();

                        cell = new HtmlTableCell();
                        TimeSpan normalizedTime = TimeSpan.Zero;
                        if (validRaceResults) normalizedTime = lowestNormalizeTimeSpan;
                        else normalizedTime = averageNormalizedTime;
                        cell.InnerHtml = String.Format("<b> {0} {1} {2} {3}</b><br/>", kPlaceIndexHolder, firstName, lastName, AgeGradingTables.GetMinimumSpan(normalizedTime));
                        row.Cells.Add(cell);

                        cell = new HtmlTableCell();
                        str = new StringBuilder();
                        str.AppendLine(String.Format("<div id='{0} {1}' style='display:none'>", firstName, lastName));
                        str.AppendLine("<ol>");
                        // Totals here...
                        foreach (string raceItem in rows)
                        {
                            str.AppendLine(String.Format("<li>{0}", raceItem));
                        }
                        str.AppendLine("</ol>");
                        cell.InnerHtml = str.ToString();
                        str.AppendLine("<br><br>");
                        str.AppendLine(String.Format("Total number of miles {0}<br>", totalMiles));
                        str.AppendLine(String.Format("Average of all normalized race times {0}<br>", AgeGradingTables.GetMinimumSpan(averageNormalizedTime)));
                        str.AppendLine(String.Format("Average world percent of all normalized times {0}%<br>", averageWorldPercent));
                        str.AppendLine("<p/>");
                        str.AppendLine("</div>");
                        cell.InnerHtml = str.ToString();
                        row.Cells.Add(cell);
                        if (validRaceResults)
                        {
                            cell = new HtmlTableCell();
                            str = new StringBuilder();
                            str.AppendLine(String.Format("<div id='{0} {1}1' style='display:none'>", firstName, lastName));
                            str.AppendLine("<ol>");
                            averageWorldPercent = 0.0;
                            totalRaceDistance = 0.0;
                            foreach (ResultCalculator.RaceResult result in raceLists)
                            {
                                str.AppendLine(String.Format("<li>{0}: {1}", result.Name, AgeGradingTables.GetMinimumSpan(result.Normalized)));
                                averageWorldPercent += result.WorldPercent;
                                totalRaceDistance += result.Distance;
                            }
                            averageWorldPercent = averageWorldPercent / raceLists.Count;
                            str.AppendLine("</ol>");
                            str.AppendLine("<br><br>");
                            str.AppendLine(String.Format("Total miles for best six normalized times = {0}<br>", Math.Round(totalRaceDistance, 3)));
                            str.AppendLine(String.Format("Average time for best six normalized times = {0}<br>", AgeGradingTables.GetMinimumSpan(lowestNormalizeTimeSpan)));
                            str.AppendLine(String.Format("Average world percent for best six normalized times = {0}%<br>", Math.Round(averageWorldPercent, 2)));
                            str.AppendLine("<p/>");
                            str.AppendLine("</div>");
                            cell.InnerHtml = str.ToString();
                            row.Cells.Add(cell);
                            validResultsTable.Add(new ResultRow(row, lowestNormalizeTimeSpan, firstName, lastName));
                        }
                        else table.Add(new ResultRow(row, averageNormalizedTime, firstName, lastName));
                        bool isMale = AgeGrading.AgeGradingTables.AgeAdjustResult.IsAMale(item[kGender].ToString());
                        if (String.Equals(firstName, "DAVID", StringComparison.OrdinalIgnoreCase) && String.Equals(lastName, "BAXTER", StringComparison.OrdinalIgnoreCase))
                        {
                        }
                        AgeGradeResult ageGradeResult = new AgeGradeResult(firstName, lastName, ageAtEndOfYear, isMale, averageWorldPercent, raceCount, totalRaceDistance, normalizedTime, totalMiles);
                        ageGradeResults.Add(ageGradeResult);
                    }
                }
                string fileName = String.Format("{0}{1}{2}", AgeGradingForm.GetFolderPath(), Path.DirectorySeparatorChar, "KQIndividualResults.html");
                using (TextWriter writer = File.CreateText(fileName))
                {
                    using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(writer))
                    {
                        str = new StringBuilder();
                        HtmlStart(ref str);
                        htmlTextWriter.Write(str.ToString());
                        str = new StringBuilder();
                        HtmlIndividualStart(ref str);
                        string start = str.ToString();
                        string latestRace = RaceInfo.GetLatestRace();
                        string today = String.Format("{0}/{1}", DateTime.Today.Month, DateTime.Today.Day);
                        if (!String.IsNullOrEmpty(latestRace)) start = start.Replace(kRaceUpdate, String.Format("{0} (updated {1})", latestRace, today));
                        htmlTextWriter.Write(start);
                        // Combine them for now.
                        validResultsTable.AddRange(table);
                        validResultsTable.Sort(new ResultRowComparer());
                        int place = 1;
                        foreach (ResultRow tableRow in validResultsTable)
                        {
                            tableRow.Row.Attributes.Add("valign", "top");
                            string color = ((place % 2) == 1 ? "#EEEEEE" : "white");
                            tableRow.Row.Attributes.Add("bgcolor", color);
                            tableRow.Row.Attributes.Add("onMouseOver", "this.style.backgroundColor='#E1EAFE'");
                            tableRow.Row.Attributes.Add("onMouseOut", String.Format("this.style.backgroundColor='{0}'", color));
                            tableRow.Row.Attributes.Add("onClick", String.Format("myMenu_sets('{0} {1}');", tableRow.FirstName, tableRow.LastName));
                            cell = tableRow.Row.Cells[0];
                            cell.InnerHtml = cell.InnerHtml.Replace(kPlaceIndexHolder, place.ToString());
                            tableRow.Row.RenderControl(htmlTextWriter);
                            place++;
                        }
                        htmlTextWriter.Flush();
                        str = new StringBuilder();
                        HtmlEnd(ref str);
                        htmlTextWriter.Write(str.ToString());
                    }
                }
                BuildAgeGradeResults(ageGradeResults);
                BuildMrMsMileageResults(ageGradeResults);
            }
            catch (Exception ex)
            {
                // If this occurs then you need to remove the KQResults.xml file so the race names get rebuilt.
                string error = String.Format("If this occurs then you may need to remove the '{0}' file so the race names get rebuilt: '{1}'", kKQResultsFileName, ex.ToString());
                Debug.Assert(false, error);
            }
        }

        private HtmlTableCell GetHeaderCell(string text)
        {
            if (text == null) text = String.Empty;
            HtmlTableCell column = new HtmlTableCell("th");
            column.Attributes.Add("align", "center");
            column.InnerText = text;
            return column;
        }

        private void AddAttributes(HtmlTableRow row, int place)
        {
            row.Attributes.Add("valign", "top");
            string color = ((place % 2) == 1 ? "#EEEEEE" : "white");
            row.Attributes.Add("bgcolor", color);
        }

        private HtmlTableRow GetHeaderRow()
        {
            HtmlTableRow header = new HtmlTableRow();
            header.Cells.Add(GetHeaderCell("Overall"));
            header.Cells.Add(GetHeaderCell("Name"));
            header.Cells.Add(GetHeaderCell("Age"));
            header.Cells.Add(GetHeaderCell("Div"));
            header.Cells.Add(GetHeaderCell("Total"));
            header.Cells.Add(GetHeaderCell("Time"));
            header.Cells.Add(GetHeaderCell("%World"));
            header.Cells.Add(GetHeaderCell("#Races"));
            header.Cells.Add(GetHeaderCell("#Miles for Best 6 Races"));
            header.Attributes.Add("valign", "top");
            header.Attributes.Add("bgcolor", "#DBDBDD");
            return header;
        }

        private HtmlTableRow GetMrMsMileageHeaderRow()
        {
            HtmlTableRow header = new HtmlTableRow();
            header.Cells.Add(GetHeaderCell("Last Name"));
            header.Cells.Add(GetHeaderCell("First Name"));
            header.Cells.Add(GetHeaderCell("Age"));
            header.Cells.Add(GetHeaderCell("Sex"));
            header.Cells.Add(GetHeaderCell("Number of Miles"));
            header.Cells.Add(GetHeaderCell("Number of Races"));
            header.Attributes.Add("valign", "top");
            header.Attributes.Add("bgcolor", "#DBDBDD");
            return header;
        }

        private void AddRows(HtmlTable table, List<AgeGradeResult> results, List<AgeGradeResult> ignore)
        {
            if (results == null || results.Count <= 0) return;
            int place = 1;
            for (int ii = 0; ii < results.Count; ii++)
            {
                AgeGradeResult result = results[ii];
                if (result == null) continue;
                if (ignore != null && ignore.Contains(result)) continue;
                HtmlTableRow row = GetTableRow(result);
                AddAttributes(row, place);
                table.Rows.Add(row);
                place++;
            }
        }

        private void AddMrMsMileageRow(HtmlTable table, AgeGradeResult ageResult)
        {
            if (ageResult == null) return;
            HtmlTableRow row = GetMrMsMileageTableRow(ageResult);
            if (row != null)
            {
                table.Rows.Add(row);
            }
        }

        private HtmlTableRow GetTableRow(AgeGradeResult result)
        {
            HtmlTableRow row = new HtmlTableRow();
            if (result != null)
            {
                // Overall
                row.Cells.Add(CreateCell(result.Place.ToString(), true));
                row.Cells.Add(CreateCell(String.Format("{0} {1}", result.FirstName, result.LastName), false));
                row.Cells.Add(CreateCell(result.Age.ToString(), true));
                row.Cells.Add(CreateCell(result.DivisionPlace.ToString(), true));
                row.Cells.Add(CreateCell(result.TotalAgeGroupCount.ToString(), true));
                row.Cells.Add(CreateCell(AgeGradingTables.GetMinimumSpan(result.NormalizedTime), true));
                row.Cells.Add(CreateCell(String.Format("{0}%", Math.Round(result.WorldPercentage, 2)), true));
                row.Cells.Add(CreateCell(result.TotalRaces.ToString(), true));
                row.Cells.Add(CreateCell(Math.Round(result.MilesForBestRaces, 3).ToString(), true));
            }
            return row;
        }

        private HtmlTableRow GetMrMsMileageTableRow(AgeGradeResult result)
        {
            if (result == null) return null;
            HtmlTableRow row = new HtmlTableRow();
            if (result != null)
            {
                // Overall
                row.Cells.Add(CreateCell(result.LastName, false));
                row.Cells.Add(CreateCell(result.FirstName, false));
                row.Cells.Add(CreateCell(result.Age.ToString(), true));
                row.Cells.Add(CreateCell((result.IsMale ? "M" : "F"), true));
                row.Cells.Add(CreateCell(Math.Round(result.TotalMiles, 3).ToString(), true));
                row.Cells.Add(CreateCell(result.TotalRaces.ToString(), true));
            }
            return row;
        }

        private HtmlTableCell CreateCell(string text, bool center)
        {
            HtmlTableCell cell = new HtmlTableCell();
            if (text == null) text = String.Empty;
            if (center) cell.Attributes.Add("align", "center");
            cell.InnerText = text;
            return cell;
        }

        private HtmlTableRow CreateRaceTypeRow(string text)
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("valign", "top");
            row.Attributes.Add("bgcolor", "#FFFFD9");
            HtmlTableCell cell = new HtmlTableCell();
            cell.Attributes.Add("colspan", "9");
            if (text == null) text = String.Empty;
            cell.InnerText = text;
            row.Cells.Add(cell);
            return row;
        }

        private void BuildAgeGradeResults(List<AgeGradeResult> ageGradeResults)
        {
            if (ageGradeResults == null || ageGradeResults.Count <= 0) return;
            ageGradeResults.Sort(new AgeGradeResultComparer(true));
            List<AgeGradeResult> top3Males = new List<AgeGradeResult>();
            List<AgeGradeResult> top3Females = new List<AgeGradeResult>();
            List<List<AgeGradeResult>> maleDivisions = CreateDivisions();
            List<List<AgeGradeResult>> femaleDivisions = CreateDivisions();
            int place = 1;
            foreach (AgeGradeResult result in ageGradeResults)
            {
                int index = (result.Age - kMinimumEligibilityAge) / 5;
                if (index < 0)
                {
                    // Since we are excluding under 15 and including all members
                    // just skip them.
                    continue;
                }
                else if (index >= ageGradeResults.Count)
                {
                    Debug.Assert(false, "Someone's age is greater than the max");
                    continue;
                }
                result.Place = place;
                if (result.IsMale)
                {
                    maleDivisions[index].Add(result);
                    if (top3Males.Count < 3) top3Males.Add(result);
                }
                else if (!result.IsMale)
                {
                    femaleDivisions[index].Add(result);
                    if (top3Females.Count < 3) top3Females.Add(result);
                }
                place++;
            }
            foreach (List<AgeGradeResult> results in maleDivisions)
            {
                if (results != null && results.Count > 0)
                {
                    results.Sort(new AgeGradeResultComparer(true));
                    for (int ii = 0; ii < results.Count; ii++)
                    {
                        AgeGradeResult result = results[ii];
                        result.DivisionPlace = ii + 1;
                        result.TotalAgeGroupCount = results.Count;
                    }
                }
            }
            foreach (List<AgeGradeResult> results in femaleDivisions)
            {
                if (results != null && results.Count > 0)
                {
                    results.Sort(new AgeGradeResultComparer(true));
                    for (int ii = 0; ii < results.Count; ii++)
                    {
                        AgeGradeResult result = results[ii];
                        result.DivisionPlace = ii + 1;
                        result.TotalAgeGroupCount = results.Count;
                    }
                }
            }

            string fileName = GetAgeGroupFileName();
            // Need to process the division information prior to writing out the data.
            HtmlTable table = new HtmlTable();
            table.Rows.Add(GetHeaderRow());
            using (TextWriter writer = File.CreateText(fileName))
            {
                using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(writer))
                {
                    table.Rows.Add(CreateRaceTypeRow("TOP 3 MALES:"));
                    AddRows(table, top3Males, null);
                    table.Rows.Add(CreateRaceTypeRow("TOP 3 FEMALES:"));
                    AddRows(table, top3Females, null);
                    for (int ii = 0; ii < maleDivisions.Count; ii++)
                    {
                        List<AgeGradeResult> femaleResults = femaleDivisions[ii];
                        if (femaleResults != null && femaleResults.Count > 0)
                        {
                            int age = (ii * 5) + kMinimumEligibilityAge;
                            table.Rows.Add(CreateRaceTypeRow(String.Format("Queen {0}-{1}:", age, age + 4)));
                            AddRows(table, femaleResults, top3Females);
                        }
                        List<AgeGradeResult> maleResults = maleDivisions[ii];
                        if (maleResults != null && maleResults.Count > 0)
                        {
                            int age = (ii * 5) + kMinimumEligibilityAge;
                            table.Rows.Add(CreateRaceTypeRow(String.Format("King {0}-{1}:", age, age + 4)));
                            AddRows(table, maleResults, top3Males);
                        }
                    }
                    table.RenderControl(htmlTextWriter);
                }
            }
        }

        private HtmlTableRow GetEmptyRow(int cellCount)
        {
            HtmlTableRow emptyRow = new HtmlTableRow();
            for (int ii = 0; ii < cellCount; ii++)
            {
                HtmlTableCell emptyCell = GetHeaderCell(" ");
                emptyRow.Cells.Add(emptyCell);
            }
            return emptyRow;
        }

        private void AddEmptyRows(HtmlTable table, int rowCount, int cellCount)
        {
            if (table == null) return;
            for (int ii = 0; ii < rowCount; ii++)
            {
                HtmlTableRow row = GetEmptyRow(cellCount);
                table.Rows.Add(row);
            }
        }

        private void BuildMrMsMileageResults(List<AgeGradeResult> ageGradeResults)
        {
            if (ageGradeResults == null || ageGradeResults.Count <= 0) return;

            int kMrMsMileageCount = 10;
            string fileName = GetMrMsMileageFileName();
            // Need to process the division information prior to writing out the data.
            HtmlTable table = new HtmlTable();
            int count = Math.Min(kMrMsMileageCount, ageGradeResults.Count);
            ageGradeResults = ageGradeResults.OrderByDescending(x => x.TotalMiles).ToList<AgeGradeResult>();
            const int kEmptyRowCount = 6;
            const int kEmptyCellCount = 6;
            using (TextWriter writer = File.CreateText(fileName))
            {
                using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(writer))
                {
                    table.Rows.Add(GetMrMsMileageHeaderRow());
                    int genderCount = 0;
                    for (int ii = 0; ii < ageGradeResults.Count; ii++)
                    {
                        if (genderCount >= count) break;
                        AgeGradeResult result = ageGradeResults[ii];
                        if (result == null || result.IsMale) continue;
                        genderCount++;
                        AddMrMsMileageRow(table, result);
                    }
                    AddEmptyRows(table, kEmptyRowCount, kEmptyCellCount);
                    table.Rows.Add(GetMrMsMileageHeaderRow());
                    genderCount = 0;
                    for (int ii = 0; ii < ageGradeResults.Count; ii++)
                    {
                        if (genderCount >= count) break;
                        AgeGradeResult result = ageGradeResults[ii];
                        if (result == null || !result.IsMale) continue;
                        genderCount++;
                        AddMrMsMileageRow(table, result);
                    }
                    table.RenderControl(htmlTextWriter);
                }
            }
        }


        private bool InDivision(int age, int index)
        {
            bool inDivision = false;
            return inDivision;
        }

        private static List<List<AgeGradeResult>> CreateDivisions()
        {
            List<List<AgeGradeResult>> divsions = new List<List<AgeGradeResult>>();
            // Need to create age group division based on five year (15-19, 20-24, etc.) span.
            int startingAge = kMinimumEligibilityAge;
            for (int ii = startingAge; ii < 100; ii += 5)
            {
                List<AgeGradeResult> division = new List<AgeGradeResult>();
                divsions.Add(division);
            }
            return divsions;
        }

        public class AgeGradeResult
        {
            public AgeGradeResult(
                string firstName,
                string lastName,
                int age,
                bool isMale,
                double worldPercentage,
                int totalRaces,
                double milesForBestRaces,
                TimeSpan normalizedTime,
                double totalMiles
                )
            {
                FirstName = firstName;
                LastName = lastName;
                Age = age;
                IsMale = isMale;
                WorldPercentage = worldPercentage;
                TotalRaces = totalRaces;
                MilesForBestRaces = milesForBestRaces;
                NormalizedTime = normalizedTime;
                TotalMiles = totalMiles;
            }

            private double mTotalMiles;
            public double TotalMiles
            {
                get { return mTotalMiles; }
                set { mTotalMiles = value; }
            }

            private int mPlace;
            public int Place
            {
                get { return mPlace; }
                set { mPlace = value; }
            }

            private bool mIsMale;
            public bool IsMale
            {
                get { return mIsMale; }
                set { mIsMale = value; }
            }

            private string mFirstName;
            public string FirstName
            {
                get { return mFirstName; }
                set { mFirstName = value; }
            }

            private string mLastName;
            public string LastName
            {
                get { return mLastName; }
                set { mLastName = value; }
            }

            private int mAge;
            public int Age
            {
                get { return mAge; }
                set { mAge = value; }
            }

            private int mDivisionPlace;
            public int DivisionPlace
            {
                get { return mDivisionPlace; }
                set { mDivisionPlace = value; }
            }

            private double mWorldPercentage;
            public double WorldPercentage
            {
                get { return mWorldPercentage; }
                set { mWorldPercentage = value; }
            }

            private int mTotalRaces;
            public int TotalRaces
            {
                get { return mTotalRaces; }
                set { mTotalRaces = value; }
            }

            private double mMilesForBestRaces;
            public double MilesForBestRaces
            {
                get { return mMilesForBestRaces; }
                set { mMilesForBestRaces = value; }
            }

            private TimeSpan mNormalizedTime;
            public TimeSpan NormalizedTime
            {
                get { return mNormalizedTime; }
                set { mNormalizedTime = value; }
            }

            private int mTotalAgeGroupCount;
            public int TotalAgeGroupCount
            {
                get { return mTotalAgeGroupCount; }
                set { mTotalAgeGroupCount = value; }
            }

            public override string ToString()
            {
                return String.Format("{0} {1} {2} {3} {4} {5} {6}",
                    Age, FirstName, LastName,
                    AgeGradingTables.GetMinimumSpan(NormalizedTime), Math.Round(WorldPercentage, 2),
                    TotalRaces, Math.Round(MilesForBestRaces, 3));
            }
        }

        public class AgeGradeResultComparer : IComparer<AgeGradeResult>
        {
            public AgeGradeResultComparer()
            {
            }

            public AgeGradeResultComparer(bool compareNormalizedTime)
            {
                CompareNormalizedTime = compareNormalizedTime;
            }

            private bool mCompareNormalizedTime;
            public bool CompareNormalizedTime
            {
                get { return mCompareNormalizedTime; }
                set { mCompareNormalizedTime = value; }
            }

            public int Compare(AgeGradeResult x, AgeGradeResult y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                if (CompareNormalizedTime)
                {
                    return ResultRowComparer.CompareTimes(x.NormalizedTime, y.NormalizedTime);
                }
                return (x.Age - y.Age);
            }
        }

        private const string kPlaceIndexHolder = "PLACEINDEXHOLDER";
        public class ResultRow
        {
            public ResultRow(HtmlTableRow row, TimeSpan normalizedTime, string firstName, string lastName)
            {
                Row = row;
                NormalizedTime = normalizedTime;
                FirstName = firstName;
                LastName = lastName;
            }

            private string mFirstName;
            public string FirstName
            {
                get { return mFirstName; }
                set { mFirstName = value; }
            }

            private string mLastName;
            public string LastName
            {
                get { return mLastName; }
                set { mLastName = value; }
            }

            private HtmlTableRow mRow;
            public HtmlTableRow Row
            {
                get { return mRow; }
                set { mRow = value; }
            }

            private TimeSpan mNormalizedTime;
            public TimeSpan NormalizedTime
            {
                get { return mNormalizedTime; }
                set { mNormalizedTime = value; }
            }
        }

        public class ResultRowComparer : IComparer<ResultRow>
        {
            public int Compare(ResultRow x, ResultRow y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return CompareTimes(x.NormalizedTime, y.NormalizedTime);
            }

            public static int CompareTimes(TimeSpan x, TimeSpan y)
            {
                double difference = x.TotalSeconds * 10 - y.TotalSeconds * 10;
                return Convert.ToInt32(difference);
            }
        }

        private string GetRaces(DataRow row, List<string> KQRaces, out List<string> rows
            , out double totalMiles
            , out TimeSpan averageNormalizedTime
            , out double averageWorldPercent
            , out List<ResultCalculator.RaceResult> raceResults
            )
        {
            raceResults = null;
            totalMiles = 0;
            averageNormalizedTime = TimeSpan.Zero;
            averageWorldPercent = 0.0;
            rows = new List<string>();
            if (row == null || KQRaces == null || KQRaces.Count <= 0) return String.Empty;
            StringBuilder str = new StringBuilder();
            int count = 0;
            double normalizedTotal = 0.0;
            double worldPercentTotal = 0.0;
            foreach (string race in KQRaces)
            {
                RaceInfo raceInfo = RaceInfo.FindRace(race);
                if (raceInfo == null) continue;

                object value = row[race];
                if (value == null) continue;
                string theRace = value.ToString();
                if (String.IsNullOrEmpty(theRace)) continue;
                string raceTime = String.Empty;
                string normalizedTime = String.Empty;
                string worldPercentage = String.Empty;
                if (!GetRaceTimes(theRace, out raceTime, out normalizedTime, out worldPercentage)) continue;
                string KQInfo = String.Format("{0}: {1} converts to {2} ({3}%)", race, raceTime, normalizedTime, worldPercentage);
                TimeSpan normalizedTimeSpan;
                if (!GetTimeSpan(normalizedTime, out normalizedTimeSpan)) continue;
                double worldPercent = 0.0;
                if (!double.TryParse(worldPercentage, out worldPercent)) continue;
                double distance = raceInfo.GetMiles();
                ResultCalculator.RaceResult raceResult = new ResultCalculator.RaceResult(race, distance, normalizedTimeSpan, worldPercent);
                if (raceResults == null) raceResults = new List<ResultCalculator.RaceResult>();
                raceResults.Add(raceResult);
                count++;
                totalMiles += distance;
                normalizedTotal += normalizedTimeSpan.TotalSeconds;
                worldPercentTotal += worldPercent;
                rows.Add(KQInfo);
                str.AppendLine(KQInfo);
            }
            if (count > 0)
            {
                totalMiles = Math.Round(totalMiles, 3);
                averageNormalizedTime = AgeGradingTables.RoundSeconds(TimeSpan.FromSeconds(normalizedTotal / count));
                averageWorldPercent = Math.Round(worldPercentTotal / count, 2);
            }
            return str.ToString();
        }

        private List<string> HtmlEnd(ref StringBuilder str)
        {
            if (str == null) str = new StringBuilder();
            List<string> end = new List<string>();
#if ADD_FULL_PAGE
            end.Add("<!--");
            end.Add("</form>");
            end.Add("-->");
            end.Add("</body>");
            end.Add("</html>");
            foreach (string item in end)
            {
                str.AppendLine(item);
            }
#endif
            return end;
        }

        private const string kRaceUpdate = "RACEANDDATEOFUPDATES";
        private List<string> HtmlStart(ref StringBuilder str)
        {
            if (str == null) str = new StringBuilder();
            List<string> start = new List<string>();
#if ADD_FULL_PAGE
            start.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            start.Add("");
            start.Add("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            start.Add("<head><title>");
            start.Add("	SFTC King &amp; Queen Competition 2012");
            start.Add("</title><link rel=\"stylesheet\" type=\"text/css\"  href=\"http://www.runtricities.org/css/main.css\" /></head>");
            start.Add("");
            start.Add("<body>");
            start.Add("");
            start.Add("<!--");
            start.Add("    <form name=\"form1\" method=\"post\" action=\"Default.aspx\" id=\"form1\">");
            start.Add("-->");
            start.Add("");
#endif
#if ADD_FULL_PAGE
            start.Add("<table border=\"0\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\">");
            start.Add("<tr>");
            start.Add("");
            start.Add("<td width=\"40%\" align=\"center\">");
            start.Add("<h2>");
            start.Add("King &amp; Queen Competition 2012<br/>");
            start.Add("    sponsored by Foot Rx JC");
            start.Add("</h2>");
            start.Add("");
            start.Add("<p/>");
            start.Add("<font face=\"Arial\">");
            start.Add("<em>");
            start.Add("");
            start.Add("    (1) Run six K & Q races that total at least 25 miles, and <br/>");
            start.Add("    (2) Work club events to obtain five volunteer service points.");
            start.Add("</em>");
            start.Add("</font>");
            start.Add("</td>");
            start.Add("");
            start.Add("    ");
            //start.Add("<td width=\"31%\" align=\"center\">");
            //start.Add("    <img border=\"0\" src=\"crown1.gif\" width=\"100\" height=\"75\"/>");
            //start.Add("    <img border=\"0\" src=\"FootRx.gif\">");
            //start.Add("</td>");
            start.Add("");
            start.Add("</tr>");
            start.Add("</table>");
            start.Add("");
            start.Add("");
            start.Add("<hr/>");
            start.Add("<table border=\"0\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\">");
            start.Add("<tr>");
            start.Add("<td valign=\"top\" align=\"center\">");
            start.Add("");
            start.Add("        ");
            //start.Add("<script language=\"javascript\" type=\"text/javascript\">");
            //start.Add("//	document.write('<ul><li><a href=\"11_kq.htm?' +  +(new Date()).getMilliseconds() + Math.floor(Math.random()*1001) + '\"> Age Groups</a></li>');");
            //start.Add("//	document.write('<ul><li><a href=\"09_kq_overall.htm?' +  +(new Date()).getMilliseconds() + Math.floor(Math.random()*1001) + '\"> Overall Standings (age graded only)</a></li>');");
            //start.Add("//	document.write('<li><a href=\"09_kq_overall_sex.htm?' +  +(new Date()).getMilliseconds() + Math.floor(Math.random()*1001) + '\"> Overall Standings (age & sex graded)</a></li>');");
            //start.Add("</script>");
            start.Add("");
            start.Add("<li><a href=\"mailto:kq@runtricities.org?subject=K&amp;Q Questions, errors or comments\">  Questions, errors or comments? Contact K&amp;Q coordinator</a></li>");
            //start.Add("</ul>");
            start.Add("</td>");
            start.Add("</tr>");
            start.Add("</table>");
            start.Add("<table border=\"0\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\">");
            start.Add("<tr>");
            start.Add("<td valign=\"top\" align=\"center\">");
            start.Add("");
            start.Add("<font class=\"site_h4\" style=\"color:black\">");
            start.Add("Any SFTC member wanting to compete in the King &amp; Queen competition must now sign up by sending an e-mail to ");
            start.Add("<a href=\"mailto:kq@runtricities.org?subject=K&amp;Q SIGNUP\">K&amp;Q coordinator</a> (only the races you run after you sign-up will count toward your K&amp;Q score).");
            start.Add("</font>");         
            start.Add("</td>");
            start.Add("</tr>");
            start.Add("</table>");
            start.Add("");
            start.Add("<p/>");
            start.Add("<hr/>");
            start.Add("<p/>");
            //start.Add("<style type=\"text/css\">");
            //start.Add("ol");
            //start.Add("{");
            //start.Add("display:list-item");
            //start.Add("}");
            //start.Add("</style>");
#endif
            foreach (string item in start)
            {
                str.AppendLine(item);
            }
            return start;
        }

        private List<string> HtmlIndividualStart(ref StringBuilder str)
        {
            if (str == null) str = new StringBuilder();
            List<string> start = new List<string>();
#if ADD_FULL_PAGE
            start.Add(String.Format("<font class=\"site_h4\">Individual Race Results: After {0}</font>", kRaceUpdate));
            start.Add("");
            start.Add("<p/>");
            start.Add("<h5>NOTE:");
            start.Add("<ol>");
            start.Add("Click on a row to show all race results for that individual.<br>");
            start.Add("Click on the row again to hide the results for that individual.");
            start.Add("</ol>");
            start.Add("</h5>");
            start.Add("");
#endif
            start.Add("<script language='javascript'>");
            start.Add("");
            start.Add("function myMenu_sets(lv_str) {");
            start.Add("	if (document.getElementById(lv_str).style.display == \"inline\") {");
            start.Add("		document.getElementById(lv_str).style.display = \"none\";");
            start.Add("		document.getElementById(lv_str + 1).style.display = \"none\";");
            start.Add("	} else {");
            start.Add("		document.getElementById(lv_str).style.display = \"inline\";");
            start.Add("		document.getElementById(lv_str + 1).style.display = \"inline\";");
            start.Add("	}");
            start.Add("}");
            start.Add("");
            start.Add("</script>");
            start.Add("<style>");
            start.Add("ol");
            start.Add("{");
            start.Add("  list-style: decimal;");
            start.Add("}");
            start.Add("</style>");
            start.Add("");
            start.Add("");
            start.Add("<table border=0 cellpadding=2 cellspacing=2>");
            start.Add("");
            start.Add("<tr valign=top bgcolor=#DBDBDD>");
            start.Add("<th scope=\"col\" style=\"width:200px;\"></th>");
            start.Add("");
            start.Add("<th scope=\"col\" style=\"width:375px;\">Race times normalized to 10K</th>");
            start.Add("<th scope=\"col\" style=\"width:275px;\">Calculated best six normalized times</th>");
            start.Add("</tr>");
            foreach (string item in start)
            {
                str.AppendLine(item);
            }
            return start;
        }

        public void SaveTable()
        {
            if (mResultsTable == null) return;
            string fileName = GetFileName();
            try
            {
                if (String.IsNullOrEmpty(fileName)) return;
                mResultsTable.WriteXml(fileName, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private DataRow AddDataRow(
            DataTable dataTable,
            string lastName,
            string firstName,
            string firstNameAliases,
            string lastNameAliases,
            string gender,
            DateTime dateOfBirth
            )
        {
            if (dataTable == null) return null;
            DataRow dataRow = mResultsTable.NewRow();
            dataRow[kLastName] = lastName;
            dataRow[kFirstName] = firstName;
            dataRow[kFirstNameAliases] = firstNameAliases;
            dataRow[kLastNameAliases] = lastNameAliases;
            dataRow[kGender] = gender;
            dataRow[kDateOfBirth] = dateOfBirth;
            dataRow[kTotalMilesForSixRaces] = 0.0;
            dataRow[kAverageNormalizeTimesForSixRaces] = 0.0;
            dataRow[kAverageWorldPercentageForSixRaces] = 0.0;
            return dataRow;
        }

        private const string kKQResultsFileName = "KQResults.xml";
        private DataTable mResultsTable;
        private void AddDataColumn(DataTable dataTable, string name, Type dataType)
        {
            if (dataTable == null) return;
            if (String.IsNullOrEmpty(name)) return;
            if (dataType == null) return;
            DataColumn column = mResultsTable.Columns[name];
            if (column != null) return;
            DataColumn dataColumn = mResultsTable.Columns.Add(name);
            dataColumn.DataType = dataType;
        }

        private const string kTableName = "King and Queen Competition";
        private DataTable GetCompetitionDataTable()
        {
            mResultsTable = new DataTable(kTableName);
            AddDataColumn(mResultsTable, kLastName, typeof(string));
            AddDataColumn(mResultsTable, kFirstName, typeof(string));
            AddDataColumn(mResultsTable, kFirstNameAliases, typeof(string));
            AddDataColumn(mResultsTable, kLastNameAliases, typeof(string));
            AddDataColumn(mResultsTable, kGender, typeof(string));
            AddDataColumn(mResultsTable, kDateOfBirth, typeof(DateTime));
            AddDataColumn(mResultsTable, kTotalMilesForSixRaces, typeof(double));
            AddDataColumn(mResultsTable, kAverageNormalizeTimesForSixRaces, typeof(double));
            AddDataColumn(mResultsTable, kAverageWorldPercentageForSixRaces, typeof(double));
            List<string> races = GetKQRaces();
            if (races != null && races.Count > 0)
            {
                foreach (string race in races)
                {
                    if (String.IsNullOrEmpty(race)) continue;
                    AddDataColumn(mResultsTable, race, typeof(string));
                }
            }
            List<DataColumn> dataColumns = new List<DataColumn>();
            dataColumns.Add(mResultsTable.Columns[kLastName]);
            dataColumns.Add(mResultsTable.Columns[kFirstName]);
            dataColumns.Add(mResultsTable.Columns[kGender]);
            mResultsTable.PrimaryKey = dataColumns.ToArray<DataColumn>();
            return mResultsTable;
        }

        private DataTable GetResultsTable(string tableName)
        {
            if (String.IsNullOrEmpty(tableName)) return null;
            DataTable dataTable = new DataTable(tableName);
            AddDataColumn(dataTable, kLastName, typeof(string));
            AddDataColumn(dataTable, kFirstName, typeof(string));
            AddDataColumn(dataTable, kFirstNameAliases, typeof(string));
            AddDataColumn(mResultsTable, kLastNameAliases, typeof(string));
            AddDataColumn(dataTable, kGender, typeof(string));
            AddDataColumn(dataTable, kDateOfBirth, typeof(DateTime));
            AddDataColumn(dataTable, kTotalMilesForSixRaces, typeof(double));
            AddDataColumn(dataTable, kAverageNormalizeTimesForSixRaces, typeof(double));
            AddDataColumn(dataTable, kAverageWorldPercentageForSixRaces, typeof(double));
            List<string> races = GetKQRaces();
            if (races != null && races.Count > 0)
            {
                foreach (string race in races)
                {
                    if (String.IsNullOrEmpty(race)) continue;
                    AddDataColumn(dataTable, race, typeof(string));
                }
            }
            return dataTable;
        }

        internal static List<string> GetKQRaces()
        {
            BuildKQRaces();
            List<string> races = null;
            races = mKingAndQueenRaces.Keys.ToList<string>();
            return races;
        }

        internal static void FillRaceTypes(ComboBox combo)
        {
            if (combo == null) return;
            combo.Items.Clear();
            List<string> races = GetKQRaces();
            foreach (string race in races)
            {
                combo.Items.Add(race);
            }
        }

        private class RaceTypeToDistance
        {
            /// <summary>
            /// Intializes a RaceToDistance object.
            /// </summary>
            /// <param name="raceType">The race type used in comparison.</param>
            /// <param name="distance">The distance in kilometers.</param>
            public RaceTypeToDistance(string raceType, double distance)
            {
                RaceType = raceType;
                Distance = distance;
            }

            private string mRaceType;
            public string RaceType
            {
                get { return mRaceType; }
                set { mRaceType = value; }
            }

            private double mDistance;
            public double Distance
            {
                get { return mDistance; }
                set { mDistance = value; }
            }

        }

        private static Dictionary<string, RaceTypeToDistance> mKingAndQueenRaces;
        private static void BuildKQRaces()
        {
            if (mKingAndQueenRaces != null && mKingAndQueenRaces.Count > 0) return;
            mKingAndQueenRaces = new Dictionary<string, RaceTypeToDistance>();
            // All distances are in kilometers.
            List<RaceInfo> races = RaceInfo.GetRaces();
            foreach (RaceInfo raceInfo in races)
            {
                mKingAndQueenRaces[raceInfo.Name] = new RaceTypeToDistance(raceInfo.AgeGradeTable, raceInfo.GetKilometers());
            }
#if NEED_TO_CHECK_RACES
            mKingAndQueenRaces["Up & At 'Em Turkey Trot 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Pioneer 5 Miler"] = new RaceTypeToDistance("5MileRoad", 8.04672);
            mKingAndQueenRaces["New Year's Day Wake Up 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["War Party 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Surgoinsville 10 Miler"] = new RaceTypeToDistance("10Mile", 16.09344);
            mKingAndQueenRaces["5K Run for St Anne School"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Chasing Snakes 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Phipps Bend River Run"] = new RaceTypeToDistance("10Mile", 17.702784); // 10 miles
            mKingAndQueenRaces["Creeper 10 Miler"] = new RaceTypeToDistance("10Mile", 16.09344);
            mKingAndQueenRaces["Take Back the Night 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Golden Eagle 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Laurel Run Ascent"] = new RaceTypeToDistance("11Mile", 17.702784);// 11 miles
            mKingAndQueenRaces["Run the Tunnel"] = new RaceTypeToDistance("3.8Mile", 6.1155072);// 3.8 miles
            mKingAndQueenRaces["Animal Chase 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Animal Chase 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Foot Rx 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Justin Foundation 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["CASA 8K"] = new RaceTypeToDistance("8kmRoad", 8);

            mKingAndQueenRaces["Phipps Bend 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Mountain States Rehab 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Varmint Half Marathon"] = new RaceTypeToDistance("Half.Mar", 21.0975);
            mKingAndQueenRaces["Rhododendron 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Firecracker 4 miler"] = new RaceTypeToDistance("4MileRoad", 6.437376);
            mKingAndQueenRaces["Red, White & Boom 4 Miler"] = new RaceTypeToDistance("4MileRoad", 6.437376);
            mKingAndQueenRaces["Crazy 8s 8K"] = new RaceTypeToDistance("8kmRoad", 8);
            mKingAndQueenRaces["Wolf Run 7 Miler"] = new RaceTypeToDistance("7Mile", 11.265408);// 7 Miles
            mKingAndQueenRaces["Railroad Days 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Schoolhouse 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Greene County YMCA 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Eastman 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Baileyton Celebration 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Rhythm & Roots 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Bays Mountain Trail Race"] = new RaceTypeToDistance("15Mile", 24.14016);// 15 miles
            mKingAndQueenRaces["Apple Festival 4 Miler"] = new RaceTypeToDistance("4MileRoad", 6.437376);
            mKingAndQueenRaces["HeartOne 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Tri-Cities Race for the Cure 5K"] = new RaceTypeToDistance("5kmRoad", 5);
            mKingAndQueenRaces["Roan Mountain 10K"] = new RaceTypeToDistance("10kmRoad", 10);
            mKingAndQueenRaces["Veterans Day Classic Half Marathon"] = new RaceTypeToDistance("Half.Mar", 21.0975);
            mKingAndQueenRaces["Santa Special Open Mile"] = new RaceTypeToDistance("1Mile", 1.609);
            foreach (RaceInfo info in races)
            {
                if (info == null) continue;
                if (!mKingAndQueenRaces.ContainsKey(info.Name))
                {
                    Debug.WriteLine(String.Format("raceInfo {0} is not in mKingAndQueenRaces", info.Name));
                    continue;
                }
                RaceTypeToDistance typeToDistance = mKingAndQueenRaces[info.Name];
                if (!String.Equals(typeToDistance.RaceType, info.AgeGradeTable, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine(String.Format("RaceType {0} does not match not in mKingAndQueenRaces RaceType {1}", info.AgeGradeTable, typeToDistance.RaceType));
                    continue;
                }
            }
            foreach (KeyValuePair<string, RaceTypeToDistance> pair in mKingAndQueenRaces)
            {
                //RaceInfo raceInfo = races.Find(s => s.Name.Equals(pair.Value, StringComparison.OrdinalIgnoreCase));
                RaceInfo raceInfo = races.Find(
                delegate(RaceInfo bk)
                {
                    return bk.Name == pair.Key;
                }
                );
                if (raceInfo == null)
                {
                    Debug.WriteLine(String.Format("mKingAndQueenRaces {0} is not in raceInfo", pair.Key));
                    continue;
                }
            }
#endif
        }

        public class VolunteerPointsList : List<VolunteerPoints>
        {
            public VolunteerPoints Find(string lastName, string firstName)
            {
                if (String.IsNullOrEmpty(lastName)) return null;
                if (String.IsNullOrEmpty(firstName)) return null;
                foreach (VolunteerPoints item in this)
                {
                    if (item == null) continue;
                    if (item.Equals(lastName, firstName)) return item;
                }
                return null;
            }
        }

        private VolunteerPoints FindVolunteerPoints(string lastName, string firstName)
        {
            InitializeVolunteePoints();
            if (mVolunteerPointsList == null || mVolunteerPointsList.Count <= 0) return null;
            return mVolunteerPointsList.Find(lastName, firstName);
        }

        private const int kRequiredVolunteerPoints = 5;
        private VolunteerPointsList mVolunteerPointsList;
        public void ClearVolunteerPoints()
        {
            mVolunteerPointsList = null;
        }

        private void InitializeVolunteePoints()
        {
            if (mVolunteerPointsList != null) return;
            const string kVolunteerPointsFileName = "VolunteerPoints2016.txt";
            string columnList = "\t";
            mVolunteerPointsList = new VolunteerPointsList();
            AgeGradeTextReader textReader = new AgeGradeTextReader();
            string fileName = String.Format("{0}{1}{2}", AgeGradingForm.GetFolderPath(), Path.DirectorySeparatorChar, kVolunteerPointsFileName);
            List<List<string>> rowColumns = textReader.GetDelimitedLines(fileName, -1, 0, columnList);
            char[] spaces = " ".ToCharArray();
            char[] commas = ",".ToCharArray();
            for (int ii = 0; ii < rowColumns.Count; ii++)
            {
                List<string> volunteerPoints = rowColumns[ii];
                if (volunteerPoints == null) continue;
                Debug.Assert(volunteerPoints.Count == 2 || volunteerPoints.Count == 3, "Wrong column count");
                if (volunteerPoints.Count != 2 && volunteerPoints.Count != 3) continue;
                string lastName = String.Empty;
                string firstName = String.Empty;
                int points = 0;
                if (volunteerPoints.Count == 2)
                {
                    string[] split = volunteerPoints[0].Split(commas, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length != 2)
                    {
                        // Try splitting the names by a space instead of a comma.
                        split = volunteerPoints[0].Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                    }
                    Debug.Assert(split.Length == 2, "Wrong name count");
                    if (split.Length != 2) continue;
                    if (!int.TryParse(volunteerPoints[1], out points))
                    {
                        Debug.Assert(false, "Can't convert points from file");
                        continue;
                    }
                    lastName = split[0].Trim();
                    string[] lastNameSplit = lastName.Split(spaces, StringSplitOptions.RemoveEmptyEntries);
                    if (lastNameSplit != null && lastNameSplit.Length > 1)
                        lastName = lastNameSplit[lastNameSplit.Length - 1];
                    firstName = split[1].Trim();
                }
                else if (volunteerPoints.Count == 3)
                {
                    lastName = volunteerPoints[0].Trim();
                    firstName = volunteerPoints[1].Trim();
                    if (!int.TryParse(volunteerPoints[2], out points))
                    {
                        Debug.Assert(false, "Can't convert points from file");
                        continue;
                    }
                }
                Debug.Assert(!String.IsNullOrEmpty(lastName), "Last name is empty");
                Debug.Assert(!String.IsNullOrEmpty(firstName), "First name is empty");
                // Throw out points less than the required.
                if (points < kRequiredVolunteerPoints) continue;
                mVolunteerPointsList.Add(new VolunteerPoints(lastName.ToUpper(), firstName.ToUpper(), points));
            }
            DataTable memberTable = GetMemberTable();
            Debug.Assert(memberTable != null, "Bad member table");
            if (memberTable == null) return;
            Dictionary<VolunteerPoints, DataRow> nameLookup = new Dictionary<VolunteerPoints, DataRow>();
            foreach (DataRow row in memberTable.Rows)
            {
                if (row == null) continue;
                string lastName = row[kLastName] as string;
                string firstName = row[kFirstName] as string;
                VolunteerPoints points = new VolunteerPoints(lastName, firstName, 0);
                nameLookup[points] = row;
            }

            HashSet<VolunteerPoints> missing = new HashSet<VolunteerPoints>();
            foreach (var item in mVolunteerPointsList)
            {
                if (item == null) continue;
                if (nameLookup.ContainsKey(item))
                {
                    // If we find a match remove it from the dictionary
                    // so it eliminates extra comparisons later.
                    nameLookup.Remove(item);
                    continue; 
                }
                missing.Add(item);
            }
            if (missing != null && missing.Count > 0)
            {
                int foundCount = 0;
                foreach (KeyValuePair<VolunteerPoints, DataRow> pair in nameLookup)
                {
                    if (missing.Count == 0) break;
                    VolunteerPoints points = pair.Key;
                    string pairFirstName = points.FirstName;
                    string pairLastName = points.LastName;
                    DataRow row = pair.Value;
                    List<VolunteerPoints> list = missing.ToList<VolunteerPoints>();
                    if (points.FirstName.StartsWith("TONY"))
                    {

                    }
                    if (
                        points.LastName.StartsWith("LEW") ||
                        points.LastName.StartsWith("WAG") ||
                        points.LastName.StartsWith("NIEL") ||
                        points.LastName.StartsWith("MCCOR") ||
                        points.LastName.StartsWith("ERCHIN") ||
                        points.LastName.StartsWith("RICK") ||
                        points.LastName.StartsWith("BORG")
                        )
                    {

                    }
                    foreach (VolunteerPoints notFound in list)
                    {
                        int matchValue = CompareMissingMembersDlg.LevenshteinDistance(points.ToString(), notFound.ToString());
                        if (matchValue > 10) continue;
                        if (matchValue == 1)
                        {
                            foundCount = ReplaceVolunteerPoints(foundCount, points, notFound);
                            missing.Remove(notFound);
                            break;
                        }
                        string lastNameAliases = row[kLastNameAliases] as string;
                        string firstNameAliases = row[kFirstNameAliases] as string;
                        string notFoundFirstName = notFound.FirstName;
                        string notFoundLastName = notFound.LastName;
                        // If the match difference is in the last name time to skip.
                        int lastNameMatchValue = 0;
                        bool lastNamesDifferent = CompareNames(matchValue, pairLastName, notFoundLastName, out lastNameMatchValue);
                        if (lastNameMatchValue == 0)
                        {
                            // Compare the alternative first names.
                            if (AliasMatches(notFound.FirstName, firstNameAliases))
                            {
                                foundCount = ReplaceVolunteerPoints(foundCount, points, notFound);
                                missing.Remove(notFound);
                                break;
                            }
                        }
                        
                        // If the match difference is in the first name time to skip.
                        int firstNameMatchValue = 0;
                        bool firstNamesDifferent = CompareNames(matchValue, pairFirstName, notFoundFirstName, out firstNameMatchValue);
                        if (firstNameMatchValue == 0)
                        {
                            // Compare the alternative first names.
                            if (AliasMatches(notFound.LastName, lastNameAliases))
                            {
                                foundCount = ReplaceVolunteerPoints(foundCount, points, notFound);
                                missing.Remove(notFound);
                                break;
                            }
                        }
                        // Both could be misspelled.
                        if (AliasMatches(notFound.FirstName, firstNameAliases) && AliasMatches(notFound.LastName, lastNameAliases))
                        {
                            foundCount = ReplaceVolunteerPoints(foundCount, points, notFound);
                            missing.Remove(notFound);
                            break;
                        }
                    }
                }
                if (missing.Count > 0)
                {
                    MissingVolunteer dlg = new MissingVolunteer();
                    dlg.MissingMembers = missing;
                    dlg.ShowDialog();
                    foreach (VolunteerPoints item in missing)
                    {
                        Debug.WriteLine(String.Format("still missing {0}", item.ToString()));
                    }
                }
            }
        }

        private int ReplaceVolunteerPoints(int foundCount, VolunteerPoints points, VolunteerPoints notFound)
        {
            foundCount++;
            mVolunteerPointsList.Remove(notFound);
            points.TotalPoints = notFound.TotalPoints;
            mVolunteerPointsList.Add(points);
            return foundCount;
        }

        private static bool AliasMatches(string name, string aliases)
        {
            if (String.IsNullOrEmpty(name)) return false;
            if (String.IsNullOrEmpty(aliases)) return false;
            name = name.Trim();
            string[] list = aliases.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (list == null || list.Length <= 0) return false;
            foreach (string item in list)
            {
                string alias = item.Trim();
                if (String.Equals(alias, name, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private static bool CompareNames(int matchValue, string pairFirstName, string notFoundFirstName, out int nameMatchValue)
        {
            nameMatchValue = CompareMissingMembersDlg.LevenshteinDistance(pairFirstName, notFoundFirstName);
            return (nameMatchValue == matchValue);
        }

        private static string UpdateDictionary(Dictionary<string, DataRow> nameLookup, DataRow row, string firstName, string lastName)
        {
            lastName = lastName.Trim();
            firstName = firstName.Trim();
            string fullName = CombineNames(firstName, lastName);
            if (!String.IsNullOrEmpty(fullName))
            {
                nameLookup[fullName] = row;
            }
            return lastName;
        }

        private const string kCombinedNamesSeparator = ":";
        private static string CombineNames(string firstName, string lastName)
        {
            if (firstName == null) firstName = String.Empty;
            if (lastName == null) lastName = String.Empty;
            return String.Format("{0}{1}{2}", firstName, kCombinedNamesSeparator, lastName);
        }

        private static string GetLastName(string combinedName, out string firstName)
        {
            firstName = String.Empty;
            if (String.IsNullOrEmpty(combinedName)) return String.Empty;
            string[] split = combinedName.Split(kCombinedNamesSeparator.ToCharArray());
            if (split == null || split.Length != 2) return String.Empty;
            firstName = split[0];
            return split[1];
        }
    }
    
    public class VolunteerPoints
    {
        private string mLastName;
        public string LastName
        {
            get { return mLastName; }
            set { mLastName = value; }
        }

        private string mFirstName;
        public string FirstName
        {
            get { return mFirstName; }
            set { mFirstName = value; }
        }

        private int mTotalPoints;
        public int TotalPoints
        {
            get { return mTotalPoints; }
            set { mTotalPoints = value; }
        }

        public VolunteerPoints(string lastName, string firstName, int totalPoints)
        {
            this.LastName = lastName;
            this.FirstName = firstName;
            this.TotalPoints = totalPoints;
        }

        public bool Equals(string lastName, string firstName)
        {
            if (String.IsNullOrEmpty(lastName)) return false;
            if (String.IsNullOrEmpty(firstName)) return false;
            bool lastNameMatches = String.Equals(lastName, this.LastName, StringComparison.OrdinalIgnoreCase);
            bool firstNameMatches = String.Equals(firstName, this.FirstName, StringComparison.OrdinalIgnoreCase);
            return (firstNameMatches && lastNameMatches);
        }

        public override string ToString()
        {
            string firstName = (FirstName != null ? FirstName : String.Empty);
            string lastName = (LastName != null ? LastName : String.Empty);
            return String.Format("{0} {1}", firstName, lastName);
        }

        public override int GetHashCode()
        {
            int hashCode = ToString().ToUpper().GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            VolunteerPoints points = obj as VolunteerPoints;
            if (obj == null) return false;
            return (this.GetHashCode() == points.GetHashCode());
        }
    }

}
