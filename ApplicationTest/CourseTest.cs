using Application;
using Domain;
using Infrastructure.DataAccessLayer;

namespace ApplicationTest
{
    [TestClass]
    public class CourseTest
    {
        Courses courseApp;

        [TestInitialize]
        public void Instantiate()
        {
            ConnectionStringManager.ConnectionString = "server=localhost;port=3306;user=root;password=CAQ(r7(@3@#104gsd60(*@#-1;database=educational_project";
            courseApp = new Courses();
        }

        [TestMethod]
        public void TestGet()
        {
            var e = courseApp.GetDataCollection().GetAwaiter().GetResult();
            Assert.IsTrue(e.Count > 0);
        }

        [TestMethod]
        public void TestInsert()
        {
            CourseItem blogItem = new CourseItem()
            {
                Id = 2,
                Title = "Test",
                Summary = "This is a test thingy",
                Description = "Random description ig",
                CourseLength = 10,
                Students = 231,
                Rating = 3.5f,
                Price = 88.5M,
                Topic = "Tech",
                PostDate = DateTime.UtcNow,
                ImageURL = "ABAB",
            };

            courseApp.InsertData(blogItem).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestDelete()
        {
            courseApp.DeleteData("id", 3).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestCount()
        {
            var data = courseApp.GetCount(new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("Topic", "Technology"),
                new KeyValuePair < string, object >("Topic", "Programming")
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TestCountCondition()
        {
            var data = courseApp.GetCount(new List<KeyValuePair<string, (string compOperator, object value)>>()
            {
                new KeyValuePair<string, (string compOperator, object value)>("Price", (">", 0)),
                new KeyValuePair <string, (string compOperator, object value)>("Price", ("=", 0))
            }).GetAwaiter().GetResult();
        }
    }
}