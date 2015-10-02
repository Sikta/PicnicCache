//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PicnicCache.Tests.Models;

namespace PicnicCache.Tests.Tests
{
    /// <summary>
    /// Summary description for PicnicCacheTests
    /// </summary>
    [TestClass]
    public class PicnicCacheTests : PicnicCacheTestBase
    {
        #region Fields

        private ICache<int, ITestObject> _cache;
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

        public PicnicCacheTests()
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
            _cache = new PicnicCache<int, ITestObject>(x => x.Id, _mockCacheable.Object);
        }

        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }

        #endregion

        #region Constructor Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: cacheable")]
        public void Constructor_CacheableIsNull_ThrowsException()
        {
            //Act
            var result = new PicnicCache<int, ITestObject>("Test", null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: keyPropertyName")]
        public void Constructor_KeyPropertyNameIsNull_ThrowsException()
        {
            //Arrange
            string keyPropertyName = null;

            //Act
            var result = new PicnicCache<int, ITestObject>(keyPropertyName, _mockCacheable.Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: keyProperty")]
        public void Constructor_KeyPropertyIsNull_ThrowsException()
        {
            //Arrange
            Func<ITestObject, int> keyProperty = null;

            //Act
            var result = new PicnicCache<int, ITestObject>(keyProperty, _mockCacheable.Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "The property (Test) does not exist on the type (ITestObject).")]
        public void Constructor_InvalidKeyPropertyName_ThrowsException()
        {
            //Act
            var result = new PicnicCache<int, ITestObject>("Test", _mockCacheable.Object);
        }

        [TestMethod]
        public void Constructor_ValidKeyPropertyName_FetchWorks()
        {
            //Arrange
            var cache = new PicnicCache<int, ITestObject>("Id", _mockCacheable.Object);
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(1)).Returns(item);

            //Act
            var result = cache.Fetch(1);

            //Assert
            Assert.AreEqual(item, result);
        }

        #endregion

        #region Add Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: value")]
        public void Add_PassNullValue_ThrowsException()
        {
            //Act
            _cache.Add(null);
        }

        [TestMethod]
        public void Add_ValuePassed_ItemIsAddedToCache()
        {
            //Arrange
            var item = GetTestList(1).First();

            //Act
            _cache.Add(item);

            //Assert
            var result = _cache.Fetch(0);
            Assert.AreEqual(item, result);
        }

        #endregion

        #region ClearAll Tests

        [TestMethod]
        public void ClearAll_ItemIsDeleted_ClearedFromCacheAndCacheableCalledToRetrieveItem()
        {
            //Arrange
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstCall = _cache.Fetch(0);
            _cache.Delete(0);

            //Act
            _cache.ClearAll();

            //Assert
            var result = _cache.Fetch(0);
            Assert.AreEqual(item, result);
            _mockCacheable.Verify(c => c.Fetch(0), Times.Exactly(2));
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: value")]
        public void Delete_NullValueIsPassed_ThrowsException()
        {
            //Act
            _cache.Delete(null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: key")]
        public void Delete_NullKeyIsPassed_ThrowsException()
        {
            //Arrange
            var cache = new PicnicCache<string, ITestObject>(x => x.Name, new Mock<ICacheable<string, ITestObject>>().Object);
            string key = null;

            //Act
            cache.Delete(key);
        }

        [TestMethod]
        public void Delete_KeyIsInCache_ReturnsTrue()
        {
            //Arrange
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstCall = _cache.Fetch(0);

            //Act
            var result = _cache.Delete(0);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Delete_KeyNotInCache_ReturnsFalse()
        {
            //Act
            var result = _cache.Delete(0);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Delete_KeyInCache_ItemNotReturnedWhenFetched()
        {
            //Arrange
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstCall = _cache.Fetch(0);

            //Act
            var result = _cache.Delete(0);

            //Assert
            var secondCall = _cache.Fetch(0);
            Assert.IsNull(secondCall);
            _mockCacheable.Verify(c => c.Fetch(0), Times.Once);
        }

        [TestMethod]
        public void Delete_ValueInCache_ReturnsTrue()
        {
            //Arrange
            var item = GetTestList(1).First();
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstCall = _cache.Fetch(0);

            //Act
            var result = _cache.Delete(item);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Delete_ItemInAddedState_RemovedFromCache()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var item = GetTestItem(0);
            _cache.Add(item);

            //Act
            _cache.Delete(item);

            //Assert
            _cache.Save();
            Assert.AreEqual(0, addedItems.Count());
            Assert.AreEqual(0, deletedItems.Count());
            Assert.AreEqual(0, updatedItems.Count());
        }

        #endregion

        #region Save Tests

        [TestMethod]
        public void Save_AnItemInEachState_CorrectItemsPassedToCacheableSaveMethod()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var newItem = GetTestItem(11);
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);
            var items = _cache.FetchAll();
            var updatedItem = items.First(i => i.Id == 4);
            var deletedItem = items.First(i => i.Id == 2);
            _cache.Update(updatedItem);
            _cache.Delete(deletedItem);
            _cache.Add(newItem);

            //Act
            _cache.Save();

            //Assert
            _mockCacheable.Verify(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>()), Times.Once);
            Assert.IsNotNull(addedItems);
            Assert.AreEqual(1, addedItems.Count());
            Assert.AreEqual(newItem, addedItems.First());
            Assert.IsNotNull(deletedItems);
            Assert.AreEqual(1, deletedItems.Count());
            Assert.AreEqual(deletedItem, deletedItems.First());
            Assert.IsNotNull(updatedItems);
            Assert.AreEqual(1, updatedItems.Count());
            Assert.AreEqual(updatedItem, updatedItems.First());
        }

        [TestMethod]
        public void Save_NoItemsNotInUnmodifiedState_CallsSaveWithEmptyLists()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);
            _cache.FetchAll();

            //Act
            _cache.Save();

            //Assert
            Assert.IsNotNull(addedItems);
            Assert.AreEqual(0, addedItems.Count());
            Assert.IsNotNull(deletedItems);
            Assert.AreEqual(0, deletedItems.Count());
            Assert.IsNotNull(updatedItems);
            Assert.AreEqual(0, updatedItems.Count());
        }

        [TestMethod]
        public void Save_CacheIsEmpty_CallsSaveWithEmptyLists()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);

            //Act
            _cache.Save();

            //Assert
            Assert.IsNotNull(addedItems);
            Assert.AreEqual(0, addedItems.Count());
            Assert.IsNotNull(deletedItems);
            Assert.AreEqual(0, deletedItems.Count());
            Assert.IsNotNull(updatedItems);
            Assert.AreEqual(0, updatedItems.Count());
        }

        #endregion

        #region Update Tests

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: value")]
        public void Update_NullValuePassed_ThrowsException()
        {
            //Arrange
            ITestObject value = null;

            //Act
            _cache.Update(value);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: values")]
        public void Update_NullValuesPassed_ThrowsException()
        {
            //Arrange
            IEnumerable<ITestObject> values = null;

            //Act
            _cache.Update(values);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: key")]
        public void Update_NullKeyPassed_ThrowsException()
        {
            //Arrange
            var cache = new PicnicCache<string, ITestObject>(x => x.Name, new Mock<ICacheable<string, ITestObject>>().Object);
            string key = null;

            //Act
            cache.Update(key, new object(), new Mock<Func<object, ITestObject, ITestObject>>().Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: dto")]
        public void Update_NullDtoPassed_ThrowsException()
        {
            //Arrange
            object dto = null;

            //Act
            _cache.Update(4, dto, new Mock<Func<object, ITestObject, ITestObject>>().Object);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: updateCacheItemMethod")]
        public void Update_NullUpdateCacheItemMethodPassed_ThrowsException()
        {
            //Arrange
            Func<object, ITestObject, ITestObject> updateCacheItemMethod = null;

            //Act
            _cache.Update(4, new object(), updateCacheItemMethod);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "The item (key: 0) is not in the cache.")]
        public void Update_ItemNotInCache_ThrowsException()
        {
            //Arrange
            var item = GetTestItem(0);

            //Act
            _cache.Update(item);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "The item (key: 0) is currently deleted.")]
        public void Update_ItemIsDeleted_ThrowsException()
        {
            //Arrange
            var item = GetTestItem(0);
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstItem = _cache.Fetch(0);
            _cache.Delete(firstItem);

            //Act
            _cache.Update(firstItem);
        }

        [TestMethod]
        public void Update_ItemIsInAddedState_ItemStaysInAddedState()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var item = GetTestItem(0);
            _cache.Add(item);

            //Arrange
            _cache.Update(item);

            //Assert
            _cache.Save();
            Assert.AreEqual(1, addedItems.Count());
            Assert.AreEqual(item, addedItems.First());
            Assert.AreEqual(0, updatedItems.Count());
        }

        [TestMethod]
        public void Update_ItemIsUnmodified_ItemChangesToModifiedState()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var item = GetTestItem(0);
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstItem = _cache.Fetch(0);

            //Act
            _cache.Update(firstItem);

            //Assert
            _cache.Save();
            Assert.AreEqual(1, updatedItems.Count());
            Assert.AreEqual(firstItem, updatedItems.First());
            Assert.AreEqual(0, deletedItems.Count());
            Assert.AreEqual(0, addedItems.Count());
        }

        [TestMethod]
        public void Update_ListAsParameter_AllMarkedAsModified()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var list = GetTestList(10);
            _mockCacheable.Setup(c => c.FetchAll()).Returns(list);
            var items = _cache.FetchAll();

            //Act
            _cache.Update(items);

            //Assert
            _cache.Save();
            ValidateListsAreEqual(items, updatedItems);
        }

        [TestMethod]
        public void Update_ItemUsingDto_ItemIsUpdated()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var dto = new DtoModel("Updated");
            var model = new TestModel() { Id = 0, Name = "Unmodified", City = "Eden Prairie", State = "MN" };
            Func<DtoModel, ITestObject, ITestObject> updateMethod = (d, m) => { m.Name = d.Name; return m; };
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(model);
            var firstItem = _cache.Fetch(0);

            //Act
            _cache.Update(0, dto, updateMethod);

            //Assert
            _cache.Save();
            Assert.AreEqual(1, updatedItems.Count());
            Assert.AreEqual(model, updatedItems.First()); 
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "The item (key: 0) is currently deleted.")]
        public void Update_UsingDtoAndItemIsDeleted_ThrowsException()
        {
            //Arrange
            var item = GetTestItem(0);
            _mockCacheable.Setup(c => c.Fetch(0)).Returns(item);
            var firstItem = _cache.Fetch(0);
            _cache.Delete(firstItem);
            var dto = new DtoModel("Fails");
            Func<DtoModel, ITestObject, ITestObject> updateMethod = (d, m) => { m.Name = d.Name; return m; };

            //Act
            _cache.Update(0, dto, updateMethod);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "The item (key: 0) is not in the cache.")]
        public void Update_UsingDtoAndItemIsNotInCache_ThowsException()
        {
            //Arrange
            var dto = new DtoModel("Fails");
            Func<DtoModel, ITestObject, ITestObject> updateMethod = (d, m) => { m.Name = d.Name; return m; };

            //Act
            _cache.Update(0, dto, updateMethod);
        }

        [TestMethod]
        public void Update_UsingDtoAndItemIsInAddedState_ItemStaysInAddedState()
        {
            //Arrange
            IEnumerable<ITestObject> addedItems = null;
            IEnumerable<ITestObject> deletedItems = null;
            IEnumerable<ITestObject> updatedItems = null;
            Action<IEnumerable<ITestObject>, IEnumerable<ITestObject>, IEnumerable<ITestObject>> callback = (a, d, u) => { addedItems = a; deletedItems = d; updatedItems = u; };
            _mockCacheable.Setup(c => c.Save(It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>(), It.IsAny<IEnumerable<ITestObject>>())).Callback(callback);
            var item = GetTestItem(0);
            _cache.Add(item);
            var dto = new DtoModel("ItemStayedModified");
            Func<DtoModel, ITestObject, ITestObject> updateMethod = (d, m) => { m.Name = d.Name; return m; };

            //Arrange
            _cache.Update(0, dto, updateMethod);

            //Assert
            _cache.Save();
            Assert.AreEqual(1, addedItems.Count());
            Assert.AreEqual(item, addedItems.First());
            Assert.AreEqual(0, updatedItems.Count());
        }

        #endregion
    }
}