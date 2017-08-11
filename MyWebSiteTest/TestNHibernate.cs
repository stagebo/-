using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MyWebSiteTest
{
   [TestFixture(Description = "测试示例")]
   　public class TestNHibernate
    {
        public TestNHibernate() { }

        [TestFixtureSetUp]
        public void Initialize()
        {
            Console.WriteLine("初始化信息");
            
        }
        [TestFixtureTearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
          
        }
        [SetUp]
        public void SetUp()
        {
         
        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("我是清理者");
        }


        public int Add(int a, int b) => a + b;
        [Test]
        [Category("优先级 1")]
        public void TestAdd()
        {
            Assert.AreEqual(5, new Cal().Add(2,3));
        }
    }
    public class Cal
    {
        public int Add(int a, int b) => a + b;
    }

}