//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PicnicCache.Tests.Models;

namespace PicnicCache.Tests.Tests
{
    /// <summary>
    /// Summary description for ReadOnlyPicnicCacheTests
    /// </summary>
    [TestClass]
    public class ReadOnlyPicnicCacheTests : PicnicCacheTestBase
    {
        #region Fields

        private IReadOnlyCache<int, ITestObject> _cache;
        private Mock<ICacheable<int, ITestObject>> _mockCacheable;
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

        public ReadOnlyPicnicCacheTests()
        {
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
            _mockCacheable = new Mock<ICacheable<int, ITestObject>>();
            _cache = new ReadOnlyPicnicCache<int, ITestObject>(x => x.Id, _mockCacheable.Object);
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }

        #endregion

        #region Constructor Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: readOnlyCacheable")]
        public void Constructor_PassNullCacheable_ThrowsException()
        {
            //Act
            var result = new ReadOnlyPicnicCache<int, ITestObject>("Id", null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: keyPropertyName")]
        public void Constructor_PassNullKeyPropertyName_ThrowsException()
        {
            //Arrange
            string keyPropertyName = null;

            //Act
            var result = new ReadOnlyPicnicCache<int, ITestObject>(keyPropertyName, _mockCacheable.Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: keyProperty")]
        public void Constructor_PassNullKeyProperty_ThrowsException()
        {
            //Arrange
            Func<ITestObject, int> keyProperty = null;

            //Act
            var result = new ReadOnlyPicnicCache<int, ITestObject>(keyProperty, _mockCacheable.Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "The property (Test) does not exist on the type (ITestObject).")]
        public void Constructor_PassInvalidKeyPropertyName_ThrowsException()
        {
            //Act
            var result = new ReadOnlyPicnicCache<int, ITestObject>("Test", _mockCacheable.Object);
        }

        [TestMethod]
        public void Constructor_PassValidKeyPropertyName_FetchWorks()
        {
            //Arrange
            var cache = new ReadOnlyPicnicCache<int, ITestObject>("Id", _mockCacheable.Object);
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(1)).Returns(item);

            //Act
            var result = cache.Fetch(1);

            //Assert
            Assert.AreEqual(item, result);
        }

        #endregion

        #region ClearAll Tests

        [TestMethod]
        public void ClearAll_CacheIsEmpty_DoesNotThrowException()
        {
            //Act
            _cache.ClearAll();
        }

        [TestMethod]
        public void ClearAll_CacheIsFull_ClearsTheCache()
        {
            //Arrange
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);
            var loading = _cache.FetchAll();

            //Act
            _cache.ClearAll();
            var result = _cache.FetchAll();

            //Assert
            _mockCacheable.Verify(c => c.FetchAll(), Times.Exactly(2));
        }

        [TestMethod]
        public void ClearAll_SingleItemIsCached_ClearsTheCache()
        {
            //Arrange
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(1)).Returns(item);
            var loading = _cache.Fetch(1);

            //Act
            _cache.ClearAll();
            var result = _cache.Fetch(1);

            //Assert
            _mockCacheable.Verify(c => c.Fetch(1), Times.Exactly(2));
        }


        #endregion

        #region Fetch Tests

        [TestMethod]
        public void Fetch_ItemIsNotInCache_RetrievesItemFromCacheable()
        {
            //Arrange
            var mockTestObject = new Mock<ITestObject>();
            _mockCacheable.Setup(c => c.Fetch(It.IsAny<int>())).Returns(mockTestObject.Object);

            //Act
            var result = _cache.Fetch(1);

            //Assert
            Assert.AreEqual(mockTestObject.Object, result);
            _mockCacheable.Verify(c => c.Fetch(1), Times.Once);
        }

        [TestMethod]
        public void Fetch_CalledTwiceForSameKey_CallsCacheableOnce()
        {
            //Arrange
            var mockTestObject = new Mock<ITestObject>();
            mockTestObject.SetupGet(t => t.Id).Returns(1);
            _mockCacheable.Setup(c => c.Fetch(It.IsAny<int>())).Returns(mockTestObject.Object);
            var call1 = _cache.Fetch(1);

            //Act
            var result = _cache.Fetch(1);

            //Assert
            Assert.AreSame(call1, result);
            _mockCacheable.Verify(c => c.Fetch(1), Times.Once);
        }

        #endregion

        #region FetchAll Tests

        [TestMethod]
        public void FetchAll_CacheIsEmpty_ReturnsItemsFromCacheable()
        {
            //Arrange
            var list = GetTestList(5);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);

            //Act
            var result = _cache.FetchAll();

            //Assert
            ValidateListsAreEqual(list, result);
            _mockCacheable.Verify(c => c.FetchAll(), Times.Once);
        }

        [TestMethod]
        public void FetchAll_CacheIsPartiallyLoaded_ReturnsAllItems()
        {
            //Arrange
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);
            _mockCacheable.Setup(c => c.Fetch(It.Is<int>(x => x == 4))).Returns(list.FirstOrDefault(l => l.Id == 4));
            var firstItem = _cache.Fetch(4);

            //Act
            var result = _cache.FetchAll();

            //Assert
            Assert.AreEqual(10, result.Count());
            ValidateListsAreEqual(list, result.OrderBy(r => r.Id).ToList());
            _mockCacheable.Verify(c => c.FetchAll(), Times.Once);
        }

        [TestMethod]
        public void FetchAll_CalledTwice_OnlyCallsCacheableOnce()
        {
            //Arrange
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);

            //Act
            var result = _cache.FetchAll();
            var secondCall = _cache.FetchAll();

            //Assert
            ValidateListsAreEqual(list, result);
            _mockCacheable.Verify(c => c.FetchAll(), Times.Once);
        }

        #endregion
    }
}