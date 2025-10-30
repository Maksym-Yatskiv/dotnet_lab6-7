using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lab6.Classes
{
    public class Student
    {
        private string _name = "";
        private string _groupNumber = "";
        private int _exam1;
        private int _exam2;
        private int _exam3;
        
        [JsonPropertyName("name")]
        public string Name 
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentOutOfRangeException("Ім'я не може бути пустим");
                }
                _name = value;
            }
        }
        [JsonPropertyName("groupNumber")]
        public string GroupNumber 
        {
            get { return _groupNumber; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentOutOfRangeException("Номер групи не може бути пустою");
                }
                _groupNumber = value.Trim();
            }
        }
        
        [JsonPropertyName("exam1")]
        public int Exam1 
        { 
            get { return _exam1; }
            set
            {
                if (value < 0 || value > 40)
                {
                    throw new ArgumentOutOfRangeException("Оцінка за іспит має бути в межах від 0 до 40");
                }
                _exam1 = value;
            }
        }
        
        [JsonPropertyName("exam2")]
        public int Exam2
        {
            get { return _exam2; }
            set
            {
                if (value < 0 || value > 40)
                {
                    throw new ArgumentOutOfRangeException("Оцінка за іспит має бути в межах від 0 до 40");
                }
                _exam2 = value;
            }
        }

        [JsonPropertyName("exam3")]
        public int Exam3
        {
            get { return _exam3; }
            set
            {
                if (value < 0 || value > 40)
                {
                    throw new ArgumentOutOfRangeException("Оцінка за іспит має бути в межах від 0 до 40");
                }
                _exam3 = value;
            }
        }
        

        public Student(string name, string groupNumber, int exam1, int exam2, int exam3)
        {
            Name = name;
            GroupNumber = groupNumber.Trim();
            Exam1 = exam1;
            Exam2 = exam2;
            Exam3 = exam3;
        }

        public int CompareTo(Student other)
        {
            return string.Compare(this.GroupNumber, other.GroupNumber, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return $"{Name}, Група: {GroupNumber}, Іспити: {Exam1}, {Exam2}, {Exam3}";
        }
    }
}
