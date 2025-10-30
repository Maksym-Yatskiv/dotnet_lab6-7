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
}
