using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicnicCache.Tests.Models;

namespace PicnicCache.Tests.Tests
{
    /// <summary>
    /// Summary description for PicnicCacheTests
    /// </summary>
    [TestClass]
    public class PicnicCacheTests
    {
        #region Fields

        private ICache<int, TestModel> _cache;
        private IList<TestModel> _list;
        private TestContext testContextInstance;

        #endregion

        #region Properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Constructors

        public PicnicCacheTests()
        {
            _list = GetTestList("Testing");
        }

        #endregion

        #region Additional test attributes

        // Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) { }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _cache = new PicnicCache<int, TestModel>(x => x.Id);
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }

        #endregion

        #region Fetch Tests

        [TestMethod]
        public void Fetch_CacheIsEmpty_ReturnCorrectItem()
        {
            var item = _list.First();

            var cacheItem = _cache.Fetch(item.Id, () => _list.First());

            Assert.AreEqual(item, cacheItem);
        }

        [TestMethod]
        public void Fetch_ItemIsCached_ReturnCorrectItem()
        {
            var item = _cache.Fetch(_list.First().Id, () => _list.First());

            var cacheItem = _cache.Fetch(_list.First().Id, () => new TestModel(999, 3, "TestingThisItem"));

            Assert.AreEqual(item, cacheItem);
        }

        [TestMethod]
        public void Fetch_ItemIsNotCachedAndCacheIsNotEmpty_ReturnCorrectItem()
        {
            var firstItem = _cache.Fetch(_list.First().Id, () => _list.First());
            var secondItem = _list.First(x => x.Id == 25);

            var cacheItem = _cache.Fetch(25, () => _list.First(x => x.Id == 25));

            Assert.AreEqual(secondItem, cacheItem);
        }

        [TestMethod]
        public void Fetch_ItemIsCachedAndCacheHasOtherItems_ReturnCorrectItem()
        {
            var firstItem = _cache.Fetch(_list.First().Id, () => _list.First());
            var secondItem = _cache.Fetch(25, () => _list.First(x => x.Id == 25));
            var thirdItem = _cache.Fetch(54, () => _list.First(x => x.Id == 54));

            var cacheItem = _cache.Fetch(25, () => new TestModel(999, 3, "TestingThisItem"));

            Assert.AreEqual(secondItem, cacheItem);
        }

        [TestMethod]
        public void Fetch_ItemIsDeleted_ReturnNull()
        {
            var firstItem = _cache.Fetch(_list.First().Id, () => _list.First());
            _cache.Delete(firstItem);

            var deletedItem = _cache.Fetch(_list.First().Id, () => _list.First());

            Assert.IsNull(deletedItem);
        }

        [TestMethod]
        public void Fetch_ItemIsModified_ReturnModifiedItem()
        {
            var firstItem = _cache.Fetch(_list.First().Id, () => _list.First());
            var modifiedItem = new TestModel(firstItem);
            modifiedItem.Text = "LOLCAT";
            _cache.Update(modifiedItem);

            var cacheItem = _cache.Fetch(_list.First().Id, () => _list.First());

            Assert.AreEqual(modifiedItem, cacheItem);
        }

        #endregion

        #region FetchAll Tests

        [TestMethod]
        public void FetchAll_CacheIsEmpty_ReturnCorrectItems()
        {
            var cacheItems = _cache.FetchAll(() => _list).ToList();

            ValidateListsAreEqual(_list, cacheItems);
        }

        [TestMethod]
        public void FetchAll_CacheIsPartiallyLoaded_ReturnCorrectItems()
        {
            var testModel = new TestModel(0, 0, "This is a replacement.");
            var firstItem = _cache.Fetch(_list.First().Id, () => testModel);

            var cacheItems = _cache.FetchAll(() => _list).ToList();
            _list[0] = testModel;

            ValidateListsAreEqual(_list, cacheItems);
        }

        [TestMethod]
        public void FetchAll_CacheIsLoaded_ReturnCorrectItems()
        {
            var secondList = GetTestList("SecondList");
            var firstLoad = _cache.FetchAll(() => _list).ToList();

            var cacheItems = _cache.FetchAll(() => secondList).ToList();

            ValidateListsAreEqual(_list, cacheItems);
        }

        #endregion

        #region Private helper methods

        private IList<TestModel> GetTestList(string text)
        {
            IList<TestModel> models = new List<TestModel>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var model = new TestModel((i * 10) + j, j, text + i + j);
                    models.Add(model);
                }
            }

            return models;
        }

        private void ValidateListsAreEqual(IList<TestModel> expected, IList<TestModel> actual)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Id, actual[i].Id);
            }
        }

        #endregion
    }
}