//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PicnicCache.Tests.Models;

namespace PicnicCache.Tests
{
    public class PicnicCacheTestBase
    {
        #region Methods

        protected ITestObject GetTestItem(int key)
        {
            var model = new Mock<ITestObject>();
            model.SetupGet(m => m.Id).Returns(key);
            return model.Object;
        }

        protected IEnumerable<ITestObject> GetTestList(int quantity)
        {
            IList<ITestObject> list = new List<ITestObject>();
            for (int i = 0; i < quantity; i++)
            {
                var model = new Mock<ITestObject>();
                model.SetupGet(m => m.Id).Returns(i);
                list.Add(model.Object);
            }
            return list;
        }

        protected void ValidateListsAreEqual(IEnumerable<ITestObject> expected, IEnumerable<ITestObject> actual)
        {
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i).Id, actual.ElementAt(i).Id);
            }
        }

        #endregion
    }
}