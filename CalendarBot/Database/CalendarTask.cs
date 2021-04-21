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
        Introduccion_a_base_de_datos = 1, Estructura_de_datos = 2, Calculo = 3,
        Psicologia = 4
    }
}