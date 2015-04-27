using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace PicnicCache.Tests
{
    [TestClass]
    public class UnitTest1
    {
        //private readonly PicnicCache<int, TestObject> _cache;
        private IList<TestObject> _objectList;

        [TestInitialize]
        public void Initialize()
        {
            //var cache = new PicnicCache<int, TestObject>(x => x.KeyProperty);
            var list = new List<TestObject>();
            for(int i = 0; i < 10; i++)
            {
                var testObject = new TestObject() { KeyProperty = i, Text = "HAHA" + i };
                list.Add(testObject);
            }

            _objectList = list;
        }

        [TestMethod]
        public void TestMethod1()
        {
            var cache = new PicnicCache<int, TestObject>(x => x.KeyProperty);

            var testItem1 = cache.Fetch(3, () =>_objectList.FirstOrDefault(x => x.KeyProperty == 3));
            Assert.AreEqual(testItem1.KeyProperty, 3);

            var testItem2 = cache.Fetch(3, () => _objectList.FirstOrDefault(x => x.KeyProperty == 5));
            Assert.AreEqual(testItem2.KeyProperty, 3);

            var testItem3 = cache.Fetch(5, () => _objectList.FirstOrDefault(x => x.KeyProperty == 5));
            Assert.AreEqual(testItem3.KeyProperty, 5);

            var testItems4 = cache.FetchAll(() => _objectList).OrderBy(x => x.KeyProperty).ToList();
            for (int i = 0; i < 10; i++)
                Assert.AreEqual(testItems4[i], _objectList[i]);
        }
    }
}