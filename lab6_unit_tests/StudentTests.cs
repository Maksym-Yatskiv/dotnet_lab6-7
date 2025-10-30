using lab6.Classes;

namespace lab6_unit_tests
{
    [TestClass]
    public sealed class StudentTests
    {
        [TestMethod]
        public void Student_ValidData_CreatesInstance()
        {
            var student = new Student("Іван", "КН-31", 20, 30, 25);
            Assert.AreEqual("Іван", student.Name);
            Assert.AreEqual("КН-31", student.GroupNumber);
            Assert.AreEqual(20, student.Exam1);
            Assert.AreEqual(30, student.Exam2);
            Assert.AreEqual(25, student.Exam3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Student_EmptyName_Throws()
        {
            var student = new Student("", "КН-31", 20, 30, 25);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Student_InvalidExam_Throws()
        {
            var student = new Student("Іван", "КН-31", -1, 30, 25);
        }
    }
}
