namespace lab6_unit_tests;
using lab6.Classes;
using lab6.Interfaces;
using System.Text.Json;
using System.Windows.Documents;

[TestClass]
public class StudentListTests
{
    private string _tempFilePath = "";

    private string GetTestJson()
    {
        var students = new[]
        {
                new Student("Марія Коваленко", "КН-32", 25, 34, 12),
                new Student("Іван Петренко", "КН-31", 21, 35, 39),
                new Student("Олег Сидоренко", "АНД-11", 19, 33, 34)
        };
        return JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
    }

    [TestInitialize]
    public void Setup()
    {
        _tempFilePath = Path.Combine(Path.GetTempPath(), $"test_students_{Path.GetRandomFileName()}.json");
        File.WriteAllText(_tempFilePath, GetTestJson());
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_tempFilePath))
            File.Delete(_tempFilePath);
    }

    [TestMethod]
    public async Task LoadAsync_LoadsStudentsFromTempFile()
    {
        var studentList = new StudentList(new JsonReader(), new JsonWriter(), _tempFilePath);
        await studentList.LoadAsync();

        Assert.AreEqual(3, studentList.Students.Count);
        Assert.IsTrue(studentList.Students.Any(s => s.Name == "Марія Коваленко"));
    }

    [TestMethod]
    public async Task SaveAsync_And_LoadAsync_WorksCorrectly()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"temp_students_{Path.GetRandomFileName()}.json");
        var studentList = new StudentList(new JsonReader(), new JsonWriter(), tempFile);

        var s1 = new Student("Тест1", "КН-01", 10, 20, 30);
        var s2 = new Student("Тест2", "КН-02", 15, 25, 35);
        studentList.AddStudent(s1);
        studentList.AddStudent(s2);

        await studentList.SaveAsync();

        var loadedList = new StudentList(new JsonReader(), new JsonWriter(), tempFile);
        await loadedList.LoadAsync();

        Assert.AreEqual(2, loadedList.Students.Count);
        Assert.AreEqual("Тест1", loadedList.Students[0].Name);
        Assert.AreEqual("Тест2", loadedList.Students[1].Name);

        if (File.Exists(tempFile)) File.Delete(tempFile);
    }

    [TestMethod]
    public void AddStudent_AddsStudent()
    {
        var studentList = new StudentList(null, null, "");
        var student = new Student("Тест Тестовий", "КН-99", 20, 20, 20);
        studentList.AddStudent(student);

        Assert.AreEqual(1, studentList.Students.Count);
        Assert.AreSame(student, studentList.Students[0]);
    }

    [TestMethod]
    public void RemoveStudent_RemovesStudent()
    {
        var studentList = new StudentList(null, null, "");
        var student = new Student("Тест Тестовий", "КН-99", 20, 20, 20);
        studentList.AddStudent(student);
        studentList.RemoveStudent(student);

        Assert.AreEqual(0, studentList.Students.Count);
    }

    [TestMethod]
    public void StudentGroupComparer_ComparesCorrectly()
    {
        var s1 = new Student("A", "КН-01", 0, 0, 0);
        var s2 = new Student("B", "АНД-11", 0, 0, 0);

        var comparer = new StudentGroupComparer();
        Assert.IsTrue(comparer.Compare(s1, s2) > 0);
    }




    [TestMethod]
    public void FindByName_FindsCaseInsensitivePartialMatch()
    {
        var list = new StudentList(null, null, "");
        list.AddStudent(new Student("Іван Петренко", "G1", 10, 10, 10));
        list.AddStudent(new Student("Марія", "G1", 20, 20, 20));
        list.AddStudent(new Student("Петро ІВАНЕНКО", "G2", 15, 15, 15));

        list.FindByName("іван");

        Assert.AreEqual(2, list.Students.Count);
        Assert.IsTrue(list.Students.Any(s => s.Name == "Іван Петренко"));
        Assert.IsTrue(list.Students.Any(s => s.Name == "Петро ІВАНЕНКО"));
    }

    [TestMethod]
    public void GetTopTenByTotalScore_ReturnsTop10Ordered()
    {
        var list = new StudentList(null, null, "");


        for (int i = 0; i < 15; i++)
        {
            list.AddStudent(new Student($"S{i}", $"G{i % 3}", i, i, i));
        }

        list.GetTopTenByTotalScore();

        
        Assert.AreEqual(10, list.Students.Count);


        int firstTotal = list.Students[0].Exam1 + list.Students[0].Exam2 + list.Students[0].Exam3;
        int secondTotal = list.Students[1].Exam1 + list.Students[1].Exam2 + list.Students[1].Exam3;
        Assert.IsTrue(firstTotal >= secondTotal);

        Assert.AreEqual("S14", list.Students[0].Name);
        Assert.AreEqual("S5", list.Students.Last().Name);
    }

    [TestMethod]
    public void GetAverageExamScore_ReturnsCorrectAveragePerExam()
    {
        var list = new StudentList(null, null, "");
        list.AddStudent(new Student("A", "G1", 10, 20, 30));
        list.AddStudent(new Student("B", "G1", 20, 30, 40));
        list.AddStudent(new Student("C", "G1", 30, 40, 40));

        double avg1 = list.GetAverageExamScore("Exam1");
        double avg2 = list.GetAverageExamScore("Exam2");
        double avg3 = list.GetAverageExamScore("Exam3");

        Assert.AreEqual((10 + 20 + 30) / 3.0, avg1);
        Assert.AreEqual((20 + 30 + 40) / 3.0, avg2);
        Assert.AreEqual((30 + 40 + 40) / 3.0, avg3);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAverageExamScore_InvalidExam_ThrowsArgumentException()
    {
        var list = new StudentList(null, null, "");
        list.AddStudent(new Student("A", "G1", 10, 20, 30));

        _ = list.GetAverageExamScore("ExamX");
    }

    [TestMethod]
    public void GetBestInGroup_ReturnsOneBestPerGroup()
    {
        var list = new StudentList(null, null, "");
        // Група G1: B(90) best, A(60)
        list.AddStudent(new Student("A", "G1", 20, 20, 20));
        list.AddStudent(new Student("B", "G1", 30, 30, 30));

        // Група G2: D(85) best, C(70)
        list.AddStudent(new Student("C", "G2", 20, 25, 25));
        list.AddStudent(new Student("D", "G2", 30, 30, 25));

        // Група G3: single E
        list.AddStudent(new Student("E", "G3", 10, 10, 10));

        list.GetBestInGroup();

        Assert.AreEqual(3, list.Students.Count);

        Assert.IsTrue(list.Students.Any(s => s.GroupNumber == "G1" && s.Name == "B"));
        Assert.IsTrue(list.Students.Any(s => s.GroupNumber == "G2" && s.Name == "D"));
        Assert.IsTrue(list.Students.Any(s => s.GroupNumber == "G3" && s.Name == "E"));
    }

    [TestMethod]
    public void GetBestStudent_ReturnsOverallBestStudentInfo()
    {
        var list = new StudentList(null, null, "");
        list.AddStudent(new Student("A", "G1", 10, 10, 10));
        list.AddStudent(new Student("B", "G2", 30, 30, 30));
        list.AddStudent(new Student("C", "G1", 20, 20, 20));

        var best = list.GetBestStudent();

        Assert.IsNotNull(best);
        Assert.AreEqual("B", best!.Name);
        Assert.AreEqual("G2", best.GroupNumber);
        Assert.AreEqual((30 + 30 + 30) / 3.0, best.AverageScore);
    }
}
