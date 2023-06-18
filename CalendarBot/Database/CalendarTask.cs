using System;
using SQLite;

namespace CalendarBot.Database {
    public class CalendarTask {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public Course course { get; set; }
        public DateTime time { get; set; }
        public string task { get; set; }
    }

    public enum Course {
        Subject = 1, Subject2 = 2, Subject3 = 3, Subject4 = 4
    }
}
