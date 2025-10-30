using lab6.Classes;
using System.Text.Json;

namespace lab6_unit_tests;

[TestClass]
public class JsonWriterTests
{
    private string _filePath = "";

    [TestInitialize]
    public void Setup()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"test_students_{Path.GetRandomFileName()}.json");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    [TestMethod]
    public async Task SaveAsync_WritesFileCorrectly()
    {
        var writer = new JsonWriter();
        var students = new List<Student>
        {
            new Student("Іван", "КН-31", 20, 30, 25)
        };

        await writer.SaveAsync(_filePath, students);

        Assert.IsTrue(File.Exists(_filePath), "Файл не створено");

        var content = await File.ReadAllTextAsync(_filePath);

        var loaded = JsonSerializer.Deserialize<List<Student>>(content);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(1, loaded.Count);
        Assert.AreEqual("Іван", loaded[0].Name);
    }
}