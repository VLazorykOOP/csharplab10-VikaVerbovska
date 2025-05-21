using System;
using System.Collections.Generic;

namespace StudentLifeSimulation
{
    public delegate void StudentEventHandler(object sender, StudentEventArgs e);

    public class StudentLife
    {
        public event StudentEventHandler StudentEvent;

        private string name;
        private int days;
        private Random rnd = new Random();
        private string[] events = { "Lecture", "Test", "Rest", "Exam", "Coursework" };

        public StudentLife(string name, int days)
        {
            this.name = name;
            this.days = days;

            // Підписка сервісів
            new Teacher(this).Subscribe();
            new Friend(this).Subscribe();
            new DeanOffice(this).Subscribe();
        }

        public void StartLife()
        {
            for (int day = 1; day <= days; day++)
            {
                Console.WriteLine($"\n--- День {day} ---");

                string currentEvent = events[rnd.Next(events.Length)];
                StudentEventArgs e = new StudentEventArgs(currentEvent, day);

                OnStudentEvent(e);
            }
        }

        protected virtual void OnStudentEvent(StudentEventArgs e)
        {
            if (StudentEvent != null)
            {
                foreach (StudentEventHandler handler in StudentEvent.GetInvocationList())
                {
                    handler(this, e);
                    if (!string.IsNullOrEmpty(e.Response))
                        Console.WriteLine(" -> " + e.Response);
                }

            }
            else
            {
                Console.WriteLine("Немає підписників на подію.");
            }
        }
    }

    public class StudentEventArgs : EventArgs
    {
        public string EventName { get; }
        public int Day { get; }
        public string Response { get; set; }

        public StudentEventArgs(string eventName, int day)
        {
            EventName = eventName;
            Day = day;
        }
    }

    public abstract class StudentService
    {
        protected StudentLife life;
        protected Random rnd = new Random();

        public StudentService(StudentLife life)
        {
            this.life = life;
        }

        public void Subscribe()
        {
            life.StudentEvent += HandleEvent;
        }

        public abstract void HandleEvent(object sender, StudentEventArgs e);
    }

    public class Teacher : StudentService
    {
        public Teacher(StudentLife life) : base(life) { }

        public override void HandleEvent(object sender, StudentEventArgs e)
        {
            if (e.EventName == "Lecture" || e.EventName == "Test")
            {
                e.Response = $"Викладач провів {e.EventName.ToLower()}.";
            }
        }
    }

    public class Friend : StudentService
    {
        public Friend(StudentLife life) : base(life) { }

        public override void HandleEvent(object sender, StudentEventArgs e)
        {
            if (e.EventName == "Rest")
            {
                e.Response = "Друг запросив пограти в комп'ютерні ігри.";
            }
        }
    }

    public class DeanOffice : StudentService
    {
        public DeanOffice(StudentLife life) : base(life) { }

        public override void HandleEvent(object sender, StudentEventArgs e)
        {
            if (e.EventName == "Exam" || e.EventName == "Coursework")
            {
                e.Response = $"Деканат зареєстрував {e.EventName.ToLower()} студента.";
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StudentLife life = new StudentLife("Іван", 10);
            life.StartLife();
        }
    }
}
