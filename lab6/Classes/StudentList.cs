using lab6.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace lab6.Classes
{
    public class StudentList
    {
        private readonly IFileLoader _fileLoader;
        private readonly IFileSaver _fileSaver;
        private readonly string _filePath;

        private List<Student> _students;

        public ObservableCollection<Student> Students { get; }

        public StudentList(IFileLoader fileLoader, IFileSaver fileSaver, string filePath)
        {
            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
            _filePath = filePath;
            _students = new List<Student>();
            Students = new ObservableCollection<Student>();
        }

        public void AddStudent(Student student)
        {
            _students.Add(student);
            Students.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            if (student == null) return;
            _students.Remove(student);
            Students.Remove(student);
        }

        public void SortByGroupNumber()
        {
            _students.Sort(new StudentGroupComparer());
            RefreshObservableCollection(_students);
        }

        public void FilterByExamScore(string ExamName, int score)
        {
            var filteredStudents = _students.Where(student =>
            {
                return ExamName switch
                {
                    "Exam1" => student.Exam1 >= score,
                    "Exam2" => student.Exam2 >= score,
                    "Exam3" => student.Exam3 >= score,
                    _ => false,
                };
            }).ToList();
            RefreshObservableCollection(filteredStudents);
        }

        private void RefreshObservableCollection(List<Student> source)
        {
            Students.Clear();
            if (source == null) return;
            foreach (var s in source)
                Students.Add(s);
        }

        public void ResetFilters()
        {
            RefreshObservableCollection(_students);
        }

        public async Task SaveAsync()
        {
            if (_fileSaver == null) throw new InvalidOperationException("File saver is not provided.");
            await _fileSaver.SaveAsync(_filePath, _students);
        }

        public async Task LoadAsync()
        {
            if (_fileLoader == null) throw new InvalidOperationException("File loader is not provided.");
            var loadedStudents = await _fileLoader.LoadAsync<Student>(_filePath);
            if (loadedStudents != null)
            {
                _students = loadedStudents;
                RefreshObservableCollection(_students);
            }
        }


        public void FindByName(string name)
        {
            var students = _students
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
            RefreshObservableCollection(students);
        }

        public void GetTopTenByTotalScore()
        {
            var students = _students
                .OrderByDescending(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0)
                .Take(10)
                .ToList();
            RefreshObservableCollection(students);
        }

        public double GetAverageExamScore(string examNumber)
        {
            if (_students.Count == 0) return 0;

            return examNumber switch
            {
                "Exam1" => _students.Average(s => s.Exam1),
                "Exam2" => _students.Average(s => s.Exam2),
                "Exam3" => _students.Average(s => s.Exam3),
                _ => throw new ArgumentException("Іспит не вказаний")
            };
        }

        public void GetBestInGroup()
        {
        var groupedStudents = _students
            .GroupBy(s => s.GroupNumber, StringComparer.OrdinalIgnoreCase)
            .Select(g => g
                .OrderByDescending(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0)
                .First())
            .ToList();
            RefreshObservableCollection(groupedStudents);
        }

        public record BestStudentInfo(string Name, string GroupNumber, double AverageScore);
        public BestStudentInfo? GetBestStudent()
        {
            var bestStudent = _students
                .OrderByDescending(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3)
                .Select(s => new BestStudentInfo(
                    s.Name, 
                    s.GroupNumber, 
                    (s.Exam1 + s.Exam2 + s.Exam3) / 3)
                )
                .FirstOrDefault();

            return bestStudent;
        }
    }
}
