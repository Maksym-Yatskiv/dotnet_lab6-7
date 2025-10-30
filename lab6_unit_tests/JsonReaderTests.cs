using lab6.Classes;
using System.Text.Json;

namespace lab6_unit_tests;

[TestClass]
public class JsonReaderTests
{
    private string _filePath = "";

    [TestInitialize]
    public void Setup()
    {
        // Генеруємо унікальний тимчасовий файл
        _filePath = Path.Combine(Path.GetTempPath(), $"test_students_{Path.GetRandomFileName()}.json");

        var students = new List<Student>
        {
            new Student("Іван", "КН-31", 20, 30, 25)
        };

        var json = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [TestMethod]
    public async Task LoadAsync_ReadsFileCorrectly()
    {
        var reader = new JsonReader();
        var students = await reader.LoadAsync<Student>(_filePath);

        Assert.IsNotNull(students);
        Assert.AreEqual(1, students.Count);
        Assert.AreEqual("Іван", students[0].Name);
    }

    [TestMethod]
    public async Task LoadAsync_FileDoesNotExist_ReturnsNull()
    {
        var reader = new JsonReader();
        var students = await reader.LoadAsync<Student>("nonexistent.json");

        Assert.IsNull(students);
    }
}
