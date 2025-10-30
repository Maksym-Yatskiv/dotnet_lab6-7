using lab6.Classes;

namespace lab6_unit_tests;

[TestClass]
public class StudentGroupComparerTests
{
    [TestMethod]
    public void Compare_SortsCorrectly()
    {
        var s1 = new Student("Іван", "КН-32", 20, 30, 25);
        var s2 = new Student("Марія", "КН-31", 25, 30, 20);

        var comparer = new StudentGroupComparer();
        int result = comparer.Compare(s1, s2);

        Assert.IsTrue(result > 0);
    }

    [TestMethod]
    public void Compare_NullStudents_ReturnsZero()
    {
        var comparer = new StudentGroupComparer();
        Assert.AreEqual(0, comparer.Compare(null, null));
        var s = new Student("Іван", "КН-31", 20, 30, 25);
        Assert.AreEqual(0, comparer.Compare(s, null));
        Assert.AreEqual(0, comparer.Compare(null, s));
    }
}
