using Application;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTest
{
    [TestClass]
    public class BlogTest
    {
        Blogs blogApp;

        [TestInitialize]
        public void Instantiate()
        {
            ConnectionStringManager.ConnectionString = "server=localhost;port=3306;user=root;password=CAQ(r7(@3@#104gsd60(*@#-1;database=educational_project";
            blogApp = new Blogs();
        }

        [TestMethod]
        public void TestGet()
        {
            var e = blogApp.GetDataCollection().GetAwaiter().GetResult();
            Assert.IsTrue(e.Count > 0);
        }
    }
}
