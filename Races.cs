using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AgeGrading
{
    class RaceInfo
    {
        public RaceInfo(string name, double distanceInKilometers, double distanceMiles, DateTime raceDate)
        {
            this.Name = name;
            this.DistanceInKilometers = distanceInKilometers;
            this.DistanceMiles = distanceMiles;
            this.RaceDate = raceDate;
            this.AgeGradeTable = AgeGradingTables.GetTableName(this.DistanceInKilometers);
        }

        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        private double mDistanceInKilometers;
        public double DistanceInKilometers
        {
            get { return mDistanceInKilometers; }
            set { mDistanceInKilometers = value; }
        }

        private double mDistanceMiles;
        public double DistanceMiles
        {
            get { return mDistanceMiles; }
            set { mDistanceMiles = value; }
        }

        public double GetKilometers()
        {
            if (DistanceInKilometers > 0.0) return DistanceInKilometers;
            if (DistanceMiles <= 0.0) return 0.0;
            const double kKilometersToMiles = 1.6093;
            return (DistanceMiles * kKilometersToMiles);
        }

        public double GetMiles()
        {
            if (DistanceMiles > 0.0) return DistanceMiles;
            if (DistanceInKilometers <= 0.0) return 0.0;
            const double kKilometersToMiles = 1.6093;
            return (DistanceInKilometers / kKilometersToMiles);
        }

        private DateTime mRaceDate;
        public DateTime RaceDate
        {
            get { return mRaceDate; }
            set { mRaceDate = value; }
        }

        private string mAgeGradeTable;
        public string AgeGradeTable
        {
            get { return mAgeGradeTable; }
            set { mAgeGradeTable = value; }
        }

        public static List<RaceInfo> GetUpToDateRaces()
        {
            BuildRaceInfo();
            if (mRaces == null || mRaces.Count <= 0) return null;
            List<RaceInfo> races = new List<RaceInfo>();
            DateTime now = DateTime.Now;
            foreach (RaceInfo race in mRaces)
            {
                if (race == null) continue;
                if (race.RaceDate > now) continue;
                races.Add(race);
            }
            return races;
        }

        public static string GetLatestRace()
        {
            BuildRaceInfo();
            if (mRaces == null || mRaces.Count <= 0) return String.Empty;
            RaceInfo latest = null;
            DateTime now = DateTime.Now;
            foreach (RaceInfo race in mRaces)
            {
                if (race == null) continue;
                if (race.RaceDate > now) continue;
                if (latest == null || race.RaceDate > latest.RaceDate) latest = race;
            }
            if (latest != null) return latest.Name;
            return String.Empty;
        }

        public override string ToString()
        {
            return this.Name;
        }

        private static List<RaceInfo> mRaces;
        public static List<RaceInfo> GetRaces()
        {
            BuildRaceInfo();
            return mRaces;
        }

        internal static DateTime GetDefaultRaceDate()
        {
            BuildRaceInfo();
            if (mRaces == null || mRaces.Count <= 0) return DateTime.Parse("1/1/2012");
            foreach (RaceInfo item in mRaces)
            {
                if (item == null) continue;
                if (String.Equals(item.Name, kDefaultRaceName, StringComparison.OrdinalIgnoreCase))
                {
                    return item.RaceDate;
                }
            }
            return DateTime.Parse("1/1/2012");
        }

        internal static RaceInfo FindRace(string race)
        {
            if (String.IsNullOrEmpty(race)) return null;
            BuildRaceInfo();
            if (mRaces == null || mRaces.Count <= 0) return null;
            foreach (RaceInfo raceInfo in mRaces)
            {
                if (raceInfo == null) continue;
                if (String.Equals(raceInfo.Name, race, StringComparison.OrdinalIgnoreCase)) return raceInfo;
            }
            return null;
        }

        private const string kDefaultRaceName = "New Year's Day Wake Up 5K";
        public static void BuildRaceInfo()
        {
            if (mRaces != null) return;
            mRaces = new List<RaceInfo>();
            string UNKNOWNDATE = "1/1/2020";
            RaceInfo raceInfo = null;
            raceInfo = new RaceInfo(kDefaultRaceName, 5.0000000, 0.0, DateTime.Parse("01/01/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Surgoinsville 10 Miler", 16.0934400, 10.0, DateTime.Parse("02/25/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Mendota 5K River Run", 5.0000000, 0.0, DateTime.Parse("03/12/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("5K Run for St Anne School", 5.0000000, 0.0, DateTime.Parse("03/19/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Chasing Snakes 10K", 10.0000000, 0.0, DateTime.Parse("03/19/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Phipps Bend River Run", 16.0934400, 10.0, DateTime.Parse("03/19/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Laurel Run Ascent", 17.7027840, 11.0, DateTime.Parse("4/17/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Run the Tunnel", 6.1155072, 3.8, DateTime.Parse("4/30/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Amis Mill 10K", 10.0000000, 0.0, DateTime.Parse("05/7/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Creeper 10 Miler", 16.0934400, 10.0, DateTime.Parse("05/14/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Justin Foundation 5K", 5.0000000, 0.0, DateTime.Parse("5/21/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("CASA 8K", 8.0000000, 0.0, DateTime.Parse("6/4/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("The River Mile", 1.6093440, 1.0, DateTime.Parse("6/7/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Phipps Bend 5K", 5.0000000, 0.0, DateTime.Parse(UNKNOWNDATE)); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Mountain States Rehab 5K", 5.0000000, 0.0, DateTime.Parse("06/18/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Varmint Half Marathon", 21.0824064, 13.1, DateTime.Parse("6/11/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Rhododendron 10K", 10.0000000, 0.0, DateTime.Parse("6/18/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Firecracker 4 miler", 6.4373760, 4.0, DateTime.Parse(UNKNOWNDATE)); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Red, White & Boom 4 Miler", 6.4373760, 4.0, DateTime.Parse("7/4/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Crazy 8s 8K", 8.0000000, 0.0, DateTime.Parse("7/17/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Wolf Run 7 Miler", 11.2654080, 7.0, DateTime.Parse("7/19/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Railroad Days 5K", 5.0000000, 0.0, DateTime.Parse("8/6/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Schoolhouse 5K", 5.0000000, 0.0, DateTime.Parse(UNKNOWNDATE)); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Christopher Todd Richardson Memorial 10k", 10.0000000, 0.0, DateTime.Parse(UNKNOWNDATE)); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Greene County YMCA 5K", 5.0000000, 0.0, DateTime.Parse("8/20/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Eastman 10K", 10.0000000, 0.0, DateTime.Parse("9/3/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Baileyton Celebration 5K", 5.0000000, 0.0, DateTime.Parse("9/10/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Bays Mountain Trail Race", 24.1401600, 15.0, DateTime.Parse("9/24/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Rhythm & Roots 5K", 5.0000000, 0.0, DateTime.Parse("9/17/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Apple Festival 4 Miler", 6.4373760, 4.0, DateTime.Parse("10/8/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("HeartOne 5K", 5.0000000, 0.0, DateTime.Parse("10/8/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Run Fur Their Lives 5K", 5.0000000, 0.0, DateTime.Parse("10/2/2016")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Run Fur Their Lives 10K", 5.0000000, 0.0, DateTime.Parse("10/2/2016")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("War Party 10K", 10.0000000, 0.0, DateTime.Parse("02/18/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("VIRGINIAN 'EXTRA MILE' 6.6k", 6.6, 0.0, DateTime.Parse(UNKNOWNDATE)); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Tri-Cities Race for the Cure 5K", 5.0000000, 0.0, DateTime.Parse("10/23/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Haunted Half Marathon", 21.0824064, 13.1, DateTime.Parse("10/29/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Veterans Day Classic Half Marathon", 21.0824064, 13.1, DateTime.Parse("11/12/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Santa Special Open Mile", 1.6093440, 1.0, DateTime.Parse("11/19/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Johnson City Turkey Trot 5K (Up & At 'Em)", 5.0000000, 0.0, DateTime.Parse("11/24/17")); mRaces.Add(raceInfo);
            raceInfo = new RaceInfo("Pioneer 5 Miler", 8.0467200, 5.0, DateTime.Parse("12/17/17")); mRaces.Add(raceInfo);
            mRaces.Sort(new RaceInfoComparer());
            double totalKilometers = 0;
            double totalMiles = 0;
            foreach (RaceInfo item in mRaces)
            {
                totalKilometers += item.GetKilometers();
                totalMiles += item.GetMiles();
            }
            Debug.WriteLine(String.Format("Miles = {0} Kilometers = {1}", totalMiles, totalKilometers));
#if SHOW_SORTED_LIST
            foreach (RaceInfo info in mRaces)
            {
                Debug.WriteLine(info.Name);
            }
#endif
        }

        public class RaceInfoComparer : IComparer<RaceInfo>
        {
            public int Compare(RaceInfo x, RaceInfo y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                if (x.RaceDate == y.RaceDate) return 0;
                if (x.RaceDate < y.RaceDate) return -1;
                return 1;
            }
        }
    }
}
