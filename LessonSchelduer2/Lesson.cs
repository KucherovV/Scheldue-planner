using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonSchelduer2
{
    class Lesson
    {
        public DateTime DTStart { get; set; }
        public DateTime DTFinish { get; set; }
        public string Place { get; set; }
        public string LessonName { get; set; }
        public string Teacher { get; set; }
        public string Group { get; set; }
        public string Day { get; set; }

        public Lesson(DateTime start, DateTime finish, string place, string lessonName, string teacher, string group, string day)
        {
            DTStart = start;
            DTFinish = finish;
            Place = place;
            LessonName = lessonName;
            Teacher = teacher;
            Group = group;
            Day = day;
        }

    }
}
