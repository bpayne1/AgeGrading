using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace AgeGrading
{
    public class ResultCalculator
    {
        public class RaceResult
        {
            public RaceResult(string name, double distance, TimeSpan normalized, double worldpercent)
            {
                Name = name;
                Distance = distance;
                Normalized = normalized;
                WorldPercent = worldpercent;
            }

            private double mWorldPercent;
            public double WorldPercent
            {
                get { return mWorldPercent; }
                set { mWorldPercent = value; }
            }

            private string mName;
            public string Name
            {
                get { return mName; }
                set { mName = value; }
            }

            private double mDistance;
            public double Distance
            {
                get { return mDistance; }
                set { mDistance = value; }
            }

            private TimeSpan mNormalized;
            public TimeSpan Normalized
            {
                get { return mNormalized; }
                set { mNormalized = value; }
            }

            public override string ToString()
            {
                return String.Format("{0}:{1}", Name, Normalized.TotalSeconds);
            }
        }

        private const int kRequiredRaceCount = 6;
        public static int RequiredRaceCount
        {
            get { return kRequiredRaceCount; }
        }

        private const int kMinimumRaceDistance = 25;
        public static int MinimumRaceDistance
        {
            get { return kMinimumRaceDistance; }
        }

        static double FindTotalSeconds(List<RaceResult> races, out double totalDistance)
        {
            totalDistance = 0;
            if (races == null || races.Count != kRequiredRaceCount) return 0;
            double totalSeconds = 0.0;
            foreach (RaceResult item in races)
            {
                totalDistance += item.Distance;
                totalSeconds += item.Normalized.TotalSeconds;
            }
            double bestNormalizedTimes = totalSeconds / races.Count;
            return bestNormalizedTimes;
        }

        public static List<RaceResult> GetAllCombination(List<RaceResult> races, int ofLength, out double lowestNormalizedTime, out double totalRaceDistance, out double averageWorldPercentage)
        {
            averageWorldPercentage = 0;
            lowestNormalizedTime = double.MaxValue;
            totalRaceDistance = 0;
            if (races == null || (races.Count < ofLength)) return null;
            int len = races.Count;
            DateTime now = DateTime.Now;
            List<UInt64> lists = GetBooleanLists(len, ofLength);
            if (lists == null || lists.Count <= 0) return null;
            List<List<RaceResult>> raceLists = new List<List<RaceResult>>();
            TimeSpan span = DateTime.Now - now;
            //Debug.WriteLine(String.Format("call GetBooleanLists took {0} millseconds", span.TotalMilliseconds));
            now = DateTime.Now;
            List<RaceResult> lowestRaceList = null;
            foreach (UInt64 combination in lists)
            {
                if (combination == 0) continue;
                UInt64 mask = combination;
                List<RaceResult> raceCombos = new List<RaceResult>();
                for (int ii = 0; ii < len; ii++)
                {
                    if ((mask & 1) == 1) raceCombos.Add(races[ii]);
                    mask = mask >> 1;
                    if (mask == 0) break;
                }
                double totalDistance = 0;
                double normalizedTime = FindTotalSeconds(raceCombos, out totalDistance);
                if (totalDistance >= 25)
                {
                    if (normalizedTime < lowestNormalizedTime)
                    {
                        totalRaceDistance = totalDistance;
                        lowestNormalizedTime = normalizedTime;
                        lowestRaceList = raceCombos;
                    }
                }
            }
            span = DateTime.Now - now;
            //Debug.WriteLine(String.Format("building lists took {0} millseconds", span.TotalMilliseconds));
            return lowestRaceList;
        }

        static List<bool> GetFalseList(int listLength)
        {
            if (listLength <= 0) return null;
            List<bool> falseList = new List<bool>();
            for (int ii = 0; ii < listLength; ii++)
            {
                falseList.Add(false);
            }
            return falseList;
        }

#if USE_OLD
        #region BOOLEAN LISTS
        static private Dictionary<int, List<UInt64>> mBooleanLists;
        static private void AddBooleanList(int length, List<UInt64> list)
        {
            if (list == null || list.Count <= 0) return;
            if (mBooleanLists == null) mBooleanLists = new Dictionary<int, List<UInt64>>();
            mBooleanLists[length] = list;
        }

        static private List<UInt64> GetBooleanList(int length)
        {
            if (mBooleanLists == null || mBooleanLists.Count <= 0) return null;
            if (!mBooleanLists.ContainsKey(length)) return null;
            return mBooleanLists[length];
        }

        const int kStartingLength = 6;
        const int kMaximumLength = 64;
        static Dictionary<UInt64, int> mEndMapToLength;
        static int GetLength(UInt64 end)
        {
            if (mEndMapToLength == null)
            {
                mEndMapToLength = new Dictionary<UInt64, int>();
                for (int ii = kStartingLength; ii < kMaximumLength; ii++)
                {
                    UInt64 temp = (Convert.ToUInt64(Math.Pow(2, kStartingLength)) - 1) << (ii - kStartingLength);
                    mEndMapToLength[temp] = ii;
                }
            }
            if (!mEndMapToLength.ContainsKey(end)) return 0;
            return mEndMapToLength[end];
        }

        static string GetBooleandListFileName()
        {
            string userPath = AgeGradingForm.GetFolderPath();
            const string name = "BOOLEAN LISTS";
            string fileName = String.Format("{0}{1}{2}.{3}", userPath, Path.DirectorySeparatorChar, name, "TXT");
            return fileName;
        }

        private const string kListBegin = "INT:";
        static void LoadBooleanListsFromFile()
        {
            if (mBooleanLists != null && mBooleanLists.Count > 0) return;
            try
            {
                string fileName = GetBooleandListFileName();
                if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return;
                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
                if (lines == null || lines.Count <= 0) return;
                if (mBooleanLists == null) mBooleanLists = new Dictionary<int, List<ulong>>();
                int currentList = 0;
                foreach (string line in lines)
                {
                    ulong theValue = 0;
                    if (line.StartsWith(kListBegin)) currentList = Convert.ToInt32(line.Substring(kListBegin.Length));
                    else if (currentList > 0 && UInt64.TryParse(line, out theValue))
                    {
                        List<ulong> values = null;
                        if (mBooleanLists.ContainsKey(currentList)) values = mBooleanLists[currentList];
                        else values = new List<ulong>();
                        values.Add(theValue);
                        mBooleanLists[currentList] = values;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        internal static void SaveBooleanListsFromFile()
        {
            if (mBooleanLists == null || mBooleanLists.Count < 0) return;
            try
            {
                string fileName = GetBooleandListFileName();
                if (String.IsNullOrEmpty(fileName)) return;
                StringBuilder str = new StringBuilder();
                foreach (KeyValuePair<int, List<ulong>> item in mBooleanLists)
                {
                    if (item.Value == null || item.Value.Count <= 0) continue;
                    str.AppendLine(string.Format("{0}{1}", kListBegin, item.Key));
                    foreach (ulong value in item.Value)
                    {
                        str.AppendLine(value.ToString());
                    }
                }
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(str.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        static List<UInt64> GetBooleanLists(int listLength, int ofLength)
        {
            LoadBooleanListsFromFile();
            if (listLength < 0) return null;
            List<UInt64> lists = GetBooleanList(listLength);
            if (lists != null && lists.Count > 0) return lists;
            UInt64 length = Convert.ToUInt64(Math.Pow(2, listLength));
            UInt64 start = Convert.ToUInt64(Math.Pow(2, ofLength)) - 1;
            UInt64 end = start << (listLength - ofLength);
            lists = new List<UInt64>();
            // Add any previous lists into the current one.
            for (int ii = ofLength; ii < listLength; ii++)
            {
                List<UInt64> tempList = GetBooleanList(ii);
                if (tempList == null) break;
                lists.AddRange(tempList);
                // Make sure to update the start so we skip the
                // ones we already have lists for.
                start = Convert.ToUInt64(Math.Pow(2, ii + 1)) - 1;
            }
            for (UInt64 ii = start; ii <= end; ii++)
            {
                UInt64 value = ii;
                int onesCount = 0;
                for (int jj = 0; jj < listLength; jj++)
                {
                    if (value == 0) break;
                    if ((value & 1) == 1) onesCount++;
                    value = value >> 1;
                    if (onesCount > ofLength) break;
                }
                if (onesCount == ofLength) lists.Add(ii);
                int tempLength = GetLength(ii);
                if (tempLength != 0)
                {
                    List<UInt64> tempList = GetBooleanList(listLength);
                    if (tempList == null) AddBooleanList(tempLength, new List<UInt64>(lists));
                }
            }
            if (lists != null) AddBooleanList(listLength, lists);
            return lists;
        }
        #endregion BOOLEAN LISTS
#endif

        #region BOOLEAN LISTS
        static List<UInt64> GetBooleanLists(int listLength, int ofLength)
        {
            LoadBooleanListsFromFile();
            if (listLength < 0) return null;
            List<UInt64> lists = GetBooleanList(listLength);
            if (lists != null && lists.Count > 0) return lists;
            UInt64 length = Convert.ToUInt64(Math.Pow(2, listLength));
            UInt64 start = Convert.ToUInt64(Math.Pow(2, ofLength)) - 1;
            UInt64 end = start << (listLength - ofLength);
            lists = new List<UInt64>();
            // Add the last list into the current one.
            List<ulong> list = null;
            int lastKey = GetTheLastKey(out list);
            if (lastKey >= 0 && list != null && list.Count > 0)
            {
                lists.AddRange(list);
                // Make sure to update the start so we skip the
                // ones we already have lists for.
                start = Convert.ToUInt64(Math.Pow(2, lastKey)) - 1;
            }
            UInt64 of64Length = Convert.ToUInt64(ofLength);
            for (UInt64 ii = start; ii <= end; ii++)
            {
                UInt64 value = ii;
                UInt64 onesCount = 0;
                for (int jj = 0; jj < listLength; jj++)
                {
                    if (value == 0) break;
                    onesCount += (value & 1);
                    value = value >> 1;
                    if (onesCount > of64Length) break;
                }
                if (onesCount != of64Length) continue;
                if (onesCount == of64Length) lists.Add(ii);
                int tempLength = GetLength(ii);
                if (tempLength != 0)
                {
                    List<UInt64> tempList = GetBooleanList(listLength);
                    if (tempList == null) AddBooleanList(tempLength, new List<UInt64>(lists));
                }
            }
            if (lists != null) AddBooleanList(listLength, lists);
            return lists;
        }

        static private Dictionary<int, List<UInt64>> mBooleanLists;
        static private void AddBooleanList(int length, List<UInt64> list)
        {
            if (list == null || list.Count <= 0) return;
            if (mBooleanLists == null) mBooleanLists = new Dictionary<int, List<UInt64>>();
            mBooleanLists[length] = list;
        }

        static private List<UInt64> GetBooleanList(int length)
        {
            if (mBooleanLists == null || mBooleanLists.Count <= 0) return null;
            if (!mBooleanLists.ContainsKey(length)) return null;
            return mBooleanLists[length];
        }

        const int kStartingLength = 6;
        const int kMaximumLength = 64;
        static Dictionary<UInt64, int> mEndMapToLength;
        static int GetLength(UInt64 end)
        {
            if (mEndMapToLength == null)
            {
                mEndMapToLength = new Dictionary<UInt64, int>();
                for (int ii = kStartingLength; ii < kMaximumLength; ii++)
                {
                    UInt64 temp = (Convert.ToUInt64(Math.Pow(2, kStartingLength)) - 1) << (ii - kStartingLength);
                    mEndMapToLength[temp] = ii;
                }
            }
            if (!mEndMapToLength.ContainsKey(end)) return 0;
            return mEndMapToLength[end];
        }

        static string GetBooleandListFileName()
        {
            string userPath = AgeGradingForm.GetFolderPath();
            const string name = "BOOLEAN LISTS";
            string fileName = String.Format("{0}{1}{2}.{3}", userPath, Path.DirectorySeparatorChar, name, "TXT");
            return fileName;
        }

        static void LoadBooleanListsFromFile()
        {
            if (mBooleanLists != null && mBooleanLists.Count > 0) return;
            try
            {
                string fileName = GetBooleandListFileName();
                if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return;
                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
                if (lines == null || lines.Count <= 0) return;
                int listCount = 0;
                if (!int.TryParse(lines[0], out listCount)) return;
                if (listCount <= 0) return;
                if (mBooleanLists == null) mBooleanLists = new Dictionary<int, List<ulong>>();
                List<ulong> values = new List<ulong>();
                for (int ii = 1; ii < lines.Count; ii++)
                {
                    string line = lines[ii];
                    UInt64 int64 = 0;
                    if (!UInt64.TryParse(line, out int64)) continue;
                    values.Add(int64);
                    if (int64 == 350)
                    {

                    }
                    int index = GetLength(int64);
                    int tempLength = GetLength(int64);
                    if (tempLength != 0)
                    {
                        List<UInt64> tempList = GetBooleanList(tempLength);
                        if (tempList == null) AddBooleanList(tempLength, new List<UInt64>(values));
                    }
                }
                AddBooleanList(listCount, new List<UInt64>(values));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        internal static void SaveBooleanListsFromFile()
        {
            if (mBooleanLists == null || mBooleanLists.Count < 0) return;
            try
            {
                string fileName = GetBooleandListFileName();
                if (String.IsNullOrEmpty(fileName)) return;
                if (File.Exists(fileName)) File.Delete(fileName);
                int count = mBooleanLists.Count;
                StringBuilder str = new StringBuilder();
                List<ulong> list = null;
                int lastKey = GetTheLastKey(out list);
                if (lastKey < 0 || list == null || list.Count <= 0) return;
                str.AppendLine(lastKey.ToString());
                foreach (ulong value in list)
                {
                    str.AppendLine(value.ToString());
                    if (value == 350)
                    {

                    }
                }
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(str.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static int GetTheLastKey(out List<ulong> list)
        {
            list = null;
            if (mBooleanLists == null || mBooleanLists.Count < 0) return -1;
            Dictionary<int, List<ulong>>.KeyCollection keys = mBooleanLists.Keys;
            int lastKey = 0;
            foreach (int key in keys)
            {
                if (key > lastKey) lastKey = key;
            }
            list = mBooleanLists[lastKey];
            return lastKey;
        }

        #endregion BOOLEAN LISTS

        static List<UInt64> GetBooleanListsOld(int listLength, int ofLength)
        {
            if (listLength < 0) return null;
            UInt64 length = Convert.ToUInt64(Math.Pow(2, listLength));
            UInt64 start = Convert.ToUInt64(Math.Pow(2, ofLength)) - 1;
            List<UInt64> lists = GetBooleanList(listLength);
            if (lists != null && lists.Count > 0) return lists;
            lists = new List<UInt64>();
            for (UInt64 ii = start; ii < length; ii++)
            {
                UInt64 value = ii;
                int onesCount = 0;
                for (int jj = 0; jj < listLength; jj++)
                {
                    if (value == 0) break;
                    if ((value & 1) == 1) onesCount++;
                    value = value >> 1;
                    if (onesCount > ofLength) break;
                }
                if (onesCount == ofLength) lists.Add(ii);
            }
            if (lists != null) AddBooleanList(listLength, lists);
            return lists;
        }

        static int GetCountOfOnes(string startString)
        {
            if (String.IsNullOrEmpty(startString)) return 0;
            int count = 0;
            foreach (char item in startString)
            {
                if (item == '1') count++;
            }
            return count;
        }
    }
}
